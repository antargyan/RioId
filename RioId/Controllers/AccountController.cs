// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using RioId.Models;

namespace RioId.Controllers
{
    public sealed class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> signInManager;
        private readonly IIdentityServerInteractionService interactions;

        public AccountController(SignInManager<ApplicationUser> signInManager, IIdentityServerInteractionService interactions)
        {
            this.signInManager = signInManager;
            this.interactions = interactions;
        }

        [HttpGet]
        [Route("/manage/")]
        public IActionResult Profile()
        {
            return Redirect("~/manage/profile");
        }

        [HttpGet]
        [Route("/")]
        public IActionResult Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                return Redirect("~/manage");
            }
            else
            {
                return Redirect("~/account/login");
            }
        }

        [Route("/account/logout/")]
        public async Task<IActionResult> Logout(string logoutId = null)
        {
            var context = await interactions.GetLogoutContextAsync(logoutId);

            await signInManager.SignOutAsync();

            if (!string.IsNullOrWhiteSpace(context?.PostLogoutRedirectUri))
            {
                return Redirect(context.PostLogoutRedirectUri);
            }
            else
            {
                return Redirect("~/account/login");
            }
        }
    }
}
