using component.logger.data.log.Model;
using component.logger.data.log.Repositories;
using component.logger.platform.common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;

namespace component.logger.platform.common
{
    public class LogConfigurations
    {
        #region Private Variable
        private static LogConfigurations instance;
        private static List<ComponentConfiguration> componentConfigurationList;
        private static List<LogConfiguration> listLogConfiguration;
        private static List<Component> listLogComponent;
        static readonly Object _lock = new Object();
        static readonly Object _lockConfig = new Object();
        static readonly Object _lockConfigObject = new Object();
        static readonly Object _lockLogConfig = new Object();
        static readonly Object _lockLogComponent = new Object();
        #endregion


        #region Singleton Implementation

        private LogConfigurations()
        {
            componentConfigurationList = new List<ComponentConfiguration>();
            listLogConfiguration = new List<LogConfiguration>();
            listLogComponent = new List<Component>();
        }

        public static LogConfigurations Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (_lock)
                    {
                        if (instance == null)
                        {
                            instance = new LogConfigurations();
                        }
                    }
                }
                return instance;
            }
        }

        public ComponentConfiguration AddComponentConfiguration(string applicationCode, string logger)
        {
            lock (_lockConfig)
            {
                LogManagementRepository logManagementRepository = new LogManagementRepository();
                bool isSucess = logManagementRepository.AddComponentConfiguration(applicationCode, logger);
                return GetConfiguration(applicationCode, logger);
            }
        }

        public ComponentConfiguration GetConfiguration(string applicationCode, string logger)
        {
            lock (_lockConfig)
            {
                if (applicationCode == null)
                    applicationCode = string.Empty;

                if (componentConfigurationList.FirstOrDefault(x => string.Equals(x.ApplicationCode, applicationCode, StringComparison.CurrentCulture)) == null)
                {
                    LogManagementRepository logManagementRepository = new LogManagementRepository();
                    DataResponse<List<ComponentConfiguration>> dataResponse = logManagementRepository.LogComponentList(applicationCode);
                    lock (_lockConfigObject)
                    {
                        if (dataResponse != null && dataResponse.Data != null && dataResponse.Data.Count > 0)
                        {
                            componentConfigurationList.AddRange(dataResponse.Data);
                            return componentConfigurationList.FirstOrDefault(x => x.Component.Name.Equals(logger, StringComparison.CurrentCultureIgnoreCase) && string.Equals(x.ApplicationCode, applicationCode, StringComparison.CurrentCulture));
                        }
                        else if ((componentConfigurationList == null && componentConfigurationList.Count == 0) || !componentConfigurationList.Exists(x => x.ApplicationCode == string.Empty))
                        {
                            DataResponse<List<ComponentConfiguration>> dataResponseEmptyApp = logManagementRepository.LogComponentList(string.Empty);
                            if (dataResponseEmptyApp != null && dataResponseEmptyApp.Data != null)
                                componentConfigurationList.AddRange(dataResponseEmptyApp.Data);
                        }
                    }
                }
            }
            return componentConfigurationList.FirstOrDefault(x => x.Component.Name.Equals(logger, StringComparison.CurrentCultureIgnoreCase) && x.ApplicationCode == applicationCode);
        }

        public LogConfiguration GetLogSeverity(LogSeverity logSeverity)
        {
            lock (_lockLogConfig)
            {
                if (listLogConfiguration == null || listLogConfiguration.Count == 0)
                {
                    LogManagementRepository logManagementRepository = new LogManagementRepository();
                    DataResponse<List<LogConfiguration>> dataResponse = logManagementRepository.GetSeverityList();
                    listLogConfiguration = dataResponse.Data;
                }
            }
            return listLogConfiguration.FirstOrDefault(x => x.Type.Equals(Convert.ToString(logSeverity), StringComparison.CurrentCultureIgnoreCase));
        }

        public Component GetLogComponents(string logComponent)
        {
            lock (_lockLogComponent)
            {
                if (listLogComponent == null || listLogComponent.Count == 0)
                {
                    LogManagementRepository logManagementRepository = new LogManagementRepository();
                    DataResponse<List<Component>> dataResponse = logManagementRepository.ComponentList();
                    listLogComponent = dataResponse.Data;
                }
            }
            return listLogComponent.FirstOrDefault(x => x.Name.Equals(Convert.ToString(logComponent), StringComparison.CurrentCultureIgnoreCase));
        }


        #endregion
    }
}
