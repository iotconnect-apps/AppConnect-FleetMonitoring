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

namespace component.helper
{
    public class HttpClientHelper : IHttpClientHelper
    {
        private readonly string _mediaType = "application/json";

        private readonly LogHandler.Logger _logger;
        public HttpClientHelper(LogHandler.Logger logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Execute Get Call
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <returns></returns>
        public T Get<T>(string url)
        {
            return Get<T>(url, string.Empty);
        }

        /// <summary>
        /// Execute Get Call
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public T Get<T>(string url, string token, Dictionary<string, string> requestHeaderKeyValue = null)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        httpClient.SetBearerToken(token);
                    }

                    if (requestHeaderKeyValue != null && requestHeaderKeyValue.Count > 0)
                    {
                        foreach (var item in requestHeaderKeyValue)
                        {
                            httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
                        }
                    }

                    using (var response = httpClient.GetAsync(url).Result)
                    {
                        if ((int)response.StatusCode == (int)HttpStatusCode.OK)
                        {
                            if (typeof(T) == typeof(string))
                                return (T)(object)response.Content.ReadAsStringAsync().Result;
                            return JsonConvert.DeserializeObject<T>(response.Content.ReadAsStringAsync().Result);
                        }
                        _logger.ErrorLog(new System.Exception($"Http client request failed with status code : {response.StatusCode.ToString()} for url : {url}"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                    }
                }
            }
            catch (System.Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return default(T);
        }

        /// <summary>
        /// Execute post call
        /// </summary>
        /// <typeparam name="TU">Request model type</typeparam>
        /// <typeparam name="T">Response model type</typeparam>
        /// <param name="url"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public T Post<TU, T>(string url, TU model)
        {
            try
            {
                return Post<TU, T>(url, model, string.Empty);
            }
            catch (System.Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return default(T);
        }

        /// <summary>
        /// Execute post call
        /// </summary>
        /// <typeparam name="TU">Request model type</typeparam>
        /// <typeparam name="T">Response model type</typeparam>
        /// <param name="url"></param>
        /// <param name="model"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public T Post<TU, T>(string url, TU model, string token)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        httpClient.SetBearerToken(token);
                    }

                    var serializedContent = JsonConvert.SerializeObject(model);
                    var httpContent = new StringContent(serializedContent, Encoding.UTF8, _mediaType);
                    using (var response = httpClient.PostAsync(url, httpContent).Result)
                    {
                        if ((int)response.StatusCode == (int)HttpStatusCode.OK)
                        {
                            if (typeof(T) == typeof(string))
                                return (T)(object)response.Content.ReadAsStringAsync().Result;
                            return JsonConvert.DeserializeObject<T>(response.Content.ReadAsStringAsync().Result);
                        }
                        _logger.ErrorLog(new System.Exception($"Http client request failed with status code : {response.StatusCode.ToString()} for url : {url} with content : {httpContent}"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                    }
                }
            }
            catch (System.Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return default(T);
        }

        public HttpResponseMessage Post<T>(string url, T model, string token)
        {
            using (var httpClient = new HttpClient())
            {
                if (!string.IsNullOrWhiteSpace(token))
                {
                    httpClient.SetBearerToken(token);
                }

                var serializedContent = JsonConvert.SerializeObject(model);
                var httpContent = new StringContent(serializedContent, Encoding.UTF8, _mediaType);
                return httpClient.PostAsync(url, httpContent).Result;
            }
        }

        public HttpResponseMessage PUT<T>(string url, T model, string token)
        {
            using (var httpClient = new HttpClient())
            {
                if (!string.IsNullOrWhiteSpace(token))
                {
                    httpClient.SetBearerToken(token);
                }

                var serializedContent = JsonConvert.SerializeObject(model);
                var httpContent = new StringContent(serializedContent, Encoding.UTF8, _mediaType);
                return httpClient.PutAsync(url, httpContent).Result;
            }
        }
        /// <summary>
        /// Execute Get Call
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="url"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<T> GetAsync<T>(string url, string token, Dictionary<string, string> requestHeaderKeyValue = null)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        httpClient.SetBearerToken(token);
                    }

                    if (requestHeaderKeyValue != null && requestHeaderKeyValue.Count > 0)
                    {
                        foreach (var item in requestHeaderKeyValue)
                        {
                            httpClient.DefaultRequestHeaders.Add(item.Key, item.Value);
                        }
                    }

                    using (var response = httpClient.GetAsync(url).Result)
                    {
                        if ((int)response.StatusCode == (int)HttpStatusCode.OK)
                        {
                            if (typeof(T) == typeof(string))
                                return (T)(object)response.Content.ReadAsStringAsync().Result;
                            return JsonConvert.DeserializeObject<T>(response.Content.ReadAsStringAsync().Result);
                        }
                        _logger.ErrorLog(new System.Exception($"Http client request failed with status code : {response.StatusCode.ToString()} for url : {url}"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                    }
                }
            }
            catch (System.Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return default(T);
        }

        /// <summary>
        /// Execute Put call
        /// </summary>
        /// <typeparam name="TU">Request model type</typeparam>
        /// <typeparam name="T">Response model type</typeparam>
        /// <param name="url"></param>
        /// <param name="model"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public T Put<TU, T>(string url, TU model, string token)
        {
            try
            {
                using (var httpClient = new HttpClient())
                {
                    if (!string.IsNullOrWhiteSpace(token))
                    {
                        httpClient.SetBearerToken(token);
                    }

                    var serializedContent = JsonConvert.SerializeObject(model);
                    var httpContent = new StringContent(serializedContent, Encoding.UTF8, _mediaType);
                    using (var response = httpClient.PutAsync(url, httpContent).Result)
                    {
                        if ((int)response.StatusCode == (int)HttpStatusCode.OK)
                        {
                            if (typeof(T) == typeof(string))
                                return (T)(object)response.Content.ReadAsStringAsync().Result;
                            return JsonConvert.DeserializeObject<T>(response.Content.ReadAsStringAsync().Result);
                        }
                        _logger.ErrorLog(new System.Exception($"Http client request failed with status code : {response.StatusCode.ToString()} for url : {url} with content : {httpContent}"), this.GetType().Name, MethodBase.GetCurrentMethod().Name);
                    }
                }
            }
            catch (System.Exception ex)
            {
                _logger.ErrorLog(ex, this.GetType().Name, MethodBase.GetCurrentMethod().Name);
            }
            return default(T);
        }
    }
}
