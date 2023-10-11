// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

namespace RioId.Services
{
    public interface IEmailSender
    {
        Task SendEmailConfirmationAsync(string email, string link);

        Task SendResetPasswordAsync(string email, string link);
    }
}
