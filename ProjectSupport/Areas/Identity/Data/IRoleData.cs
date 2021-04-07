using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.Areas.Identity.Data
{
    public interface IRoleData
    {
        IEnumerable<IdentityRole> GetAll();
        IdentityRole Get(string id);
        void Delete(string id);
    }
}
