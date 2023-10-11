// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


using System.ComponentModel.DataAnnotations;

namespace rioid.Pages.Register
{
    public class InputModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = nameof(Email))]
        public string Email { get; set; }

        [Required]
        [Display(Name = nameof(Password))]
        public string Password { get; set; }

        [Compare(nameof(Password), ErrorMessage = "PasswordsNotSame")]
        [Display(Name = nameof(ConfirmPassword))]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = nameof(AcceptPrivacyPolicy))]
        public bool AcceptPrivacyPolicy { get; set; }

        [Required]
        [Display(Name = nameof(AcceptTermsOfService))]
        public bool AcceptTermsOfService { get; set; }
    }
}