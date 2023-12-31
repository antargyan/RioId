﻿// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Duende.IdentityServer.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RioId.Models;
using RioId.Services;

namespace RioId.Extensions
{
    public abstract class PageModelBase<TDerived> : PageModel
    {
        private readonly Lazy<SignInManager<ApplicationUser>> signInManager;
        private readonly Lazy<UserManager<ApplicationUser>> userManager;
        private readonly Lazy<ILogger<TDerived>> logger;
        private readonly Lazy<ISettingsProvider> settings;
        private readonly Lazy<IStringLocalizer<AppResources>> localizer;
        private readonly Lazy<IEventService> events;

        public SignInManager<ApplicationUser> SignInManager
        {
            get { return signInManager.Value; }
        }

        public UserManager<ApplicationUser> UserManager
        {
            get { return userManager.Value; }
        }

        public ILogger<TDerived> Logger
        {
            get { return logger.Value; }
        }

        public ISettingsProvider Settings
        {
            get { return settings.Value; }
        }

        public IStringLocalizer<AppResources> T
        {
            get { return localizer.Value; }
        }

        public IEventService Events
        {
            get { return events.Value; }
        }

        [TempData]
        public string StatusMessage { get; set; }

        [TempData]
        public string ErrorMessage { get; set; }

        protected PageModelBase()
        {
            SetupService(ref events);
            SetupService(ref localizer);
            SetupService(ref logger);
            SetupService(ref settings);
            SetupService(ref signInManager);
            SetupService(ref userManager);
        }

        private void SetupService<TService>(ref Lazy<TService> value)
        {
#pragma warning disable RECS0002 // Convert anonymous method to method group
            value = new Lazy<TService>(() => HttpContext.RequestServices.GetRequiredService<TService>());
#pragma warning restore RECS0002 // Convert anonymous method to method group
        }

        public override void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            base.OnPageHandlerExecuting(context);
        }

        protected IActionResult RedirectTo(string returnUrl)
        {
            if (Uri.IsWellFormedUriString(returnUrl, UriKind.RelativeOrAbsolute))
            {
                return LocalRedirect(returnUrl);
            }
            else
            {
                return Redirect("~/");
            }
        }

        protected async Task<ApplicationUser> GetUserAsync()
        {
            var user = await UserManager.GetUserAsync(User);

            if (user == null)
            {
                throw new ApplicationException($"Unable to load user with ID '{UserManager.GetUserId(User)}'.");
            }

            return user;
        }
    }
}
