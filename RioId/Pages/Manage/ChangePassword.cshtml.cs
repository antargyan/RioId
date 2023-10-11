// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Microsoft.AspNetCore.Mvc;
using RioId.Extensions;
using System.ComponentModel.DataAnnotations;

#pragma warning disable SA1649 // File name should match first type name

namespace RioId.Pages.Manage
{
    public sealed class ChangePasswordModel : ManagePageModelBase<ChangePasswordModel>
    {
        [BindProperty]
        public ChangePasswordInputModel Input { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            if (!await UserManager.HasPasswordAsync(UserInfo))
            {
                return RedirectToPage("./SetPassword");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.ChangePasswordAsync(UserInfo, Input.OldPassword, Input.NewPassword);

                if (result.Succeeded)
                {
                    await SignInManager.SignInAsync(UserInfo, false);

                    StatusMessage = T["PasswordChanged"];

                    return RedirectToPage();
                }

                ModelState.AddModelErrors(result);
            }

            return Page();
        }
    }

    public sealed class ChangePasswordInputModel
    {
        [Required]
        [Display(Name = nameof(OldPassword))]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [Display(Name = nameof(NewPassword))]
        public string NewPassword { get; set; }

        [Compare(nameof(NewPassword), ErrorMessage = "PasswordsNotSame")]
        [Display(Name = nameof(ConfirmPassword))]
        public string ConfirmPassword { get; set; }
    }
}
