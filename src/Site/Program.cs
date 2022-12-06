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

    // Download a specific SHA of the theme, so updates to the theme won't disrupt the site.
    // This SHA is when https://github.com/statiqdev/CleanBlog/pull/23 was merged.
    .AddThemeFromUri(new Uri("https://github.com/statiqdev/CleanBlog/archive/6f532dce88b46ded2cc5262a352c3b30fa293e51.zip"))
    .AddWeb()
    .RunAsync()
    .ConfigureAwait(false);
