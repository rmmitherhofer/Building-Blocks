﻿using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Options;

namespace Zypher.Security.JwtSigningCredentials.AspNetCore;

public class JwtPostConfigureOptions : IPostConfigureOptions<JwtBearerOptions>
{
    private readonly IServiceProvider _serviceProvider;
    public JwtPostConfigureOptions(IServiceProvider serviceProvider) => _serviceProvider = serviceProvider;
    public void PostConfigure(string? name, JwtBearerOptions options)
    {
        options.TokenHandlers.Clear();
        options.TokenHandlers.Add(new JwtServiceValidationHandler(_serviceProvider));
    }
}
