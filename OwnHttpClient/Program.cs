using OwnHttpClient.BasicSimpleHTTP;

namespace OwnHttpClient
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            BasicHttp basicHttp = new BasicHttp();
            await basicHttp.RunExample();
        }
    }
}
