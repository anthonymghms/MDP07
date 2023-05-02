using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ServiceLayer.EspService
{
    public class EspService : IEspService
    {
        public EspService() { }
        public async Task Vibrate(string ipEspAddress)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(ipEspAddress);
                var content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("message", "start") });
                HttpResponseMessage _ = await client.PostAsync("", content);
            }
            catch (Exception ex)
            {

            }
        }
        public async Task StopVibrate(string ipEspAddress)
        {
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri(ipEspAddress);
                var content = new FormUrlEncodedContent(new[] { new KeyValuePair<string, string>("message", "stop") });
                HttpResponseMessage _ = await client.PostAsync("", content);
            }
            catch (Exception ex)
            {

            }
        }
        // stop vibrate method

    }
}
