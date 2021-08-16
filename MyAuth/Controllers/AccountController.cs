using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MyAuth.Enums;
using MyAuth.Models.ApiResponseModels;
using MyAuth.Models.Data;
using MyAuth.Models.RequestModels;
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
        
        [HttpPost("change-user-details")]
        [EnableCors]
        public async Task<ActionResult<HttpResponseData<bool?, ClientsApiErrorCodes>>> ChangeUserDetails([FromBody] ChangeDetailsRequestModel req)
        {
            HttpResponseData<bool?, ClientsApiErrorCodes> response = await _accountService.ChangeUserDetails(req);

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
            var result = new ObjectResult(new HttpResponseData<bool?, ClientsApiErrorCodes>(ClientsApiErrorCodes.InternalError));
            result.StatusCode = 500;
            return result;
        UnauthorzsedCase:
            var result2 = new ObjectResult(new HttpResponseData<bool?, ClientsApiErrorCodes>(ClientsApiErrorCodes.Unauthorized));
            result2.StatusCode = 401;
            return result2;
        }
        
        [HttpPost("change-user-password")]
        [EnableCors]
        public async Task<ActionResult<HttpResponseData<bool?, ClientsApiErrorCodes>>> ChangeUserPassword([FromBody] ChangePassRequestModel req)
        {
            HttpResponseData<bool?, ClientsApiErrorCodes> response = await _accountService.ChangeUserPassword(req);

            if (response.Success == true)
            {
                return Ok(response);
            }

            ClientsApiErrorCodes val = response.Error.ErrorCode;

            switch (val)
            {
                case ClientsApiErrorCodes.InternalError:
                    goto FailureCase;
                case ClientsApiErrorCodes.NotValidPayload:
                    goto NotValidPayloadCase;
                case ClientsApiErrorCodes.Unauthorized:
                    goto UnauthorzsedCase;
            }

        FailureCase:
            var result = new ObjectResult(new HttpResponseData<bool?, ClientsApiErrorCodes>(ClientsApiErrorCodes.InternalError));
            result.StatusCode = 500;
            return result;
        NotValidPayloadCase:
            var result2 = new ObjectResult(new HttpResponseData<bool?, ClientsApiErrorCodes>(ClientsApiErrorCodes.NotValidPayload));
            result2.StatusCode = 500;
            return result2;
        UnauthorzsedCase:
            var result3 = new ObjectResult(new HttpResponseData<bool?, ClientsApiErrorCodes>(ClientsApiErrorCodes.Unauthorized));
            result3.StatusCode = 401;
            return result3;
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

        [HttpPost("disconnect-app/{id}")]
        [EnableCors]
        public async Task<ActionResult<HttpResponseData<bool?, ClientsApiErrorCodes>>> DisconnectApp([FromRoute] Guid id)
        {
            HttpResponseData<bool?, ClientsApiErrorCodes> response = await _accountService.DisconnectApp(id);

            if (response.Success == true)
            {
                return Ok(response);
            }

            ClientsApiErrorCodes val = response.Error.ErrorCode;

            switch (val)
            {
                case ClientsApiErrorCodes.InternalError:
                    goto FailureCase;
                case ClientsApiErrorCodes.NotValidPayload:
                    goto NotValidPayloadCase;
                case ClientsApiErrorCodes.Unauthorized:
                    goto UnauthorzsedCase;
            }

        FailureCase:
            var result = new ObjectResult(new HttpResponseData<bool?, ClientsApiErrorCodes>(ClientsApiErrorCodes.InternalError));
            result.StatusCode = 500;
            return result;
        NotValidPayloadCase:
            var result2 = new ObjectResult(new HttpResponseData<bool?, ClientsApiErrorCodes>(ClientsApiErrorCodes.NotValidPayload));
            result2.StatusCode = 500;
            return result2;
        UnauthorzsedCase:
            var result3 = new ObjectResult(new HttpResponseData<bool?, ClientsApiErrorCodes>(ClientsApiErrorCodes.Unauthorized));
            result3.StatusCode = 401;
            return result3;
        }

        [HttpGet("my-apps")]
        [EnableCors]
        public async Task<ActionResult<HttpResponseData<UserConnectedAppsRespModel, ClientsApiErrorCodes>>> MyApps()
        {
            HttpResponseData<UserConnectedAppsRespModel, ClientsApiErrorCodes> response = await _accountService.MyApps();

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
        
        [HttpPost("add-new-app")]
        [EnableCors]
        public async Task<ActionResult<HttpResponseData<Guid, ClientsApiErrorCodes>>> AddApp([FromBody] AddAppRequestModel req)
        {
            HttpResponseData<Guid, ClientsApiErrorCodes> response = await _accountService.AddApp(req);

            if (response.Success == true)
            {
                return Ok(response);
            }

            ClientsApiErrorCodes val = response.Error.ErrorCode;

            switch (val)
            {
                case ClientsApiErrorCodes.InternalError:
                    goto FailureCase;
                case ClientsApiErrorCodes.AlreadyExistingApp:
                    goto AlreadyExistingAppCase;
                case ClientsApiErrorCodes.Unauthorized:
                    goto UnauthorzsedCase;
            }

        FailureCase:
            var result = new ObjectResult(new HttpResponseData<Guid, ClientsApiErrorCodes>(ClientsApiErrorCodes.InternalError));
            result.StatusCode = 500;
            return result;
        UnauthorzsedCase:
            var result2 = new ObjectResult(new HttpResponseData<Guid, ClientsApiErrorCodes>(ClientsApiErrorCodes.Unauthorized));
            result2.StatusCode = 401;
            return result2;
        AlreadyExistingAppCase:
            var result3 = new ObjectResult(new HttpResponseData<Guid, ClientsApiErrorCodes>(ClientsApiErrorCodes.AlreadyExistingApp));
            result3.StatusCode = 500;
            return result3;
        }
        
        [HttpGet("get-app-details/{id}")]
        [EnableCors]
        public async Task<ActionResult<HttpResponseData<AppDetailsRespModel, ClientsApiErrorCodes>>> AppDetails([FromRoute] Guid id)
        {
            HttpResponseData<AppDetailsRespModel, ClientsApiErrorCodes> response = await _accountService.AppDetails(id);

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
            var result = new ObjectResult(new HttpResponseData<AppDetailsRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.InternalError));
            result.StatusCode = 500;
            return result;
        UnauthorzsedCase:
            var result2 = new ObjectResult(new HttpResponseData<AppDetailsRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.Unauthorized));
            result2.StatusCode = 401;
            return result2;
        }
        
        [HttpPost("edit-app-details/{id}")]
        [EnableCors]
        public async Task<ActionResult<HttpResponseData<bool?, ClientsApiErrorCodes>>> EditAppDetails([FromBody] AddAppRequestModel req, [FromRoute] Guid id)
        {
            HttpResponseData<bool?, ClientsApiErrorCodes> response = await _accountService.EditAppDetails(req,id);

            if (response.Success == true)
            {
                return Ok(response);
            }

            ClientsApiErrorCodes val = response.Error.ErrorCode;

            switch (val)
            {
                case ClientsApiErrorCodes.InternalError:
                    goto FailureCase;
                case ClientsApiErrorCodes.AlreadyExistingApp:
                    goto AlreadyExistingAppCase;
                case ClientsApiErrorCodes.Unauthorized:
                    goto UnauthorzsedCase;
            }

        FailureCase:
            var result = new ObjectResult(new HttpResponseData<bool?, ClientsApiErrorCodes>(ClientsApiErrorCodes.InternalError));
            result.StatusCode = 500;
            return result;
        UnauthorzsedCase:
            var result2 = new ObjectResult(new HttpResponseData<bool?, ClientsApiErrorCodes>(ClientsApiErrorCodes.Unauthorized));
            result2.StatusCode = 401;
            return result2;
        AlreadyExistingAppCase:
            var result3 = new ObjectResult(new HttpResponseData<Guid, ClientsApiErrorCodes>(ClientsApiErrorCodes.AlreadyExistingApp));
            result3.StatusCode = 500;
            return result3;
        }
        
        [HttpPost("refresh-app-secrets/{id}")]
        [EnableCors]
        public async Task<ActionResult<HttpResponseData<bool?, ClientsApiErrorCodes>>> RefreshAppSecrets([FromRoute] Guid id)
        {
            HttpResponseData<bool?, ClientsApiErrorCodes> response = await _accountService.RefreshAppSecrets(id);

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
            var result = new ObjectResult(new HttpResponseData<bool?, ClientsApiErrorCodes>(ClientsApiErrorCodes.InternalError));
            result.StatusCode = 500;
            return result;
        UnauthorzsedCase:
            var result2 = new ObjectResult(new HttpResponseData<bool?, ClientsApiErrorCodes>(ClientsApiErrorCodes.Unauthorized));
            result2.StatusCode = 401;
            return result2;
        }


    }  
}
