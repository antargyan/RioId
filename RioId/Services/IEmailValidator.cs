// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace RioId.Services
{
    public interface IEmailValidator
    {
        Task<IdentityResult> ValidateAsync(string email);
    }
}
