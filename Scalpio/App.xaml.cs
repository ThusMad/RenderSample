using Autofac.Features.ResolveAnything;
using Autofac;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Scalpio.Core.Services.Interfaces;
using Scalpio.Core.Services;
using Scalpio.Controls.Extensions;
using log4net;
using Serilog;
using Serilog.Sinks.RichTextBox.Themes;
using Binance.Net.Interfaces.Clients;
using Binance.Net.Clients;

namespace Scalpio
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void AppShutDown(object sender, ExitEventArgs e)
        {
            Environment.Exit(0);
        }

        private void Loaded(object sender, System.Windows.Navigation.NavigationEventArgs e)
        {

        }

        private void OnStartUp(object sender, StartupEventArgs e)
        {
            ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod()?.DeclaringType);

            var builder = new ContainerBuilder();

            builder.Register<IBinanceClient>((c, p) =>
            {
                return new BinanceClient(new Binance.Net.Objects.BinanceClientOptions()
                {
                    LogLevel = Microsoft.Extensions.Logging.LogLevel.None
                });
            }).SingleInstance();

            builder.Register<IBinanceSocketClient>((c, p) =>
            {
                return new BinanceSocketClient();
            }).SingleInstance();

            builder.Register<ILogger>((c, p) =>
            {
                return new LoggerConfiguration()
                  .WriteTo.RichTextBox(DISource.LogSource, theme: RichTextBoxConsoleTheme.None)
                  .CreateLogger();
            }).SingleInstance();

            builder.RegisterSource(new AnyConcreteTypeNotAlreadyRegisteredSource());

            builder.RegisterType<DataService>().As<IDataService>().SingleInstance();


            IContainer container = builder.Build();

            DISource.Resolver = (type) => {
                return container.Resolve(type);
            };
        }
    }
}
