using EasilyNET.AutoDependencyInjection.Core.Attributes;
using LiteDB;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using System;
using WinRT.Interop;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace WinUIAutoDISample;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
[DependencyInjection(ServiceLifetime.Singleton, AddSelf = true, SelfOnly = true)]
public sealed partial class MainWindow
{
    private readonly ILiteDatabase _liteDb;
    private readonly ILogger<MainWindow> _logger;

    public MainWindow(ILogger<MainWindow> logger, ILiteDatabase liteDb)
    {
        InitializeComponent();
        ExtendsContentIntoTitleBar = true;
        _logger = logger;
        _liteDb = liteDb;
    }

    private void myButton_Click(object sender, RoutedEventArgs e)
    {
        // 测试LiteDB写入数据
        myButton.Content = "Clicked";
        var coll = _liteDb.GetCollection<dynamic>();
        coll.Insert(new
        {
            A = "test",
            B = 1,
            C = DateTime.Now
        });
    }

    /// <summary>
    /// 让窗体启动的时候居中
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="args"></param>
    private void MainWindow_OnActivated(object sender, WindowActivatedEventArgs args)
    {
        DispatcherQueue.TryEnqueue(() =>
        {
            var handle = WindowNative.GetWindowHandle(sender);
            var id = Win32Interop.GetWindowIdFromWindow(handle);
            var appWindow = AppWindow.GetFromWindowId(id);
            if (appWindow is null) return;
            var displayArea = DisplayArea.GetFromWindowId(id, DisplayAreaFallback.Nearest);
            if (displayArea is null) return;
            var CenteredPosition = appWindow.Position;
            CenteredPosition.X = (displayArea.WorkArea.Width - appWindow.Size.Width) / 2;
            CenteredPosition.Y = (displayArea.WorkArea.Height - appWindow.Size.Height) / 2;
            appWindow.Move(CenteredPosition);
        });
    }
}