using AutoUpdaterDotNET;
using Prism.Events;
using System;
using System.Globalization;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using TheLast.Common;
using TheLast.Extensions;

namespace TheLast.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly IDialogHostService dialogHostService;

        public MainWindow(IEventAggregator aggregator, IDialogHostService dialogHostService)
        {
            InitializeComponent();
            Assembly assembly = Assembly.GetEntryAssembly();
            Thread.CurrentThread.CurrentCulture =
            Thread.CurrentThread.CurrentUICulture = CultureInfo.CreateSpecificCulture("zh");
            AutoUpdater.LetUserSelectRemindLater = true;
            AutoUpdater.RemindLaterTimeSpan = RemindLaterFormat.Minutes;
            AutoUpdater.RemindLaterAt = 1;
            AutoUpdater.ReportErrors = true;
            DispatcherTimer timer = new DispatcherTimer { Interval = TimeSpan.FromMinutes(2) };//定时去检测更新根据自己业务需求
            timer.Tick += delegate { AutoUpdater.Start("http://10.12.113.68/Downloads/AutoUpdaterStarter.xml"); };
            timer.Start();
            aggregator.ResgiterMessage(arg =>
            {
                Snackbar.MessageQueue.Enqueue(arg);
            });
            aggregator.Resgiter(arg =>
            {
                DialogHost.IsOpen = arg.IsOpen;

                if (DialogHost.IsOpen)
                    DialogHost.DialogContent = new ProgressView();
            });
            btnMin.Click += (s, e) => { this.WindowState = WindowState.Minimized; };
            btnMax.Click += (s, e) =>
            {
                if (this.WindowState == WindowState.Maximized)
                    this.WindowState = WindowState.Normal;
                else
                    this.WindowState = WindowState.Maximized;
            };
            btnClose.Click += async (s, e) =>
            {
                var dialogResult = await dialogHostService.Question("温馨提示", "确认退出系统?");
                if (dialogResult.Result != Prism.Services.Dialogs.ButtonResult.OK) return;
                this.Close();
            };
            ColorZone.MouseMove += (s, e) =>
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                    this.DragMove();
            };

            ColorZone.MouseDoubleClick += (s, e) =>
            {
                if (this.WindowState == WindowState.Normal)
                    this.WindowState = WindowState.Maximized;
                else
                    this.WindowState = WindowState.Normal;
            };


            this.dialogHostService = dialogHostService;
        }
        private void ButtonCheckForUpdate_Click(object sender, RoutedEventArgs e)
        {
            AutoUpdater.Start("http://10.12.113.68/Downloads/AutoUpdaterStarter.xml");
        }

    }
}
