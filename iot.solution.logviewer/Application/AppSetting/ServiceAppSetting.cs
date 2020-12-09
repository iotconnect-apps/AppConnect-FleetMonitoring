using component.logger.data.log.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using iot.solution.common;

namespace component.services.logger.viewer.Application.AppSetting
{
    public class ServiceAppSetting
    {
        readonly IAppSettingRepository _appSettingRepository;
        private static List<component.logger.data.log.Model.AppSetting> _appSettings { get; set; }

        private static ServiceAppSetting instance = null;
        private static readonly object _lock = new object();

        private ServiceAppSetting()
        {
        }
        public static ServiceAppSetting Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (_lock)
                    {
                        Object _object;

                        // create the instance only if the instance is null
                        if (instance == null)
                        {
                            instance = new ServiceAppSetting();
                        }
                    }
                }
                // Otherwise return the already existing instance
                return instance;
            }
        }
        public ServiceAppSetting(IAppSettingRepository appSettingRepository)
        {
            _appSettingRepository = appSettingRepository ?? throw new ArgumentNullException(nameof(appSettingRepository));
        }
        public void GetDefaultServiceAppSettings()
        {
            try
            {
                List<component.logger.data.log.Model.AppSetting> appSettingList = new List<component.logger.data.log.Model.AppSetting>();

                System.Threading.Tasks.Task<List<component.logger.data.log.Model.AppSetting>> getTask = _appSettingRepository.GetAll();
                appSettingList = getTask.Result;

                if (appSettingList != null && appSettingList.Count > 0)
                {
                    _appSettings = new List<component.logger.data.log.Model.AppSetting>();
                    _appSettings.AddRange(appSettingList);
                }
            }
            catch
            {
            }
        }
        public string GetAppSettingByKey(string appSettingKey)
        {
            if (_appSettings != null && _appSettings.Count > 0)
                return _appSettings.Where(x => x.Key.ToLower() == appSettingKey.ToLower()).Select(x => x.Value).FirstOrDefault();
            else
                return string.Empty;
        }
        public string GetRequiredAppSettingByKey(string appSettingKey)
        {
            if (_appSettings?.Any() == true)
                return _appSettings.Where(x => string.Equals(x.Key, appSettingKey, StringComparison.OrdinalIgnoreCase)).Select(x => x.Value).FirstOrDefault() ?? General.RaiseConfigurationMissingException(appSettingKey);
            else
                return General.RaiseConfigurationMissingException(appSettingKey);
        }
    }
}
