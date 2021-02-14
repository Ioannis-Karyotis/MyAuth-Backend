using Jose;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyAuth.Data;
using MyAuth.Enums;
using MyAuth.Models.ApiResponseModels;
using MyAuth.Models.ConfigurationModels;
using MyAuth.Models.Data;
using MyAuth.Models.Database;
using MyAuth.Models.RequestModels;
using MyAuth.Utils;
using MyAuth.Utils.Extentions;
using MyAuth.Utils.HttpClients;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MyAuth.services
{
    public class AuthServices
    {
        private readonly AuthPasswordHash _authPassword;
        private readonly EncrypterHashAuth _encrypterDecrypter;
        private readonly IHttpContextAccessor _actionContext;
        private readonly SignInManager<MyAuthUser> _signInManager;
        private readonly UserManager<MyAuthUser> _userManager;
        private readonly ILogger<AuthServices> _logger;
        private readonly RequestValidatorPartsHelper _requestValidatorPartsHelper;
        private readonly FlaskFaceAuthServices _flaskFaceAuthServices;

        public AuthServices(
            IOptions<AuthPasswordHash> authPassword,
            IHttpContextAccessor actionContext,
            SignInManager<MyAuthUser> signInManager,
            UserManager<MyAuthUser> userManager,
            ILogger<AuthServices> logger,
            RequestValidatorPartsHelper requestValidatorPartsHelper,
            IOptions<FlaskFaceAuthServiceConfig> flaskFaceAuth,
            FlaskFaceAuthServices flaskFaceAuthServices
            )
        {
            _authPassword = authPassword.Value;
            _encrypterDecrypter = new EncrypterHashAuth(authPassword.Value.PasswordHash);
            _actionContext = actionContext;
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
            _requestValidatorPartsHelper = requestValidatorPartsHelper;
            _flaskFaceAuthServices = flaskFaceAuthServices;
        }
        public AuthModel GetAthenticatedByMiddlewareInfo()
        {
            //already checked renewed from Middleware and by the filter if the filter is used at endpoint
            if (!_actionContext.HttpContext.Items.ContainsKey("auth"))
            {
                return null;
            }
            var res = _actionContext.HttpContext.Items["auth"];
            return res != null ? (AuthModel)res : null;
        }


        public async Task<HttpResponseData<SuccessfulLoginRespModel, ClientsApiErrorCodes>> DoLoginUser(LoginRequestModel Input)
        {

            var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, false , lockoutOnFailure: false);
            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in.");
                var existingUser = await _userManager.FindByEmailAsync(Input.Email);


                AuthModel userAuth = new AuthModel();
                userAuth.ID = existingUser.Id.ToString();

                //its wrong not hours but days 30 default TODO: Change It
                userAuth.ValidUntil = DateTime.Now.AddMinutes(30);

                var finalEncrypted = _encrypterDecrypter.EncryptObject<AuthModel>(userAuth);
                string hash = _requestValidatorPartsHelper.CombineAndSaveHash(existingUser.Email, Guid.Parse(existingUser.Id));

                _actionContext.HttpContext.Response.Headers.Add("X-AUTH-DASH", finalEncrypted);

                var internalRequest = new SuccessfulLoginRespModel()
                {

                    AuthToken = finalEncrypted,
                    X_Seq = hash,
                    Id = existingUser.Id,
                    DateCreated = DateTime.Now,
                    DateExpired = DateTime.Now.AddMinutes(30)
                };

                return new HttpResponseData<SuccessfulLoginRespModel, ClientsApiErrorCodes>(internalRequest);
            }
            /*if (result.RequiresTwoFactor)
            {
                return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = Input.RememberMe });
            }
            if (result.IsLockedOut)
            {
                _logger.LogWarning("User account locked out.");
                return RedirectToPage("./Lockout");
            }*/
            else
            {
                return new HttpResponseData<SuccessfulLoginRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.NotExistingUser);
            }
        }

        public async Task<HttpResponseData<SuccessfulLoginRespModel, ClientsApiErrorCodes>> FacialRecognition(LoginFacialRequestModel Input)
        {
            //Check if the hash thats was in headers returns null
            if (_requestValidatorPartsHelper.RetrieveValidateDiscardHash(Input.X_seq))
            {
                List<string> hashValues = _requestValidatorPartsHelper.RetrieveHashValues(Input.X_seq);

                if (hashValues == null)
                {
                    return new HttpResponseData<SuccessfulLoginRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.Unauthorized);
                }

                string? id = hashValues[0];

                if (id != null)
                {
                    //Do Facial stuff
                    _logger.LogInformation("User logged in.");
                    var existingUser = await _userManager.FindByIdAsync(id);

                    if (existingUser == null)
                    {
                        return new HttpResponseData<SuccessfulLoginRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.Unauthorized);
                    }

                    var base64Img = Input.Base64Img.Replace("data:image/jpeg;base64,", "");

                    var response = await _flaskFaceAuthServices.IdentifyUser(new FlaskFaceAuthIdentifyUserRequestModel() {Base64Img =  base64Img });

                    if (response.Success == false)
                    {
                        return new HttpResponseData<SuccessfulLoginRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.FlaskFaceAuthInternalError);
                    }

                    if (response.Success ==  true && response.Data.IsMatch == false)
                    {
                        return new HttpResponseData<SuccessfulLoginRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.BiometricAuthenticationFailure);
                    }

                    AuthModel userAuth = new AuthModel();
                    userAuth.ID = existingUser.Id.ToString();

                    //its wrong not hours but days 30 default TODO: Change It
                    userAuth.ValidUntil = DateTime.Now.AddMinutes(30);

                    var finalEncrypted = _encrypterDecrypter.EncryptObject<AuthModel>(userAuth);

                    _actionContext.HttpContext.Response.Headers.Add("X-AUTH-DASH", finalEncrypted);

                    var internalRequest = new SuccessfulLoginRespModel()
                    {
                        AuthToken = finalEncrypted,
                        Id = existingUser.Id,
                        DateCreated = DateTime.Now,
                        DateExpired = DateTime.Now.AddMinutes(30)
                    };

                    return new HttpResponseData<SuccessfulLoginRespModel, ClientsApiErrorCodes>(internalRequest);
                }
                else
                {
                    return new HttpResponseData<SuccessfulLoginRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.Unauthorized);
                }
            }
            else
            {
                return new HttpResponseData<SuccessfulLoginRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.UnauthorizedApplication);
            }
        }

        public async Task<HttpResponseData<SuccessfulRegisterRespModel, ClientsApiErrorCodes>> DoSignupUser(RegisterReqModel newUser)
        {
            var user = new MyAuthUser()
            {
                UserName = newUser.Email,
                Name = newUser.Name,
                Surname = newUser.Surname,
                Email = newUser.Email
            };
            var result = await _userManager.CreateAsync(user, newUser.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");
                return new HttpResponseData<SuccessfulRegisterRespModel, ClientsApiErrorCodes>();
                /*var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));*/
            }
            else
            {
                return new HttpResponseData<SuccessfulRegisterRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.InternalError);
            }
            /*var callbackUrl = Url.Page(
                "/Account/ConfirmEmail",
                pageHandler: null,
                values: new { area = "Identity", userId = user.Id, code = code, returnUrl = returnUrl },
                protocol: Request.Scheme);*/

            /* await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                 $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");*/

            /*if (_userManager.Options.SignIn.RequireConfirmedAccount)
            {
                return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
            }
            else
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                return LocalRedirect(returnUrl);
            }*/
        }

        public AuthModel GetAuthModelFromBearerValue(string val) => _encrypterDecrypter.DecryptObject<AuthModel>(val);

        public void SetNewBearer(AuthModel userAuth)
        {
            if (userAuth == null)
                return;
            var finalEncrypted = _encrypterDecrypter.EncryptObject<AuthModel>(userAuth);

            _actionContext.HttpContext.Response.Headers.Add("X-AUTH-DASH", finalEncrypted);
        }

        public class EncrypterHashAuth
        {
            private readonly string _mainPass;
            public EncrypterHashAuth(string pass)
            {
                _mainPass = pass;
            }

            public string EncryptObject<T>(T objectJson)
            {
                try
                {
                    return Jose.JWT.Encode(JsonConvert.SerializeObject(objectJson), _mainPass, JweAlgorithm.PBES2_HS256_A128KW, JweEncryption.A256CBC_HS512);
                }
                catch (Exception)
                {
                    return null;
                }
            }

            public T DecryptObject<T>(string json) where T : class
            {
                try
                {
                    return JsonConvert.DeserializeObject<T>(Jose.JWT.Decode(json, _mainPass));
                }
                catch (Exception e)
                {
                    return null;
                }
            }

            public string EncryptString(string val)
            {
                try
                {
                    return Jose.JWT.Encode(val, _mainPass, JweAlgorithm.PBES2_HS256_A128KW, JweEncryption.A256CBC_HS512);
                }
                catch (Exception)
                {
                    return null;
                }
            }

            public string DecryptString(string val)
            {
                try
                {
                    return Jose.JWT.Decode(val, _mainPass);
                }
                catch (Exception e)
                {
                    return null;
                }
            }


            public string MD5Hash(string text)
            {
                try
                {
                    MD5 md5 = new MD5CryptoServiceProvider();

                    //compute hash from the bytes of text  
                    md5.ComputeHash(Encoding.UTF8.GetBytes(text));

                    //get hash result after compute it  
                    byte[] result = md5.Hash;

                    StringBuilder strBuilder = new StringBuilder();
                    for (int i = 0; i < result.Length; i++)
                    {
                        //change it into 2 hexadecimal digits  
                        //for each byte  
                        strBuilder.Append(result[i].ToString("x2"));
                    }

                    return strBuilder.ToString();
                }
                catch (Exception)
                {
                    return null;
                }

            }

        }
    }
}
