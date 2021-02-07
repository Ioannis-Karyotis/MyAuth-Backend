using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyAuth.Models.Data
{
    public class AuthModel
    {
        public string ID { get; set; }
        public DateTime ValidUntil { get; set; }

        public AuthModel()
        {
        }

        public AuthModel(string id, int availableDays = 30)
        {
            ID = id;
            ValidUntil = DateTime.Now.AddDays(availableDays);
        }

        public void RenewValidUntilDate(int availableDays = 30)
        {
            ValidUntil = DateTime.Now.AddDays(availableDays);
        }

        public bool ValidateExpiredAuthPeriod(int availableDays = 30)
        {
            if (ValidUntil != null)
            {
                return ValidUntil > DateTime.Now || (ValidUntil < DateTime.Now && (DateTime.Now - ValidUntil).TotalDays <= availableDays);
            }
            return false;
        }

        public bool IsValidGUIDMyID() => Guid.TryParse(ID, out Guid guidOutput);

        public bool CheckAndUpdateRenewValidUntil(int availableDays = 30)
        {
            if (!ValidateExpiredAuthPeriod(availableDays))
                return false;
            RenewValidUntilDate(availableDays);
            return true;
        }
    }
}
