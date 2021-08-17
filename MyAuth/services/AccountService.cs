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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static MyAuth.services.AuthServices;

namespace MyAuth.Services
{
    public class AccountService
    {
        private readonly ApplicationDbContext _context;
        private readonly AuthServices _authServices;
        private readonly AuthPasswordHash _authPassword;
        private readonly EncrypterHashAuth _encrypterDecrypter;

        public AccountService(
            ApplicationDbContext context,
            AuthServices authServices,
            IOptions<AuthPasswordHash> authPassword
        )
        {
            _context = context;
            _authServices = authServices;
            _encrypterDecrypter = new EncrypterHashAuth(authPassword.Value.PasswordHash);
        }

        public async Task<HttpResponseData<UserDetailsRespModel, ClientsApiErrorCodes>> GetUserDetails()
        {
            AuthModel authUser = _authServices.GetAthenticatedByMiddlewareInfo();
            if (authUser == null)
            {
                return new HttpResponseData<UserDetailsRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.Unauthorized);
            }

            MyAuthUser existingUser = await _context.MyAuthUsers.Where(d => d.Id == new Guid(authUser.ID)).FirstOrDefaultAsync();
            if (existingUser == null)
            {
                return new HttpResponseData<UserDetailsRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.InternalError);
            }

            UserDetailsRespModel resp = new UserDetailsRespModel()
            {
                Email = existingUser.Email,
                FirstName = existingUser.Name,
                LastName = existingUser.Surname
            };

