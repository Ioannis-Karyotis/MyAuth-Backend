using Jose;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MyAuth.Data;
using MyAuth.Enums;
using MyAuth.Models.ApiResponseModels;
using MyAuth.Models.ConfigurationModels;
using MyAuth.Models.Data;
using MyAuth.Models.Database;
using MyAuth.Models.RequestModels;
using MyAuth.Services;
using MyAuth.Utils;
using MyAuth.Utils.Extentions;
using MyAuth.Utils.HttpClients;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;
using Newtonsoft.Json.Linq;
using Numpy;

namespace MyAuth.services
{
    public class AuthServices
    {
        private readonly AuthPasswordHash _authPassword;
        private readonly EncrypterHashAuth _encrypterDecrypter;
        private readonly IHttpContextAccessor _actionContext;
        private readonly ILogger<AuthServices> _logger;
        private readonly RequestValidatorPartsHelper _requestValidatorPartsHelper;
        private readonly FlaskFaceAuthServices _flaskFaceAuthServices;
        private readonly ApplicationDbContext _context;
        private readonly TxtFileValidatorService _txtService;

        public AuthServices(
            IOptions<AuthPasswordHash> authPassword,
            IHttpContextAccessor actionContext,
            ILogger<AuthServices> logger,
            RequestValidatorPartsHelper requestValidatorPartsHelper,
            IOptions<FlaskFaceAuthServiceConfig> flaskFaceAuth,
            FlaskFaceAuthServices flaskFaceAuthServices,
            ApplicationDbContext context,
            TxtFileValidatorService txtService
            )
        {
            _authPassword = authPassword.Value;
            _encrypterDecrypter = new EncrypterHashAuth(authPassword.Value.PasswordHash);
            _actionContext = actionContext;
            _logger = logger;
            _requestValidatorPartsHelper = requestValidatorPartsHelper;
            _flaskFaceAuthServices = flaskFaceAuthServices;
            _context = context;
            _txtService = txtService;
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


        public async Task<HttpResponseData<SuccessfulLoginFirstStepRespModel, ClientsApiErrorCodes>> DoLoginUser(LoginRequestModel Input)
        {
            MyAuthUser existingUser = await _context.MyAuthUsers.Where(d => d.Email == Input.Email).FirstOrDefaultAsync();

            if (existingUser != null)
            {
                if (existingUser.Password == _encrypterDecrypter.MD5Hash(Input.Password))
                {
                    _logger.LogInformation("User logged in.");

                    string hash = _requestValidatorPartsHelper.CombineAndSaveHash(existingUser.Email, existingUser.Id);

                    var internalRequest = new SuccessfulLoginFirstStepRespModel()
                    {
                        Id = existingUser.Id,
                        X_seq = hash
                    };

                    return new HttpResponseData<SuccessfulLoginFirstStepRespModel, ClientsApiErrorCodes>(internalRequest);
                }
            }
            return new HttpResponseData<SuccessfulLoginFirstStepRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.InvalidCredentials);

        }


        public async Task<HttpResponseData<SuccessfulLoginRespModel, ClientsApiErrorCodes>> FacialRecognition(LoginFacialRequestModel input)
        {
           
            if (_requestValidatorPartsHelper.RetrieveValidateDiscardHash(input.X_seq))
            {
                List<string> hashValues = _requestValidatorPartsHelper.RetrieveHashValues(input.X_seq);

                if (hashValues == null)
                {
                    return new HttpResponseData<SuccessfulLoginRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.Unauthorized);
                }

                string? id = hashValues[0];

                if (id != null)
                {

                    var existingUser = await _context.MyAuthUsers.Where(d => d.Id == new Guid(id)).FirstOrDefaultAsync();

                    if (existingUser == null)
                    {
                        return new HttpResponseData<SuccessfulLoginRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.Unauthorized);
                    }

                    if (existingUser.FaceDescriptor == null || existingUser.HasFaceRegistered == false)
                    {
                        return new HttpResponseData<SuccessfulLoginRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.InvalidCredentials);
                    }

                    string encryptedFaceDescriptor;
                    using (StreamReader streamReader = new StreamReader(Path.Combine(existingUser.FaceDescriptor.Path, existingUser.FaceDescriptor.FileName)))
                    {
                        encryptedFaceDescriptor = streamReader.ReadLine();
                    }

                    string decryptedFaceDescriptor = _encrypterDecrypter.DecryptString(encryptedFaceDescriptor);

                    float[] actualFaceDescriptor = JsonConvert.DeserializeObject<float[]>(decryptedFaceDescriptor);
                    float[] comparingFaceDescriptor = JsonConvert.DeserializeObject<float[]>(input.FaceDescriptor);


                    var P1 = np.array(actualFaceDescriptor);
                    var P2 = np.array(comparingFaceDescriptor);
                    float ex = (float) np.linalg.norm(P2 - P1);

