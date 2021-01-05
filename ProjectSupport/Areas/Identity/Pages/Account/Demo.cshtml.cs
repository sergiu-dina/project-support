using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ProjectSupport.Areas.Identity.Data;

namespace ProjectSupport.Areas.Identity.Pages.Account
{
    [BindProperties]
    public class DemoModel : PageModel
    {
        private readonly SignInManager<AppUser> _signInManager;

        public DemoModel(SignInManager<AppUser> signInManager)
        {
            _signInManager = signInManager;
        }

        public InputModel Input1 { get; set; } = new InputModel()
        {
            Email = "demo@admin.com",
            Password = "parola1",
            IsSelected = false
        };

        public InputModel Input2 { get; set; } = new InputModel()
        {
            Email = "demo@manager.com",
            Password = "parola1",
            IsSelected = false
        };

        public InputModel Input3 { get; set; } = new InputModel()
        {
            Email = "demo@developer.com",
            Password = "parola1",
            IsSelected = false
        };

        public class InputModel
        {
            [EmailAddress]
            public string Email { get; set; }

            [DataType(DataType.Password)]
            public string Password { get; set; }

            public bool IsSelected { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (Input1.IsSelected == true)
            {
                Input1.Email = "demo@admin.com";
                Input1.Password = "parola1";
                var result = await _signInManager.PasswordSignInAsync(Input1.Email, Input1.Password, Input1.IsSelected, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return LocalRedirect(returnUrl);
                }
            }
            if (Input2.IsSelected == true)
            {
                Input2.Email = "demo@manager.com";
                Input2.Password = "parola1";
                var result = await _signInManager.PasswordSignInAsync(Input2.Email, Input2.Password, Input2.IsSelected, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return LocalRedirect(returnUrl);
                }
            }
            if (Input3.IsSelected == true)
            {
                Input3.Email = "demo@developer.com";
                Input3.Password = "parola1";
                var result = await _signInManager.PasswordSignInAsync(Input3.Email, Input3.Password, Input3.IsSelected, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return LocalRedirect(returnUrl);
                }
            }
            return Page();
        }
    }
}
