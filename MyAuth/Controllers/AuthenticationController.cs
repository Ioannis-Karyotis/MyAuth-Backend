using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyAuth.Enums;
using MyAuth.Models.ApiResponseModels;
using MyAuth.Models.Data;
using MyAuth.Models.RequestModels;
using MyAuth.services;
using MyAuth.Utils;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly AuthServices _authServices;
        public AuthenticationController(AuthServices authServices)
        {
            _authServices =  authServices;
        }

        [HttpPost("signin")]
        [EnableCors]
        public async Task<ActionResult<HttpResponseData<SuccessfulLoginRespModel, ClientsApiErrorCodes>>> Signin([FromBody] LoginRequestModel input)
        {
            HttpResponseData<SuccessfulLoginRespModel, ClientsApiErrorCodes> response = await _authServices.DoLoginUser(input);

            if (response.Success == true)
            {
                return Ok(response);
            }

            ClientsApiErrorCodes val = response.Error.ErrorCode;

            switch (val)
            {
                case ClientsApiErrorCodes.NotExistingUser:
                    goto NotExistingUserCase;
            }
            

        NotExistingUserCase: return StatusCode(StatusCodes.Status500InternalServerError, new HttpResponseData<SuccessfulLoginRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.NotExistingUser));

        }       
        
        [HttpPost("facial/recognition")]
        [EnableCors]
        public async Task<ActionResult<HttpResponseData<SuccessfulLoginRespModel, ClientsApiErrorCodes>>> SigninFacialRecognition([FromBody] LoginFacialRequestModel input)
        {
            HttpResponseData<SuccessfulLoginRespModel, ClientsApiErrorCodes> response = await _authServices.FacialRecognition(input);

            if (response.Success == true)
            {
                return Ok(response);
            }

            ClientsApiErrorCodes val = response.Error.ErrorCode;

            switch (val)
            {
                case ClientsApiErrorCodes.NotExistingUser:
                    goto NotExistingUserCase;
            }

        NotExistingUserCase: return StatusCode(StatusCodes.Status500InternalServerError, new HttpResponseData<SuccessfulLoginRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.NotExistingUser));
            
        }

        // GET api/<AuthenticationController>/5
        [HttpPost("fingerAuth")]
        public string FingerAuth()
        {
            return "value";
        }
    }
}
