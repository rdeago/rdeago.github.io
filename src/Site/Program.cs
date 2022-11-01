// Copyright (c) Riccardo De Agostini. Licensed under the MIT license.
// See the LICENSE.md file in the project root for full license information.

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Devlead.Statiq.Themes;
using Microsoft.Extensions.Configuration;
using Statiq.App;
using Statiq.Common;
using Statiq.Web;

var metadataAttributes = Assembly
    .GetExecutingAssembly()
    .GetCustomAttributes()
    .OfType<AssemblyMetadataAttribute>();

var webDirectory = metadataAttributes
    .First(attr => attr.Key == "WebDirectory")
    .Value!;

var artifactsDirectory = metadataAttributes
    .First(attr => attr.Key == "ArtifactsDirectory")
    .Value!;

return await Bootstrapper
    .Factory
    .CreateDefaultWithout(args, DefaultFeatures.Settings)
    .BuildConfiguration(builder => builder
        .SetBasePath(webDirectory)
        .AddSettingsFile("appsettings")
        .AddSettingsFile("settings")
        .AddSettingsFile("statiq"))
    .ConfigureFileSystem(fileSystem =>
    {
        fileSystem.RootPath = webDirectory;
        fileSystem.CachePath = Path.Combine(artifactsDirectory, "cache");
        fileSystem.OutputPath = Path.Combine(artifactsDirectory, "output");
        fileSystem.TempPath = Path.Combine(artifactsDirectory, "temp");
    })

    // Download theme from temporary PR branch until https://github.com/statiqdev/CleanBlog/pull/23 is merged
    .AddThemeFromUri(new Uri("https://github.com/rdeago/Statiq-CleanBlog/archive/refs/heads/update-dependencies.zip"))
    .AddWeb()
    .RunAsync()
    .ConfigureAwait(false);
