using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MyAuth.Enums;
using MyAuth.Models.ApiResponseModels;
using MyAuth.Models.Data;
using MyAuth.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MyAuth.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly AccountService _accountService;
        public AccountController(
            AccountService accountService
        )
        {
            _accountService = accountService;
        }

        [HttpGet("user-details")]
        [EnableCors]
        public async Task<ActionResult<HttpResponseData<UserDetailsRespModel, ClientsApiErrorCodes>>> UserDetails ()
        {
            HttpResponseData<UserDetailsRespModel, ClientsApiErrorCodes> response = await _accountService.GetUserDetails();

            if (response.Success == true)
            {
                return Ok(response);
            }

            ClientsApiErrorCodes val = response.Error.ErrorCode;

            switch (val)
            {
                case ClientsApiErrorCodes.InternalError:
                    goto FailureCase;
                case ClientsApiErrorCodes.Unauthorized:
                    goto UnauthorzsedCase;
            }

        FailureCase:
            var result = new ObjectResult(new HttpResponseData<UserDetailsRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.InternalError));
            result.StatusCode = 500;
            return result;
        UnauthorzsedCase:
            var result2 = new ObjectResult(new HttpResponseData<UserDetailsRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.Unauthorized));
            result2.StatusCode = 401;
            return result2;
        }
        
        [HttpGet("user-connected-apps")]
        [EnableCors]
        public async Task<ActionResult<HttpResponseData<UserConnectedAppsRespModel, ClientsApiErrorCodes>>> UserConnectedApps()
        {
            HttpResponseData<UserConnectedAppsRespModel, ClientsApiErrorCodes> response = await _accountService.GetUserConnectedApps();

            if (response.Success == true)
            {
                return Ok(response);
            }

            ClientsApiErrorCodes val = response.Error.ErrorCode;

            switch (val)
            {
                case ClientsApiErrorCodes.InternalError:
                    goto FailureCase;
                case ClientsApiErrorCodes.Unauthorized:
                    goto UnauthorzsedCase;
            }

        FailureCase:
            var result = new ObjectResult(new HttpResponseData<UserConnectedAppsRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.InternalError));
            result.StatusCode = 500;
            return result;
        UnauthorzsedCase:
            var result2 = new ObjectResult(new HttpResponseData<UserConnectedAppsRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.Unauthorized));
            result2.StatusCode = 401;
            return result2;
        }


    }  
}
