using IdentityModel.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MyClient
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Console.WriteLine("Test 1");
            Test1().Wait();
            Console.WriteLine("========================================");
            Console.WriteLine("Test 2");
            Test2().Wait();
            Console.ReadLine();
        }

        public static async Task Test1()
        {
            var disco = await DiscoveryClient.GetAsync("http://localhost:5000");
            var tokentClient = new TokenClient(disco.TokenEndpoint, "client", "mysecret");
            var tokenResponse = await tokentClient.RequestClientCredentialsAsync("MyApi");
            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }
            Console.Write(tokenResponse.Json);

            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);

            var response = await client.GetAsync("http://localhost:5001/api/identity");
            if(!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));
            }
        }

        public static async Task Test2()
        {
            var disco = await DiscoveryClient.GetAsync("http://localhost:5000");
            var tokentClient = new TokenClient(disco.TokenEndpoint, "ro.client", "mysecret");
            var tokenResponse = await tokentClient.RequestResourceOwnerPasswordAsync("alice","password","MyApi");
            if (tokenResponse.IsError)
            {
                Console.WriteLine(tokenResponse.Error);
                return;
            }
            Console.Write(tokenResponse.Json);

            var client = new HttpClient();
            client.SetBearerToken(tokenResponse.AccessToken);

            var response = await client.GetAsync("http://localhost:5001/api/identity");
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine(response.StatusCode);
            }
            else
            {
                var content = await response.Content.ReadAsStringAsync();
                Console.WriteLine(JArray.Parse(content));
            }
        }
    }
}
