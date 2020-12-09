using component.logger;
using component.messaging.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection;
using LogHandler = component.services.loghandler;

namespace component.messaging.Database
{
    public class DatabaseManager : IDatabaseManager
    {
        private readonly string _connectionString;
        private readonly LogHandler.Logger _logger;

        public DatabaseManager(string connectionString, LogHandler.Logger logger)
        {
            _connectionString = connectionString;
            _logger = logger;
        }

        public void CompanyProcessMessage(MessageModel subscribeData)
        {
            try
            {
                var xmlCompanyData = Convert.ToString(JsonConvert.DeserializeXNode(JsonConvert.SerializeObject(subscribeData.Data), "items"));

                var baseDataAccess = new BaseDataAccess(_connectionString);
                var sqlParameters = new List<SqlParameter>()
                {
                   baseDataAccess.AddInParameter("ComapnyXml", xmlCompanyData),
                   baseDataAccess.AddInParameter("action", subscribeData.Action),
                   baseDataAccess.AddInParameter("companyGuid", subscribeData.Company)
                };
                baseDataAccess.Execute("IotConnect_ManageCompany", sqlParameters);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(new Exception($"Error in sync iotconnect company data : {ex.Message}, StackTrace : {ex.StackTrace}, Data : {JsonConvert.SerializeObject(subscribeData)}"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
        }

        public void TemplateProcessMessage(MessageModel subscribeData)
        {
            try
            {
                var xmlTemplateData = Convert.ToString(JsonConvert.DeserializeXNode(JsonConvert.SerializeObject(subscribeData.Data), "items"));
                var baseDataAccess = new BaseDataAccess(_connectionString);
                var sqlParameters = new List<SqlParameter>
                {
                    baseDataAccess.AddInParameter("DeviceTemplateXml", xmlTemplateData),
                    baseDataAccess.AddInParameter("action", subscribeData.Action),
                    baseDataAccess.AddInParameter("companyGuid", subscribeData.Company)
                };
                baseDataAccess.Execute("IotConnect_ManageDeviceTemplate ", sqlParameters);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(new Exception($"Error in sync iotconnect template data : {ex.Message}, StackTrace : {ex.StackTrace}, Data : {JsonConvert.SerializeObject(subscribeData)}"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
        }
        public void EntityProcessMessage(MessageModel subscribeData)
        {
            try
            {
                
                var xmlEntityData = Convert.ToString(JsonConvert.DeserializeXNode(JsonConvert.SerializeObject(subscribeData.Data), "items"));
                var baseDataAccess = new BaseDataAccess(_connectionString);
                var sqlParameters = new List<SqlParameter>
                {
                    baseDataAccess.AddInParameter("EntityXml", xmlEntityData),
                    baseDataAccess.AddInParameter("action", subscribeData.Action),
                    baseDataAccess.AddInParameter("companyGuid", subscribeData.Company)
                };
                baseDataAccess.Execute("IotConnect_ManageEntity", sqlParameters);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(new Exception($"Error in sync iotconnect entity data : {ex.Message}, StackTrace : {ex.StackTrace}, Data : {JsonConvert.SerializeObject(subscribeData)}"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
        }

        public void UserProcessMessage(MessageModel subscribeData)
        {
            try
            {
                var xmlUserData = Convert.ToString(JsonConvert.DeserializeXNode(JsonConvert.SerializeObject(subscribeData.Data), "items"));
                var baseDataAccess = new BaseDataAccess(_connectionString);
                var sqlParameters = new List<SqlParameter>
                {
                    baseDataAccess.AddInParameter("UserXml", xmlUserData),
                    baseDataAccess.AddInParameter("action", subscribeData.Action),
                    baseDataAccess.AddInParameter("companyGuid", subscribeData.Company)
                };
                baseDataAccess.Execute("IotConnect_ManageUser ", sqlParameters);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(new Exception($"Error in sync iotconnect user data : {ex.Message}, StackTrace : {ex.StackTrace}, Data : {JsonConvert.SerializeObject(subscribeData)}"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
        }

        public void RoleProcessMessage(MessageModel subscribeData)
        {
            try
            {
                var xmlRoleData = Convert.ToString(JsonConvert.DeserializeXNode(JsonConvert.SerializeObject(subscribeData.Data), "items"));
                var baseDataAccess = new BaseDataAccess(_connectionString);
                var sqlParameters = new List<SqlParameter>
                {
                    baseDataAccess.AddInParameter("RoleXml", xmlRoleData),
                    baseDataAccess.AddInParameter("action", subscribeData.Action),
                    baseDataAccess.AddInParameter("companyGuid", subscribeData.Company)
                };
                baseDataAccess.Execute("IotConnect_ManageRole", sqlParameters);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(new Exception($"Error in sync iotconnect role data : {ex.Message}, StackTrace : {ex.StackTrace}, Data : {JsonConvert.SerializeObject(subscribeData)}"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
        }

        public void DeviceProcessMessage(MessageModel subscribeData)
        {
            try
            {
                var xmlDeviceData = Convert.ToString(JsonConvert.DeserializeXNode(JsonConvert.SerializeObject(subscribeData.Data)));
                var baseDataAccess = new BaseDataAccess(_connectionString);
                var sqlParameters = new List<SqlParameter>
                {
                    baseDataAccess.AddInParameter("DeviceXml", xmlDeviceData),
                    baseDataAccess.AddInParameter("action", subscribeData.Action),
                    baseDataAccess.AddInParameter("companyGuid", subscribeData.Company)
                };
                baseDataAccess.Execute("IotConnect_ManageDevice", sqlParameters);
            }
            catch (Exception ex)
            {
                _logger.ErrorLog(new Exception($"Error in sync iotconnect device data : {ex.Message}, StackTrace : {ex.StackTrace}, Data : {JsonConvert.SerializeObject(subscribeData)}"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
        }
    }
}
