using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using IronPython.Hosting;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyAuth.Data;
using MyAuth.Models.ApiResponseModels;
using MyAuth.Models.Database;
using MyAuth.Models.Interfaces;
using MyAuth.Utils.FaceAuthentication;

namespace MyAuth.Controllers
{
    [Controller]
    public class HomeController : ControllerBase
    {

        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;


        public HomeController(ILogger<HomeController> logger , ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpGet("home")]
        public async Task<ActionResult> Home()
        {
            ExternalApp app = new ExternalApp()
            {
                MyAuthUserId = new Guid("e1f1b049-529b-44be-8b66-b8f8102ebd94"),
                CallbackUrl = "https://scorpionclothing.gr/auth/google/callback",
                ClientId = "229507214379-q4vnk966nlih2992uqeobr0g98uebvrd.apps.googleusercontent.com",
                Created = DateTime.Now,
                Id = Guid.NewGuid(),
                LastUpdated = DateTime.Now
            };

            _context.ExternalApps.Add(app);
            await _context.SaveChangesAsync();

            return Ok();
        }
    }
}
