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
        public async Task<ActionResult<HttpResponseData<SuccessfulLoginRespModel>>> Signin([FromBody] LoginRequestModel input)
        {
            InternalDataTransfer<SuccessfulLoginRespModel> response = await _authServices.DoLoginUser(input);

            InternalDataStatuses val = response.Status;

            switch (val)
            {
                case InternalDataStatuses.Success:
                    goto SuccessCase;

                case InternalDataStatuses.NotExistingUser:
                    goto NotExistingUserCase;

                default:
                    goto FailureCase;

            }

        SuccessCase: return Ok(new HttpResponseData<SuccessfulLoginRespModel>() 
            {
                Success = true,
                Data = response.Data,
                Error = null
            });
        NotExistingUserCase: return StatusCode(StatusCodes.Status500InternalServerError, new HttpResponseData<SuccessfulLoginRespModel>()
        {
            Success = false,
            Data = null,
            Error = val.ToString()
        });
        FailureCase: return StatusCode(StatusCodes.Status500InternalServerError, new HttpResponseData<SuccessfulLoginRespModel>()
            {
                Success = false,
                Data = null,
                Error = val.ToString()
            });
        }       
        
        [HttpPost("facial/recognition")]
        [EnableCors]
        public async Task<ActionResult<HttpResponseData<SuccessfulLoginRespModel>>> SigninFacialRecognition([FromBody] LoginFacialRequestModel input)
        {
            InternalDataTransfer<SuccessfulLoginRespModel> response = await _authServices.FacialRecognition(input);

            InternalDataStatuses val = response.Status;

            switch (val)
            {
                case InternalDataStatuses.Success:
                    goto SuccessCase;

                case InternalDataStatuses.NotExistingUser:
                    goto NotExistingUserCase;

                default:
                    goto FailureCase;

            }

        SuccessCase: return Ok(new HttpResponseData<SuccessfulLoginRespModel>()
        {
            Success = true,
            Data = response.Data,
            Error = null
        });
        NotExistingUserCase: return StatusCode(StatusCodes.Status500InternalServerError, new HttpResponseData<SuccessfulLoginRespModel>()
        {
            Success = false,
            Data = null,
            Error = val.ToString()
        });
        FailureCase: return StatusCode(StatusCodes.Status500InternalServerError, new HttpResponseData<SuccessfulLoginRespModel>()
        {
            Success = false,
            Data = null,
            Error = val.ToString()
        });
            
        }

        // GET api/<AuthenticationController>/5
        [HttpPost("fingerAuth")]
        public string FingerAuth()
        {
            return "value";
        }
    }
}
