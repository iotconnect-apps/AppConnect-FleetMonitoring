using component.helper;
using component.logger;
using iot.solution.data;
using iot.solution.entity;
using iot.solution.model.Repository.Interface;
using iot.solution.service.Interface;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using Entity = iot.solution.entity;
using LogHandler = component.services.loghandler;
namespace iot.solution.service.Data
{
    public class LookupService : ILookupService
    {
        private readonly IDeviceRepository _deviceRepository;
        private readonly IFleetRepository _fleetRepository;
        private readonly IHardwareKitRepository _hardwareKitRepository;
        private readonly IEntityRepository _entityRepository;
        private readonly ICompanyRepository _companyRepository;
        private readonly LogHandler.Logger _logger;
        private readonly IotConnectClient _iotConnectClient;
        private readonly IKitTypeRepository _kitTypeRepository;
        
        private readonly IKitTypeAttributeRepository _kitTypeAttributeRepository;
        private readonly IkitTypeCommandRepository _kitTypeCommandRepository;
        private readonly IDeviceTypeRepository _deviceTypeRepository;
        private readonly IDriverRepository _driverRepository;

        public string ConnectionString = component.helper.SolutionConfiguration.Configuration.ConnectionString;
        public LookupService(LogHandler.Logger logManager,IFleetRepository fleetRepository, IDeviceRepository deviceRepository,IkitTypeCommandRepository kitTypeCommandRepository
           , IEntityRepository entityRepository, IHardwareKitRepository hardwareKitRepository
             , ICompanyRepository companyRepository
           , IKitTypeRepository kitTypeRepository, 
            IKitTypeAttributeRepository kitTypeAttributeRepository
            ,IDeviceTypeRepository deviceTypeRepository
            , IDriverRepository driverRepository)
        {
            _logger = logManager;
            _deviceRepository = deviceRepository;
            _fleetRepository = fleetRepository;
            _kitTypeCommandRepository = kitTypeCommandRepository;
            _entityRepository = entityRepository;
            _hardwareKitRepository = hardwareKitRepository;
            _companyRepository = companyRepository;
            _kitTypeAttributeRepository = kitTypeAttributeRepository;
            _kitTypeRepository = kitTypeRepository;
             _deviceTypeRepository=deviceTypeRepository;
            _driverRepository = driverRepository;
            _iotConnectClient = new IotConnectClient(SolutionConfiguration.BearerToken, SolutionConfiguration.Configuration.EnvironmentCode, SolutionConfiguration.Configuration.SolutionKey);
        }
        public string GetIotTemplateGuidByName(string templateName)
        {
            string templateGuid = string.Empty;
            var templates = _iotConnectClient.Template.All(new IoTConnect.Model.PagingModel() { SearchText = templateName, PageNo = 1, PageSize = 1000 }).Result;
            if (templates != null && templates.data != null && templates.data.Any())
            {
                templateGuid = templates.data[0].Guid;
            }
            return templateGuid;
        }
        public List<Entity.LookupItem> Get(string type, string param)
        {
            List<Entity.LookupItem> result = new List<Entity.LookupItem>();
            switch (type.ToLower())
            {
                #region endpoints for notifications module
                case "iotentity":
                    var entity = _iotConnectClient.Entity.All().Result;
                    if (entity != null && entity.data != null && entity.data.Any())
                    {
                        result = (from r in entity.data select new Entity.LookupItem() { Value = r.Guid, Text = r.Name }).ToList();
                    }
                    break;
                case "iotdevice":
                    if (string.IsNullOrWhiteSpace(param)) { throw new System.Exception("Companyid is missing in request"); }
                    var device = _iotConnectClient.Device.AllDevice(new IoTConnect.Model.AllDeviceModel { templateGuid = param }).Result;
                    if (device != null && device.Data != null && device.Data.Any())
                    {
                        result = (from r in device.Data select new Entity.LookupItem() { Value = r.Guid, Text = r.DisplayName }).ToList();
                    }
                    break;
                case "iotuser":
                    var user = _iotConnectClient.User.Search(new IoTConnect.Model.SearchUserModel { SearchText = "", PageNo = -1, PageSize = -1, SortBy = "" }).Result;
                    if (user != null && user.data != null && user.data.data.Any())
                    {
                        result = (from r in user.data.data.Where(t => t.IsActive) select new Entity.LookupItem() { Value = r.Guid, Text = r.Name }).ToList();
                    }
                    break;
                case "severitylevel":
                    result = new List<Entity.LookupItem>();
                    result.Add(new LookupItem() { Text = "Critical ", Value = "48C15691-F2EB-40BC-9BF2-0091821AE89B" });
                    result.Add(new LookupItem() { Text = "Information ", Value = "AB1D53A6-009C-4867-8E0E-EC34011EEBC0" });
                    result.Add(new LookupItem() { Text = "Major ", Value = "D6392057-8E35-428D-9281-EFD2BA3C6ED7" });
                    result.Add(new LookupItem() { Text = "Minor ", Value = "6E6D2DCD-E432-442D-9EAC-23CAE1F0CE03" });
                    result.Add(new LookupItem() { Text = "Warning ", Value = "704F4CA0-DB95-4F22-85D3-670F66DEEBA7" });
                    break;
                case "condition":
                    result = new List<Entity.LookupItem>();
                    result.Add(new LookupItem() { Text = "is equal to", Value = "=" });
                    result.Add(new LookupItem() { Text = "is not equal to", Value = "!" });
                    result.Add(new LookupItem() { Text = "is greater than", Value = ">" });
                    result.Add(new LookupItem() { Text = "is greater than or equal to", Value = ">=" });
                    result.Add(new LookupItem() { Text = "is less than", Value = "<" });
                    result.Add(new LookupItem() { Text = "is less than or equal to", Value = "<=" });
                    break;
                #endregion
                case "fleettype":
                    result = _fleetRepository.GetTypeLookup();
                    break;
                case "fleet":
                    result = _fleetRepository.GetLookup(SolutionConfiguration.CompanyId);
                    break;
                case "fleetmaterialtype":
                    result = _fleetRepository.GetMaterialTypeLookup();
                    break;
                case "device":
                    result = _deviceRepository.GetDeviceLookup();
                    break;
               
                case "templates":
                    var templates = _iotConnectClient.Template.All(new IoTConnect.Model.PagingModel() { PageNo = 1, PageSize = 1000 }).Result;
                    if (templates != null && templates.data != null && templates.data.Any())
                    {
                        result = (from r in templates.data.Where(t => (!string.IsNullOrWhiteSpace(t.Tag)) == false) select new Entity.LookupItem() { Value = r.Guid, Text = r.Name }).ToList();
                    }
                    break;
                case "devicetags":
                    if (string.IsNullOrWhiteSpace(param)) { throw new System.Exception("templateGuid is missing in request"); }
                    var tags = _iotConnectClient.Master.AllAttributeLookup(param).Result;
                    if (tags != null && tags.data != null && tags.data.Any())
                    {
                        result = (from r in tags.data select new Entity.LookupItem() { Value = r.guid, Text = r.localname }).ToList();
                    }
                    break;
                case "templatecommand":
                    var templatecommands = _iotConnectClient.Template.AllTemplateCommand(param, new IoTConnect.Model.PagingModel() { PageNo = 1, PageSize = 100000 }).Result;
                    if (templatecommands != null && templatecommands.data != null && templatecommands.data.Any())
                    {
                        result = (from r in templatecommands.data select new Entity.LookupItem() { Value = r.guid, Text = r.name }).ToList();
                    }
                    break;
                
                //case "location":
                //    if (string.IsNullOrWhiteSpace(param)) { throw new System.Exception("Companyid is missing in request"); }
                //    result = _entityRepository.GetLookup(System.Guid.Parse(param));
                //    break;
                case "type":
                    result = _deviceRepository.GetDeviceLookup();
                    break;
                case "role":
                    var roles = _iotConnectClient.Role.AllRoleLookup().Result;
                    if (roles != null && roles.data != null && roles.data.Any())
                    {
                        result = (from r in roles.data.Where(t => t.IsActive) select new Entity.LookupItem() { Value = r.Guid, Text = r.Name }).ToList();
                    }
                    break;
                case "country":
                    var countries = _iotConnectClient.Master.Countries().Result;
                    if (countries != null && countries.data != null && countries.data.Any())
                    {
                        result = (from r in countries.data select new Entity.LookupItem() { Value = r.guid.ToLower(), Text = r.name }).ToList();
                    }
                    break;
                case "state":
                    if (string.IsNullOrWhiteSpace(param)) { throw new System.Exception("CountryId is missing in request"); }
                    var states = _iotConnectClient.Master.States(param).Result;
                    if (states != null && states.data != null && states.data.Any())
                    {
                        result = (from r in states.data select new Entity.LookupItem() { Value = r.guid.ToLower(), Text = r.name }).ToList();
                    }
                    break;
                case "timezone":
                    var timeZones = _iotConnectClient.Master.TimeZones().Result;

                    if (timeZones != null && timeZones.data != null && timeZones.data.Any())
                    {
                        result = (from r in timeZones.data select new Entity.LookupItem() { Value = r.guid, Text = r.name }).ToList();
                    }
                    break;
                default:
                    result = new List<Entity.LookupItem>();
                    break;
            }
            return result;
        }
        public List<Entity.LookupItem> GetAllTemplate()
        {
            List<Entity.LookupItem> result = new List<Entity.LookupItem>();           
                result = (from template in _kitTypeRepository.FindBy(r => r.IsActive.HasValue && r.IsActive.Value && !r.IsDeleted)
                          join company in _companyRepository.FindBy(c => c.IsActive.HasValue && c.IsActive.Value && !c.IsDeleted) on template.CompanyGuid equals company.Guid
                          select new Entity.LookupItem()
                          {
                              Text = string.Format("{0} - {1}", company.Name, template.Name),
                              Value = template.Guid.ToString().ToUpper()
                          }).ToList();           
            return result;
        }
        public List<Entity.LookupItem> GetTemplate(bool isGateway)
        {
            List<Entity.LookupItem> result = new List<Entity.LookupItem>();

            result = (from t in _kitTypeRepository.FindBy(r => r.IsActive.HasValue && r.IsActive.Value && !r.IsDeleted)
                      select new Entity.LookupItem()
                      {
                          Text = t.Name,
                          Value = t.Guid.ToString().ToUpper()
                      }).ToList();

           
            return result;
        }
        public List<Entity.TagLookup> GetTagLookup(Guid templateId)
        {
            List<Entity.TagLookup> result = new List<Entity.TagLookup>();

            var template = _kitTypeRepository.FindBy(t => t.Guid == templateId).FirstOrDefault();
            if (template != null)
            {
                result.Add(new Entity.TagLookup() { tag = template.Tag, templateTag = true });

                result.AddRange(from t in _kitTypeAttributeRepository.FindBy(t => t.TemplateGuid == templateId)
                                select new Entity.TagLookup()
                                {
                                    tag = t.LocalName,
                                    templateTag = false
                                });
            }
           
            return result;
        }
        public List<Entity.LookupItem> GetSensors(Guid deviceId)
        {
            List<Entity.LookupItem> result = new List<Entity.LookupItem>();
            try
            {
                var template = _kitTypeRepository.FindBy(t => !t.IsDeleted).FirstOrDefault();//.Name == SolutionConfiguration.DefaultIoTTemplateName
                if (template != null)
                {
                    var childAttribute = (from child in _kitTypeAttributeRepository.FindBy(t => t.TemplateGuid == template.Guid && t.ParentTemplateAttributeGuid != null)
                                          select child.ParentTemplateAttributeGuid).ToList();


                    result.AddRange(from device in _deviceRepository.FindBy(t => t.Guid == deviceId)
                                    join attribute in _kitTypeAttributeRepository.FindBy(t => t.TemplateGuid == template.Guid) on 1 equals 1
                                    where !childAttribute.Contains(attribute.Guid)
                                    select new Entity.LookupItem()
                                    {
                                        Text = string.Format("{0}", attribute.LocalName),
                                        Value = attribute.Guid.ToString()
                                    });
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }
        public List<Entity.LookupItemWithDescription> GetTemplateAttribute(Guid templateId)
        {
            List<Entity.LookupItemWithDescription> result = new List<Entity.LookupItemWithDescription>();
            try
            {
                var template = _kitTypeRepository.FindBy(t => t.Guid == templateId).FirstOrDefault();
                if (template != null)
                {
                  
                    result.AddRange(from t in _kitTypeAttributeRepository.FindBy(t => t.TemplateGuid == templateId).ToList()
                                    select new Entity.LookupItemWithDescription()
                                    {
                                        Text = t.LocalName,//string.Format("{0}({1})", t.LocalName, t.Tag),
                                        Value = t.Guid.ToString().ToUpper(), //string.Format("{0}({1})", t.LocalName, t.Tag)
                                        Description=t.Description.ToString()
                                    });
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }
        public List<Entity.LookupItem> GetTemplateCommands(Guid templateId)
        {
            List<Entity.LookupItem> result = new List<Entity.LookupItem>();
            try
            {
                var template = _kitTypeRepository.FindBy(t => t.Guid == templateId).FirstOrDefault();
                if (template != null)
                {
                    result = (from t in _kitTypeCommandRepository.GetAll()
                              select new Entity.LookupItem()
                              {
                                  Text = t.Name,
                                  Value = t.Guid.ToString().ToUpper()
                              }).ToList();
                }
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }
        public List<Entity.LookupItem> GetAllTemplateFromIoT()
        {
            List<Entity.LookupItem> result = new List<Entity.LookupItem>();
            try
            {
                var templateList = _iotConnectClient.Template.All(new IoTConnect.Model.PagingModel()
                {
                    PageNo = 1,
                    PageSize = 50,
                    SearchText = "",
                    SortBy = ""
                }).Result.data;

                result = templateList.Select(x => new Entity.LookupItem()
                {
                    Text = x.Name,
                    Value = x.Guid.ToString().ToUpper()
                }).ToList();

            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }


        public List<Entity.KitTypeAttribute> GetAllAttributesFromIoT(string templateGuid)
        {
            List<Entity.KitTypeAttribute> result = new List<Entity.KitTypeAttribute>();
            try
            {
                List<IoTConnect.Model.AttributeResult> attributeList = _iotConnectClient.Template.AllAttribute(templateGuid, new IoTConnect.Model.PagingModel() { }, "").Result.data;

                result = attributeList.Select(x => new Entity.KitTypeAttribute()
                {
                    LocalName = x.localName,
                    Guid = new Guid(x.guid),
                    Tag = x.tag !=null ? x.tag.ToString() : string.Empty
                }).ToList();

            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }

            return result;
        }


        public List<Entity.LookupItem> GetAllCommandsFromIoT(string templateGuid)
        {
            List<Entity.LookupItem> result = new List<Entity.LookupItem>();
            try
            {
                List<IoTConnect.Model.AllCommandResult> attributeList = _iotConnectClient.Template.AllTemplateCommand(templateGuid, new IoTConnect.Model.PagingModel() { }).Result.data;

                return attributeList.Select(x => new Entity.LookupItem()
                {
                    Text = x.name,
                    Value = x.guid.ToUpper()
                }).ToList();

            }
            catch (Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return result;
        }

        public List<Entity.LookupItemWithStatus> FacilityLookup(Guid companyId)
        {
            List<Entity.LookupItemWithStatus> result = new List<Entity.LookupItemWithStatus>();
            var template = _companyRepository.FindBy(t => t.Guid == companyId).FirstOrDefault();
            if (template != null)
            {
                result = (from t in _entityRepository.GetAll()
                          where t.CompanyGuid.Equals(companyId) && !t.Guid.Equals(component.helper.SolutionConfiguration.EntityGuid) &&  t.ParentEntityGuid == null  && !t.IsDeleted
                          select new Entity.LookupItemWithStatus()
                          {
                              Text = t.Name,
                              Value = t.Guid.ToString(),
                              IsActive=t.IsActive
                          }).ToList();
            }
            return result;
        }
        public List<Entity.LookupItemWithStatus> GetDeviceByTemplateLookup(Guid templateGuid,Guid? deviceGuid)
        {
           // var d = _deviceRepository.GetAll().Where(t=>t.CompanyGuid.ToString().ToUpper().Equals(SolutionConfiguration.CompanyId.ToString().ToUpper())).ToList();
            var devices = _deviceRepository.GetAll().Where(t => t.CompanyGuid.ToString().ToUpper().Equals(SolutionConfiguration.CompanyId.ToString().ToUpper()) && 
            (t.Guid.Equals(deviceGuid) || !t.FleetGuid.HasValue) && t.TemplateGuid.ToString().ToUpper().Equals(templateGuid.ToString().ToUpper()) && !t.IsDeleted)
                .Select(g => new Entity.LookupItemWithStatus()
            {
                Value = g.Guid.ToString(),
                Text = g.UniqueId,
                IsActive = g.IsActive.HasValue && g.IsActive.Value
            }).ToList();
            return devices;
        }
        public List<Entity.LookupItem> ZoneLookup(Guid entityId)
        {
            List<Entity.LookupItem> result = new List<Entity.LookupItem>();
            var template = _entityRepository.FindBy(t => t.Guid == entityId).FirstOrDefault();
            if (template != null)
            {
                result = (from t in _entityRepository.GetAll()
                          where t.CompanyGuid.Equals(SolutionConfiguration.CompanyId) && t.ParentEntityGuid.Equals(entityId) && !t.IsDeleted 
                          select new Entity.LookupItem()
                          {
                              Text = t.Name,
                              Value = t.Guid.ToString()
                          }).ToList();
            }
            return result;
        }

        public List<Entity.LookupItem> GetDriverFleetLookup(Guid? fleetId)
        {
            List<Entity.LookupItem> result = new List<Entity.LookupItem>();
            List<Guid> lstGuid = new List<Guid>();
            if(fleetId == null || fleetId == Guid.Empty)
            {
                lstGuid = (from b in _driverRepository.GetAll() where !b.IsDeleted select b.FleetGuid).ToList();
            }
            else
            {
                lstGuid = (from b in _driverRepository.GetAll() where b.FleetGuid !=fleetId && !b.IsDeleted select b.FleetGuid).ToList();
            }
            result = (from t in _fleetRepository.GetAll()
                          where t.CompanyGuid.Equals(SolutionConfiguration.CompanyId) && !t.IsDeleted && !lstGuid.Contains(t.Guid)
                      select new Entity.LookupItem()
                          {
                              Text = t.FleetId,
                              Value = t.Guid.ToString()
                          }).ToList();
           
            return result;
        }
        public List<Entity.LookupItemWithTemplateGuid> DeviceTypeLookup(Guid entityId)
        {
            List<Entity.LookupItemWithTemplateGuid> result = new List<Entity.LookupItemWithTemplateGuid>();
            var template = _entityRepository.FindBy(t => t.Guid == entityId).FirstOrDefault();
            if (template != null)
            {
                result = (from t in _deviceTypeRepository.GetAll()
                          where t.CompanyGuid.Equals(SolutionConfiguration.CompanyId) && t.EntityGuid.Equals(entityId) && !t.IsDeleted
                          select new Entity.LookupItemWithTemplateGuid()
                          {
                              Text = t.Name,
                              Value = t.Guid.ToString(),
                              TemplateGuid= t.TemplateGuid.ToString()
                          }).ToList();
            }
            return result;
        }
        public List<Entity.LookupItemWithStatus> DeviceLookup(Guid subentityId)
        {
            List<Entity.LookupItemWithStatus> result = new List<Entity.LookupItemWithStatus>();
            var template = _entityRepository.FindBy(t => t.Guid == subentityId).FirstOrDefault();
            if (template != null)
            {
                result = (from t in _deviceRepository.GetAll()
                          where t.CompanyGuid.Equals(SolutionConfiguration.CompanyId) && t.EntityGuid.Equals(subentityId)  && !t.IsDeleted
                          select new Entity.LookupItemWithStatus()
                          {
                              Text = t.UniqueId,
                              Value = t.Guid.ToString(),
                              IsActive=t.IsActive
                          }).ToList();
            }
            return result;
        }
        public Entity.BaseResponse<List<Entity.AttributeItem>> DeviceAttributeLookup(Guid deviceId)
        {
            Entity.BaseResponse<List<Entity.AttributeItem>> result = new Entity.BaseResponse<List<Entity.AttributeItem>>();
            try
            {
                _logger.InfoLog(Constants.ACTION_ENTRY, "Device_LatestAttributeValue_Get.Get");
                using (var sqlDataAccess = new SqlDataAccess(ConnectionString))
                {
                    List<DbParameter> parameters = sqlDataAccess.CreateParams(component.helper.SolutionConfiguration.CurrentUserId, component.helper.SolutionConfiguration.Version);
                    parameters.Add(sqlDataAccess.CreateParameter("guid", deviceId, DbType.Guid, ParameterDirection.Input));
                    parameters.Add(sqlDataAccess.CreateParameter("syncDate", DateTime.UtcNow, DbType.DateTime, ParameterDirection.Output));

                    DbDataReader dbDataReader = sqlDataAccess.ExecuteReader(sqlDataAccess.CreateCommand("[Device_LatestAttributeValue_Get]", CommandType.StoredProcedure, null), parameters.ToArray());
                    result.Data = DataUtils.DataReaderToList<Entity.AttributeItem>(dbDataReader, null);
                    if (parameters.Where(p => p.ParameterName.Equals("syncDate")).FirstOrDefault() != null)
                    {
                        result.LastSyncDate = Convert.ToString(parameters.Where(p => p.ParameterName.Equals("syncDate")).FirstOrDefault().Value);
                    }
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