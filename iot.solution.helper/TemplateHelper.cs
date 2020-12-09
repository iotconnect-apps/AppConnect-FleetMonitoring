using component.helper.Interface;
using component.logger;
using IdentityModel.Client;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using LogHandler = component.services.loghandler;
using System.Reflection;
using System;
using System.Linq;

namespace component.helper
{
    public static class TemplateHelper
    {
        private readonly static IotConnectClient _iotConnectClient;
        public static List<IoTConnect.Model.AttributeResult> attributeList;
        static TemplateHelper()
        {
            _iotConnectClient = new IotConnectClient(SolutionConfiguration.BearerToken, SolutionConfiguration.Configuration.EnvironmentCode, SolutionConfiguration.Configuration.SolutionKey);
        }


        public static Guid GetTemplateDetailsFromIoT(string templateName)
        {
            var result = Guid.Empty;
            try
            {
                var result1 = _iotConnectClient.Template.All(new IoTConnect.Model.PagingModel()
                {
                    PageNo = 1,
                    PageSize = 50,
                    SearchText = templateName,
                    SortBy = ""
                }).Result;

                attributeList = _iotConnectClient.Template.AllAttribute(result1.data.FirstOrDefault().Guid, new IoTConnect.Model.PagingModel() { }, "").Result.data;
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return result;
        }

        public static Guid GetAttributeGuidFromName(string attributeName)
        {
            string attributeGuid = string.Empty;
            try
            {
                attributeGuid = attributeList.Where(x=>x.localName.Equals(attributeName)).FirstOrDefault().guid.ToUpper();
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return new Guid(attributeGuid);
        }        
    }
}
