using component.logger.data.log.Model;
using component.logger.data.log.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace component.logger.platform.common
{
    /// <summary>
    /// 
    /// </summary>
    public class AzureLogAnalytics
    {
        /// <summary>
        /// Gets or sets the workspace identifier.
        /// </summary>
        /// <value>
        /// The workspace identifier.
        /// </value>
        private static String WorkspaceId { get; set; }

        /// <summary>
        /// Gets or sets the shared key.
        /// </summary>
        /// <value>
        /// The shared key.
        /// </value>
        private static String SharedKey { get; set; }

        /// <summary>
        /// Gets or sets the API version.
        /// </summary>
        /// <value>
        /// The API version.
        /// </value>
        private static String ApiVersion { get; set; }

        /// <summary>
        /// Gets or sets the type of the log.
        /// </summary>
        /// <value>
        /// The type of the log.
        /// </value>
        private static String LogType { get; set; }

        /// <summary>
        /// The application settings
        /// </summary>
        private static List<AppSetting> appSettings;

        /// <summary>
        /// The instance
        /// </summary>
        private static AzureLogAnalytics instance;

        /// <summary>
        /// The lock configuration
        /// </summary>
        static readonly Object _lockConfig = new Object();

        /// <summary>
        /// The lock
        /// </summary>
        static readonly Object _lock = new Object();

        /// <summary>
        /// Prevents a default instance of the <see cref="AzureLogAnalytics"/> class from being created.
        /// </summary>
        private AzureLogAnalytics()
        {
            appSettings = new List<AppSetting>();
            SetConfig();
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        /// <value>
        /// The instance.
        /// </value>
        public static AzureLogAnalytics Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (_lock)
                    {
                        if (instance == null)
                        {
                            instance = new AzureLogAnalytics();
                        }
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// Sets the configuration.
        /// </summary>
        /// <exception cref="System.Exception">Configuration Mission</exception>
        private void SetConfig()
        {
            WorkspaceId = GetApplicationKey("OMSWorkSpaceId");
            SharedKey = GetApplicationKey("OMSSharedKey");
            ApiVersion = GetApplicationKey("OMSApiVersion");

            if (string.IsNullOrWhiteSpace(WorkspaceId) || string.IsNullOrWhiteSpace(SharedKey))
                throw new Exception("Configuration Mission");

            ApiVersion = string.IsNullOrWhiteSpace(ApiVersion) ? "2016-04-01" : ApiVersion;
        }

        /// <summary>
        /// Errors the specified entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">The entity.</param>
        /// <exception cref="System.Exception"></exception>
        public void Error<T>(T entity)
        {
            string json = JsonConvert.SerializeObject(entity);
            string requestUriString = $"https://{WorkspaceId}.ods.opinsights.azure.com/api/logs?api-version={ApiVersion}";
            DateTime dateTime = DateTime.UtcNow;
            string dateString = dateTime.ToString("r");
            string signature = GetSignature("POST", json.Length, "application/json", dateString, "/api/logs");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUriString);
            request.ContentType = "application/json";
            request.Method = "POST";
            request.Headers["Log-Type"] = "Error";
            request.Headers["x-ms-date"] = dateString;
            request.Headers["Authorization"] = signature;
            byte[] content = Encoding.UTF8.GetBytes(json);
            using (Stream requestStreamAsync = request.GetRequestStream())
            {
                requestStreamAsync.Write(content, 0, content.Length);
            }
            using (HttpWebResponse responseAsync = (HttpWebResponse)request.GetResponse())
            {
                if (responseAsync.StatusCode != HttpStatusCode.OK && responseAsync.StatusCode != HttpStatusCode.Accepted)
                {
                    Stream responseStream = responseAsync.GetResponseStream();
                    if (responseStream != null)
                    {
                        using (StreamReader streamReader = new StreamReader(responseStream))
                        {
                            throw new Exception(streamReader.ReadToEnd());
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Debugs the specified entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">The entity.</param>
        /// <exception cref="System.Exception"></exception>
        public void Debug<T>(T entity)
        {
            string json = JsonConvert.SerializeObject(entity);
            string requestUriString = $"https://{WorkspaceId}.ods.opinsights.azure.com/api/logs?api-version={ApiVersion}";
            DateTime dateTime = DateTime.UtcNow;
            string dateString = dateTime.ToString("r");
            string signature = GetSignature("POST", json.Length, "application/json", dateString, "/api/logs");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUriString);
            request.ContentType = "application/json";
            request.Method = "POST";
            request.Headers["Log-Type"] = "Debug";
            request.Headers["x-ms-date"] = dateString;
            request.Headers["Authorization"] = signature;
            byte[] content = Encoding.UTF8.GetBytes(json);
            using (Stream requestStreamAsync = request.GetRequestStream())
            {
                requestStreamAsync.Write(content, 0, content.Length);
            }
            using (HttpWebResponse responseAsync = (HttpWebResponse)request.GetResponse())
            {
                if (responseAsync.StatusCode != HttpStatusCode.OK && responseAsync.StatusCode != HttpStatusCode.Accepted)
                {
                    Stream responseStream = responseAsync.GetResponseStream();
                    if (responseStream != null)
                    {
                        using (StreamReader streamReader = new StreamReader(responseStream))
                        {
                            throw new Exception(streamReader.ReadToEnd());
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Informations the specified entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">The entity.</param>
        /// <exception cref="System.Exception"></exception>
        public void Info<T>(T entity)
        {
            string json = JsonConvert.SerializeObject(entity);
            string requestUriString = $"https://{WorkspaceId}.ods.opinsights.azure.com/api/logs?api-version={ApiVersion}";
            DateTime dateTime = DateTime.UtcNow;
            string dateString = dateTime.ToString("r");
            string signature = GetSignature("POST", json.Length, "application/json", dateString, "/api/logs");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUriString);
            request.ContentType = "application/json";
            request.Method = "POST";
            request.Headers["Log-Type"] = "Info";
            request.Headers["x-ms-date"] = dateString;
            request.Headers["Authorization"] = signature;
            byte[] content = Encoding.UTF8.GetBytes(json);
            using (Stream requestStreamAsync = request.GetRequestStream())
            {
                requestStreamAsync.Write(content, 0, content.Length);
            }
            using (HttpWebResponse responseAsync = (HttpWebResponse)request.GetResponse())
            {
                if (responseAsync.StatusCode != HttpStatusCode.OK && responseAsync.StatusCode != HttpStatusCode.Accepted)
                {
                    Stream responseStream = responseAsync.GetResponseStream();
                    if (responseStream != null)
                    {
                        using (StreamReader streamReader = new StreamReader(responseStream))
                        {
                            throw new Exception(streamReader.ReadToEnd());
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Warns the specified entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">The entity.</param>
        /// <exception cref="System.Exception"></exception>
        public void Warn<T>(T entity)
        {
            string json = JsonConvert.SerializeObject(entity);
            string requestUriString = $"https://{WorkspaceId}.ods.opinsights.azure.com/api/logs?api-version={ApiVersion}";
            DateTime dateTime = DateTime.UtcNow;
            string dateString = dateTime.ToString("r");
            string signature = GetSignature("POST", json.Length, "application/json", dateString, "/api/logs");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUriString);
            request.ContentType = "application/json";
            request.Method = "POST";
            request.Headers["Log-Type"] = "Warn";
            request.Headers["x-ms-date"] = dateString;
            request.Headers["Authorization"] = signature;
            byte[] content = Encoding.UTF8.GetBytes(json);
            using (Stream requestStreamAsync = request.GetRequestStream())
            {
                requestStreamAsync.Write(content, 0, content.Length);
            }
            using (HttpWebResponse responseAsync = (HttpWebResponse)request.GetResponse())
            {
                if (responseAsync.StatusCode != HttpStatusCode.OK && responseAsync.StatusCode != HttpStatusCode.Accepted)
                {
                    Stream responseStream = responseAsync.GetResponseStream();
                    if (responseStream != null)
                    {
                        using (StreamReader streamReader = new StreamReader(responseStream))
                        {
                            throw new Exception(streamReader.ReadToEnd());
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Fatals the specified entity.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">The entity.</param>
        /// <exception cref="System.Exception"></exception>
        public void Fatal<T>(T entity)
        {
            string json = JsonConvert.SerializeObject(entity);
            string requestUriString = $"https://{WorkspaceId}.ods.opinsights.azure.com/api/logs?api-version={ApiVersion}";
            DateTime dateTime = DateTime.UtcNow;
            string dateString = dateTime.ToString("r");
            string signature = GetSignature("POST", json.Length, "application/json", dateString, "/api/logs");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUriString);
            request.ContentType = "application/json";
            request.Method = "POST";
            request.Headers["Log-Type"] = "Fatal";
            request.Headers["x-ms-date"] = dateString;
            request.Headers["Authorization"] = signature;
            byte[] content = Encoding.UTF8.GetBytes(json);
            using (Stream requestStreamAsync = request.GetRequestStream())
            {
                requestStreamAsync.Write(content, 0, content.Length);
            }
            using (HttpWebResponse responseAsync = (HttpWebResponse)request.GetResponse())
            {
                if (responseAsync.StatusCode != HttpStatusCode.OK && responseAsync.StatusCode != HttpStatusCode.Accepted)
                {
                    Stream responseStream = responseAsync.GetResponseStream();
                    if (responseStream != null)
                    {
                        using (StreamReader streamReader = new StreamReader(responseStream))
                        {
                            throw new Exception(streamReader.ReadToEnd());
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Losts the messsage.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="entity">The entity.</param>
        /// <exception cref="System.Exception"></exception>
        public void LostMesssage<T>(T entity)
        {
            string json = JsonConvert.SerializeObject(entity);
            string requestUriString = $"https://{WorkspaceId}.ods.opinsights.azure.com/api/logs?api-version={ApiVersion}";
            DateTime dateTime = DateTime.UtcNow;
            string dateString = dateTime.ToString("r");
            string signature = GetSignature("POST", json.Length, "application/json", dateString, "/api/logs");
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(requestUriString);
            request.ContentType = "application/json";
            request.Method = "POST";
            request.Headers["Log-Type"] = "LostEventMessage";
            request.Headers["x-ms-date"] = dateString;
            request.Headers["Authorization"] = signature;
            byte[] content = Encoding.UTF8.GetBytes(json);
            using (Stream requestStreamAsync = request.GetRequestStream())
            {
                requestStreamAsync.Write(content, 0, content.Length);
            }
            using (HttpWebResponse responseAsync = (HttpWebResponse)request.GetResponse())
            {
                if (responseAsync.StatusCode != HttpStatusCode.OK && responseAsync.StatusCode != HttpStatusCode.Accepted)
                {
                    Stream responseStream = responseAsync.GetResponseStream();
                    if (responseStream != null)
                    {
                        using (StreamReader streamReader = new StreamReader(responseStream))
                        {
                            throw new Exception(streamReader.ReadToEnd());
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Gets the signature.
        /// </summary>
        /// <param name="method">The method.</param>
        /// <param name="contentLength">Length of the content.</param>
        /// <param name="contentType">Type of the content.</param>
        /// <param name="date">The date.</param>
        /// <param name="resource">The resource.</param>
        /// <returns></returns>
        private string GetSignature(string method, int contentLength, string contentType, string date, string resource)
        {
            string message = $"{method}\n{contentLength}\n{contentType}\nx-ms-date:{date}\n{resource}";
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            using (HMACSHA256 encryptor = new HMACSHA256(Convert.FromBase64String(SharedKey)))
            {
                return $"SharedKey {WorkspaceId}:{Convert.ToBase64String(encryptor.ComputeHash(bytes))}";
            }
        }

        /// <summary>
        /// Gets the application key.
        /// </summary>
        /// <param name="Key">The key.</param>
        /// <returns></returns>
        public string GetApplicationKey(string Key)
        {
            lock (_lockConfig)
            {
                if (appSettings == null || appSettings.Count == 0)
                {
                    LogManagementRepository logManagementRepository = new LogManagementRepository();
                    appSettings = logManagementRepository.GetAppSetting();
                }
            }
            AppSetting data = appSettings.FirstOrDefault(x => x.Key.Equals(Convert.ToString(Key), StringComparison.CurrentCultureIgnoreCase));

            if (data != null)
                return data.Value;
            else
                return string.Empty;
        }
    }
}
