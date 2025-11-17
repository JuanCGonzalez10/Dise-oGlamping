using System.Text;
using Newtonsoft.Json;
using OmniPyme.Web.Core;
using OmniPyme.Web.Helpers;

namespace OmniPyme.Web.Services
{
    public interface IApiService
    {
        public Task<Response<T>> GetAsync<T>(string url, List<HeaderItem>? headers);
        public Task<Response<T>> PostAsync<T>(string url, object content, List<HeaderItem>? headers);

        public Task<Response<T>> PutAsync<T>(string url, object content, List<HeaderItem>? headers);

        public Task<Response<T>> DeleteAsync<T>(string url, List<HeaderItem>? headers);
    }

    public class ApiService : IApiService
    {
        public async Task<Response<T>> DeleteAsync<T>(string url, List<HeaderItem>? headers)
        {
            try
            {
                HttpClient client = new HttpClient();

                if (headers is not null && headers.Count > 0)
                {
                    foreach (HeaderItem item in headers)
                    {
                        client.DefaultRequestHeaders.Add(item.Name, item.Value);
                    }
                }

                HttpResponseMessage response = await client.DeleteAsync(url);

                string answer = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return ResponseHelper<T>.MakeResponseFail(answer);
                }

                T result = JsonConvert.DeserializeObject<T>(answer);

                return ResponseHelper<T>.MakeResponseSuccess(result);
            }
            catch (Exception ex)
            {
                return ResponseHelper<T>.MakeResponseFail(ex);
            }
        }

        public async Task<Response<T>> GetAsync<T>(string url, List<HeaderItem>? headers)
        {
            try
            {
                HttpClient client = new HttpClient();

                if (headers is not null && headers.Count > 0)
                {
                    foreach (HeaderItem item in headers)
                    {
                        client.DefaultRequestHeaders.Add(item.Name, item.Value);
                    }
                }

                HttpResponseMessage response = await client.GetAsync(url);

                string answer = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return ResponseHelper<T>.MakeResponseFail(answer);
                }

                T result = JsonConvert.DeserializeObject<T>(answer);

                return ResponseHelper<T>.MakeResponseSuccess(result);
            }
            catch (Exception ex)
            {
                return ResponseHelper<T>.MakeResponseFail(ex);
            }
        }

        public async Task<Response<T>> PostAsync<T>(string url, object content, List<HeaderItem>? headers)
        {
            try
            {
                HttpClient client = new HttpClient();

                if (headers is not null && headers.Count > 0)
                {
                    foreach (HeaderItem item in headers)
                    {
                        client.DefaultRequestHeaders.Add(item.Name, item.Value);
                    }
                }

                string request = JsonConvert.SerializeObject(content);
                StringContent body = new StringContent(request, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(url, body);

                string answer = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return ResponseHelper<T>.MakeResponseFail(answer);
                }

                T result = JsonConvert.DeserializeObject<T>(answer);

                return ResponseHelper<T>.MakeResponseSuccess(result);
            }
            catch (Exception ex)
            {
                return ResponseHelper<T>.MakeResponseFail(ex);
            }
        }

        public async Task<Response<T>> PutAsync<T>(string url, object content, List<HeaderItem>? headers)
        {
            try
            {
                HttpClient client = new HttpClient();

                if (headers is not null && headers.Count > 0)
                {
                    foreach (HeaderItem item in headers)
                    {
                        client.DefaultRequestHeaders.Add(item.Name, item.Value);
                    }
                }

                string request = JsonConvert.SerializeObject(content);
                StringContent body = new StringContent(request, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PutAsync(url, body);

                string answer = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return ResponseHelper<T>.MakeResponseFail(answer);
                }

                T result = JsonConvert.DeserializeObject<T>(answer);

                return ResponseHelper<T>.MakeResponseSuccess(result);
            }
            catch (Exception ex)
            {
                return ResponseHelper<T>.MakeResponseFail(ex);
            }
        }
    }
}
