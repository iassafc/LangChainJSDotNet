using System;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace LangChainJSDotNet
{
    internal sealed class Http
    {
        private readonly HttpClient _httpClient;

        internal class HttpResponse
        {
            public int StatusCode { get; set; } = 0;

            public Dictionary<string, string> Headers { get; set; } = new Dictionary<string, string>();

            public string Content { get; set; } = "";
        }

        public Http(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<string> SendAsync(string url, string method, IDictionary<string, object> headers, string body)
        {
            var response = new HttpResponse();
            string contentType = null;
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(new HttpMethod(method), url);
                foreach (var header in headers)
                {
                    if (header.Key.ToLower() != "content-type")
                    {
                        if (!request.Headers.TryAddWithoutValidation(header.Key, header.Value.ToString()))
                        {
                            throw new Exception($"Couldn't add header {header.Key} with value {header.Value}");
                        }
                    }
                    else
                    {
                        contentType = header.Value.ToString();
                    }
                }
                if (!string.IsNullOrEmpty(body))
                {
                    request.Content = new StringContent(body, Encoding.UTF8);
                    if (contentType != null)
                    {
                        request.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType);
                    }
                }

                // send request
                HttpResponseMessage clientResponse = await _httpClient.SendAsync(request);

                response.StatusCode = (int)clientResponse.StatusCode;

                foreach (var header in clientResponse.Headers)
                {
                    response.Headers[header.Key] = string.Join(",", header.Value);
                }

                if (clientResponse.IsSuccessStatusCode)
                {
                    response.Content = await clientResponse.Content.ReadAsStringAsync();
                }
            }
            catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
            //var jsonResponse = JsonSerializer.Serialize(response);
            var jsonResponse = JsonConvert.SerializeObject(response);
            return jsonResponse;
        }
    }
}
