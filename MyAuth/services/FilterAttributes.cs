using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MyAuth.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyAuth.services
{
    public class AuthAttributeServices : ActionFilterAttribute
    {
        public AuthAttributeServices()
        {

        }
        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            bool allowed = false;

            if (context.HttpContext.Items.ContainsKey("auth") && context.HttpContext.Request.Headers.ContainsKey("AuthGate"))
            {
                var validateObj = (AuthModel)context.HttpContext.Items["auth"];
                allowed = validateObj.ValidateExpiredAuthPeriod() && validateObj.IsValidGUIDMyID();
            }
          /*else if (context.HttpContext.Items.ContainsKey("auth") && context.HttpContext.Request.Headers.ContainsKey("AuthGate") && context.HttpContext.Request.Cookies.ContainsKey("AuthGate") && (thisCallIS != null ? thisCallIS.IsWEBMEMBER() : false))
            {
                var validateObj = (MobileAuthModel)context.HttpContext.Items["auth"];
                allowed = validateObj.ValidateExpiredAuthPeriod() && validateObj.IsValidGUIDMyID();
            }*/

            if (allowed)
            {
                await next();
            }
            else
            {
                context.Result = new UnauthorizedResult();
            }           
        }
    }
}
