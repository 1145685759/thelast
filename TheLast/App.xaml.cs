using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using Modbus.Device;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Services.Dialogs;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Windows;
using TheLast.Common;
using TheLast.Dtos;
using TheLast.Entities;
using TheLast.ViewModels;
using TheLast.Views;
using TheLast.Views.Dialog;
using TestStep = TheLast.Views.TestStep;

namespace TheLast
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        public static ModbusSerialMaster ModbusSerialMaster;
        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }
        public static void LoginOut(IContainerProvider containerProvider)
        {
            Current.MainWindow.Hide();

            var dialog = containerProvider.Resolve<IDialogService>();

            dialog.ShowDialog("LoginView", callback =>
            {
                if (callback.Result != ButtonResult.OK)
                {
                    Environment.Exit(0);
                    return;
                }

                Current.MainWindow.Show();
            });
        }
        protected override void OnInitialized()
        {
            var dialog = Container.Resolve<IDialogService>();

            dialog.ShowDialog("LoginView", callback =>
            {
                if (callback.Result != ButtonResult.OK)
                {
                    Environment.Exit(0);
                    return;
                }

                var service = App.Current.MainWindow.DataContext as IConfigureService;
                if (service != null)
                    service.Configure();
                base.OnInitialized();
            });
        }
        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<BasicParameters, BasicParametersViewModel>();
            containerRegistry.Register<IDialogHostService, DialogHostService>();
            containerRegistry.RegisterForNavigation<AddRegister, AddRegisterViewModel>();
            containerRegistry.RegisterForNavigation<AddInit, AddInitViewModel>();
            containerRegistry.RegisterForNavigation<AddFeedback, AddFeedbackViewModel>();
            containerRegistry.RegisterForNavigation<MsgView, MsgViewModel>();
            containerRegistry.RegisterForNavigation<CheckRegister, CheckRegisterViewModel>();
            containerRegistry.RegisterForNavigation<ProjectManager, ProjectManagerViewModel>();
            containerRegistry.RegisterForNavigation<ModuleView, ModuleViewModel>();
            containerRegistry.RegisterForNavigation<AutomaticTestingView, AutomaticTestingViewModel>();
            containerRegistry.RegisterForNavigation<TestStep, TestStepViewModel>();
            containerRegistry.RegisterForNavigation<ComSetting, ComSettingViewModel>();
            containerRegistry.RegisterForNavigation<ManualTest, ManualTestViewModel>();
            containerRegistry.RegisterForNavigation<RealTimeCurve, RealTimeCurveViewModel>();
            containerRegistry.RegisterForNavigation<SkinView, SkinViewModel>();
            containerRegistry.RegisterForNavigation<LoginView, LoginViewModel>();
        }
        protected override IContainerExtension CreateContainerExtension()
        {
            SqlSugarScope sqlSugar = new SqlSugarScope(new ConnectionConfig()
            {
                DbType = SqlSugar.DbType.Sqlite,
                ConnectionString = "Data Source=AuxWtm.db",
                IsAutoCloseConnection = true,
                ConfigureExternalServices = new ConfigureExternalServices
                {
                    EntityService = (c, p) =>
                    {
                        // int?  decimal?这种 isnullable=true
                        if (c.PropertyType.IsGenericType &&
                        c.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                        {
                            p.IsNullable = true;
                        }
                        else if (c.PropertyType == typeof(string) &&
                                 c.GetCustomAttribute<RequiredAttribute>() == null)
                        { //string类型如果没有Required isnullable=true
                            p.IsNullable = true;
                        }
                    }
                }
            });
            Type[] types = Assembly.LoadFrom("TheLast.dll").GetTypes().Where(it => it.FullName.Contains("Entities")).ToArray();//断点调试一下是不是需要的Type，不是需要的在进行过滤
            sqlSugar.CodeFirst.InitTables(types);
            var serviceCollection = new ServiceCollection();
            serviceCollection.AddAutoMapper(cfg =>
            {
                cfg.CreateMap<Register, RegisterDto>().ReverseMap();
                cfg.CreateMap<Project, ProjectDto>().ReverseMap();
                cfg.CreateMap<Entities.Module, ModuleDto>().ReverseMap();
                cfg.CreateMap<DelayModel, DelayModelDto>().ReverseMap();
                cfg.CreateMap<FeedBack, FeedBackDto>().ReverseMap();
                cfg.CreateMap<Init, InitDto>().ReverseMap();
                cfg.CreateMap<TestStep, TestStepDto>().ReverseMap();
                cfg.CreateMap<ValueDictionary,ValueDictionaryDto>().ReverseMap();
            });
            serviceCollection.AddSingleton<ISqlSugarClient>(sqlSugar);
            return new DryIocContainerExtension(new Container(CreateContainerRules()).WithDependencyInjectionAdapter(serviceCollection));
        }
    }
}
