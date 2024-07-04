using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.UI.Xaml;
using Serilog;
using Serilog.Events;
using System;
using System.IO;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUIAutoDISample;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App
{
    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        InitializeComponent();
    }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override async void OnLaunched(LaunchActivatedEventArgs args)
    {
        // 创建一个通用主机
        using var host = CreateHostBuilder().Build();
        host.InitializeApplication();
        await host.StartAsync().ConfigureAwait(true);
        var m_window = host.Services.GetRequiredService<MainWindow>();
        m_window.Activate();
        await host.StopAsync().ConfigureAwait(true);
    }

    private static IHostBuilder CreateHostBuilder() =>
        Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration(c =>
            {
                c.SetBasePath(AppContext.BaseDirectory);
                c.AddJsonFile("appsettings.json", false, false);
            })
            .ConfigureLogging((hbc, lb) =>
            {
                var logger = new LoggerConfiguration()
                             .ReadFrom.Configuration(hbc.Configuration)
                             .Enrich.FromLogContext()
                             .WriteTo.Async(wt =>
                             {
                                 if (hbc.HostingEnvironment.IsDevelopment())
                                 {
                                     wt.Debug();
                                 }
                                 wt.Map(MapData, (key, log) =>
                                     log.Async(o =>
                                         o.File(Path.Combine($"{AppContext.BaseDirectory}{Path.DirectorySeparatorChar}logs",
                                             $"{key.level.ToString().ToLower()}{Path.DirectorySeparatorChar}{key.time:yyyy-MM-dd}.log"), rollingInterval: RollingInterval.Day)));
                             }).CreateLogger();
                lb.ClearProviders();
                lb.AddSerilog(logger, true);
            })
            .ConfigureServices(sc => sc.AddApplicationModules<AppServiceModules>());

    /// <summary>
    /// Map data to log file.
    /// </summary>
    /// <param name="logEvent"></param>
    /// <returns></returns>
    private static (DateTime time, LogEventLevel level) MapData(LogEvent logEvent) => (new(logEvent.Timestamp.Year, logEvent.Timestamp.Month, logEvent.Timestamp.Day, logEvent.Timestamp.Hour, logEvent.Timestamp.Minute, logEvent.Timestamp.Second), logEvent.Level);
}