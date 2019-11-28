﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Program.cs" company="Chromely Projects">
//   Copyright (c) 2017-2019 Chromely Projects
// </copyright>
// <license>
//      See the LICENSE.md file in the project root for more information.
// </license>
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Chromely.Core;
using Chromely.Core.Host;
using Chromely.Core.Infrastructure;
using WinApi.User32;

namespace Chromely.CefGlue.Winapi.Demo
{
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1400:AccessModifierMustBeDeclared", Justification = "Reviewed. Suppression is OK here.")]
    [SuppressMessage("StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented", Justification = "Reviewed. Suppression is OK here.")]
    class Program
    {
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
                // var startUrl = "https://google.com";

                // Options 2 - using local resource file handling with default/custom local scheme handler 
                // Requires - (sample) UseDefaultResourceSchemeHandler("local", string.Empty)
                //            or register new resource scheme handler - RegisterSchemeHandler("local", string.Empty,  new CustomResourceHandler())
                var startUrl = "local://app/chromely.html";

                // Options 3 - using file protocol - using default/custom scheme handler for Ajax/Http requests
                // Requires - (sample) UseDefaultResourceSchemeHandler("local", string.Empty)
                //            or register new resource handler - RegisterSchemeHandler("local", string.Empty,  new CustomResourceHandler())
                // Requires - (sample) UseDefaultHttpSchemeHandler("http", "chromely.com")
                //            or register new http scheme handler - RegisterSchemeHandler("http", "test.com",  new CustomHttpHandler())
                // var startUrl = $"file:///{appDirectory}app/chromely.html";

                // var windowStyle = new WindowCreationStyle();
                // windowStyle.WindowStyles = WindowStyles.WS_OVERLAPPEDWINDOW | WindowStyles.WS_CLIPCHILDREN | WindowStyles.WS_CLIPSIBLINGS;
                // windowStyle.WindowExStyles = WindowExStyles.WS_EX_APPWINDOW | WindowExStyles.WS_EX_WINDOWEDGE;

                var config = ChromelyConfiguration
                                .Create()
                                .WithHostMode(WindowState.Normal)
                                .WithHostTitle("chromely")
                                .WithHostIconFile("chromely.ico")
                                .WithAppArgs(args)
                                .WithHostBounds(1200, 700)
                                //.WithHostCustomStyle(windowStyle)
                                //.WithHostFlag(HostFlagKey.CenterScreen, true | false - default true)
                                //.WithHostFlag(HostFlagKey.Frameless, true | false -  default false)
                                //.WithHostFlag(HostFlagKey.KioskMode, true | false - default false)
                                //.WithHostFlag(HostFlagKey.NoResize, true | false - default false)
                                //.WithHostFlag(HostFlagKey.NoMinMaxBoxes, true | false - default false)
                                .WithAppArgs(args)
                                .WithStartUrl(startUrl)
                                .WithLogSeverity(LogSeverity.Info)
                                .UseDefaultResourceSchemeHandler("local", string.Empty)
                                .UseDefaultHttpSchemeHandler("http", "chromely.com");
                                //.UseDefaultWebsocketHandler(string.Empty, 8181, true);

                using (var window = CefGlue.ChromelyWindow.Create(config))
                {
                    // Register external url schemes
                    window.RegisterUrlScheme(new UrlScheme("https://github.com/chromelyapps/Chromely", true));

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
