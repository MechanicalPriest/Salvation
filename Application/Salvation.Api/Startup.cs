using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Salvation.Core.Constants;
using Salvation.Core.Interfaces.Constants;
using System;
using System.Collections.Generic;
using System.Text;

[assembly: FunctionsStartup(typeof(Salvation.Api.Startup))]
namespace Salvation.Api
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            //builder.Services.AddHttpClient();

            builder.Services.AddSingleton<IConstantsService>((s) => {
                return new ConstantsService();
            });

            //builder.Services.AddSingleton<ILoggerProvider, MyLoggerProvider>();
        }
    }
}
