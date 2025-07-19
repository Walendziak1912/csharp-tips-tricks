using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;

namespace OwnHttpClient.BasicSimpleHTTP
{
    public class BasicHttp
    {

        public BasicHttp()
        {
                
        }

        public async Task RunExample()
        {
            try
            {
                using (HttpClient client = new HttpClient())
                {
                    var response = await client.GetAsync("http://numbersapi.com/random/year");

                    if (response.IsSuccessStatusCode)
                    {
                        var content = await response.Content.ReadAsStringAsync();
                        Console.WriteLine($"Status: {response.StatusCode}");
                        Console.WriteLine($"Content: {content}");
                    }
                    else
                    {
                        Console.WriteLine($"Błąd: {response.StatusCode}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Wystąpił błąd: {ex.Message}");
            }

        }
    }
}
