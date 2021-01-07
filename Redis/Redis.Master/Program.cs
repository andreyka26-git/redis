using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Redis.Common;
using Redis.Common.Infrastructure;

namespace Redis.Master
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
