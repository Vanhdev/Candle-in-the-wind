using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CandleInTheWind.Controllers
{
    public class BaseController : Controller
    {
        public BaseController(Microsoft.Extensions.Caching.Distributed.IDistributedCache cache)
        {
            if (cache.GetString("login_admin") == null)
            {
            }
        }
    }
}