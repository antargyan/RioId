// ==========================================================================
//  Squidex Headless CMS
// ==========================================================================
//  Copyright (c) Squidex UG (haftungsbeschraenkt)
//  All rights reserved. Licensed under the MIT license.
// ==========================================================================

using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using RioId.Models;

namespace RioId.Services
{
    public sealed class SettingsProvider : CachingProvider, ISettingsProvider
    {
        private readonly IOptions<SettingsData> defaults;

        public SettingsProvider(IMemoryCache cache, IHttpContextAccessor httpContextAccessor, IOptions<SettingsData> defaults)
            : base(cache, httpContextAccessor)
        {
            this.defaults = defaults;
        }

        public Task<SettingsData> GetSettingsAsync()
        {
            return GetOrAddAsync(nameof(SettingsProvider), async () =>
            {
                var result = new SettingsData();

                foreach (var property in result.GetType().GetProperties())
                {
                    if (property.GetValue(result) == null)
                    {
                        property.SetValue(result, property.GetValue(defaults.Value));
                    }
                }

                return await Task.FromResult(result);
            });
        }
    }
}
