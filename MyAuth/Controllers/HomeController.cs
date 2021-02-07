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
        public ActionResult Home()
        {

            var filePythonNamePath = @"C:\Users\tejoh\python\Face_Registration.py";
            var filePythonExePath = @"C:\Users\tejoh\AppData\Local\Programs\Python\Python38\python.exe";
            var filename1 = @"C:\Users\tejoh\python\john1.jpg";
            var filename2 = @"C:\Users\tejoh\python\john2.jpg";


            byte[] imageArray = System.IO.File.ReadAllBytes(filename1);
            string base64ImageRepresentation = Convert.ToBase64String(imageArray);



            string outputText, standardError;

            // Instantiate Machine Learning C# - Python class object            
            IMLSharpPython mlSharpPython = new MLSharpPython(filePythonExePath);
    
            // Define Python script file and input parameter name
            string fileNameParameter = $"{filePythonNamePath}" /*{filePythonParameterName} {imagePathName}*/;
            string arguments = $"{base64ImageRepresentation}" /*{filePythonParameterName} {imagePathName}*/;
            // Execute the python script file 
            
            outputText = mlSharpPython.ExecutePythonScript(fileNameParameter, arguments , out standardError);
            if (string.IsNullOrEmpty(standardError))
            {
                
            }
            else
            {
                Console.WriteLine(standardError);
            }
            Console.ReadKey();

            return Ok("Testing page");
        }
    }
}
