using component.logger;
using iot.solution.data;
using iot.solution.entity;
using iot.solution.model.Repository.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Xml.Linq;
using System.Xml.Serialization;
using Entity = iot.solution.entity;
using Model = iot.solution.model.Models;
using LogHandler = component.services.loghandler;
namespace iot.solution.model.Repository.Implementation
{
    public class HardwareKitRepository : GenericRepository<Model.HardwareKit>, IHardwareKitRepository
    {
        private readonly LogHandler.Logger logger;
        public HardwareKitRepository(IUnitOfWork unitOfWork, LogHandler.Logger logManager) : base(unitOfWork, logManager)
        {
            logger = logManager;
            _uow = unitOfWork;
        }

        public List<LookupItem> GetKitTypeLookup()
        {
            using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
            {
                return sqlDataAccess.QueryList<Entity.LookupItem>("SELECT CONVERT(NVARCHAR(50),[Guid]) AS [Value], [name] AS [Text] FROM [KitType] WHERE [isActive] = 1 AND [isDeleted] = 0");
            }
        }

        public Entity.SearchResult<List<Entity.HardwareKitResponse>> List(Entity.SearchRequest request, bool isAssigned, string companyId)
        {
            Entity.SearchResult<List<Entity.HardwareKitResponse>> result = new Entity.SearchResult<List<Entity.HardwareKitResponse>>();
            try
            {
                logger.InfoLog(Constants.ACTION_ENTRY, "HardwareKitRepository.Get");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<System.Data.Common.DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, request.Version);
                    if(!string.IsNullOrEmpty(companyId))
                    parameters.Add(sqlDataAccess.CreateParameter("companyguid", Guid.Parse(companyId), DbType.Guid, ParameterDirection.Input));

                    parameters.Add(sqlDataAccess.CreateParameter("search", request.SearchText, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("isAssigned", isAssigned ? 1 : 0, DbType.Int32, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("pagesize", request.PageSize, DbType.Int32, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("pagenumber", request.PageNumber, DbType.Int32, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("orderby", request.OrderBy, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("count", DbType.Int32, ParameterDirection.Output, 16));
                    System.Data.Common.DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[HardwareKit_List]", CommandType.StoredProcedure, null), parameters.ToArray());
                    result.Items = DataUtils.DataReaderToList<Entity.HardwareKitResponse>(dbDataReader, null);
                    result.Count = int.Parse(parameters.Where(p => p.ParameterName.Equals("count")).FirstOrDefault().Value.ToString());
                }
                logger.InfoLog(Constants.ACTION_EXIT, "HardwareKitRepository.Get");
            }
            catch (Exception ex)
            {
                logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
            }
            return result;
        }
        public ActionStatus VerifyHardwareKit(KitVerifyRequest request, bool isEdit = false)
        {
            var response = new ActionStatus(true);
            var result = new List<BulkUploadResponse>();
            try
            {
                logger.InfoLog(Constants.ACTION_ENTRY, "HardwareKitRepository.VerifyHardwareKit");
                var createdHardwareKits = new List<BulkUploadResponse>();
                var xmlData = string.Empty;                
                using (var stringwriter = new System.IO.StringWriter())
                {
                    var serializer = new XmlSerializer(request.GetType());
                    serializer.Serialize(stringwriter, request);
                    xmlData = stringwriter.ToString();
                }

                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    
                    List<System.Data.Common.DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, component.helper.SolutionConfiguration.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("data", xmlData.ToString(), DbType.Xml, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("isEdit", (isEdit == false ? 0 : 1), DbType.Boolean, ParameterDirection.Input));
                  
                    System.Data.Common.DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[HardwareKit_Validate]", CommandType.StoredProcedure, null), parameters.ToArray());
                    result = DataUtils.DataReaderToList<Entity.BulkUploadResponse>(dbDataReader, null);

                    response.Data = result;
                    var errorResult = result.Where(x => !string.IsNullOrEmpty(x.hardwareKitError));
                    var isValid = errorResult.Count();
                    if (isValid > 0)
                    {
                        response.Success = false;
                        response.Message = errorResult.FirstOrDefault().hardwareKitError;

                        //foreach (var error in errorResult)
                        //{
                        //    response.Message += error.hardwareKitError + ". ";
                        //}                   
                    }
                    else
                    {
                        response.Success = true;
                    }
                }
                logger.InfoLog(Constants.ACTION_EXIT, "HardwareKitRepository.VerifyHardwareKit");
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message.ToString();
                logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
            }

            return response;
        }
        public ActionStatus SaveHardwareKit(Entity.KitVerifyRequest requestData, bool isEdit)
        {
            var response = new ActionStatus();
            try
            {
                logger.InfoLog(Constants.ACTION_ENTRY, "HardwareKitRepository.SaveHardwareKit");
                var createdHardwareKits = new List<BulkUploadResponse>();
                var xmlData = string.Empty;

                using (var stringwriter = new System.IO.StringWriter())
                {
                    var serializer = new XmlSerializer(requestData.GetType());
                    serializer.Serialize(stringwriter, requestData);
                    xmlData = stringwriter.ToString();
                }

                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<System.Data.Common.DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, component.helper.SolutionConfiguration.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("data", xmlData.ToString(), DbType.Xml, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("isEdit", (isEdit == false ? 0 : 1), DbType.Boolean, ParameterDirection.Input));
                    System.Data.Common.DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[HardwareKit_AddUpdate]", CommandType.StoredProcedure, null), parameters.ToArray());

                   
                    while (dbDataReader.Read())
                    {
                        var item = new Entity.BulkUploadResponse();
                        if (dbDataReader["kitGuid"] != null)
                        {
                            item.kitGuid = new Guid(dbDataReader["kitGuid"].ToString().ToUpper());
                            item.kitTypeGuid = new Guid(dbDataReader["kitTypeGuid"].ToString().ToUpper());
                            item.kitCode = (dbDataReader["kitCode"].ToString());

                            createdHardwareKits.Add(item);

                        }
                        else
                        {
                            response.Message = dbDataReader["fieldname"].ToString().ToUpper();
                            response.Data = "Unable to save Hardware Kit!"; 
                            response.Success = false;
                        }
                    }
                    if (createdHardwareKits.Count != 0 || createdHardwareKits.Count == 1)
                    {
                      
                        response.Success = true;
                        response.Message = "Hardware Kit saved successfully!";
                        response.Data = createdHardwareKits;
                    }
                    else
                    {
                        response.Success = false;
                        response.Data = createdHardwareKits;
                        response.Message = "Unable to save Hardware Kit!";
                    }
                    
                }
                logger.InfoLog(Constants.ACTION_EXIT, "HardwareKitRepository.SaveHardwareKit");
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message.ToString();
                logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
            }

            return response;
        }


        public ActionStatus UploadHardwareKit(List<Entity.HardwareKitDTO> requestData)
        {
            var response = new ActionStatus();
            try
            {
                logger.InfoLog(Constants.ACTION_ENTRY, "HardwareKitRepository.SaveHardwareKit");
                List<Guid> createdHardwareKits = new List<Guid>();
                var stringResult = string.Empty;
                using (var stringwriter = new System.IO.StringWriter())
                {
                    var serializer = new XmlSerializer(requestData.GetType());
                    serializer.Serialize(stringwriter, requestData);
                    stringResult = stringwriter.ToString();
                }

                stringResult = stringResult.Replace("HardwareKitDTO", "HardwareKit").Replace("KitDeviceDTO", "KitDevice");

                XDocument requestXML = XDocument.Parse(stringResult);
                requestXML = new XDocument(new XElement("HardwareKits", requestXML.Root));

                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<System.Data.Common.DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, component.helper.SolutionConfiguration.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("data", requestXML.ToString(), DbType.Xml, ParameterDirection.Input));

                    System.Data.Common.DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[KitDevice_AddUpdate]", CommandType.StoredProcedure, null), parameters.ToArray());

                    while (dbDataReader.Read())
                    {
                        if (dbDataReader["guid"] != null)
                        {
                            createdHardwareKits.Add(new Guid(dbDataReader["guid"].ToString().ToUpper()));
                        }
                        else
                        {
                            response.Message = dbDataReader["fieldname"].ToString().ToUpper();
                            response.Data = null;
                            response.Success = false;
                        }
                    }

                    if (createdHardwareKits.Count != 0 && createdHardwareKits.Count == 1)
                    {
                      //  requestData.Guid = createdHardwareKits.FirstOrDefault();
                        response.Success = true;
                        response.Data = requestData;
                    }
                    else
                    {
                        response.Success = false;
                        response.Data = requestData;
                        response.Message = "Unable to save Hardware-Kit";
                    }
                    //var result = DataUtils.DataReaderToList<Entity.HardwareKitResponse>(dbDataReader, null);
                    //result.Count = int.Parse(parameters.Where(p => p.ParameterName.Equals("count")).FirstOrDefault().Value.ToString());
                }
                logger.InfoLog(Constants.ACTION_EXIT, "HardwareKitRepository.SaveHardwareKit");
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message.ToString();
                logger.ErrorLog(Constants.ACTION_EXCEPTION, ex);
            }

            return response;
        }
    }
}
