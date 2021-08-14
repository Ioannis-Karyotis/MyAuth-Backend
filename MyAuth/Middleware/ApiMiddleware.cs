using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using MyAuth.Models.ConfigurationModels;
using MyAuth.services;
using MyAuth.Services;
using MyAuth.Utils.Extentions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyAuth.Middleware
{
    public class ApiMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ApiMiddleware> _logger;
        private readonly AuthPasswordHash _hashKey;
        private readonly AppApiKeys _apiKeys;
        private readonly string _apiPath;
        private readonly string _mobilesWebApiLogout;
        private readonly string _externalResourcesPath;

        public ApiMiddleware(RequestDelegate next, ILogger<ApiMiddleware> logger, IOptions<AppApiKeys> apiKeys, IOptions<AuthPasswordHash> hashKey)
        {
            _next = next;
            _logger = logger;
            _hashKey = hashKey.Value;
            _apiKeys = apiKeys.Value;
            _apiPath = "api";
            _externalResourcesPath = "accounts/oauth2";
        }

        private bool IsForAPI(string endpoint) => endpoint.ToLower().Contains(_apiPath);
        private bool IsForExternalResources(string endpoint) => endpoint.ToLower().Contains(_externalResourcesPath);
        private bool IsForMobileWebAppLogout(string endpoint) => endpoint.ToLower().Contains(_mobilesWebApiLogout);

        private ActionResult UnauthorizedAppResponse() => new UnauthorizedResult();
        private ActionResult SuccessWebLogoutResponse() => new OkResult();

        private async void WriteJsonResponseAsync<T>(HttpContext context, int statusCode, T jsonObject)
        {
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsync(JsonConvert.SerializeObject(jsonObject), Encoding.UTF8);
        }

        private void RenewAuthBearerToken(HttpContext httpContext, AuthServices authServices)
        {
            if (httpContext.Request.Headers.TryGetValue("AuthGate", out StringValues bearrer))
            {
                if (bearrer.Count() > 0 && _hashKey.PasswordHash != null)
                {
                    if (bearrer.Last().CheckAllCasesIsNotNull())
                    {
                        var finalbearrer = bearrer.Last().Replace("Bearer", "");
                        finalbearrer = finalbearrer.Replace(" ", "");

                        var objAuth = authServices.GetAuthModelFromBearerValue(finalbearrer);
                        if (objAuth != null && objAuth.ValidateExpiredAuthPeriod())
                        {
                            if (objAuth.CheckAndUpdateRenewValidUntil())
                            {
                                authServices.SetNewBearer(objAuth);
                                if (!httpContext.Items.ContainsKey("auth"))
                                {
                                    httpContext.Items.Add("auth", objAuth);
                                }
                                else
                                {
                                    httpContext.Items["auth"] = objAuth;
                                }
                            }
                        }
                        else
                        {
                            if (httpContext.Items.ContainsKey("auth"))
                            {
                                httpContext.Items.Remove("auth");
                            }
                        }
                    }
                }
            }
        } 
        
        private void VerifyExternalAuthBearerToken(HttpContext httpContext, ExternalAuthService externalAuthService)
        {
            if (httpContext.Request.Headers.TryGetValue("AuthBearer", out StringValues bearrer))
            {
                if (bearrer.Count() > 0 && _hashKey.PasswordHash != null)
                {
                    if (bearrer.Last().CheckAllCasesIsNotNull())
                    {
                        var finalbearrer = bearrer.Last().Replace("Bearer_", "");
                        finalbearrer = finalbearrer.Replace(" ", "");

                        var objAuth = externalAuthService.GetAccessTokenFromBearerValue(finalbearrer);
                        if (objAuth != null && objAuth.ValidateExpiredAuthPeriod())
                        {
                            if (objAuth.CheckAndUpdateRenewValidUntil())
                            {
                                //authServices.SetNewBearer(objAuth);
                                if (!httpContext.Items.ContainsKey("external_auth"))
                                {
                                    httpContext.Items.Add("external_auth", objAuth);
                                }
                                else
                                {
                                    httpContext.Items["external_auth"] = objAuth;
                                }
                            }
                        }
                        else
                        {
                            if (httpContext.Items.ContainsKey("external_auth"))
                            {
                                httpContext.Items.Remove("external_auth");
                            }
                        }
                    }
                }
            }
        }

        

        /*private void SetOnRequestDeviceFlag(DevicesEnum enumUser, HttpContext httpContext)
        {
            if (!httpContext.Items.ContainsKey("DeviceEnum"))
            {
                httpContext.Items.Add("DeviceEnum", new DeviceEnumData(enumUser));
            }
            else
            {
                httpContext.Items["DeviceEnum"] = new DeviceEnumData(enumUser);
            }
        }*/

        public async Task Invoke(HttpContext httpContext, AuthServices authMobileServices , ExternalAuthService externalAuthService)
        {
            var endpoint = httpContext.Request.Path.Value;

            if (IsForAPI(endpoint))
            {
                if (httpContext.Request.Headers.TryGetValue("X-API-KEY", out StringValues apiKeyHeader))
                {
                    if (apiKeyHeader.Count() > 0)
                    {
                        if (_apiKeys.AvailableKeys.Count() > 0 && _apiKeys.AvailableKeys.Contains(apiKeyHeader.Last()))
                        {
                            goto NormalAct;
                        }
                    }
                }
                goto UnauthorizedAppResponseAct;
            }
            else if (IsForExternalResources(endpoint))
            {
                goto ExternalResourcesCase;
            }
            goto NormalNonApiAct;


        UnauthorizedAppResponseAct:
            {
                WriteJsonResponseAsync(httpContext, 401, UnauthorizedAppResponse());
                await Task.FromResult(0);
                return;
            }

        NormalAct:
            {
                RenewAuthBearerToken(httpContext, authMobileServices);
                //SetOnRequestDeviceFlag(DevicesEnum.MOBILEMEMBER, httpContext);
                await _next(httpContext);
                return;
            }
        
        ExternalResourcesCase:
            {
                VerifyExternalAuthBearerToken(httpContext, externalAuthService);
                await _next(httpContext);
                return;
            }

        NormalNonApiAct:
            {
                await _next(httpContext);
                return;
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class DevicesMiddlewareExtensions
    {
        public static IApplicationBuilder UseDevicesMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ApiMiddleware>();
        }
    }
}

