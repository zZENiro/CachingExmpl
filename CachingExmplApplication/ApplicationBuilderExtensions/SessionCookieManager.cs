using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CachingExmplApplication.ApplicationBuilderExtensions
{
    public static class SessionCookieManager
    {
        public static void UseSessionCookieManager(this IApplicationBuilder app)
        {
            app.Use(async (context, next) =>
            {
                if (context.Request.Cookies["name"] is null)
                    context.Response.Cookies.Append("name", "Nikita");

                await next.Invoke();
            });
        }
    }
}
