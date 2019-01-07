using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Amazon.DynamoDBv2.DataModel;
using CommunityNetwork.Common;
using CommunityNetWork.Common.Enums;
using CommunityNetWork.Common.Models;
using CommunityNetWork.Dal;

using CommunityNetWork.Dal.Interfaces;
using Newtonsoft.Json;

using SocialSerivce.Models;

namespace ConsoleApp1
{
    class Test1 : Node
    {
        public string Name1 { get; set; }
    }
    public class Test2
    {
        [DynamoDBHashKey]
        public int Idnum { get; set; }
        public string MyProperty { get; set; }
    }
    class Program
    {
        static HttpClient client = new HttpClient();
        const string _api = "http://localhost:52225/api";
        const string _social = "/SocialActions";
        const string _persistence = "/Persistence";
        static Guid Create()
        {
            Profile profile = new Profile();
            
            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(_api+_persistence);
                string url = _api + _persistence+"/CreateProfile";

                HttpContent content = new StringContent(JsonConvert.SerializeObject(profile), Encoding.UTF8, "application/json");
                //var response = client.GetAsync(url).Result;
                var response = client.PostAsync(url, content).Result;
                //var response = client.GetAsync(_api + _persistence ).Result;
                if (response.IsSuccessStatusCode)
                {
                    var cont = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    Console.WriteLine(cont);
                    Profile p = JsonConvert.DeserializeObject<Profile>(cont);
                    return p.Id;
                }
                return default(Guid);
            }
            

        }
        static void Follow(Guid id1,Guid id2)
        {

            SocialAction socialAction = new SocialAction
            {
                FromId = id1,
                ToId = id2,
                linkage = Linkage.Follow.ToString()
            };
            


            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new Uri(_api+_social);
                HttpContent content = new StringContent(JsonConvert.SerializeObject(socialAction), Encoding.UTF8, "application/json");
                string url = _api + _social + "/Follow";
                var response = client.PostAsync(url, content).Result;

                if (response.IsSuccessStatusCode)
                {
                    var cont = response.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                    Console.WriteLine(cont);
                }
            }

        }
        static void TestDynamoDbAndGraph()
        {
            Task<bool> t = null;
            Test2 t2 = new Test2
            {
                Idnum = 1111,
                MyProperty = "pzz"
            };
            Test1 t1 = new Test1
            {
                Id = Guid.NewGuid(),
                Name1 = "fifty five",

            };
            Test1 t3 = new Test1
            {
                Id = Guid.NewGuid(),
                Name1 = "sixty six",

            };
            using (IDynamoDB db = new AwsDynamoDBFactory().Create(true))

                try
                {
                    db.RemoveModel(typeof(Test2));
                    t = db.AddModel(typeof(Test2));
                    t.Wait();
                    db.Add(t2);
                    Test2 t4 = db.Get<Test2>(1111, true);
                    using (var cypher = new Neo4jConnector())

                    {
                        cypher.Put(t1);
                        //cypher.Delete<Test1>(55);
                        cypher.Put(t1);
                        var t5 = cypher.Create(t3);
                        Linkage linkage = Linkage.Like;
                        var list = cypher.GetNodesLinks<Test1, Test1>(linkage);
                        var res = cypher.Get<Test1>(t1.Id);

                    }


                    Console.WriteLine();


                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex + "::" + ex.Message);
                    Exception e = ex.InnerException;

                    while (e != null)
                    {
                        Console.WriteLine("Inner::" + e.Message);
                        e = e.InnerException;
                    }

                }
            if (!t.IsCompleted)
                Console.WriteLine("time out canceled operation...");
            Console.WriteLine("Enter to Quit");
            Console.ReadLine();
        }
        static void Main(string[] args)
        {
            Console.WriteLine(
                );// Guid p1=Create();
            //Guid p2 = Create();
            //Follow(p1, p2);

        }
    }


}