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
    public sealed class ProfileModel : ManagePageModelBase<ProfileModel>
    {
        [BindProperty]
        public ChangeProfileInputModel Input { get; set; }

        public bool IsEmailConfirmed { get; set; }

        public async Task<IActionResult> OnGetAsync()
        {
            Input = new ChangeProfileInputModel { Email = UserInfo.Email };

            IsEmailConfirmed = await UserManager.IsEmailConfirmedAsync(UserInfo);

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                if (!string.Equals(Input.Email, UserInfo.Email, StringComparison.OrdinalIgnoreCase))
                {
                    var result = await UserManager.SetEmailAsync(UserInfo, Input.Email);

                    if (!result.Succeeded)
                    {
                        throw new ApplicationException($"Unexpected error occurred setting email for user with ID '{UserInfo.Id}'.");
                    }

                    StatusMessage = T["ProfileUpdated"];
                }

                return RedirectToPage();
            }

            return Page();
        }

        public IActionResult OnPostSendVerificationEmail()
        {
            if (ModelState.IsValid)
            {
                StatusMessage = T["VerificationEmailSent"];

                return RedirectToPage();
            }

            return Page();
        }
    }

    public sealed class ChangeProfileInputModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = nameof(Email))]
        public string Email { get; set; }
    }
}
