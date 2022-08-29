using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Salvation.Core;
using Salvation.Core.Interfaces.Modelling;
using Salvation.Explorer.Modelling;
using Salvation.Utility.SpellDataUpdate;
using Salvation.Utility.TalentStructureUpdate;
using Serilog;
using Serilog.Events;
using System;
using System.IO;

namespace Salvation.Explorer
{
    class Program
    {
        static void Main(string[] args)
        {
            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .WriteTo.File("logs" + Path.DirectorySeparatorChar + "salvation.explorer.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();

            try
            {
                CreateHostBuilder(args).Build().Run();
                return;
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "Application start-up failed");
            }
            finally
            {
                Log.CloseAndFlush();
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureServices((hostContext, services) =>
                {
                    services.AddSalvationCore();

                    // Explorer specific utility services
                    services.AddSingleton<IComparisonModeller<CovenantComparisonsResult>, CovenantComparisons>();
                    services.AddSingleton<IComparisonModeller<AdvancedComparisonResult>, AdvancedComparison>();

                    services.AddSingleton<IHolyPriestExplorer, HolyPriestExplorer>();

                    services.AddSingleton<ISpellDataUpdateService, SpellDataUpdateService>();
                    services.AddSingleton<ITalentStructureUpdateService, TalentStructureUpdateService>();
                    services.AddSingleton<ISpellDataService<HolyPriestSpellDataService>, HolyPriestSpellDataService>();

                    // Application service
                    services.AddHostedService(serviceProvider =>
                        new Explorer(
                            args,
                            serviceProvider.GetService<IHolyPriestExplorer>(),
                            serviceProvider.GetService<ISpellDataUpdateService>(),
                            serviceProvider.GetService<ITalentStructureUpdateService>()));
                })
                .UseSerilog();
    }
}
