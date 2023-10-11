// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using RioId.Models;

namespace RioId.Services
{
    public interface ISettingsProvider
    {
        Task<SettingsData> GetSettingsAsync();
    }
}
