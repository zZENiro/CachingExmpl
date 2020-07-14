using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace CachingExmplApplication
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var app = WebHost.CreateDefaultBuilder(args)
                        .UseKestrel()
                        .UseStartup<Startup>()
                        .Build();

            var lifetime = app.Services.GetRequiredService<IHostApplicationLifetime>();
            lifetime.ApplicationStopped.Register(() =>
            {
                Console.WriteLine("Server stopped!");
            });

            app.Run();
        }
    }
}
