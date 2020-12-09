using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace component.eventbus.Queue
{
    /// <summary>
    /// ApiClient
    /// </summary>
    public static class ApiClient
    {
        /// <summary>Gets the API response.</summary>
        /// <param name="url">The URL.</param>
        /// <param name="method">The method.</param>
        /// <param name="headers">The headers.</param>
        /// <param name="strParams">The string parameters.</param>
        /// <returns></returns>
        public static HttpResponseMessage GetApiResponse(string url, HttpMethod method, Dictionary<string, string> headers, string strParams = "")
        {
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("application/json"));
                StringContent httpContent = new StringContent(strParams, Encoding.UTF8, "application/json");
                HttpRequestMessage msg = new HttpRequestMessage()
                {
                    Content = httpContent,
                    Method = method,
                    RequestUri = new Uri(url),
                };
                if (headers.Count != 0)
                {
                    foreach (KeyValuePair<string, string> header in headers)
                    {
                        client.DefaultRequestHeaders.Add(header.Key, header.Value);
                    }
                }
                return client.SendAsync(msg).Result;
            }
        }
    }
}
