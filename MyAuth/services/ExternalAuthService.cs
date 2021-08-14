using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using MyAuth.Data;
using MyAuth.Enums;
using MyAuth.Models.ApiResponseModels;
using MyAuth.Models.ConfigurationModels;
using MyAuth.Models.Data;
using MyAuth.Models.Database;
using MyAuth.Models.RequestModels;
using MyAuth.services;
using MyAuth.Utils;
using MyAuth.Utils.Extentions;
using Newtonsoft.Json;
using Numpy;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using static MyAuth.services.AuthServices;

namespace MyAuth.Services
{
    public class ExternalAuthService
    {
        private readonly IHttpContextAccessor _actionContext;
        private readonly AuthServices _authServices;
        private readonly AuthPasswordHash _authPassword;
        private readonly EncrypterHashAuth _encrypterDecrypter;
        private readonly ApplicationDbContext _context;
        private readonly RequestValidatorPartsHelper _requestValidatorPartsHelper;

        public ExternalAuthService(
            IOptions<AuthPasswordHash> authPassword,
            ApplicationDbContext context,
            AuthServices authServices,
            IHttpContextAccessor actionContext,
            RequestValidatorPartsHelper requestValidatorPartsHelper)
        {
            _context = context;
            _authServices = authServices;
            _authPassword = authPassword.Value;
            _requestValidatorPartsHelper = requestValidatorPartsHelper;
            _encrypterDecrypter = new EncrypterHashAuth(authPassword.Value.PasswordHash);
            _actionContext = actionContext;
        }

        public ExternalAccessTokenModel GetAccessTokenFromBearerValue(string val) => _encrypterDecrypter.DecryptObject<ExternalAccessTokenModel>(val);

        public void SetNewAccessToken(ExternalAccessTokenModel userAuth)
        {
            if (userAuth == null)
                return;
            var finalEncrypted = _encrypterDecrypter.EncryptObject<ExternalAccessTokenModel>(userAuth);

            _actionContext.HttpContext.Response.Headers.Add("Access_Token", finalEncrypted);
        }

        public ExternalAccessTokenModel GetAccessTokenByMiddlewareInfo()
        {
            //already checked renewed from Middleware and by the filter if the filter is used at endpoint
            if (!_actionContext.HttpContext.Items.ContainsKey("external_auth"))
            {
                return null;
            }
            var res = _actionContext.HttpContext.Items["external_auth"];
            return res != null ? (ExternalAccessTokenModel)res : null;
        }

        public async Task<HttpResponseData<SuccessfulLoginFirstStepRespModel, ClientsApiErrorCodes>> DoExternalLoginUser(ExternalLoginRequestModel input)
        {

            var verifySource = await VerifySource(input.Response_type, input.Client_id, input.Redirect_uri, input.Scope);
            if (verifySource.Status == false)
            {
                return new HttpResponseData<SuccessfulLoginFirstStepRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.InternalError);
            }

            MyAuthUser existingUser = await _context.MyAuthUsers.Where(d => d.Email == input.Email).FirstOrDefaultAsync();