            return new HttpResponseData<UserDetailsRespModel, ClientsApiErrorCodes>(resp);
        }
        
        public async Task<HttpResponseData<bool?, ClientsApiErrorCodes>> ChangeUserDetails(ChangeDetailsRequestModel req)
        {
            AuthModel authUser = _authServices.GetAthenticatedByMiddlewareInfo();
            if (authUser == null)
            {
                return new HttpResponseData<bool?, ClientsApiErrorCodes>(ClientsApiErrorCodes.Unauthorized);
            }

            MyAuthUser existingUser = await _context.MyAuthUsers.Where(d => d.Id == new Guid(authUser.ID)).FirstOrDefaultAsync();
            if (existingUser == null)
            {
                return new HttpResponseData<bool?, ClientsApiErrorCodes>(ClientsApiErrorCodes.InternalError);
            }

            existingUser.Name = req.FirstName;
            existingUser.Surname = req.LastName;

            _context.MyAuthUsers.Update(existingUser);
            await _context.SaveChangesAsync();
            return new HttpResponseData<bool?, ClientsApiErrorCodes>(true);
        }
        
        public async Task<HttpResponseData<bool?, ClientsApiErrorCodes>> ChangeUserPassword(ChangePassRequestModel req)
        {
            AuthModel authUser = _authServices.GetAthenticatedByMiddlewareInfo();
            if (authUser == null)
            {
                return new HttpResponseData<bool?, ClientsApiErrorCodes>(ClientsApiErrorCodes.Unauthorized);
            }

            MyAuthUser existingUser = await _context.MyAuthUsers.Where(d => d.Id == new Guid(authUser.ID)).FirstOrDefaultAsync();
            if (existingUser == null)
            {
                return new HttpResponseData<bool?, ClientsApiErrorCodes>(ClientsApiErrorCodes.InternalError);
            }

            if (existingUser.Password != _encrypterDecrypter.MD5Hash(req.OldPass) || req.NewPass != req.NewPassVal)
            {
                return new HttpResponseData<bool?, ClientsApiErrorCodes>(ClientsApiErrorCodes.NotValidPayload);
            }

            existingUser.Password = _encrypterDecrypter.MD5Hash(req.NewPass);

            _context.MyAuthUsers.Update(existingUser);
            await _context.SaveChangesAsync();
            return new HttpResponseData<bool?, ClientsApiErrorCodes>(true);
        }
               
        public async Task<HttpResponseData<UserConnectedAppsRespModel, ClientsApiErrorCodes>> GetUserConnectedApps()
        {
            AuthModel authUser = _authServices.GetAthenticatedByMiddlewareInfo();
            if (authUser == null)
            {
                return new HttpResponseData<UserConnectedAppsRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.Unauthorized);
            }

            List<ExternalAppAuthUser> existingConnections = await _context.ExternalAppsAuthUsers.Where(d => d.MyAuthUserId == new Guid(authUser.ID)).Include(d => d.ExternalApp).ToListAsync();

            UserConnectedAppsRespModel resp = new UserConnectedAppsRespModel()
            {
                ConnectedApps = existingConnections.Select(s => new AppConnectedApp()
                {
                    Id = s.Id,
                    AppName = s.ExternalApp.AppName,
                    AppUrl = s.ExternalApp.BaseUrl,
                    DateConnected = s.Created
                }).ToList()
            };
            
            return new HttpResponseData<UserConnectedAppsRespModel, ClientsApiErrorCodes>(resp);
        }

        public async Task<HttpResponseData<bool?, ClientsApiErrorCodes>> DisconnectApp(Guid Id)
        {
            AuthModel authUser = _authServices.GetAthenticatedByMiddlewareInfo();
            if (authUser == null)
            {
                return new HttpResponseData<bool?, ClientsApiErrorCodes>(ClientsApiErrorCodes.Unauthorized);
            }

            ExternalAppAuthUser connection = await _context.ExternalAppsAuthUsers.Where(d => d.Id == Id).FirstOrDefaultAsync();

            if (connection == null || connection.MyAuthUserId != new Guid(authUser.ID))
            {
                return new HttpResponseData<bool?, ClientsApiErrorCodes>(ClientsApiErrorCodes.NotValidPayload);
            }

            _context.ExternalAppsAuthUsers.Remove(connection);
            await _context.SaveChangesAsync();
            return new HttpResponseData<bool?, ClientsApiErrorCodes>(true);
        }

        public async Task<HttpResponseData<UserConnectedAppsRespModel, ClientsApiErrorCodes>> MyApps()
        {
            AuthModel authUser = _authServices.GetAthenticatedByMiddlewareInfo();
            if (authUser == null)
            {
                return new HttpResponseData<UserConnectedAppsRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.Unauthorized);
            }

            List<ExternalApp> myApps = await _context.ExternalApps.Where(d => d.MyAuthUserId == new Guid(authUser.ID)).Include(d => d.ExternalAppAuthUsers).ToListAsync();

            UserConnectedAppsRespModel resp = new UserConnectedAppsRespModel()
            {
                ConnectedApps = myApps.Select(s => new AppConnectedApp()
                {
                    Id = s.Id,
                    AppName = s.AppName,
                    AppUrl = s.BaseUrl,
                    DateConnected = s.Created
                }).ToList()
            };
            
            return new HttpResponseData<UserConnectedAppsRespModel, ClientsApiErrorCodes>(resp);
        }
        
        public async Task<HttpResponseData<Guid, ClientsApiErrorCodes>> AddApp(AddAppRequestModel req)
        {
            AuthModel authUser = _authServices.GetAthenticatedByMiddlewareInfo();
            if (authUser == null)
            {
                return new HttpResponseData<Guid, ClientsApiErrorCodes>(ClientsApiErrorCodes.Unauthorized);
            }

            ExternalApp existingApp = await _context.ExternalApps.Where(d => d.AppName == req.AppName || d.BaseUrl == req.BaseUrl || d.CallbackUrl == req.RedirectUrl).FirstOrDefaultAsync();
            if (existingApp != null)
            {
                return new HttpResponseData<Guid, ClientsApiErrorCodes>(ClientsApiErrorCodes.AlreadyExistingApp);
            }

            Guid newId = Guid.NewGuid();

            ExternalApp newApp = new ExternalApp()
            {
                Id = newId,
                MyAuthUserId = new Guid(authUser.ID),
                AppName = req.AppName,
                BaseUrl = req.BaseUrl,
                CallbackUrl = req.RedirectUrl,
                ClientId = string.Join("_","Client_ID",_encrypterDecrypter.EncryptString(string.Join("_", authUser.ID, DateTime.Now.ToString()))),
                ClientSecret = string.Join("_", "Client_SECRET", _encrypterDecrypter.EncryptString(string.Join("_", newId.ToString() , DateTime.Now.ToString()))),
                Created = DateTime.Now,
                LastUpdated = DateTime.Now
            };

            _context.ExternalApps.Add(newApp);
            await _context.SaveChangesAsync();

            return new HttpResponseData<Guid, ClientsApiErrorCodes>(newId);
        }

        public async Task<HttpResponseData<AppDetailsRespModel, ClientsApiErrorCodes>> AppDetails( Guid id)
        {
            AuthModel authUser = _authServices.GetAthenticatedByMiddlewareInfo();
            if (authUser == null)
            {
                return new HttpResponseData<AppDetailsRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.Unauthorized);
            }

            ExternalApp existingApp = await _context.ExternalApps.Where(d => d.Id == id).FirstOrDefaultAsync();
            if (existingApp == null)
            {
                return new HttpResponseData<AppDetailsRespModel, ClientsApiErrorCodes>(ClientsApiErrorCodes.NotValidPayload);
            }

            AppDetailsRespModel resp = new AppDetailsRespModel()
            {
                AppName = existingApp.AppName,
                BaseUrl = existingApp.BaseUrl,
                RedirectUrl = existingApp.CallbackUrl,
                ClientId = existingApp.ClientId,
                ClientSecret = existingApp.ClientSecret
            };

            return new HttpResponseData<AppDetailsRespModel, ClientsApiErrorCodes>(resp);

        }

        public async Task<HttpResponseData<bool?, ClientsApiErrorCodes>> EditAppDetails(AddAppRequestModel req, Guid id)
        {
            AuthModel authUser = _authServices.GetAthenticatedByMiddlewareInfo();
            if (authUser == null)
            {
                return new HttpResponseData<bool?, ClientsApiErrorCodes>(ClientsApiErrorCodes.Unauthorized);
            }

            ExternalApp existingApp = await _context.ExternalApps.Where(d => d.Id == id).FirstOrDefaultAsync();
            if (existingApp == null)
            {
                return new HttpResponseData<bool?, ClientsApiErrorCodes>(ClientsApiErrorCodes.NotValidPayload);
            }

            ExternalApp otherApp = await _context.ExternalApps.Where(d => (d.AppName == req.AppName || d.BaseUrl == req.BaseUrl || d.CallbackUrl == req.RedirectUrl) && d.Id != id).FirstOrDefaultAsync();
            if (otherApp != null)
            {
                return new HttpResponseData<bool?, ClientsApiErrorCodes>(ClientsApiErrorCodes.AlreadyExistingApp);
            }

            existingApp.AppName = req.AppName;
            existingApp.BaseUrl = req.BaseUrl;
            existingApp.CallbackUrl = req.RedirectUrl;

            _context.ExternalApps.Update(existingApp);
            await _context.SaveChangesAsync();

            return new HttpResponseData<bool?, ClientsApiErrorCodes>(true);

        }

        public async Task<HttpResponseData<bool?, ClientsApiErrorCodes>> RefreshAppSecrets(Guid id)
        {
            AuthModel authUser = _authServices.GetAthenticatedByMiddlewareInfo();
            if (authUser == null)
            {
                return new HttpResponseData<bool?, ClientsApiErrorCodes>(ClientsApiErrorCodes.Unauthorized);
            }

            ExternalApp existingApp = await _context.ExternalApps.Where(d => d.Id == id).FirstOrDefaultAsync();
            if (existingApp == null)
            {
                return new HttpResponseData<bool?, ClientsApiErrorCodes>(ClientsApiErrorCodes.NotValidPayload);
            }

            existingApp.ClientId = string.Join("_", "Client_ID", _encrypterDecrypter.EncryptString(string.Join("_", authUser.ID, DateTime.Now.ToString())));
            existingApp.ClientSecret = string.Join("_", "Client_SECRET", _encrypterDecrypter.EncryptString(string.Join("_", existingApp.Id.ToString(), DateTime.Now.ToString())));

            _context.ExternalApps.Update(existingApp);
            await _context.SaveChangesAsync();

            return new HttpResponseData<bool?, ClientsApiErrorCodes>(true);

        }

        public async Task<HttpResponseData<bool?, ClientsApiErrorCodes>> DeleteApp(Guid Id)
        {
            AuthModel authUser = _authServices.GetAthenticatedByMiddlewareInfo();
            if (authUser == null)
            {
                return new HttpResponseData<bool?, ClientsApiErrorCodes>(ClientsApiErrorCodes.Unauthorized);
            }

            ExternalApp app = await _context.ExternalApps.Where(d => d.Id == Id && d.MyAuthUserId == new Guid(authUser.ID)).FirstOrDefaultAsync();

            if (app == null || app.MyAuthUserId != new Guid(authUser.ID))
            {
                return new HttpResponseData<bool?, ClientsApiErrorCodes>(ClientsApiErrorCodes.NotValidPayload);
            }

            _context.ExternalApps.Remove(app);
            await _context.SaveChangesAsync();
            return new HttpResponseData<bool?, ClientsApiErrorCodes>(true);
        }

    }
}
