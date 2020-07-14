using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CachingExmplApplication.Models
{
    [Serializable]
    public class Person
    {
        public int Id { get; set; }

        public string Name { get; set; }
    }
}
