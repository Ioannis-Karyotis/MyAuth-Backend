using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyAuth.Enums;
using MyAuth.Filters;
using MyAuth.Models.ApiResponseModels;
using MyAuth.Models.Data;
using MyAuth.Models.RequestModels;
using MyAuth.services;
using System.Threading.Tasks;

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

        [HttpPost("sign-up")]
        [EnableCors]
        public async Task<ActionResult<HttpResponseData<SuccessfulLoginRespModel, ClientsApiErrorCodes>>> SignUp([FromBody] RegisterReqModel newUser)
        {
            HttpResponseData<SuccessfulLoginRespModel, ClientsApiErrorCodes> response = await _authServices.DoSignupUser(newUser);

            if (response.Success == true)
            {
                return Ok(response);
            }

            ClientsApiErrorCodes val = response.Error.ErrorCode;

            switch (val)
            {
                case ClientsApiErrorCodes.InternalError:
                    goto FailureCase;
                case ClientsApiErrorCodes.AlreadyExistingUser:
                    goto AlreadyExistingCase;
                case ClientsApiErrorCodes.NotValidPayload:
                    goto NotValidPayloadCase;
            }

        FailureCase:
            var result = new ObjectResult(new HttpResponseData<SuccessfulLoginRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.InternalError));
            result.StatusCode = 500;
            return result;
        AlreadyExistingCase:
            var result2 = new ObjectResult(new HttpResponseData<SuccessfulLoginRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.AlreadyExistingUser));
            result2.StatusCode = 500;
            return result2;
        NotValidPayloadCase:
            var result3 = new ObjectResult(new HttpResponseData<SuccessfulLoginRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.NotValidPayload));
            result3.StatusCode = 500;
            return result3;
        }

        //[HttpPost("facial/registration")]
        //[EnableCors]
        //public async Task<ActionResult<HttpResponseData<SuccessfulRegisterRespModel, ClientsApiErrorCodes>>> SignUpFacialRecognition([FromBody] LoginFacialRequestModel input)
        //{
        //    HttpResponseData<SuccessfulLoginRespModel, ClientsApiErrorCodes> response = await _authServices.FacialRegistration(input);

        //    if (response.Success == true)
        //    {
        //        return Ok(response);
        //    }

        //    ClientsApiErrorCodes val = response.Error.ErrorCode;

        //    switch (val)
        //    {
        //        case ClientsApiErrorCodes.InvalidCredentials:
        //            goto NotExistingUserCase;
        //        case ClientsApiErrorCodes.BiometricAuthenticationFailure:
        //            goto BiometricAuthenticationFailureCase;
        //        case ClientsApiErrorCodes.FlaskFaceAuthInternalError:
        //            goto FlaskInternalErrorCase;
        //    }

        //NotExistingUserCase: return StatusCode(StatusCodes.Status500InternalServerError, new HttpResponseData<SuccessfulLoginRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.InvalidCredentials));
        //BiometricAuthenticationFailureCase: return StatusCode(StatusCodes.Status500InternalServerError, new HttpResponseData<SuccessfulLoginRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.BiometricAuthenticationFailure));
        //FlaskInternalErrorCase: return StatusCode(StatusCodes.Status500InternalServerError, new HttpResponseData<SuccessfulLoginRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.FlaskFaceAuthInternalError));

        //}

        [HttpPost("sign-in")]
        [EnableCors]
        public async Task<ActionResult<HttpResponseData<SuccessfulLoginFirstStepRespModel, ClientsApiErrorCodes>>> Signin([FromBody] LoginRequestModel input)
        {
            HttpResponseData<SuccessfulLoginFirstStepRespModel, ClientsApiErrorCodes> response = await _authServices.DoLoginUser(input);

            if (response.Success == true)
            {
                return Ok(response);
            }

            ClientsApiErrorCodes val = response.Error.ErrorCode;

            switch (val)
            {
                case ClientsApiErrorCodes.InvalidCredentials:
                    goto NotExistingUserCase;
            }
            

        NotExistingUserCase: return StatusCode(StatusCodes.Status500InternalServerError, new HttpResponseData<SuccessfulLoginFirstStepRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.InvalidCredentials));

        }

        

        [HttpPost("facial/authentication")]
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
                case ClientsApiErrorCodes.InvalidCredentials:
                    goto NotExistingUserCase;
                case ClientsApiErrorCodes.BiometricAuthenticationFailure:
                    goto BiometricAuthenticationFailureCase;
                case ClientsApiErrorCodes.FlaskFaceAuthInternalError:
                    goto FlaskInternalErrorCase;
            }

        NotExistingUserCase: return StatusCode(StatusCodes.Status500InternalServerError, new HttpResponseData<SuccessfulLoginRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.InvalidCredentials));
        BiometricAuthenticationFailureCase: return StatusCode(StatusCodes.Status500InternalServerError, new HttpResponseData<SuccessfulLoginRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.BiometricAuthenticationFailure));
        FlaskInternalErrorCase: return StatusCode(StatusCodes.Status500InternalServerError, new HttpResponseData<SuccessfulLoginRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.FlaskFaceAuthInternalError));
            
        }

        // GET api/<AuthenticationController>/5
        [HttpPost("fingerAuth")]
        public string FingerAuth()
        {
            return "value";
        }
    }
}
