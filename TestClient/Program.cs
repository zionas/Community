using CommunityNetwork.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TestClient
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var httpClient = new HttpClient())
            {
                Profile profile = new Profile()
                {
                    Email = "san@walla.com"
                };
                httpClient.BaseAddress = new Uri("http://localhost:54056/");
                var content = new StringContent(JsonConvert.SerializeObject(profile), Encoding.UTF8, "application/json");
                var response = httpClient.PutAsync($"api/Identity/Edit", content).Result;
                if (response.IsSuccessStatusCode)
                {
                    int b = 5;
                }
            }
        }
    }
}
