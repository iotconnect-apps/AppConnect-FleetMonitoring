using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Serialization;

namespace component.helper
{
    public static class SolutionConfiguration
    {
        public static void Init(string contentRootPath)
        {
            var xmlDoc = new XmlDocument();
            xmlDoc.Load(string.Format(@"{0}\{1}", contentRootPath, "Configuration.xml"));

            using (var stringReader = new System.IO.StringReader(xmlDoc.OuterXml))
            {
                var serializer = new XmlSerializer(typeof(Configuration));
                Configuration = serializer.Deserialize(stringReader) as Configuration;
            }
        }
        public static Configuration Configuration { get; set; }
        public static Guid CurrentUserId { get; set; }
        public static Guid SolutionId { get; set; }
        public static Guid CompanyId { get; set; }
        public static Guid EntityGuid { get; set; }
        public static string BearerToken { get; set; }
        public static string Culture { get { return "en-Us"; } }
        public static char EnableDebugInfo { get { return '0'; } }
        public static string Version { get; set; } = "v1";
        public static string UploadBasePath { get; set; } = "wwwroot/";
        public static bool isTitleCase { get; set; } = true;
        public static string CompanyFilePath { get; set; } = "CompanyFiles/EntityImages/";
        public static string FleetFilePath { get; set; } = "CompanyFiles/FleetFiles/";
        public static string ShipmentFilePath { get; set; } = "CompanyFiles/ShipmentFiles/";
        public static string DeviceFilePath { get; set; } = "CompanyFiles/DeviceFiles/";
        public static string DriverImageFilePath { get; set; } = "CompanyFiles/DriverImageFiles/";
        public static string DriverLicenceFilePath { get; set; } = "CompanyFiles/DriverLicenceFiles/";
        public static string EmailTemplatePath { get; set; } = "EmailTemplate/";
        public static string[] AllowedImages { get; set; } = new string[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".tiff" };
        public static string[] AllowedDocs { get; set; } = new string[] { ".doc", ".docx", ".ppt", ".pptx", ".xls", ".xlsx", ".pdf", ".txt" };
    }

    public class Configuration
    {
        public string ConnectionString { get; set; }
        public string SolutionName { get; set; }
        public string SolutionKey { get; set; }
        public string EnvironmentCode { get; set; }
        public SubscriptionAPISetting SubscriptionAPI { get; set; }
        public Messaging Messaging { get; set; }
        public TokenSetting Token { get; set; }
        public LoggerSetting Logger { get; set; }
        public HangFireSetting HangFire { get; set; }
        public List<IOTConnectSetting> IOTConnectSettings { get; set; }
        public EmailTemplateSettings EmailTemplateSettings { get; set; }
        public SmtpSetting SmtpSetting { get; set; }

    }

    public class SmtpSetting
    {
        public string Host { get; set; }
        public int Port { get; set; }
        public string UserName { get; set; }
        public string FromDisplayName { get; set; }
        public string Password { get; set; }
        public string Regards { get; set; }
    }
    public class EmailTemplateSettings
    {
        public string FleetDelaySubject { get; set; }
        public string UserList { get; set; }
        public string FleetRadiusSubject { get; set; }
        public string RadiusUserList { get; set; }
        public string FleetRadiusDurationMinutes { get; set; }
        public string CompanyRegistrationSubject { get; set; }
        public string SubscriptionExpirySubject { get; set; }
        public string SubscriptionExpiryUserList { get; set; }
        public string CompanyUserList { get; set; }
    }
    public class IOTConnectSetting
    {
        public string SettingType { get; set; }
        public string Host { get; set; }
        public int IsSecure { get; set; }
        public string Password { get; set; }
        public int Port { get; set; }
        public string Url { get; set; }
        public string User { get; set; }
        public string Vhost { get; set; }
    }

    public class SubscriptionAPISetting
    {
        public string BaseUrl { get; set; }
        public string ClientID { get; set; }
        public string ClientSecret { get; set; }
        public string SolutionCode { get; set; }
        public string SolutionId { get; set; }
        public string UserName { get; set; }
        public string StripeAPIKey { get; set; }
    }

    public class Messaging
    {
        public string ServicebusEndPoint { get; set; }
        public string TopicName { get; set; }
        public string SubscriptionName { get; set; }
       
    }

    public class TokenSetting
    {
        public string Issuer { get; set; }
        public string Audience { get; set; }
        public string SecurityKey { get; set; }
        public string Authority { get; set; }
        public string ApiName { get; set; }
        public string ApiSecret { get; set; }
        public bool EnableCaching { get; set; }
        public int CacheDurationMinutes { get; set; }
        public bool RequireHttpsMetadata { get; set; }
        public string OAuthClientID { get; set; }
        public string OAuthRealm { get; set; }
    }

    public class LoggerSetting
    {
        public string BrokerConnection { get; set; }
        public string SolutionName { get; set; }
    }
    public class HangFireSetting
    {
        public bool Enabled { get; set; }
        public int TelemetryHours { get; set; }
        public int ShelfConsumptionMinutes { get; set; }
        public int DelayedMinutes { get; set; }
        public bool IsSendTripMailEnabled { get; set; }
        public bool IsSendRadiusMailEnabled { get; set; }
    }
}
