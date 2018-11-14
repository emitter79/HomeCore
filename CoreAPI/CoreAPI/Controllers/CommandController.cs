using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CoreHardware;
using CoreHardware.Models;
using CoreAPI.Models;

namespace CoreAPI.Controllers
{
    [Route("api/[controller]")]
    public class CommandController : Controller
    {

        [HttpPost]
        public IActionResult Post([FromBody]Command command)
        {
            var values = new RGBValues(Program.systemNode.SendCommand(command));
            if (values.Valid) return Ok(values);
            return new StatusCodeResult(510);
        }

    }
}