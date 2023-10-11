using Duende.IdentityServer.Services;
using Duende.IdentityServer.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Localization;
using RioId;
using RioId.Models;
using RioId.Services;
using IEmailSender = RioId.Services.IEmailSender;

namespace rioid.Pages.Register
{
    //[SecurityHeaders]
    [AllowAnonymous]
    public class Index : PageModel
    {
        private readonly IEmailSender emailSender;
        private readonly IEmailValidator emailValidator;

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly IIdentityServerInteractionService _interaction;
        private readonly IEventService _events;
        private readonly IAuthenticationSchemeProvider _schemeProvider;
        private readonly IIdentityProviderStore _identityProviderStore;
        private readonly Lazy<IStringLocalizer<AppResources>> localizer;
        private readonly Lazy<ISettingsProvider> settings;

        public IStringLocalizer<AppResources> T
        {
            get { return localizer.Value; }
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public Index(
            IIdentityServerInteractionService interaction,
            IAuthenticationSchemeProvider schemeProvider,
            IIdentityProviderStore identityProviderStore,
            IEventService events,
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _interaction = interaction;
            _schemeProvider = schemeProvider;
            _identityProviderStore = identityProviderStore;
            _events = events;
            SetupService(ref localizer);
            SetupService(ref settings);
        }

        public ISettingsProvider Settings
        {
            get { return settings.Value; }
        }

        [BindProperty(SupportsGet = true)]
        public string ReturnUrl { get; set; }

        public string TermsOfServiceUrl { get; set; }

        public string PrivacyPolicyUrl { get; set; }

        public bool MustAcceptsTermsOfService { get; set; }

        public bool MustAcceptsPrivacyPolicy { get; set; }

        private void SetupService<TService>(ref Lazy<TService> value)
        {
#pragma warning disable RECS0002 // Convert anonymous method to method group
            value = new Lazy<TService>(() => HttpContext.RequestServices.GetRequiredService<TService>());
#pragma warning restore RECS0002 // Convert anonymous method to method group
        }

        public async override Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            var settings = await Settings.GetSettingsAsync();

            if (!string.IsNullOrWhiteSpace(settings.PrivacyPolicyUrl))
            {
                PrivacyPolicyUrl = settings.PrivacyPolicyUrl;

                MustAcceptsPrivacyPolicy = true;
            }

            if (!string.IsNullOrWhiteSpace(settings.TermsOfServiceUrl))
            {
                TermsOfServiceUrl = settings.TermsOfServiceUrl;

                MustAcceptsTermsOfService = true;
            }

            await next();
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (MustAcceptsPrivacyPolicy && !Input.AcceptPrivacyPolicy)
            {
                var field = nameof(Input.AcceptPrivacyPolicy);

                ModelState.AddModelError($"{nameof(Input)}.{field}", T[$"{field}Error"]);
            }

            if (MustAcceptsTermsOfService && !Input.AcceptTermsOfService)
            {
                var field = nameof(Input.AcceptTermsOfService);

                ModelState.AddModelError($"{nameof(Input)}.{field}", T[$"{field}Error"]);
            }
            //TODO
            //var emailCheck = await emailValidator.ValidateAsync(Input.Email);

            //if (emailCheck.Errors.Any())
            //{
            //    foreach (var error in emailCheck.Errors)
            //    {
            //        ModelState.AddModelError(error.Code, error.Description);
            //    }
            //}

            if (ModelState.IsValid)
            {
                var user = new ApplicationUser()
                {
                    UserName = Input.Email,
                    Email = Input.Email
                };

                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    var callbackCode = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.EmailConfirmationLink(user.Id.ToString(), callbackCode, Request.Scheme);
                    //TODO: to fix
                    //await emailSender.SendEmailConfirmationAsync(Input.Email, callbackUrl);

                    await _signInManager.SignInAsync(user, false);

                    return RedirectTo(ReturnUrl);
                }
                foreach (IdentityError err in result.Errors)
                {
                    ModelState.AddModelError(err.Code, err.Description);
                }
            }

            return Page();
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
    }
}