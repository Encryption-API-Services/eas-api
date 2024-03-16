using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Models.UserAdmin
{
    public class UserAdminStatusRequest
    {
        public string UserId { get; set; }
        public bool IsAdmin { get; set; }
    }
}
