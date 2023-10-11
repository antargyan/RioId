using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RioId.Models;
using IEmailSender = RioId.Services.IEmailSender;

namespace RioId.Pages.ForgotPassword
{
    //TODO: to be enabled again after all work done
    //[SecurityHeaders]
    [AllowAnonymous]
    public class Index : PageModel
    {
        private readonly IEmailSender _emailSender;

        private readonly UserManager<ApplicationUser> _userManager;

        [BindProperty]
        public InputModel Input { get; set; }

        public Index(
            IEmailSender emailSender,
            UserManager<ApplicationUser> userManager)
        {
            _emailSender = emailSender;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);

                if (user != null && await _userManager.IsEmailConfirmedAsync(user))
                {
                    var callbackCode = await _userManager.GeneratePasswordResetTokenAsync(user);
                    var callbackUrl = Url.ResetPasswordCallbackLink(user.Id.ToString(), callbackCode, Request.Scheme);

                    await _emailSender.SendResetPasswordAsync(Input.Email, callbackUrl);
                }

                return RedirectToPage("/Account/ForgotPasswordConfirmation/Index");
            }

            return Page();
        }
    }
}