using Microsoft.EntityFrameworkCore;
using MyAuth.Data;
using MyAuth.Enums;
using MyAuth.Models.ApiResponseModels;
using MyAuth.Models.Data;
using MyAuth.Models.Database;
using MyAuth.services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MyAuth.Services
{
    public class AccountService
    {
        private readonly ApplicationDbContext _context;
        private readonly AuthServices _authServices;

        public AccountService(
            ApplicationDbContext context,
            AuthServices authServices
        )
        {
            _context = context;
            _authServices = authServices;
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
                    AppName = s.ExternalApp.AppName,
                    AppUrl = s.ExternalApp.BaseUrl,
                    DateConnected = s.Created
                }).ToList()
            };
            
            return new HttpResponseData<UserConnectedAppsRespModel, ClientsApiErrorCodes>(resp);
        }
            
    }
}
