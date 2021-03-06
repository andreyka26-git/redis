using Microsoft.Extensions.Hosting;
using Redis.Common.Infrastructure;

namespace Redis.Child
{
    public class Program
    {
        public static void Main(string[] args)
        {
            WebHost.CreateWebHostBuilder<Startup>()
                .Build()
                .Run();
        }
    }
}
