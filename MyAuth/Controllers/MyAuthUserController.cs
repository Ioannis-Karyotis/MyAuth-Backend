using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using MyAuth.Enums;
using MyAuth.Models.ApiResponseModels;
using MyAuth.Models.Data;
using MyAuth.Models.Database;
using MyAuth.Models.RequestModels;
using MyAuth.services;
using MyAuth.Utils;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MyAuthUserController : ControllerBase
    {

        private readonly SignInManager<MyAuthUser> _signInManager;
        private readonly UserManager<MyAuthUser> _userManager;
        private readonly ILogger<RegisterReqModel> _logger;
        private readonly AuthServices _authServices;


        public MyAuthUserController(
            UserManager<MyAuthUser> userManager,
            SignInManager<MyAuthUser> signInManager,
            ILogger<RegisterReqModel> logger,
            AuthServices authServices)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _authServices = authServices;
        }

        [HttpGet("acttest")]
        public object ActTest()
        {
            return "ok";
        }


        // GET: api/<RegisterController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET api/<RegisterController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        

        // PUT api/<RegisterController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<RegisterController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
