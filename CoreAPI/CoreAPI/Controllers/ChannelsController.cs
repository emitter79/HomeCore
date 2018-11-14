using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CoreHardware;
using CoreHardware.Models;

namespace CoreAPI.Controllers
{
    [Route("api/[controller]")]
    public class ChannelsController : Controller
    {
        
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
        
        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            return Ok(255);
        }

        [HttpPost]
        public IActionResult Post([FromBody]ValueCommand value)
        {
            Program.systemNode.SendCommandAsync(value);
            return Ok(value);
        }

    }
}
