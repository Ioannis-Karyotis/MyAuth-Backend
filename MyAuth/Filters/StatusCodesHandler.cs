using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using MyAuth.Enums;
using MyAuth.Models.ApiResponseModels;
using MyAuth.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyAuth.Filters
{
    public class StatusCodesHandler : ActionFilterAttribute
    {
        public StatusCodesHandler()
        {
        }

        public override async Task OnResultExecutionAsync(ResultExecutingContext context, ResultExecutionDelegate next)
        {

            var result = context.Result;
            if (result is ObjectResult res)
            {
                var x = res.Value;
                HttpResponseData<dynamic, ClientsApiErrorCodes> respData = (HttpResponseData<dynamic, ClientsApiErrorCodes>)x;

                if (respData.Success == false)
                {
                    ClientsApiErrorCodes val = respData.Error.ErrorCode;

                    if (val == ClientsApiErrorCodes.Unauthorized || val == ClientsApiErrorCodes.UnauthorizedApplication) 
                    { 
                        context.HttpContext.Response.StatusCode = 401;
                    }
                    else
                    {
                        context.HttpContext.Response.StatusCode = 500;
                    }
                }
                
            }

            await next();

        }
    }
}
