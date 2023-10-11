// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Microsoft.AspNetCore.Mvc.Filters;
using RioId.Models;

namespace RioId.Extensions
{
    public abstract class ManagePageModelBase<TDerived> : PageModelBase<TDerived>
    {
        public ApplicationUser UserInfo { get; set; }

        public override async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            UserInfo = await GetUserAsync();

            await base.OnPageHandlerExecutionAsync(context, next);
        }
    }
}
