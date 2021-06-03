using ProjectSupport.Areas.Identity.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProjectSupport.Models
{
    public class Notification
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public string UserId { get; set; }
        public AppUser User { get; set; }
        public bool IsRead { get; set; }
        public DateTime Created { get; set; }
        public bool IsSuccess { get; set; }
    }
}
