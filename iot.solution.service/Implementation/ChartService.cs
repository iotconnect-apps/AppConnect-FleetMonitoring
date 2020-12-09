using iot.solution.data;
using iot.solution.model.Repository.Interface;
using iot.solution.service.Interface;
using System.Collections.Generic;
using Request = iot.solution.entity.Request;
using Response = iot.solution.entity.Response;
using System.Data;
using System.Data.Common;
using System.Reflection;
using component.logger;
using System;
using Entity = iot.solution.entity;
using LogHandler = component.services.loghandler;
using System.Linq;
using component.helper.Interface;
using component.helper;
using System.Configuration;

namespace iot.solution.service.Implementation
{
    public class ChartService : IChartService
    {
        private readonly IEntityRepository _entityRepository;
        private readonly IDriverRepository _driverRepository;
        private readonly ITripRepository _tripRepository;
        private readonly IEmailHelper _emailHelper;
        private readonly IUserRepository _userRepository;
        private readonly LogHandler.Logger _logger;
        public string ConnectionString = component.helper.SolutionConfiguration.Configuration.ConnectionString;
        //private readonly LogHandler.Logger _logger;
        public ChartService(ITripRepository tripRepository,IDriverRepository driverRepository,IUserRepository userRepository,IEntityRepository entityRepository, LogHandler.Logger logger, IEmailHelper emailHelper)//, LogHandler.Logger logger)
        {
            _entityRepository = entityRepository;
            _userRepository = userRepository;
            _driverRepository = driverRepository;
            _tripRepository = tripRepository;
            _logger = logger;
            _emailHelper = emailHelper;
        }
        public Entity.ActionStatus TelemetrySummary_DayWise()
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                _logger.InfoLog(LogHandler.Constants.ACTION_ENTRY, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<DbParameter> parameters = new List<DbParameter>();
                    sqlDataAccess.ExecuteNonQuery(sqlDataAccess.CreateCommand("[TelemetrySummary_DayWise_Add]", CommandType.StoredProcedure, null), parameters.ToArray());
                }
                _logger.InfoLog(LogHandler.Constants.ACTION_EXIT, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);

            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }
            return actionStatus;
        }
        public Entity.ActionStatus TelemetrySummary_HourWise()
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                _logger.InfoLog(LogHandler.Constants.ACTION_ENTRY, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<DbParameter> parameters = new List<DbParameter>();
                    sqlDataAccess.ExecuteNonQuery(sqlDataAccess.CreateCommand("[TelemetrySummary_HourWise_Add]", CommandType.StoredProcedure, null), parameters.ToArray());
                }
                _logger.InfoLog(LogHandler.Constants.ACTION_EXIT, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);

            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }
            return actionStatus;
        }

        public Entity.ActionStatus SendEmailNotification_HourWise()
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                _logger.InfoLog(LogHandler.Constants.ACTION_ENTRY, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<DbParameter> parameters = new List<DbParameter>();
                    System.Data.Common.DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[Get_TripDelayData]", CommandType.StoredProcedure, null), parameters.ToArray());
                    var result = DataUtils.DataReaderToList<Response.FleetDelayData>(dbDataReader, null);
                    foreach (var item in result)
                    {
                        string dalay = string.Empty;
                        //dalay = item.DelayInMin / 24 / 60 + "Day :" + item.DelayInMin / 60 % 24 + "Hours:" + item.DelayInMin % 60 + "Min";
                        dalay = item.DelayInMin / 60 + ":" + item.DelayInMin % 60 + " (HH:MM)";
                        string dynamicTr = string.Empty;
                        dynamicTr = "<tr><td>" + item.FleetName + "</td><td>" + item.DriverName + "</td><td>" + item.SourceLocation + "</td><td>" + item.destinationLocation + "</td><td>" + item.StartDateTime.ToString() + "</td><td>" + dalay + "</td></tr>";
                        _emailHelper.SendFleetDelayNotificationEmail(item.FleetName, item.OwnerName, dynamicTr, item.OwnerEmail, item.DriverEmail);
                    }
                }
                _logger.InfoLog(LogHandler.Constants.ACTION_EXIT, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);

            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }
            return actionStatus;
        }

        public Entity.ActionStatus SendSubscriptionNotification()
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                _logger.InfoLog(LogHandler.Constants.ACTION_ENTRY, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<DbParameter> parameters = new List<DbParameter>();
                    System.Data.Common.DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[Get_UserSubscriptionEndData]", CommandType.StoredProcedure, null), parameters.ToArray());
                    var result = DataUtils.DataReaderToList<Response.SubscriptionEndData>(dbDataReader, null);
                    foreach (var item in result)
                    {
                        _emailHelper.SendSubscriptionOverEmail(item.CustomerName, item.ExpiryDate.ToString("dd MMM yyy"), item.Email);
                    }
                }
                _logger.InfoLog(LogHandler.Constants.ACTION_EXIT, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);

            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }
            return actionStatus;
        }
        public Entity.ActionStatus SendEmailNotification_Radius(Entity.FleetDetail fleetdata, double radiusInKM)
        {
            Entity.ActionStatus actionStatus = new Entity.ActionStatus(true);
            try
            {
                _logger.InfoLog(LogHandler.Constants.ACTION_ENTRY, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                if (fleetdata != null )
                {
                    Entity.User user = _userRepository.FindBy(r => r.CompanyGuid == fleetdata.CompanyGuid).Select(p => Mapper.Configuration.Mapper.Map<Entity.User>(p)).FirstOrDefault();
                    Entity.Driver driver = _driverRepository.FindBy(r=>r.FleetGuid == fleetdata.Guid).Select(p => Mapper.Configuration.Mapper.Map<Entity.Driver>(p)).FirstOrDefault();
                    Entity.Trip trip = _tripRepository.FindBy(r => r.FleetGuid == fleetdata.Guid && r.IsStarted && !r.IsCompleted).Select(p => Mapper.Configuration.Mapper.Map<Entity.Trip>(p)).FirstOrDefault();
                    if (user != null && driver!=null && trip!=null)
                    {
                        string radius = string.Empty;

                        radius = fleetdata.Radius / 1000 +" KM";
                        string dynamicTr = string.Empty;
                        dynamicTr = "<tr><td>" + fleetdata.FleetId + "</td><td>" + driver.DriverId + "</td><td>" + trip.TripId + "</td><td>" + radiusInKM.ToString("0.##") + " KM</td><td>" + radius + "</td></tr>";

                        _emailHelper.SendFleetRadiusNotificationEmail(fleetdata.FleetId, user.FirstName + ' ' + user.LastName, dynamicTr, user.Email,driver.Email);
                    }
                }
                _logger.InfoLog(LogHandler.Constants.ACTION_EXIT, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);

            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                actionStatus.Success = false;
                actionStatus.Message = ex.Message;
            }
            return actionStatus;
        }
        public List<Response.EnergyUsageResponse> GetEnergyUsage(Request.ChartRequest request)
        {
            List<Response.EnergyUsageResponse> result = new List<Response.EnergyUsageResponse>();
            try
            {
                _logger.InfoLog(Constants.ACTION_ENTRY, "Chart_EnergyConsumptionByFleet.Get");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, component.helper.SolutionConfiguration.Version);

                    parameters.Add(sqlDataAccess.CreateParameter("guid", component.helper.SolutionConfiguration.CompanyId, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("syncDate", DateTime.UtcNow, DbType.DateTime, ParameterDirection.Output));
                    parameters.Add(sqlDataAccess.CreateParameter("enableDebugInfo", component.helper.SolutionConfiguration.EnableDebugInfo, DbType.String, ParameterDirection.Input));
                    DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[Chart_EnergyConsumption]", CommandType.StoredProcedure, null), parameters.ToArray());
                    result = DataUtils.DataReaderToList<Response.EnergyUsageResponse>(dbDataReader, null);
                }
                _logger.InfoLog(Constants.ACTION_EXIT, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }
        public List<Response.TripsByDriverResponse> GetTripsByDriver(Request.ChartRequest request)
        {
            List<Response.TripsByDriverResponse> result = new List<Response.TripsByDriverResponse>();
            try
            {
                _logger.InfoLog(Constants.ACTION_ENTRY, "Chart_TripsByDriver.Get");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, component.helper.SolutionConfiguration.Version);

                    parameters.Add(sqlDataAccess.CreateParameter("guid", request.DriverGuid, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("frequency", request.Frequency, DbType.String, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("syncDate", DateTime.UtcNow, DbType.DateTime, ParameterDirection.Output));
                    parameters.Add(sqlDataAccess.CreateParameter("enableDebugInfo", component.helper.SolutionConfiguration.EnableDebugInfo, DbType.String, ParameterDirection.Input));
                    DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[Chart_TripsByDriver]", CommandType.StoredProcedure, null), parameters.ToArray());
                    result = DataUtils.DataReaderToList<Response.TripsByDriverResponse>(dbDataReader, null);
                }
                _logger.InfoLog(Constants.ACTION_EXIT, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }
        public List<Response.FleetStatusResponse> GetFleetStatus(Request.ChartRequest request)
        {
            List<Response.FleetStatusResponse> result = new List<Response.FleetStatusResponse>();
            try
            {
                _logger.InfoLog(Constants.ACTION_ENTRY, "Chart_FleetStatus.Get");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, component.helper.SolutionConfiguration.Version);

                    parameters.Add(sqlDataAccess.CreateParameter("guid", component.helper.SolutionConfiguration.CompanyId, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("syncDate", DateTime.UtcNow, DbType.DateTime, ParameterDirection.Output));
                    parameters.Add(sqlDataAccess.CreateParameter("enableDebugInfo", component.helper.SolutionConfiguration.EnableDebugInfo, DbType.String, ParameterDirection.Input));
                    DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[Chart_FleetStatus]", CommandType.StoredProcedure, null), parameters.ToArray());
                    result = DataUtils.DataReaderToList<Response.FleetStatusResponse>(dbDataReader, null);
                }
                _logger.InfoLog(Constants.ACTION_EXIT, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }
        public List<Response.EnergyUsageResponse> GetEnergyUsageByFleet(Request.ChartRequest request)
        {
            List<Response.EnergyUsageResponse> result = new List<Response.EnergyUsageResponse>();
            try
            {
                _logger.InfoLog(Constants.ACTION_ENTRY, "Chart_EnergyConsumptionByFleet.Get");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, component.helper.SolutionConfiguration.Version);
                   
                    parameters.Add(sqlDataAccess.CreateParameter("guid", request.FleetGuid, DbType.Guid, ParameterDirection.Input));                  
                    parameters.Add(sqlDataAccess.CreateParameter("syncDate", DateTime.UtcNow, DbType.DateTime, ParameterDirection.Output));
                    parameters.Add(sqlDataAccess.CreateParameter("enableDebugInfo", component.helper.SolutionConfiguration.EnableDebugInfo, DbType.String, ParameterDirection.Input));
                    DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[Chart_EnergyConsumptionByFleet]", CommandType.StoredProcedure, null), parameters.ToArray());
                    result = DataUtils.DataReaderToList<Response.EnergyUsageResponse>(dbDataReader, null);
                }
                _logger.InfoLog(Constants.ACTION_EXIT, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }
        public List<Response.OdometerResponse> GetOdometerByFleet(Request.ChartRequest request)
        {
            List<Response.OdometerResponse> result = new List<Response.OdometerResponse>();
            try
            {
                _logger.InfoLog(Constants.ACTION_ENTRY, "Chart_OdometerReadingByFleet.Get");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, component.helper.SolutionConfiguration.Version);

                    parameters.Add(sqlDataAccess.CreateParameter("guid", request.FleetGuid, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("syncDate", DateTime.UtcNow, DbType.DateTime, ParameterDirection.Output));
                    parameters.Add(sqlDataAccess.CreateParameter("enableDebugInfo", component.helper.SolutionConfiguration.EnableDebugInfo, DbType.String, ParameterDirection.Input));
                    DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[Chart_OdometerReadingByFleet]", CommandType.StoredProcedure, null), parameters.ToArray());
                    result = DataUtils.DataReaderToList<Response.OdometerResponse>(dbDataReader, null);
                }
                _logger.InfoLog(Constants.ACTION_EXIT, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }
        //public List<Response.DeviceTypeUsageResponse> GetDeviceUsage(Request.ChartRequest request)
        //{
        //    List<Response.DeviceTypeUsageResponse> result = new List<Response.DeviceTypeUsageResponse>();
        //    try
        //    {
        //        _logger.InfoLog(Constants.ACTION_ENTRY, "Chart_Utilization.Get");
        //        using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
        //        {
        //            List<DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, component.helper.SolutionConfiguration.Version);


        //            if (request.DeviceGuid != null && request.DeviceGuid != Guid.Empty)
        //            {
        //                parameters.Add(sqlDataAccess.CreateParameter("guid", request.DeviceGuid, DbType.Guid, ParameterDirection.Input));
        //            }
        //            if (request.EntityGuid != null && request.EntityGuid != Guid.Empty)
        //            {
        //                parameters.Add(sqlDataAccess.CreateParameter("entityGuid", request.EntityGuid, DbType.Guid, ParameterDirection.Input));
        //            }

        //            parameters.Add(sqlDataAccess.CreateParameter("frequency", request.Frequency, DbType.String, ParameterDirection.Input));

        //            parameters.Add(sqlDataAccess.CreateParameter("syncDate", DateTime.UtcNow, DbType.DateTime, ParameterDirection.Output));
        //            parameters.Add(sqlDataAccess.CreateParameter("enableDebugInfo", component.helper.SolutionConfiguration.EnableDebugInfo, DbType.String, ParameterDirection.Input));
        //            DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[Chart_Utilization]", CommandType.StoredProcedure, null), parameters.ToArray());
        //            result = DataUtils.DataReaderToList<Response.DeviceTypeUsageResponse>(dbDataReader, null);
        //        }
        //        _logger.InfoLog(Constants.ACTION_EXIT, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
        //    }
        //    return result;
        //}
        //public List<Response.CompanyUsageResponse> GetCompanyUsage(Request.ChartRequest request)
        //{
        //    List<Response.CompanyUsageResponse> result = new List<Response.CompanyUsageResponse>();
        //    try
        //    {
        //        _logger.InfoLog(Constants.ACTION_ENTRY, "Chart_UtilizationByCompany.Get");
        //        using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
        //        {
        //            List<DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, component.helper.SolutionConfiguration.Version);

        //            parameters.Add(sqlDataAccess.CreateParameter("companyGuid", component.helper.SolutionConfiguration.CompanyId, DbType.Guid, ParameterDirection.Input));

        //            parameters.Add(sqlDataAccess.CreateParameter("frequency", request.Frequency, DbType.String, ParameterDirection.Input));

        //            parameters.Add(sqlDataAccess.CreateParameter("syncDate", DateTime.UtcNow, DbType.DateTime, ParameterDirection.Output));
        //            parameters.Add(sqlDataAccess.CreateParameter("enableDebugInfo", component.helper.SolutionConfiguration.EnableDebugInfo, DbType.String, ParameterDirection.Input));
        //            DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[Chart_UtilizationByCompany]", CommandType.StoredProcedure, null), parameters.ToArray());
        //            result = DataUtils.DataReaderToList<Response.CompanyUsageResponse>(dbDataReader, null);
        //        }
        //        _logger.InfoLog(Constants.ACTION_EXIT, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
        //    }
        //    return result;
        //}
        //public Entity.BaseResponse<List<Response.DeviceStatisticsResponse>> GetStatisticsByDevice(Request.ChartRequest request)
        //{
        //    Entity.BaseResponse<List<Response.DeviceStatisticsResponse>> result = new Entity.BaseResponse<List<Response.DeviceStatisticsResponse>>();
        //    try
        //    {
        //        _logger.InfoLog(Constants.ACTION_ENTRY, "Chart_StatisticsByDevice.Get");
        //        using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
        //        {
        //            List<DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, component.helper.SolutionConfiguration.Version);
        //            parameters.Add(sqlDataAccess.CreateParameter("guid", request.DeviceGuid, DbType.Guid, ParameterDirection.Input));
        //            parameters.Add(sqlDataAccess.CreateParameter("frequency", request.Frequency, DbType.String, ParameterDirection.Input));
        //            parameters.Add(sqlDataAccess.CreateParameter("attribute", request.Attribute, DbType.String, ParameterDirection.Input));
        //            parameters.Add(sqlDataAccess.CreateParameter("syncDate", DateTime.UtcNow, DbType.DateTime, ParameterDirection.Output));
        //            parameters.Add(sqlDataAccess.CreateParameter("enableDebugInfo", component.helper.SolutionConfiguration.EnableDebugInfo, DbType.String, ParameterDirection.Input));
        //            DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[Chart_StatisticsByDevice]", CommandType.StoredProcedure, null), parameters.ToArray());
        //            result.Data = DataUtils.DataReaderToList<Response.DeviceStatisticsResponse>(dbDataReader, null);
        //            if (parameters.Where(p => p.ParameterName.Equals("syncDate")).FirstOrDefault() != null)
        //            {
        //                result.LastSyncDate = Convert.ToString(parameters.Where(p => p.ParameterName.Equals("syncDate")).FirstOrDefault().Value);
        //            }
        //        }
        //        _logger.InfoLog(Constants.ACTION_EXIT, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
        //    }
        //    return result;
        //}

        public List<Response.FleetTypeUsageResponse> GetFleetTypeUsage(Request.ChartRequest request)
        {
            List<Response.FleetTypeUsageResponse> result = new List<Response.FleetTypeUsageResponse>();
            try
            {
                _logger.InfoLog(Constants.ACTION_ENTRY, "Chart_UtilizationByFleetType.Get");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, component.helper.SolutionConfiguration.Version);

                    parameters.Add(sqlDataAccess.CreateParameter("companyGuid", component.helper.SolutionConfiguration.CompanyId, DbType.Guid, ParameterDirection.Input));
                    if (request.EntityGuid != null && request.EntityGuid != Guid.Empty)
                    {
                        parameters.Add(sqlDataAccess.CreateParameter("parentEntityGuid", request.EntityGuid, DbType.Guid, ParameterDirection.Input));
                    }

                  
                    parameters.Add(sqlDataAccess.CreateParameter("syncDate", DateTime.UtcNow, DbType.DateTime, ParameterDirection.Output));
                    parameters.Add(sqlDataAccess.CreateParameter("enableDebugInfo", component.helper.SolutionConfiguration.EnableDebugInfo, DbType.String, ParameterDirection.Input));
                    DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[Chart_UtilizationByFleetType]", CommandType.StoredProcedure, null), parameters.ToArray());
                    result = DataUtils.DataReaderToList<Response.FleetTypeUsageResponse>(dbDataReader, null);
                }
                _logger.InfoLog(Constants.ACTION_EXIT, null, "", "", this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }

    }
}
