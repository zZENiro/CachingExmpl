using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using CachingExmplApplication.Data;
using CachingExmplApplication.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace CachingExmplApplication.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        IDistributedCache _distrCache;
        PeopleDbContext _peopleDb;

        public HomeController(IDistributedCache distributedCache, PeopleDbContext peopleDb)
        {
            _distrCache = distributedCache;
            _peopleDb = peopleDb;
        }

        [HttpGet("[action]")]
        public async Task<JsonResult> GetAll()
        {
            List<Person> people = null;
            var resultBinary = await _distrCache.GetAsync("peopleCollection");
            var formatter = new BinaryFormatter();

            if (resultBinary is null)
            {
                using (MemoryStream stream = new MemoryStream())
                {
                    var src = _peopleDb.People.ToList();
                    formatter.Serialize(stream, src);
                    var binaryCollection = stream.ToArray();

                    await _distrCache.SetAsync(
                        key:     "peopleCollection",
                        value:   binaryCollection,
                        options: new DistributedCacheEntryOptions()
                        {
                            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(10),
                            SlidingExpiration = TimeSpan.FromSeconds(5)
                        });

                    return new JsonResult(src);
                }
            }
            else
                using (MemoryStream stream = new MemoryStream(resultBinary))
                {
                    return new JsonResult(formatter.Deserialize(stream) as List<Person>);
                }
        }

    }
}
