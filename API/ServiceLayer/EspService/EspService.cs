using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ServiceLayer.EspService
{
    public class EspService : IEspService
    {
        public EspService() { }
        public async Task Vibrate(bool start)
        {
            HttpClient client = new HttpClient();
            client.BaseAddress = new Uri("http://192.168.0.10");
            var content = new FormUrlEncodedContent(new[] {new KeyValuePair<string, string>("message", start ? "start" : "stop")});
            HttpResponseMessage _ = await client.PostAsync("",content);
        }
    }
}
