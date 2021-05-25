using ProjectSupport.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.ViewModels
{
    public class EditUserViewModel
    {
        public AppUser AppUser { get; set; }
        public string UserId { get; set; }
    }
}
