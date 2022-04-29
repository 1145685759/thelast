using DryIoc;
using DryIoc.Microsoft.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using MiniExcelLibs;
using Modbus.Device;
using Prism.DryIoc;
using Prism.Ioc;
using Prism.Services.Dialogs;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
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
        public static int IndoorCount;
        public static int OutdoorCount;
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
            containerRegistry.RegisterForNavigation<JumpConfig, JumpConfigViewModel>();
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
            containerRegistry.RegisterForNavigation<HistoricalCurve, HistoricalCurveViewModel>();
        }
        protected override IContainerExtension CreateContainerExtension()
        {
            SqlSugarScope sqlSugar = new SqlSugarScope(new ConnectionConfig()
            {
                DbType = SqlSugar.DbType.Sqlite,
                ConnectionString = "Data Source=AuxWtm.db",
                IsAutoCloseConnection = true,
                LanguageType=LanguageType.Chinese,
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
            //for (int i = 0; i < 63; i++)
            //{
            //    List<ValueDictionary> valueDictionaries = new List<ValueDictionary>();
            //    valueDictionaries.Add(new ValueDictionary { DisplayValue = "自动", RealValue = "0", RegisterId = 3025 + i * 40 });
            //    valueDictionaries.Add(new ValueDictionary { DisplayValue = "制冷", RealValue = "1", RegisterId = 3025 + i * 40 });
            //    valueDictionaries.Add(new ValueDictionary { DisplayValue = "除湿", RealValue = "2", RegisterId = 3025 + i * 40 });
            //    valueDictionaries.Add(new ValueDictionary { DisplayValue = "健康除湿", RealValue = "3", RegisterId = 3025 + i * 40 });
            //    valueDictionaries.Add(new ValueDictionary { DisplayValue = "制热", RealValue = "4", RegisterId = 3025 + i * 40 });
            //    valueDictionaries.Add(new ValueDictionary { DisplayValue = "干衣", RealValue = "5", RegisterId = 3025 + i * 40 });
            //    valueDictionaries.Add(new ValueDictionary { DisplayValue = "送风", RealValue = "6", RegisterId = 3025 + i * 40 });

            //    valueDictionaries.Add(new ValueDictionary { DisplayValue = "高风", RealValue = "1", RegisterId = 3027 + i * 40 });
            //    valueDictionaries.Add(new ValueDictionary { DisplayValue = "中风", RealValue = "2", RegisterId = 3027 + i * 40 });
            //    valueDictionaries.Add(new ValueDictionary { DisplayValue = "低风", RealValue = "3", RegisterId = 3027 + i * 40 });
            //    valueDictionaries.Add(new ValueDictionary { DisplayValue = "微风", RealValue = "4", RegisterId = 3027 + i * 40 });
            //    valueDictionaries.Add(new ValueDictionary { DisplayValue = "自动", RealValue = "5", RegisterId = 3027 + i * 40 });

            //    valueDictionaries.Add(new ValueDictionary { DisplayValue = "高风", RealValue = "1", RegisterId = 3033 + i * 40 });
            //    valueDictionaries.Add(new ValueDictionary { DisplayValue = "中风", RealValue = "2", RegisterId = 3033 + i * 40 });
            //    valueDictionaries.Add(new ValueDictionary { DisplayValue = "低风", RealValue = "3", RegisterId = 3033 + i * 40 });

            //    valueDictionaries.Add(new ValueDictionary { DisplayValue = "关", RealValue = "0", RegisterId = 3043 + i * 40 });
            //    valueDictionaries.Add(new ValueDictionary { DisplayValue = "开", RealValue = "1", RegisterId = 3043 + i * 40 });

            //    valueDictionaries.Add(new ValueDictionary { DisplayValue = "关", RealValue = "0", RegisterId = 3044 + i * 40 });
            //    valueDictionaries.Add(new ValueDictionary { DisplayValue = "开", RealValue = "1", RegisterId = 3044 + i * 40 });

            //    valueDictionaries.Add(new ValueDictionary { DisplayValue = "关", RealValue = "0", RegisterId = 3045 + i * 40 });
            //    valueDictionaries.Add(new ValueDictionary { DisplayValue = "开", RealValue = "1", RegisterId = 3045 + i * 40 });

            //    valueDictionaries.Add(new ValueDictionary { DisplayValue = "关", RealValue = "0", RegisterId = 3046 + i * 40 });
            //    valueDictionaries.Add(new ValueDictionary { DisplayValue = "开", RealValue = "1", RegisterId = 3046 + i * 40 });

            //    valueDictionaries.Add(new ValueDictionary { DisplayValue = "关", RealValue = "0", RegisterId = 3047 + i * 40 });
            //    valueDictionaries.Add(new ValueDictionary { DisplayValue = "开", RealValue = "1", RegisterId = 3047 + i * 40 });

            //    valueDictionaries.Add(new ValueDictionary { DisplayValue = "关", RealValue = "0", RegisterId = 3048 + i * 40 });
            //    valueDictionaries.Add(new ValueDictionary { DisplayValue = "开", RealValue = "1", RegisterId = 3048 + i * 40 });

            //    valueDictionaries.Add(new ValueDictionary { DisplayValue = "否", RealValue = "0", RegisterId = 3049 + i * 40 });
            //    valueDictionaries.Add(new ValueDictionary { DisplayValue = "是", RealValue = "1", RegisterId = 3049 + i * 40 });

            //    valueDictionaries.Add(new ValueDictionary { DisplayValue = "关", RealValue = "0", RegisterId = 3050 + i * 40 });
            //    valueDictionaries.Add(new ValueDictionary { DisplayValue = "开", RealValue = "1", RegisterId = 3050 + i * 40 });

            //    valueDictionaries.Add(new ValueDictionary { DisplayValue = "关", RealValue = "0", RegisterId = 3051 + i * 40 });
            //    valueDictionaries.Add(new ValueDictionary { DisplayValue = "开", RealValue = "1", RegisterId = 3051 + i * 40 });

            //    valueDictionaries.Add(new ValueDictionary { DisplayValue = "关", RealValue = "0", RegisterId = 3052 + i * 40 });
            //    valueDictionaries.Add(new ValueDictionary { DisplayValue = "开", RealValue = "1", RegisterId = 3052 + i * 40 });

            //    valueDictionaries.Add(new ValueDictionary { DisplayValue = "关", RealValue = "0", RegisterId = 3053 + i * 40 });
            //    valueDictionaries.Add(new ValueDictionary { DisplayValue = "开", RealValue = "1", RegisterId = 3053 + i * 40 });

            //    valueDictionaries.Add(new ValueDictionary { DisplayValue = "闭合", RealValue = "0", RegisterId = 3054 + i * 40 });
            //    valueDictionaries.Add(new ValueDictionary { DisplayValue = "断开", RealValue = "1", RegisterId = 3054 + i * 40 });

            //    valueDictionaries.Add(new ValueDictionary { DisplayValue = "闭合", RealValue = "0", RegisterId = 3055 + i * 40 });
            //    valueDictionaries.Add(new ValueDictionary { DisplayValue = "断开", RealValue = "1", RegisterId = 3055 + i * 40 });

            //    valueDictionaries.Add(new ValueDictionary { DisplayValue = "关", RealValue = "0", RegisterId = 3056 + i * 40 });
            //    valueDictionaries.Add(new ValueDictionary { DisplayValue = "开", RealValue = "1", RegisterId = 3056 + i * 40 });

            //    valueDictionaries.Add(new ValueDictionary { DisplayValue = "关", RealValue = "0", RegisterId = 3057 + i * 40 });
            //    valueDictionaries.Add(new ValueDictionary { DisplayValue = "开", RealValue = "1", RegisterId = 3057 + i * 40 });

            //    valueDictionaries.Add(new ValueDictionary { DisplayValue = "关", RealValue = "0", RegisterId = 3058 + i * 40 });
            //    valueDictionaries.Add(new ValueDictionary { DisplayValue = "开", RealValue = "1", RegisterId = 3058 + i * 40 });
            //    sqlSugar.Insertable(valueDictionaries).ExecuteCommand();

            //}
            #region
            //for (int i = 3; i < 65; i++)
            //{
            //    List<Register> registers = new List<Register>();
            //    registers.Add(new Register 
            //    { 
            //        Name= "设定模式-"+i.ToString(),
            //        RegisterType="内机参数",
            //        IsEnable=false,
            //        Address= (ushort)(40 * (i - 1) + 325)
            //    });
            //    registers.Add(new Register
            //    {
            //        Name = "设定温度-" + i.ToString(),
            //        RegisterType = "内机参数",
            //        IsEnable = false,
            //        Address = (ushort)(40 * (i - 1) + 326)
            //    });
            //    registers.Add(new Register
            //    {
            //        Name = "设定风档-" + i.ToString(),
            //        RegisterType = "内机参数",
            //        IsEnable = false,
            //        Address = (ushort)(40 * (i - 1) + 327)
            //    });
            //    registers.Add(new Register
            //    {
            //        Name = "室内温度-" + i.ToString(),
            //        RegisterType = "内机参数",
            //        IsEnable = false,
            //        Address = (ushort)(40 * (i - 1) + 328)
            //    });
            //    registers.Add(new Register
            //    {
            //        Name = "内盘温度-" + i.ToString(),
            //        RegisterType = "内机参数",
            //        IsEnable = false,
            //        Address = (ushort)(40 * (i - 1) + 329)
            //    });
            //    registers.Add(new Register
            //    {
            //        Name = "入口温度-" + i.ToString(),
            //        RegisterType = "内机参数",
            //        IsEnable = false,
            //        Address = (ushort)(40 * (i - 1) + 330)
            //    });
            //    registers.Add(new Register
            //    {
            //        Name = "出口温度-" + i.ToString(),
            //        RegisterType = "内机参数",
            //        IsEnable = false,
            //        Address = (ushort)(40 * (i - 1) + 331)
            //    });
            //    registers.Add(new Register
            //    {
            //        Name = "出风温度-" + i.ToString(),
            //        RegisterType = "内机参数",
            //        IsEnable = false,
            //        Address = (ushort)(40 * (i - 1) + 332)
            //    });
            //    registers.Add(new Register
            //    {
            //        Name = "实际风档-" + i.ToString(),
            //        RegisterType = "内机参数",
            //        IsEnable = false,
            //        Address = (ushort)(40 * (i - 1) + 333)
            //    });
            //    registers.Add(new Register
            //    {
            //        Name = "协议版本-" + i.ToString(),
            //        RegisterType = "内机参数",
            //        IsEnable = false,
            //        Address = (ushort)(40 * (i - 1) + 334)
            //    });
            //    registers.Add(new Register
            //    {
            //        Name = "实际转速-" + i.ToString(),
            //        RegisterType = "内机参数",
            //        IsEnable = false,
            //        Address = (ushort)(40 * (i - 1) + 335)
            //    });
            //    registers.Add(new Register
            //    {
            //        Name = "故障代码-" + i.ToString(),
            //        RegisterType = "内机参数",
            //        IsEnable = false,
            //        Address = (ushort)(40 * (i - 1) + 336)
            //    });
            //    registers.Add(new Register
            //    {
            //        Name = "风门位置-" + i.ToString(),
            //        RegisterType = "内机参数",
            //        IsEnable = false,
            //        Address = (ushort)(40 * (i - 1) + 337)
            //    });
            //    registers.Add(new Register
            //    {
            //        Name = "膨胀阀步数-" + i.ToString(),
            //        RegisterType = "内机参数",
            //        IsEnable = false,
            //        Address = (ushort)(40 * (i - 1) + 338)
            //    });
            //    registers.Add(new Register
            //    {
            //        Name = "软件版本-" + i.ToString(),
            //        RegisterType = "内机参数",
            //        IsEnable = false,
            //        Address = (ushort)(40 * (i - 1) + 339)
            //    });
            //    registers.Add(new Register
            //    {
            //        Name = "Scode-" + i.ToString(),
            //        RegisterType = "内机参数",
            //        IsEnable = false,
            //        Address = (ushort)(40 * (i - 1) + 340)
            //    });
            //    registers.Add(new Register
            //    {
            //        Name = "机型-" + i.ToString(),
            //        RegisterType = "内机参数",
            //        IsEnable = false,
            //        Address = (ushort)(40 * (i - 1) + 341)
            //    });
            //    registers.Add(new Register
            //    {
            //        Name = "匹数-" + i.ToString(),
            //        RegisterType = "内机参数",
            //        IsEnable = false,
            //        Address = (ushort)(40 * (i - 1) + 342)
            //    });
            //    registers.Add(new Register
            //    {
            //        Name = "睡眠-" + i.ToString(),
            //        RegisterType = "内机参数",
            //        IsEnable = false,
            //        Address = (ushort)(40 * (i - 1) + 343)
            //    });
            //    registers.Add(new Register
            //    {
            //        Name = "ECO-" + i.ToString(),
            //        RegisterType = "内机参数",
            //        IsEnable = false,
            //        Address = (ushort)(40 * (i - 1) + 344)
            //    });
            //    registers.Add(new Register
            //    {
            //        Name = "高效/强力-" + i.ToString(),
            //        RegisterType = "内机参数",
            //        IsEnable = false,
            //        Address = (ushort)(40 * (i - 1) + 345)
            //    });
            //    registers.Add(new Register
            //    {
            //        Name = "静音-" + i.ToString(),
            //        RegisterType = "内机参数",
            //        IsEnable = false,
            //        Address = (ushort)(40 * (i - 1) + 346)
            //    });
            //    registers.Add(new Register
            //    {
            //        Name = "开关机-" + i.ToString(),
            //        RegisterType = "内机参数",
            //        IsEnable = false,
            //        Address = (ushort)(40 * (i - 1) + 347)
            //    });
            //    registers.Add(new Register
            //    {
            //        Name = "防霉-" + i.ToString(),
            //        RegisterType = "内机参数",
            //        IsEnable = false,
            //        Address = (ushort)(40 * (i - 1) + 348)
            //    });
            //    registers.Add(new Register
            //    {
            //        Name = "设定0.5℃-" + i.ToString(),
            //        RegisterType = "内机参数",
            //        IsEnable = false,
            //        Address = (ushort)(40 * (i - 1) + 349)
            //    });
            //    registers.Add(new Register
            //    {
            //        Name = "设定摆风-" + i.ToString(),
            //        RegisterType = "内机参数",
            //        IsEnable = false,
            //        Address = (ushort)(40 * (i - 1) + 350)
            //    });
            //    registers.Add(new Register
            //    {
            //        Name = "设定辅热-" + i.ToString(),
            //        RegisterType = "内机参数",
            //        IsEnable = false,
            //        Address = (ushort)(40 * (i - 1) + 351)
            //    });
            //    registers.Add(new Register
            //    {
            //        Name = "清洁-" + i.ToString(),
            //        RegisterType = "内机参数",
            //        IsEnable = false,
            //        Address = (ushort)(40 * (i - 1) + 352)
            //    });
            //    registers.Add(new Register
            //    {
            //        Name = "房卡-" + i.ToString(),
            //        RegisterType = "内机参数",
            //        IsEnable = false,
            //        Address = (ushort)(40 * (i - 1) + 353)
            //    });
            //    registers.Add(new Register
            //    {
            //        Name = "浮子开关-" + i.ToString(),
            //        RegisterType = "内机参数",
            //        IsEnable = false,
            //        Address = (ushort)(40 * (i - 1) + 354)
            //    });
            //    registers.Add(new Register
            //    {
            //        Name = "摆风-" + i.ToString(),
            //        RegisterType = "内机参数",
            //        IsEnable = false,
            //        Address = (ushort)(40 * (i - 1) + 355)
            //    });
            //    registers.Add(new Register
            //    {
            //        Name = "辅热-" + i.ToString(),
            //        RegisterType = "内机参数",
            //        IsEnable = false,
            //        Address = (ushort)(40 * (i - 1) + 356)
            //    });
            //    registers.Add(new Register
            //    {
            //        Name = "Thermo On/Off-" + i.ToString(),
            //        RegisterType = "内机参数",
            //        IsEnable = false,
            //        Address = (ushort)(40 * (i - 1) + 357)
            //    });
            //    registers.Add(new Register
            //    {
            //        Name = "预留-" + i.ToString(),
            //        RegisterType = "内机参数",
            //        IsEnable = false,
            //        Address = (ushort)(40 * (i - 1) + 358)
            //    });
            //    registers.Add(new Register
            //    {
            //        Name = "预留-" + i.ToString(),
            //        RegisterType = "内机参数",
            //        IsEnable = false,
            //        Address = (ushort)(40 * (i - 1) + 359)
            //    });
            //    registers.Add(new Register
            //    {
            //        Name = "预留-" + i.ToString(),
            //        RegisterType = "内机参数",
            //        IsEnable = false,
            //        Address = (ushort)(40 * (i - 1) + 360)
            //    });
            //    registers.Add(new Register
            //    {
            //        Name = "预留-" + i.ToString(),
            //        RegisterType = "内机参数",
            //        IsEnable = false,
            //        Address = (ushort)(40 * (i - 1) + 361)
            //    });
            //    registers.Add(new Register
            //    {
            //        Name = "预留-" + i.ToString(),
            //        RegisterType = "内机参数",
            //        IsEnable = false,
            //        Address = (ushort)(40 * (i - 1) + 362)
            //    });
            //    registers.Add(new Register
            //    {
            //        Name = "预留-" + i.ToString(),
            //        RegisterType = "内机参数",
            //        IsEnable = false,
            //        Address = (ushort)(40 * (i - 1) + 363)
            //    });
            //    registers.Add(new Register
            //    {
            //        Name = "预留-" + i.ToString(),
            //        RegisterType = "内机参数",
            //        IsEnable = false,
            //        Address = (ushort)(40 * (i - 1) + 364)
            //    });
            //    sqlSugar.Insertable(registers).ExecuteCommandAsync();
            //}
            #endregion
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
