using System;
using Microsoft.AspNetCore.Mvc;

namespace WebAPI.Infrastructure.Gateway.Controllers
{
    [Route("api/solution")]
    public class SolutionController:Controller
    {

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("API Working...");
        }
    }
}