            if (existingUser != null)
            {
                if (existingUser.Password == _encrypterDecrypter.MD5Hash(input.Password))
                {
                    List<string> hashValues = new List<string>();
                    hashValues.Add(input.Email);
                    hashValues.Add(input.Response_type);
                    hashValues.Add(input.Client_id);
                    hashValues.Add(input.Redirect_uri);
                    hashValues.Add(input.Scope);
                    hashValues.Add(input.State);

                    string hash = _requestValidatorPartsHelper.CombineAndSaveHashListValues(hashValues, existingUser.Id);

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

        public async Task<HttpResponseData<ExternalSuccessfulLoginRespModel, ClientsApiErrorCodes>> ExternalFacialRecognition(LoginFacialRequestModel input)
        {

            if (_requestValidatorPartsHelper.RetrieveValidateDiscardHash(input.X_seq))
            {
                List<string> hashValues = _requestValidatorPartsHelper.RetrieveHashValues(input.X_seq);

                if (hashValues == null)
                {
                    return new HttpResponseData<ExternalSuccessfulLoginRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.Unauthorized);
                }

                string? id = hashValues[0];

                var verifySource = await VerifySource(hashValues[2], hashValues[3], hashValues[4], hashValues[5]);
                if (verifySource.Status == false)
                {
                    return new HttpResponseData<ExternalSuccessfulLoginRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.InternalError);
                }

                if (id != null)
                {

                    var existingUser = await _context.MyAuthUsers.Where(d => d.Id == new Guid(id)).FirstOrDefaultAsync();

                    if (existingUser == null)
                    {
                        return new HttpResponseData<ExternalSuccessfulLoginRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.Unauthorized);
                    }

                    if (existingUser.FaceDescriptor == null || existingUser.HasFaceRegistered == false)
                    {
                        return new HttpResponseData<ExternalSuccessfulLoginRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.InvalidCredentials);
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
                    float ex = (float)np.linalg.norm(P2 - P1);

                    if (ex >= 0.4)
                    {
                        return new HttpResponseData<ExternalSuccessfulLoginRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.BiometricAuthenticationFailure);
                    }

                    return await RedirectAuthCode(existingUser, hashValues[3], hashValues[4], hashValues[6]);
                }
                else
                {
                    return new HttpResponseData<ExternalSuccessfulLoginRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.Unauthorized);
                }
            }
            else
            {
                return new HttpResponseData<ExternalSuccessfulLoginRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.UnauthorizedApplication);
            }
        }

        public async Task<HttpResponseData<ExternalAccessTokenRespModel, ClientsApiErrorCodes>> VerifyCode(string grant_type, string code, string redirect_uri, string client_id)
        {
            if (grant_type != "authorization_code")
            {
                return new HttpResponseData<ExternalAccessTokenRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.NotValidPayload);
            }

            ExternalCodeModel codeModel = _encrypterDecrypter.DecryptObject<ExternalCodeModel>(code);

            if (codeModel.Client_Id != client_id || codeModel.Redirect_Uri != redirect_uri)
            {
                return new HttpResponseData<ExternalAccessTokenRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.NotValidPayload);
            }

            var externalAppClient = await _context.ExternalApps.Where(d => d.ClientId == client_id && redirect_uri.Contains(d.CallbackUrl) && new Guid(codeModel.ID) == d.MyAuthUserId).FirstOrDefaultAsync();

            if (externalAppClient == null)
            {
                return new HttpResponseData<ExternalAccessTokenRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.NotValidPayload);
            }

            if (codeModel.ValidUntil < DateTime.Now)
            {
                return new HttpResponseData<ExternalAccessTokenRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.Unauthorized);
            }

            ExternalAccessTokenModel access_token = new ExternalAccessTokenModel()
            {
                ID = externalAppClient.ClientId,
                ValidUntil = DateTime.Now.AddMinutes(5)
            };

            ExternalAccessTokenRespModel resp = new ExternalAccessTokenRespModel()
            {
                access_token = _encrypterDecrypter.EncryptObject<ExternalAccessTokenModel>(access_token),
                expires_in = "300",
                token_type = "bearer"
            };

            return new HttpResponseData<ExternalAccessTokenRespModel, ClientsApiErrorCodes>(resp);
        }

        public async Task<HttpResponseData<ExternalSuccessfulLoginRespModel, ClientsApiErrorCodes>> RedirectAuthCode (MyAuthUser existingUser,string client_id, string client_uri, string state)
        {
            ExternalCodeModel userCode = new ExternalCodeModel();
            userCode.ID = existingUser.Id.ToString();
            userCode.Client_Id = client_id;
            userCode.Redirect_Uri = client_uri;

            userCode.ValidUntil = DateTime.Now.AddMinutes(5);

            var finalEncrypted = _encrypterDecrypter.EncryptObject<ExternalCodeModel>(userCode);

            var internalRequest = new ExternalSuccessfulLoginRespModel()
            {
                AuthCode = finalEncrypted,
                State = state,
                Client_Url = client_uri
            };

            return new HttpResponseData<ExternalSuccessfulLoginRespModel, ClientsApiErrorCodes>(internalRequest);
        }

        public async Task<InternalDataTransfer<bool?>> VerifySource(string response_type, string client_id, string redirect_uri, string scope)
        {
            if (response_type != "code" || scope != "profile")
            {
                return new InternalDataTransfer<bool?>(false, ClientsApiErrorCodes.NotValidPayload.GetName());
            }

            var externalAppClient = await _context.ExternalApps.Where(d => d.ClientId == client_id && redirect_uri.StartsWith(d.CallbackUrl)).FirstOrDefaultAsync();

            if (externalAppClient == null)
            {
                return new InternalDataTransfer<bool?>(false, ClientsApiErrorCodes.NotValidPayload.GetName());
            }

            return new InternalDataTransfer<bool?>(true);
        }

        public async Task<HttpResponseData<ExternalUserInfoRespModel, ClientsApiErrorCodes>> GetUserInfo()
        {
            ExternalAccessTokenModel accessToken = GetAccessTokenByMiddlewareInfo();

            if (accessToken == null)
            {
                return new HttpResponseData<ExternalUserInfoRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.Unauthorized);
            }
            ExternalApp externalApp = await _context.ExternalApps.Where(d => d.ClientId == accessToken.ID).FirstOrDefaultAsync();

            if (externalApp == null)
            {
                return new HttpResponseData<ExternalUserInfoRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.InternalError);
            }

            MyAuthUser existingUser = await _context.MyAuthUsers.Where(d => d.Id == externalApp.MyAuthUserId).FirstOrDefaultAsync();

            if (existingUser == null)
            {
                return new HttpResponseData<ExternalUserInfoRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.InternalError);
            }


            ExternalUserInfoRespModel resp = new ExternalUserInfoRespModel()
            {
                email = existingUser.Email,
                id = accessToken.ID
            };
            

            return new HttpResponseData<ExternalUserInfoRespModel, ClientsApiErrorCodes>(resp);
        }
    }
}
