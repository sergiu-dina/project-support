using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.Areas.Identity.Data
{
    interface IUserData
    {
        IEnumerable<AppUser> GetAll();
        AppUser Get(string email);
        void Delete(string id);
    }
}