                    if (ex >= 0.4)
                    {
                        return new HttpResponseData<SuccessfulLoginRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.BiometricAuthenticationFailure);
                    }

                    return await DoActualUserlogin(existingUser);
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

        public async Task<HttpResponseData<SuccessfulLoginRespModel, ClientsApiErrorCodes>> DoSignupUser(RegisterReqModel newUser)
        {
            if (!newUser.HasValidPayload())
            {
                return new HttpResponseData<SuccessfulLoginRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.NotValidPayload);
            }

            try
            {

                MyAuthUser UserExists = await _context.MyAuthUsers.Where(d => d.Email == newUser.Email).FirstOrDefaultAsync();
                if (UserExists == null)
                {
                    string encryptedFaceDescriptor = _encrypterDecrypter.EncryptString(newUser.FaceDescriptor);
                    Guid newUserId = Guid.NewGuid(); 

                    var txtFile = await _txtService.WriteToNewTxt(encryptedFaceDescriptor, newUserId, "FaceDescriptors");

                    if (txtFile.Status == false)
                    {
                        return new HttpResponseData<SuccessfulLoginRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.InternalError);
                    }

                    var newUserRecord = new MyAuthUser()
                    {
                        Id = newUserId,
                        Name = newUser.Name,
                        Surname = newUser.Surname,
                        Email = newUser.Email,
                        Created = DateTime.Now,
                        HasFaceRegistered = true,
                        FaceDescriptor = txtFile.Data,
                        Password = _encrypterDecrypter.MD5Hash(newUser.Password)
                    };

                    _context.MyAuthUsers.Add(newUserRecord);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("User created a new account with password.");

                    return await DoActualUserlogin(newUserRecord);
                }
                return new HttpResponseData<SuccessfulLoginRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.AlreadyExistingUser);

            }
            catch (Exception e)
            {
                return new HttpResponseData<SuccessfulLoginRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.InternalError);
            }
           
        }


        //public async Task<HttpResponseData<SuccessfulLoginRespModel, ClientsApiErrorCodes>> FacialRegistration(LoginFacialRequestModel input)
        //{
        //    if (_requestValidatorPartsHelper.RetrieveValidateDiscardHash(input.X_seq))
        //    {
        //        List<string> hashValues = _requestValidatorPartsHelper.RetrieveHashValues(input.X_seq);

        //        if (hashValues == null)
        //        {
        //            return new HttpResponseData<SuccessfulLoginRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.Unauthorized);
        //        }

        //        string? id = hashValues[0];

        //        if (id != null)
        //        {

        //            var existingUser = await _context.MyAuthUsers.Where(d => d.Id == new Guid(id)).FirstOrDefaultAsync();

        //            if (existingUser == null)
        //            {
        //                return new HttpResponseData<SuccessfulLoginRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.Unauthorized);
        //            }

        //            if (existingUser.FaceDescriptor != null || existingUser.HasFaceRegistered == true)
        //            {
        //                return new HttpResponseData<SuccessfulLoginRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.InternalError);
        //            }

        //            string encryptedFaceDescriptor = _encrypterDecrypter.EncryptString(input.FaceDescriptor);

        //            var txtFile = await _txtService.WriteToNewTxt(encryptedFaceDescriptor, new Guid(id), "FaceDescriptors");

        //            if (txtFile.Status == false)
        //            {
        //                return new HttpResponseData<SuccessfulLoginRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.InternalError);
        //            }

        //            existingUser.FaceDescriptor = txtFile.Data;
        //            existingUser.HasFaceRegistered = true;

        //            _context.MyAuthUsers.Update(existingUser);
        //            await _context.SaveChangesAsync();

        //            return await DoActualUserlogin(existingUser);
        //        }
        //        else
        //        {
        //            return new HttpResponseData<SuccessfulLoginRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.Unauthorized);
        //        }
        //    }
        //    else
        //    {
        //        return new HttpResponseData<SuccessfulLoginRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.UnauthorizedApplication);
        //    }
        //}

        public async Task<HttpResponseData<SuccessfulLoginRespModel, ClientsApiErrorCodes>> DoActualUserlogin(MyAuthUser existingUser)
        {
            AuthModel userAuth = new AuthModel();
            userAuth.ID = existingUser.Id.ToString();

            userAuth.ValidUntil = DateTime.Now.AddMinutes(30);

            var finalEncrypted = _encrypterDecrypter.EncryptObject<AuthModel>(userAuth);

            //_actionContext.HttpContext.Response.Headers.Add("X-AUTH-DASH", finalEncrypted);

            var internalRequest = new SuccessfulLoginRespModel()
            {
                AuthToken = finalEncrypted,
                Id = existingUser.Id,
                DateCreated = DateTime.Now,
                DateExpired = DateTime.Now.AddMinutes(30)
            };

            return new HttpResponseData<SuccessfulLoginRespModel, ClientsApiErrorCodes>(internalRequest);
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
            
            public string EncryptFloat(float val)
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

            public string DecryptFloat(string val)
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
