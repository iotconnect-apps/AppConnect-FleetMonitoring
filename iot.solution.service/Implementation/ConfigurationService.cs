using iot.solution.entity.Response;
using iot.solution.model.Repository.Interface;
using iot.solution.service.Interface;
using System;
using System.Linq;

using component.helper;

namespace iot.solution.service.Implementation
{
    public class ConfigurationService : IConfigurationService
    {
        private readonly ICompanyRepository _companyRepository;
        public ConfigurationService(ICompanyRepository companyRepository)
        {
            _companyRepository = companyRepository;
        }
        public ConfgurationResponse GetConfguration(string key)
        {
            var companyDetail = _companyRepository.GetByUniqueId(r => r.Guid == SolutionConfiguration.CompanyId);
            ConfgurationResponse confgurationResponse = new ConfgurationResponse();
            var setting = SolutionConfiguration.Configuration.IOTConnectSettings.Where(s => s.SettingType.Equals(key, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefault();
            if (setting != null && companyDetail != null)
            {
                confgurationResponse = new ConfgurationResponse()
                {
                    cpId = companyDetail.CpId,
                    host = setting.Host,
                    isSecure = setting.IsSecure,
                    password = setting.Password,
                    port = setting.Port,
                    url = setting.Url,
                    user = setting.User,
                    vhost = setting.Vhost,
                };
            }
            return confgurationResponse;
        }
    }
}
