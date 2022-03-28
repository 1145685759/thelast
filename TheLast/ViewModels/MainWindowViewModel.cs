using MaterialDesignExtensions.Controls;
using MaterialDesignExtensions.Model;
using MaterialDesignThemes.Wpf;
using Modbus.Device;
using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;
using TheLast.Common;
using TheLast.Extensions;

namespace TheLast.ViewModels
{
    
    public class MainWindowViewModel : NavigationViewModel, IConfigureService
    {
        private IRegionNavigationJournal journal;
        private string _title = "Prism Application";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        public DelegateCommand GoBackCommand { get; private set; }
        public DelegateCommand GoForwardCommand { get; private set; }
        private List<INavigationItem> navigationItems;
        public List<INavigationItem> NavigationItems
        {
            get { return navigationItems; }
            set { SetProperty(ref navigationItems, value); }
        }
        private List<Navigation2View> Views = new List<Navigation2View>();
        public MainWindowViewModel(IRegionManager regionManager, IContainerProvider containerProvider, IDialogHostService dialog) :base(containerProvider)
        {
            GoBackCommand = new DelegateCommand(() =>
            {
                if (journal != null && journal.CanGoBack)
                    journal.GoBack();
            });
            GoForwardCommand = new DelegateCommand(() =>
            {
                if (journal != null && journal.CanGoForward)
                    journal.GoForward();
            });
            Views = GetNavigation2Views();
            NavigationItems = new List<INavigationItem>()
            {
                new SubheaderNavigationItem() { Subheader = "菜单" },
                new FirstLevelNavigationItem() { Label = "系统设置", Icon = PackIconKind.SetAll},
                new SecondLevelNavigationItem() { Label = "端口设置", Icon = PackIconKind.Baseball },
                 new SecondLevelNavigationItem() { Label = "后台任务管理", Icon = PackIconKind.TicketConfirmation },
                new SecondLevelNavigationItem() { Label = "个性化", Icon = PackIconKind.Magic },
                new FirstLevelNavigationItem() { Label = "寄存器配置", Icon = PackIconKind.CashRegister},
                new SecondLevelNavigationItem() { Label = "基础参数配置", Icon = PackIconKind.Baseball },
                new SecondLevelNavigationItem() { Label = "内机控制参数配置", Icon = PackIconKind.Allergy },
                new SecondLevelNavigationItem() { Label = "步进电机脉冲检测", Icon = PackIconKind.ApplicationSettingsOutline },
                new SecondLevelNavigationItem() { Label = "模拟量输入配置", Icon = PackIconKind.AlphaABox },
                new SecondLevelNavigationItem() { Label = "模拟量输出配置", Icon = PackIconKind.AlphaABoxOutline },
                new SecondLevelNavigationItem() { Label = "数字量输入配置", Icon = PackIconKind.AlphaDBox },
                new SecondLevelNavigationItem() { Label = "数字量输出配置", Icon = PackIconKind.AlphaDBoxOutline },
                new SecondLevelNavigationItem() { Label = "内机数据配置", Icon = PackIconKind.DatabaseAlert },
                new SecondLevelNavigationItem() { Label = "外机数据配置", Icon = PackIconKind.DatabaseAlertOutline },
                new FirstLevelNavigationItem() { Label = "用例管理", Icon = PackIconKind.TestTube },
                new SecondLevelNavigationItem() { Label = "编辑用例", Icon = PackIconKind.ApplicationEdit },
                new FirstLevelNavigationItem() { Label = "测试管理", Icon = PackIconKind.TestTubeEmpty },
                new SecondLevelNavigationItem() { Label = "手动测试", Icon = PackIconKind.HandBackRight },
                new SecondLevelNavigationItem() { Label = "实时曲线", Icon = PackIconKind.ChartBellCurve },
                new FirstLevelNavigationItem() { Label = "历史数据管理", Icon = PackIconKind.DatabaseExportOutline },
                new SecondLevelNavigationItem() { Label = "历史曲线", Icon = PackIconKind.ChartBox },
                new FirstLevelNavigationItem() { Label = "用户管理", Icon = PackIconKind.AccountBox },
                new SecondLevelNavigationItem() { Label = "用户列表", Icon = PackIconKind.CardAccountDetails },
            };
            this.regionManager = regionManager;
            this.dialog = dialog;
        }
        private DelegateCommand<WillSelectNavigationItemEventArgs> willSelectNavigationItemCommand;
        private readonly IRegionManager regionManager;
        private readonly IDialogHostService dialog;

        public DelegateCommand<WillSelectNavigationItemEventArgs> WillSelectNavigationItemCommand =>
            willSelectNavigationItemCommand ?? (willSelectNavigationItemCommand = new DelegateCommand<WillSelectNavigationItemEventArgs>(ExecuteWillSelectNavigationItemCommand));

        void ExecuteWillSelectNavigationItemCommand(WillSelectNavigationItemEventArgs eventArgs)
        {

            var s = ((NavigationItem)eventArgs.NavigationItemToSelect).Label;
            var menu = Views.FirstOrDefault(x => x.NavigationName == s);
            if (menu != null)
            {
                UpdateLoading(true);
                NavigationParameters keyValuePairs = new NavigationParameters();
                keyValuePairs.Add("Type", s);
                regionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate(menu.View, back =>
                {
                    journal = back.Context.NavigationService.Journal;
                }, keyValuePairs);
                UpdateLoading(false);
            }
            else
            {
                return;
            }
        }

        private List<Navigation2View> GetNavigation2Views()
        {
            return new List<Navigation2View> 
            {
                new Navigation2View{NavigationName="基础参数配置",View="BasicParameters" },
                new Navigation2View{NavigationName="内机控制参数配置",View="BasicParameters" },
                new Navigation2View{NavigationName="步进电机脉冲检测",View="BasicParameters" },
                new Navigation2View{NavigationName="模拟量输入配置",View="BasicParameters" },
                new Navigation2View{NavigationName="模拟量输出配置",View="BasicParameters" },
                new Navigation2View{NavigationName="数字量输入配置",View="BasicParameters" },
                new Navigation2View{NavigationName="数字量输出配置",View="BasicParameters" },
                new Navigation2View{NavigationName="内机数据配置",View="BasicParameters" },
                new Navigation2View{NavigationName="外机数据配置",View="BasicParameters" },
                new Navigation2View{NavigationName="编辑用例",View="ProjectManager" },
                new Navigation2View{NavigationName="手动测试",View="ManualTest" },
                new Navigation2View{NavigationName="实时曲线",View="RealTimeCurve" },
                new Navigation2View{NavigationName="历史曲线",View="HistoricalCurve" },
                new Navigation2View{NavigationName="用户列表",View="UserList" },
                new Navigation2View{NavigationName="端口设置",View="ComSetting" },
                new Navigation2View{NavigationName="个性化",View="SkinView" },
                new Navigation2View{ NavigationName="后台任务管理",View="QuartzManager"}
            };
        }

        public void Configure()
        {
            
            regionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate("ProjectManager");
        }
    }
}
