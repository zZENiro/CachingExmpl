using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using CachingExmplApplication.Data;
using CachingExmplApplication.Models;
using EasyCaching.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace CachingExmplApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [ResponseCache(Location = ResponseCacheLocation.Client, Duration = 100, NoStore = false, VaryByHeader = "User-Agent")]
    public class HomeController : ControllerBase
    {
        PeopleDbContext _peopleDb;
        IEasyCachingProvider _localCacheProvider;
        IEasyCachingProvider _compCacheProvider;

        public HomeController(PeopleDbContext peopleDb, IEasyCachingProviderFactory factory)
        {
            _localCacheProvider = factory.GetCachingProvider("local_cache");
            _compCacheProvider = factory.GetCachingProvider("comp_cache");
            _peopleDb = peopleDb;
        }

        [HttpGet("[action]")]
        public async Task<JsonResult> GetAll()
        {
            var peopleCache = _compCacheProvider.Get<List<Person>>("peopleCollection");

            if (!peopleCache.HasValue)
            {
                var srcCollection = _peopleDb.People.ToList();
                _compCacheProvider.Set("peopleCollection", srcCollection, TimeSpan.FromSeconds(15));
                return new JsonResult(srcCollection);
            }

            return new JsonResult(peopleCache.Value);
        }

    }
}
