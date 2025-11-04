using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using Service.Domain;

namespace Service.Domain.ViewModels.LoginAndRegistration
{
    public class LoginViewModel
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }

}
