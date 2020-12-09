using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace component.helper.Interface
{
    public interface IHttpClientHelper
    {
        T Get<T>(string url);
        T Post<TU, T>(string url, TU model);
        T Get<T>(string url, string token, Dictionary<string, string> requestHeaderKeyValue = null);
        T Post<TU, T>(string url, TU model, string token);
        HttpResponseMessage Post<T>(string url, T model, string token);
        HttpResponseMessage PUT<T>(string url, T model, string token);
        T Put<TU, T>(string url, TU model, string token);
        Task<T> GetAsync<T>(string url, string token, Dictionary<string, string> requestHeaderKeyValue = null);
    }
}
