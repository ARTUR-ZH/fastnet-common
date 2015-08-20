using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Fastnet.Web.Common
{
    public class WebApiClient : IDisposable
    {
        private HttpClient client;
        private string location;
        public WebApiClient(string url)
        {
            this.location = url;
            client = new HttpClient();
        }
        ~WebApiClient()
        {
            Dispose();
        }
        protected async Task<dynamic> PostAsync<T>(string url, T data)
        {
            url = this.location + "/" + url;
            var r = await client.PostAsJsonAsync<T>(url, data);
            //if (!r.IsSuccessStatusCode)
            //{
            //    //Debug.Print("PostAsync() Error: destination: {0}, url: {1}", this.location, url);
            //    Log.Write("PostAsync() Error: destination: {0}, url: {1}", this.location, url);
            //}
            r.EnsureSuccessStatusCode();
            return await r.Content.ReadAsAsync<dynamic>();
        }
        protected async Task<dynamic> GetAsync(string url)
        {
            url = this.location + "/" + url;
            var r = await client.GetAsync(url);
            //if (!r.IsSuccessStatusCode)
            //{
            //    //Debug.Print("GetAsync() Error: destination: {0}, url: {1}", this.location, url);
            //    Log.Write("GetAsync() Error: destination: {0}, url: {1}", this.location, url);
            //}
            r.EnsureSuccessStatusCode();
            return await r.Content.ReadAsAsync<dynamic>();
        }
        public void Dispose()
        {
            if (client != null)
            {
                client.Dispose();
            }
        }
    }
}
