using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MyAuth.Data;
using MyAuth.Enums;
using MyAuth.Models.ApiResponseModels;
using MyAuth.Models.Data;
using MyAuth.Models.RequestModels;
using MyAuth.services;
using MyAuth.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyAuth.Controllers
{
    [Route("accounts/oauth2")]
    [ApiController]
    public class ExternalAuthenticationController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly ExternalAuthService _externalAuthService;
        public ExternalAuthenticationController(
            ApplicationDbContext context,
            ExternalAuthService externalAuthService)
        {
            _context = context;
            _externalAuthService = externalAuthService;
        }

        [HttpGet("verify-source")]
        public ActionResult VerifySource([FromQuery]string response_type, [FromQuery] string client_id , [FromQuery] string redirect_uri, [FromQuery] string scope, [FromQuery] string state)
        {
            string queryParamsPart = $"?response_type={response_type}&client_id={client_id}&redirect_uri={redirect_uri}&scope={scope}&state={state}";
            return Redirect("http://localhost:4200/accounts/external-auth/oauth/verify-client" + queryParamsPart);
        }


        [HttpPost("external/sign-in")]
        [EnableCors]
        public async Task<ActionResult<HttpResponseData<SuccessfulLoginFirstStepRespModel, ClientsApiErrorCodes>>> Signin([FromBody] ExternalLoginRequestModel input)
        {
            HttpResponseData<SuccessfulLoginFirstStepRespModel, ClientsApiErrorCodes> response = await _externalAuthService.DoExternalLoginUser(input);

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



        [HttpPost("external/facial/authentication")]
        [EnableCors]
        public async Task<ActionResult<HttpResponseData<ExternalSuccessfulLoginRespModel, ClientsApiErrorCodes>>> SigninFacialRecognition([FromBody] LoginFacialRequestModel input)
        {
            HttpResponseData<ExternalSuccessfulLoginRespModel, ClientsApiErrorCodes> response = await _externalAuthService.ExternalFacialRecognition(input);

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

        NotExistingUserCase: return StatusCode(StatusCodes.Status500InternalServerError, new HttpResponseData<ExternalSuccessfulLoginRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.InvalidCredentials));
        BiometricAuthenticationFailureCase: return StatusCode(StatusCodes.Status500InternalServerError, new HttpResponseData<ExternalSuccessfulLoginRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.BiometricAuthenticationFailure));
        FlaskInternalErrorCase: return StatusCode(StatusCodes.Status500InternalServerError, new HttpResponseData<ExternalSuccessfulLoginRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.FlaskFaceAuthInternalError));

        }

        [HttpPost("external/sign-in/auth/token")]
        [EnableCors]
        public async Task<ActionResult<HttpResponseData<ExternalSuccessfulLoginRespModel, ClientsApiErrorCodes>>> SigninAuthToken([FromBody] LoginFacialRequestModel input)
        {
            HttpResponseData<ExternalSuccessfulLoginRespModel, ClientsApiErrorCodes> response = await _externalAuthService.ExternalFacialRecognition(input);

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

        NotExistingUserCase: return StatusCode(StatusCodes.Status500InternalServerError, new HttpResponseData<ExternalSuccessfulLoginRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.InvalidCredentials));
        BiometricAuthenticationFailureCase: return StatusCode(StatusCodes.Status500InternalServerError, new HttpResponseData<ExternalSuccessfulLoginRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.BiometricAuthenticationFailure));
        FlaskInternalErrorCase: return StatusCode(StatusCodes.Status500InternalServerError, new HttpResponseData<ExternalSuccessfulLoginRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.FlaskFaceAuthInternalError));

        }

        [HttpPost("verify-code")]
        public async Task<ActionResult<HttpResponseData<ExternalAccessTokenRespModel, ClientsApiErrorCodes>>>  VerifyCode(VerifyCodeRequestModel req)
        {

            HttpResponseData<ExternalAccessTokenRespModel, ClientsApiErrorCodes> response = await _externalAuthService.VerifyCode(req.grant_type, req.code , req.redirect_uri, req.client_id);

            if (response.Success == true)
            {
                return Ok(response);
            }

            ClientsApiErrorCodes val = response.Error.ErrorCode;

            switch (val)
            {
                case ClientsApiErrorCodes.NotValidPayload:
                    goto NotValidPayloadCase;
                case ClientsApiErrorCodes.Unauthorized:
                    goto UnauthorizedCase;
            }

        UnauthorizedCase: return StatusCode(StatusCodes.Status500InternalServerError, new HttpResponseData<ExternalSuccessfulLoginRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.Unauthorized));
        NotValidPayloadCase: return StatusCode(StatusCodes.Status500InternalServerError, new HttpResponseData<ExternalSuccessfulLoginRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.NotValidPayload));
        }
        
        [HttpGet("user-info")]
        public async Task<ActionResult<HttpResponseData<ExternalUserInfoRespModel, ClientsApiErrorCodes>>>UserInfo()
        {
            HttpResponseData<ExternalUserInfoRespModel, ClientsApiErrorCodes> response = await _externalAuthService.GetUserInfo();

            if (response.Success == true)
            {
                return Ok(response);
            }

            ClientsApiErrorCodes val = response.Error.ErrorCode;

            switch (val)
            {
                case ClientsApiErrorCodes.NotValidPayload:
                    goto NotValidPayloadCase;
                case ClientsApiErrorCodes.Unauthorized:
                    goto UnauthorizedCase;
            }

        UnauthorizedCase: return StatusCode(StatusCodes.Status500InternalServerError, new HttpResponseData<ExternalUserInfoRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.Unauthorized));
        NotValidPayloadCase: return StatusCode(StatusCodes.Status500InternalServerError, new HttpResponseData<ExternalUserInfoRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.NotValidPayload));
        }



    }
}
