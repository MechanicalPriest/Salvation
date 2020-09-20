using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Salvation.Core;
using Salvation.Core.Constants;
using Salvation.Core.Interfaces.Constants;
using Salvation.Core.Models;
using Salvation.Core.Models.Common;
using Salvation.Core.Models.HolyPriest;
using Salvation.Core.Profile;
using Salvation.Explorer.Modelling;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Salvation.Explorer
{
    class Program
    {
        static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();

            Console.ReadLine();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSingleton<IConstantsService, ConstantsService>();
                    services.AddSingleton<IHolyPriestExplorer, HolyPriestExplorer>();
                    services.AddHostedService<Explorer>();
                });
    }
}
