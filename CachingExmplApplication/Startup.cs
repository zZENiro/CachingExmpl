using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CachingExmplApplication.ApplicationBuilderExtensions;
using CachingExmplApplication.Data;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using EasyCaching.Core;
using EasyCaching.InMemory;
using EasyCaching.Redis;
using EasyCaching.Core.Configurations;

namespace CachingExmplApplication
{
    public class Startup
    {
        IConfiguration _configFromLoh;

        IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            // controllers
            services.AddControllers();
            
            // db
            services.AddDbContext<PeopleDbContext>();
            
            // resp cache
            services.AddResponseCaching();

            // redis distr caching
            services.AddDistributedMemoryCache();
            services.AddEasyCaching(setup =>
            {
                setup.UseRedis(redisConfig =>
                {
                    redisConfig.DBConfig.Endpoints.Add(new ServerEndPoint("192.168.0.4", 6379));
                    redisConfig.DBConfig.AllowAdmin = true;
                    redisConfig.EnableLogging = true;
                }, name: "comp_cache");

                setup.UseRedis(redisConfig =>
                {
                    redisConfig.DBConfig.Endpoints.Add(new ServerEndPoint("localhost", 6379));
                    redisConfig.EnableLogging = true;
                    redisConfig.DBConfig.AllowAdmin = true;
                }, "local_cache");
            });
            
            // sess
            services.AddSession(sess =>
            {
                sess.Cookie.Name = "MySessionCookies";
                sess.IdleTimeout = TimeSpan.FromSeconds(10);
                sess.Cookie.Domain = "localhost";
                sess.Cookie.Path = "/";
            });
        }

        public void Configure(IWebHostEnvironment env, IApplicationBuilder app)
        {
            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseSession();

            app.UseSessionCookieManager();

            app.UseResponseCaching();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "default",
                    "{controller}/{action}");
            });
        }
    }
}
