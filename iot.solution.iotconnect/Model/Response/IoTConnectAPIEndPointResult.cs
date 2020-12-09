using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace IoTConnect.Model
{
    public class IoTConnectAPIEndPointResult
    {
        [JsonProperty("solutionKey")]
        public string SolutionKey { get; set; }

        [JsonProperty("environment")]
        public string Environment { get; set; }

        [JsonProperty("data")]
        public IoTConnectEndpoint IoTConnectEndpoints { get; set; }
    }

    public class ResponseIoTConnectAPIEndpoint
    {
        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("data")]
        public IoTConnectEndpoint IoTConnectEndpoints { get; set; }
    }

    public class IoTConnectEndpoint
    {
        [JsonProperty("authBaseUrl")]
        public string AuthBaseUrl { get; set; }

        [JsonProperty("userBaseUrl")]
        public string UserBaseUrl { get; set; }

        [JsonProperty("eventBaseUrl")]
        public string EventBaseUrl { get; set; }

        [JsonProperty("masterBaseUrl")]
        public string MasterBaseUrl { get; set; }

        [JsonProperty("faqBaseUrl")]
        public string FaqBaseUrl { get; set; }

        [JsonProperty("propertyBaseUrl")]
        public string PropertyBaseUrl { get; set; }

        [JsonProperty("dashboardBaseUrl")]
        public string DashboardBaseUrl { get; set; }

        [JsonProperty("templateBaseUrl")]
        public string TemplateBaseUrl { get; set; }

        [JsonProperty("companyBaseUrl")]
        public string CompanyBaseUrl { get; set; }

        [JsonProperty("templateSettingBaseUrl")]
        public string TemplateSettingBaseUrl { get; set; }

        [JsonProperty("templateAttributeBaseUrl")]
        public string TemplateAttributeBaseUrl { get; set; }

        [JsonProperty("roleBaseUrl")]
        public string RoleBaseUrl { get; set; }

        [JsonProperty("entityBaseUrl")]
        public string EntityBaseUrl { get; set; }

        [JsonProperty("authorizeBaseUrl")]
        public string AuthorizeBaseUrl { get; set; }

        [JsonProperty("deviceBaseUrl")]
        public string DeviceBaseUrl { get; set; }

        [JsonProperty("certificateBaseUrl")]
        public string CertificateBaseUrl { get; set; }

        [JsonProperty("firmwareBaseUrl")]
        public string FirmwareBaseUrl { get; set; }

        [JsonProperty("dpsBaseUrl")]
        public string DpsBaseUrl { get; set; }

        [JsonProperty("solutionBaseUrl")]
        public string SolutionBaseUrl { get; set; }

        [JsonProperty("telemetryBaseUrl")]
        public string TelemetryBaseUrl { get; set; }

        [JsonProperty("templateCommandBaseUrl")]
        public string TemplateCommandBaseUrl { get; set; }

        [JsonProperty("firmwareOTAUpdateBaseUrl")]
        public string FirmwareOTAUpdateBaseUrl { get; set; }

        [JsonProperty("firmwareUpgradeBaseUrl")]
        public string FirmwareUpgradeBaseUrl { get; set; }

        [JsonProperty("ruleBaseUrl")]
        public string RuleBaseUrl { get; set; }

        [JsonProperty("configBaseUrl")]
        public string ConfigBaseUrl { get; set; }

        [JsonProperty("agentBaseUrl")]
        public string AgentBaseUrl { get; set; }

        [JsonProperty("simulatorBaseUrl")]
        public string SimulatorBaseUrl { get; set; }

        [JsonProperty("fileBaseUrl")]
        public string FileBaseUrl { get; set; }
    }
}
