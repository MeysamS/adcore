using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            GetAPI().Wait();
        }

        public static async Task GetAPI()
        {
            HttpClient client = new HttpClient();
            client.DefaultRequestHeaders.Clear();
            client.BaseAddress = new Uri("http://localhost:5000");

            var r1 = await client.GetAsync("/home");

            client.DefaultRequestHeaders.Clear();
            var r2 =await client.GetAsync("/homex");

            client.DefaultRequestHeaders.Clear();
            var r3 =await client.GetAsync("/homey");

            Console.WriteLine($"call api-1 : {r1.Content.ReadAsStringAsync().Result}");
            Console.WriteLine();
            Console.WriteLine($"call api-2 : {r2.Content.ReadAsStringAsync().Result}");
            Console.WriteLine();
            Console.WriteLine($"call api-3 : {r3.Content.ReadAsStringAsync().Result}");

            Console.ReadKey();
        }
    }
}
