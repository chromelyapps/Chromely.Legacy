﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Chromely Projects">
//   Copyright (c) 2017-2019 Chromely Projects
// </copyright>
// <license>
//      See the LICENSE.md file in the project root for more information.
// </license>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using Chromely.CefSharp.Winapi.BrowserWindow;
using Chromely.Core;
using Chromely.Core.Host;
using Chromely.Core.Infrastructure;

namespace Chromely.CefSharp.Winapi.Demo
{
    /// <summary>
    /// The program.
    /// </summary>
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1400:AccessModifierMustBeDeclared", Justification = "Reviewed. Suppression is OK here.")]
    class Program
    {
        /// <summary>
        /// The main.
        /// </summary>
        /// <param name="args">
        /// The args.
        /// </param>
        /// <returns>
        /// The <see cref="int"/>.
        /// </returns>
        static int Main(string[] args)
        {
            try
            {
                HostHelpers.SetupDefaultExceptionHandlers();
                var appDirectory = AppDomain.CurrentDomain.BaseDirectory;

                /*
                * Start url (load html) options:
                */

                // Options 1 - real standard urls 
                //string startUrl = "https://google.com";

                // Options 2 - using local resource file handling with default/custom local scheme handler 
                // Requires - (sample) UseDefaultResourceSchemeHandler("local", string.Empty)
                //            or register new resource scheme handler - RegisterSchemeHandler("local", string.Empty,  new CustomResourceHandler())
                var startUrl = "local://app/chromely_frameless.html";

                // Options 3 - using file protocol - using default/custom scheme handler for Ajax/Http requests
                // Requires - (sample) UseDefaultResourceSchemeHandler("local", string.Empty)
                //            or register new resource handler - RegisterSchemeHandler("local", string.Empty,  new CustomResourceHandler())
                // Requires - (sample) UseDefaultHttpSchemeHandler("http", "chromely.com")
                //            or register new http scheme handler - RegisterSchemeHandler("http", "test.com",  new CustomHttpHandler())
                // var startUrl = $"file:///{appDirectory}app/chromely_frameless.html";
                var config = ChromelyConfiguration
                                .Create()
                                .WithHostMode(WindowState.Normal)
                                .WithHostFlag(HostFlagKey.Frameless, true)
                                //.WithHostFlag(HostFlagKey.CenterScreen, true | false - default true)
                                //.WithHostFlag(HostFlagKey.Frameless, true) // | false -  default false)
                               // .WithHostFlag(HostFlagKey.KioskMode, true) // | false - default false)
                                //.WithHostFlag(HostFlagKey.NoResize, true) // - default false)
                                //.WithHostFlag(HostFlagKey.NoMinMaxBoxes, true) // - default false)
                                .WithHostBounds(1200, 700)
                                .WithHostTitle("chromely")
                                .WithHostIconFile("chromely.ico")
                                .WithAppArgs(args)
                                .WithStartUrl(startUrl)
                                .WithLogSeverity(LogSeverity.Info)
                                .UseDefaultLogger()
                                .UseDefaultResourceSchemeHandler("local", string.Empty)
                                .UseDefaultHttpSchemeHandler("http", "chromely.com")
                                .UseDefaultJsHandler("boundControllerAsync", true);

                //var process = Process.GetCurrentProcess(); // Or whatever method you are using
                //string fullPath = process.MainModule.FileName;
                //Log.Error($"Path:{fullPath}");

                //string module = process.MainModule.ModuleName;
                //Log.Error($"Module:{module}");

                //Log.Error($"AppDir:{appDirectory}");
                //Log.Error($"WorkingDirectory:{System.Environment.CurrentDirectory}");
                //string wdir = @"C:\Projects\ProjectDurbar\ChromelySolution\src\Demos\Chromely.CefSharp.Winapi.Demo\bin\x64\Debug\net461";
                //Directory.SetCurrentDirectory(wdir);

                using (var window = ChromelyWindow.Create(config))
                {
                    // Register command url scheme
                    window.RegisterUrlScheme(new UrlScheme("https://github.com/chromelyapps/Chromely", UrlSchemeType.External));

                    // Register external url schemes
                    window.RegisterUrlScheme(new UrlScheme("http://command.com", UrlSchemeType.Command));

                    /*
                     * Register service assemblies
                     * Uncomment relevant part to register assemblies
                     */

                    // 1. Register current/local assembly:
                    window.RegisterServiceAssembly(Assembly.GetExecutingAssembly());

                    // 2. Register external assembly with file name:
                    var externalAssemblyFile = System.IO.Path.Combine(appDirectory, "Chromely.Service.Demo.dll");
                    window.RegisterServiceAssembly(externalAssemblyFile);

                    // 3. Register external assemblies with list of filenames:
                    // string serviceAssemblyFile1 = @"C:\ChromelyDlls\Chromely.Service.Demo.dll";
                    // List<string> filenames = new List<string>();
                    // filenames.Add(serviceAssemblyFile1);
                    // app.RegisterServiceAssemblies(filenames);

                    // 4. Register external assemblies directory:
                    // var serviceAssembliesFolder = @"C:\ChromelyDlls";
                    // window.RegisterServiceAssemblies(serviceAssembliesFolder);

                    // Scan assemblies for Controller routes 
                    window.ScanAssemblies();
                    return window.Run(args);
                }
            }
            catch (Exception exception)
            {
                Log.Error(exception);
            }

            return 0;
        }
    }
}