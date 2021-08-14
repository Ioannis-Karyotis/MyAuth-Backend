using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MyAuth.Enums;
using MyAuth.Models.Data;
using MyAuth.Utils;
using MyAuth.Utils.Extentions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MyAuth.Services
{
    public class TxtFileValidatorService
    {
        private readonly ILogger<TxtFileValidatorService> _logger;

        public TxtFileValidatorService(ILogger<TxtFileValidatorService> logger)
        {
            _logger = logger;
        }

        public async Task<InternalDataTransfer<TxtFile>> WriteToNewTxt(string encryptedFaceDescriptor, Guid newId, string storeFolder, TxtFile existingFaceDecriptorFile = null)
        {

            await File.WriteAllTextAsync("WriteText.txt", encryptedFaceDescriptor);

            IFormFile txtReq = new FormFile(new System.IO.MemoryStream(), 0, 0, "name", "fileName.txt")
            {
                Headers = new HeaderDictionary(),
                ContentType = "text/plain"
            };

            

            bool txtValidator = true;

            if (txtReq != null)
            {
                var checker = txtReq.IsTxtFile();
                bool validChecker = checker.Status ? checker.Data : false;
                txtValidator = validChecker;
                if (!txtValidator && checker.Error != null)
                {
                    var theError = $"{checker.Error?.Error ?? ""} :DESC: {checker.Error?.Description ?? ""}";
                    _logger.LogError(theError);
                }
            }
            if (txtValidator)
            {
                try
                {
                    if (txtValidator && txtReq != null)
                    {
                        var finalTxt = await txtReq.TxtFileStore(
                                newId.ToString(),
                                encryptedFaceDescriptor,
                                storeFolder,
                                EnvVariablesRetriever.GetAppWebRootPath() ?? Path.Combine($"{Directory.GetCurrentDirectory()}", "wwwroot"),
                                existingFaceDecriptorFile
                            );

                        if (finalTxt != null && finalTxt.Status)
                        {
                            return finalTxt;
                        }
                        else
                        {
                            if (finalTxt.Error != null)
                            {
                                var theError = $"{finalTxt.Error?.Error ?? ""} :DESC: {finalTxt.Error?.Description ?? ""}";
                                _logger.LogError(theError);
                            }
                        }
                    }


                }
                catch (DbUpdateConcurrencyException e)
                {
                    throw;
                }
            }
            else if (!txtValidator)
            {
                return new InternalDataTransfer<TxtFile>(false, ClientsApiErrorCodes.NotValidPayload.GetName());
            }
            return new InternalDataTransfer<TxtFile>(false, ClientsApiErrorCodes.InternalError.GetName());
        }
    }
}
