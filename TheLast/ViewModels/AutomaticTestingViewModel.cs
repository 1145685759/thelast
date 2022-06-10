using AutoMapper;
using HandyControl.Controls;
using MiniExcelLibs;
using Prism.Commands;
using Prism.Ioc;
using Prism.Regions;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using TheLast.Dtos;
using TheLast.Entities;
using System.Text.RegularExpressions;
using Modbus.Device;

namespace TheLast.ViewModels
{
    public class AutomaticTestingViewModel: NavigationViewModel
    {
        private string stepResult;
        public string StepResult
        {
            get { return stepResult; }
            set { SetProperty(ref stepResult, value); }
        }
        private ProjectDto currentProjectDto;
        public ProjectDto CurrentProjectDto
        {
            get { return currentProjectDto; }
            set { SetProperty(ref currentProjectDto, value); }
        }
        public ModbusSerialMaster ModbusSerialMaster { get; set; }
        private List<Module> currentModules;
        private readonly ISqlSugarClient sqlSugarClient;
        private readonly IMapper mapper;
        public List<Module> CurrentModules
        {
            get { return currentModules; }
            set { SetProperty(ref currentModules, value); }
        }
        private Module currentModule;
        public Module CurrentModule
        {
            get { return currentModule; }
            set { SetProperty(ref currentModule, value); }
        }
        private double proValue;
        public double ProValue
        {
            get { return proValue; }
            set { SetProperty(ref proValue, value); }
        }
        private ModuleDto currentDto;
        public ModuleDto CurrentDto
        {
            get { return currentDto; }
            set { SetProperty(ref currentDto, value); }
        }
        private SolidColorBrush color;
        public SolidColorBrush Color
        {
            get { return color; }
            set { SetProperty(ref color, value); }
        }
        private List<TestStepDto> currentTestStepDtos;
        public List<TestStepDto> CurrentTestStepDtos
        {
            get { return currentTestStepDtos; }
            set { SetProperty(ref currentTestStepDtos, value); }
        }
        private ObservableCollection<TestStepDto> testStepDtos;
        public ObservableCollection<TestStepDto> TestStepDtos
        {
            get { return testStepDtos; }
            set { SetProperty(ref testStepDtos, value); }
        }
        public AutomaticTestingViewModel(IContainerProvider containerProvider,ISqlSugarClient sqlSugarClient,IMapper mapper) :base(containerProvider)
        {
            TestStepDtos = new ObservableCollection<TestStepDto>();
            this.sqlSugarClient = sqlSugarClient;
            this.mapper = mapper;
        }
         static bool isNumber(string str)
        {
            bool isMatch = Regex.IsMatch(str, @"^\d+$"); // 判断字符串是否为数字 的正则表达式
            return isMatch;
        }
        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
            CurrentProjectDto=navigationContext.Parameters.GetValue<ProjectDto>("Project");
            ModbusSerialMaster = App.ModbusSerialMaster;
            CurrentModules =await sqlSugarClient.Queryable<Module>().Where(x => x.ProjectId == CurrentProjectDto.Id).ToListAsync();
            CurrentDto = mapper.Map<ModuleDto>(CurrentModules[0]);
            var list = await sqlSugarClient.Queryable<TestStep>().Mapper(it => it.Inits, it => it.Inits.First().TestStepId).Mapper(it => it.FeedBacks, it => it.FeedBacks.First().TestStepId).Where(x => x.ModuleId == CurrentDto.Id).ToListAsync();
            TestStepDtos.Clear();
            foreach (var item in list)
            {
                TestStepDtos.Add(new TestStepDto 
                {
                    Conditions = item.Conditions,
                    FeedBacks = item.FeedBacks,
                    Id = item.Id,
                    Inits = item.Inits,
                    JudgmentContent = item.JudgmentContent,
                    ModuleId = item.ModuleId,
                    Remark = item.Remark,
                    Result = item.Result,
                    TestContent = item.TestContent,
                    TestProcess = item.TestProcess
                });
            }
        }


        private DelegateCommand startRun;
        public DelegateCommand StartRun =>
            startRun ?? (startRun = new DelegateCommand(ExecuteStartRun));

        async void ExecuteStartRun()
        {
            

            await Task.Run(async () =>
            {
               await Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    if (ModbusSerialMaster == null)
                    {
                        Growl.Info("未设置串口");
                        return;
                    }
                    Growl.Info("开始运行");
                    var sheets = new Dictionary<string, object>();
                    string name = (await sqlSugarClient.Queryable<Project>().FirstAsync(x => x.Id == CurrentDto.ProjectId)).ProjectName;
                    foreach (var item in CurrentModules)
                    {
                        CurrentDto = mapper.Map<ModuleDto>(item);
                        ProValue = 0;

                        var testSteps = await sqlSugarClient.Queryable<TestStep>().Mapper(it => it.Inits, it => it.Inits.First().TestStepId).Mapper(it => it.FeedBacks, it => it.FeedBacks.First().TestStepId).Where(x => x.ModuleId == item.Id).ToListAsync();
                        CurrentTestStepDtos = new List<TestStepDto>();
                        foreach (var item1 in testSteps)
                        {
                            CurrentTestStepDtos.Add(new TestStepDto
                            {
                                Conditions = item1.Conditions,
                                FeedBacks = item1.FeedBacks,
                                Id = item1.Id,
                                Inits = item1.Inits,
                                JudgmentContent = item1.JudgmentContent,
                                ModuleId = item1.ModuleId,
                                Remark = item1.Remark,
                                Result = item1.Result,
                                TestContent = item1.TestContent,
                                TestProcess = item1.TestProcess
                            });
                        }
                        foreach (var testStep in testSteps)
                        {
                            StepResult ="写入："+ testStep.TestProcess+"判断：" + testStep.JudgmentContent;
                            foreach (var init in testStep.Inits)
                            {
                                var registerinit = await sqlSugarClient.Queryable<Register>().FirstAsync(x => x.Id == init.RegisterId);
                                if (registerinit.RegisterType == "基础参数" || registerinit.RegisterType == "内机控制参数" || registerinit.RegisterType == "模拟量输出")
                                {
                                    if (registerinit.Name.Contains("温度"))
                                    {
                                        await ModbusSerialMaster.WriteSingleRegisterAsync(registerinit.StationNum, registerinit.Address, (ushort)(Convert.ToDouble(init.WriteValue) * 10));
                                    }
                                    else if (registerinit.Name.Contains("电压"))
                                    {
                                        var value = float.Parse(init.WriteValue) * 10;
                                        await ModbusSerialMaster.WriteSingleRegisterAsync(registerinit.StationNum, registerinit.Address, Convert.ToUInt16(value));

                                    }
                                    else
                                    {
                                        await ModbusSerialMaster.WriteSingleRegisterAsync(registerinit.StationNum, registerinit.Address, Convert.ToUInt16(init.WriteValue));
                                    }
                                }
                                if (registerinit.RegisterType == "数字量输出")
                                {
                                    if (init.WriteValue == "1")
                                    {
                                        await ModbusSerialMaster.WriteSingleCoilAsync(registerinit.StationNum, registerinit.Address, true);
                                    }
                                    if (init.WriteValue == "0")
                                    {
                                        await ModbusSerialMaster.WriteSingleCoilAsync(registerinit.StationNum, registerinit.Address, false);
                                    }
                                }
                                if (registerinit.RegisterType == "20个温度设置")
                                {
                                    if (registerinit.AccessAddress == null)
                                    {
                                        Growl.Error(registerinit.Name + "接入寄存器地址未配置");
                                        return;
                                    }
                                    ushort[] data = new ushort[5] { (ushort)(Convert.ToDouble(init.WriteValue) * 10), (ushort)registerinit.Type, (ushort)registerinit.FineTuning, (ushort)registerinit.AccessAddress, registerinit.AllowableRangeDeviation };
                                    for (int y = 0; y < data.Length; y++)
                                    {
                                        try
                                        {
                                            await ModbusSerialMaster.WriteSingleRegisterAsync(registerinit.StationNum, (ushort)(registerinit.Address + y), data[y]);

                                        }
                                        catch (Exception ex)
                                        {

                                            Growl.Info(ex.Message);
                                        }
                                    }
                                }
                            }
                            foreach (var feedback in testStep.FeedBacks)
                            {
                                if (feedback.IsJump)
                                {
                                    if (testStep.FeedBacks.Count == 1)
                                    {
                                        testStep.Result = "通过";
                                        await sqlSugarClient.Updateable(testStep).ExecuteCommandAsync();
                                        var list = await sqlSugarClient.Queryable<TestStep>().Mapper(it => it.Inits, it => it.Inits.First().TestStepId).Mapper(it => it.FeedBacks, it => it.FeedBacks.First().TestStepId).Where(x => x.ModuleId == CurrentDto.Id).ToListAsync();
                                        TestStepDtos.Clear();
                                        foreach (var item2 in list)
                                        {
                                            TestStepDtos.Add(new TestStepDto
                                            {
                                                Conditions = item2.Conditions,
                                                FeedBacks = item2.FeedBacks,
                                                Id = item2.Id,
                                                Inits = item2.Inits,
                                                JudgmentContent = item2.JudgmentContent,
                                                ModuleId = item2.ModuleId,
                                                Remark = item2.Remark,
                                                Result = item2.Result,
                                                TestContent = item2.TestContent,
                                                TestProcess = item2.TestProcess
                                            });
                                        }
                                    }
                                    continue;
                                }
                                string result = string.Empty;
                                var now = DateTime.Now;
                                var register = await sqlSugarClient.Queryable<Register>().FirstAsync(x => x.Id == feedback.RegisterId);
                                if (feedback.DelayModeId == 1)
                                {

                                    if (register.RegisterType == "基础参数" || register.RegisterType == "内机控制参数" || register.RegisterType == "模拟量输出")
                                    {

                                        if (register.Name.Contains("温度"))
                                        {

                                            result = ((await ModbusSerialMaster.ReadHoldingRegistersAsync(register.StationNum, register.Address, 1))[0] / 10.0).ToString();
                                        }
                                        else
                                        {
                                            result = (await ModbusSerialMaster.ReadHoldingRegistersAsync(register.StationNum, register.Address, 1))[0].ToString();
                                        }
                                    }
                                    if (register.RegisterType == "模拟量输入")
                                    {
                                        if (register.Name.Contains("温度"))
                                        {
                                            result = ((await ModbusSerialMaster.ReadInputRegistersAsync(register.StationNum, register.Address, 1))[0] / 10).ToString();
                                        }
                                        else
                                        {
                                            result = (await ModbusSerialMaster.ReadInputRegistersAsync(register.StationNum, register.Address, 1))[0].ToString();
                                        }
                                    }
                                    if (register.RegisterType == "数字量输出")
                                    {
                                        var s = (await ModbusSerialMaster.ReadCoilsAsync(register.StationNum, register.Address, 1))[0].ToString();
                                        if (s == "True")
                                        {
                                            result = "1";
                                        }
                                        if (s == "False")
                                        {
                                            result = "0";
                                        }
                                    }
                                    if (register.RegisterType == "数字量输入")
                                    {
                                        var s = (await ModbusSerialMaster.ReadInputsAsync(register.StationNum, register.Address, 1))[0].ToString();
                                        if (s == "True")
                                        {
                                            result = "1";
                                        }
                                        if (s == "False")
                                        {
                                            result = "0";
                                        }
                                    }
                                    if (register.RegisterType == "步进电机脉冲检测")
                                    {
                                        result = (await ModbusSerialMaster.ReadInputRegistersAsync(register.StationNum, register.Address, 1))[0].ToString();
                                    }
                                    if (register.RegisterType == "内机数据" || register.RegisterType == "外机数据" )
                                    {
                                        if (register.Name.Contains("温度") || register.Name.Contains("频率") || register.Name.Contains("电流") || register.Name.Contains("功率") || register.Name.Contains("容量"))
                                        {
                                            result = ((await ModbusSerialMaster.ReadInputRegistersAsync(register.StationNum, register.Address, 1))[0] / 10.0).ToString();
                                        }
                                        else
                                        {
                                            result = (await ModbusSerialMaster.ReadInputRegistersAsync(register.StationNum, register.Address, 1))[0].ToString();
                                        }
                                    }
                                    if (feedback.Operational == "等于")
                                    {
                                        if (isNumber(result))
                                        {
                                            var num = Convert.ToDouble(result);
                                            double tagetvalue;
                                            if (register.RegisterType== "步进电机脉冲检测")
                                            {
                                                tagetvalue = Convert.ToDouble((await ModbusSerialMaster.ReadInputRegistersAsync(register.StationNum, Convert.ToUInt16(feedback.TagetValue), 1))[0]);
                                            }
                                            else
                                            {
                                                tagetvalue = Convert.ToDouble(feedback.TagetValue);
                                            }
                                            
                                            if (tagetvalue - feedback.Offset <= num && num <= tagetvalue + feedback.Offset)
                                            {
                                                testStep.Result = "通过";
                                                await sqlSugarClient.Updateable(testStep).ExecuteCommandAsync();
                                                var list = await sqlSugarClient.Queryable<TestStep>().Mapper(it => it.Inits, it => it.Inits.First().TestStepId).Mapper(it => it.FeedBacks, it => it.FeedBacks.First().TestStepId).Where(x => x.ModuleId == CurrentDto.Id).ToListAsync();
                                                TestStepDtos.Clear();
                                                foreach (var item2 in list)
                                                {
                                                    TestStepDtos.Add(new TestStepDto
                                                    {
                                                        Conditions = item2.Conditions,
                                                        FeedBacks = item2.FeedBacks,
                                                        Id = item2.Id,
                                                        Inits = item2.Inits,
                                                        JudgmentContent = item2.JudgmentContent,
                                                        ModuleId = item2.ModuleId,
                                                        Remark = item2.Remark,
                                                        Result = item2.Result,
                                                        TestContent = item2.TestContent,
                                                        TestProcess = item2.TestProcess
                                                    });
                                                }
                                            }
                                            else
                                            {
                                                testStep.Result = $"未通过,当前值为{result}";
                                                Color = new SolidColorBrush(Colors.Red);
                                                await sqlSugarClient.Updateable(testStep).ExecuteCommandAsync();
                                                var list = await sqlSugarClient.Queryable<TestStep>().Mapper(it => it.Inits, it => it.Inits.First().TestStepId).Mapper(it => it.FeedBacks, it => it.FeedBacks.First().TestStepId).Where(x => x.ModuleId == CurrentDto.Id).ToListAsync();
                                                TestStepDtos.Clear();
                                                foreach (var item2 in list)
                                                {
                                                    TestStepDtos.Add(new TestStepDto
                                                    {
                                                        Conditions = item2.Conditions,
                                                        FeedBacks = item2.FeedBacks,
                                                        Id = item2.Id,
                                                        Inits = item2.Inits,
                                                        JudgmentContent = item2.JudgmentContent,
                                                        ModuleId = item2.ModuleId,
                                                        Remark = item2.Remark,
                                                        Result = item2.Result,
                                                        TestContent = item2.TestContent,
                                                        TestProcess = item2.TestProcess
                                                    });
                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (result != feedback.TagetValue)
                                            {
                                                testStep.Result = $"未通过,当前值为{result}";
                                                Color = new SolidColorBrush(Colors.Red);
                                                await sqlSugarClient.Updateable(testStep).ExecuteCommandAsync();
                                                var list = await sqlSugarClient.Queryable<TestStep>().Mapper(it => it.Inits, it => it.Inits.First().TestStepId).Mapper(it => it.FeedBacks, it => it.FeedBacks.First().TestStepId).Where(x => x.ModuleId == CurrentDto.Id).ToListAsync();
                                                TestStepDtos.Clear();
                                                foreach (var item2 in list)
                                                {
                                                    TestStepDtos.Add(new TestStepDto
                                                    {
                                                        Conditions = item2.Conditions,
                                                        FeedBacks = item2.FeedBacks,
                                                        Id = item2.Id,
                                                        Inits = item2.Inits,
                                                        JudgmentContent = item2.JudgmentContent,
                                                        ModuleId = item2.ModuleId,
                                                        Remark = item2.Remark,
                                                        Result = item2.Result,
                                                        TestContent = item2.TestContent,
                                                        TestProcess = item2.TestProcess
                                                    });
                                                }
                                            }
                                            else
                                            {
                                                testStep.Result = "通过";
                                                await sqlSugarClient.Updateable(testStep).ExecuteCommandAsync();
                                                var list = await sqlSugarClient.Queryable<TestStep>().Mapper(it => it.Inits, it => it.Inits.First().TestStepId).Mapper(it => it.FeedBacks, it => it.FeedBacks.First().TestStepId).Where(x => x.ModuleId == CurrentDto.Id).ToListAsync();
                                                TestStepDtos.Clear();
                                                foreach (var item2 in list)
                                                {
                                                    TestStepDtos.Add(new TestStepDto
                                                    {
                                                        Conditions = item2.Conditions,
                                                        FeedBacks = item2.FeedBacks,
                                                        Id = item2.Id,
                                                        Inits = item2.Inits,
                                                        JudgmentContent = item2.JudgmentContent,
                                                        ModuleId = item2.ModuleId,
                                                        Remark = item2.Remark,
                                                        Result = item2.Result,
                                                        TestContent = item2.TestContent,
                                                        TestProcess = item2.TestProcess
                                                    });
                                                }
                                            }
                                        }
                                    }
                                    if (feedback.Operational == "大于")
                                    {
                                        if (Convert.ToUInt16(result) < Convert.ToUInt16(feedback.TagetValue))
                                        {
                                            testStep.Result = $"未通过,当前值为{result}";
                                            Color = new SolidColorBrush(Colors.Red);
                                            await sqlSugarClient.Updateable(testStep).ExecuteCommandAsync();
                                            var list = await sqlSugarClient.Queryable<TestStep>().Mapper(it => it.Inits, it => it.Inits.First().TestStepId).Mapper(it => it.FeedBacks, it => it.FeedBacks.First().TestStepId).Where(x => x.ModuleId == CurrentDto.Id).ToListAsync();
                                            TestStepDtos.Clear();
                                            foreach (var item2 in list)
                                            {
                                                TestStepDtos.Add(new TestStepDto
                                                {
                                                    Conditions = item2.Conditions,
                                                    FeedBacks = item2.FeedBacks,
                                                    Id = item2.Id,
                                                    Inits = item2.Inits,
                                                    JudgmentContent = item2.JudgmentContent,
                                                    ModuleId = item2.ModuleId,
                                                    Remark = item2.Remark,
                                                    Result = item2.Result,
                                                    TestContent = item2.TestContent,
                                                    TestProcess = item2.TestProcess
                                                });
                                            }
                                        }
                                        else
                                        {
                                            testStep.Result = "通过";
                                            await sqlSugarClient.Updateable(testStep).ExecuteCommandAsync();
                                            var list = await sqlSugarClient.Queryable<TestStep>().Mapper(it => it.Inits, it => it.Inits.First().TestStepId).Mapper(it => it.FeedBacks, it => it.FeedBacks.First().TestStepId).Where(x => x.ModuleId == CurrentDto.Id).ToListAsync();
                                            TestStepDtos.Clear();
                                            foreach (var item2 in list)
                                            {
                                                TestStepDtos.Add(new TestStepDto
                                                {
                                                    Conditions = item2.Conditions,
                                                    FeedBacks = item2.FeedBacks,
                                                    Id = item2.Id,
                                                    Inits = item2.Inits,
                                                    JudgmentContent = item2.JudgmentContent,
                                                    ModuleId = item2.ModuleId,
                                                    Remark = item2.Remark,
                                                    Result = item2.Result,
                                                    TestContent = item2.TestContent,
                                                    TestProcess = item2.TestProcess
                                                });
                                            }
                                        }
                                    }
                                    if (feedback.Operational == "小于")
                                    {
                                        if (Convert.ToUInt16(result) > Convert.ToUInt16(feedback.TagetValue))
                                        {
                                            testStep.Result = $"未通过,当前值为{result}";
                                            Color = new SolidColorBrush(Colors.Red);
                                            await sqlSugarClient.Updateable(testStep).ExecuteCommandAsync();
                                            var list = await sqlSugarClient.Queryable<TestStep>().Mapper(it => it.Inits, it => it.Inits.First().TestStepId).Mapper(it => it.FeedBacks, it => it.FeedBacks.First().TestStepId).Where(x => x.ModuleId == CurrentDto.Id).ToListAsync();
                                            TestStepDtos.Clear();
                                            foreach (var item2 in list)
                                            {
                                                TestStepDtos.Add(new TestStepDto
                                                {
                                                    Conditions = item2.Conditions,
                                                    FeedBacks = item2.FeedBacks,
                                                    Id = item2.Id,
                                                    Inits = item2.Inits,
                                                    JudgmentContent = item2.JudgmentContent,
                                                    ModuleId = item2.ModuleId,
                                                    Remark = item2.Remark,
                                                    Result = item2.Result,
                                                    TestContent = item2.TestContent,
                                                    TestProcess = item2.TestProcess
                                                });
                                            }
                                        }
                                        else
                                        {
                                            testStep.Result = "通过";
                                            await sqlSugarClient.Updateable(testStep).ExecuteCommandAsync();
                                            var list = await sqlSugarClient.Queryable<TestStep>().Mapper(it => it.Inits, it => it.Inits.First().TestStepId).Mapper(it => it.FeedBacks, it => it.FeedBacks.First().TestStepId).Where(x => x.ModuleId == CurrentDto.Id).ToListAsync();
                                            TestStepDtos.Clear();
                                            foreach (var item2 in list)
                                            {
                                                TestStepDtos.Add(new TestStepDto
                                                {
                                                    Conditions = item2.Conditions,
                                                    FeedBacks = item2.FeedBacks,
                                                    Id = item2.Id,
                                                    Inits = item2.Inits,
                                                    JudgmentContent = item2.JudgmentContent,
                                                    ModuleId = item2.ModuleId,
                                                    Remark = item2.Remark,
                                                    Result = item2.Result,
                                                    TestContent = item2.TestContent,
                                                    TestProcess = item2.TestProcess
                                                });
                                            }
                                        }
                                    }
                                    await Task.Delay(feedback.DelayTime * 1000);
                                }
                                if (feedback.DelayModeId == 2)
                                {
                                    while ((DateTime.Now - now).TotalSeconds < feedback.DelayTime)
                                    {
                                        if (register.RegisterType == "基础参数" || register.RegisterType == "内机控制参数" || register.RegisterType == "模拟量输出")
                                        {
                                            if (register.Name.Contains("温度"))
                                            {
                                                result = ((await ModbusSerialMaster.ReadHoldingRegistersAsync(register.StationNum, register.Address, 1))[0] / 10.0).ToString();
                                            }
                                            else
                                            {
                                                result = (await ModbusSerialMaster.ReadHoldingRegistersAsync(register.StationNum, register.Address, 1))[0].ToString();
                                            }
                                        }
                                        if (register.RegisterType == "模拟量输入")
                                        {
                                            if (register.Name.Contains("温度"))
                                            {
                                                result = ((await ModbusSerialMaster.ReadInputRegistersAsync(register.StationNum, register.Address, 1))[0] / 10).ToString();
                                            }
                                            else
                                            {
                                                result = (await ModbusSerialMaster.ReadInputRegistersAsync(register.StationNum, register.Address, 1))[0].ToString();
                                            }
                                        }
                                        if (register.RegisterType == "数字量输出")
                                        {
                                            result = (await ModbusSerialMaster.ReadCoilsAsync(register.StationNum, register.Address, 1))[0].ToString();
                                        }
                                        if (register.RegisterType == "数字量输入")
                                        {
                                            result = (await ModbusSerialMaster.ReadInputsAsync(register.StationNum, register.Address, 1))[0].ToString();

                                        }
                                        if (register.RegisterType == "内机数据" || register.RegisterType == "外机数据" || register.RegisterType == "步进电机脉冲检测")
                                        {
                                            if (register.Name.Contains("温度") || register.Name.Contains("频率") || register.Name.Contains("电流") || register.Name.Contains("功率") || register.Name.Contains("容量"))
                                            {
                                                result = ((await ModbusSerialMaster.ReadInputRegistersAsync(register.StationNum, register.Address, 1))[0] / 10.0).ToString();
                                            }
                                            else
                                            {
                                                result = (await ModbusSerialMaster.ReadInputRegistersAsync(register.StationNum, register.Address, 1))[0].ToString();
                                            }
                                        }
                                        if (feedback.Operational == "等于")
                                        {
                                            if (result != feedback.TagetValue)
                                            {
                                                testStep.Result = $"未通过,当前值为{result}";
                                                Color = new SolidColorBrush(Colors.Red);
                                                await sqlSugarClient.Updateable(testStep).ExecuteCommandAsync();
                                                var list = await sqlSugarClient.Queryable<TestStep>().Mapper(it => it.Inits, it => it.Inits.First().TestStepId).Mapper(it => it.FeedBacks, it => it.FeedBacks.First().TestStepId).Where(x => x.ModuleId == CurrentDto.Id).ToListAsync();
                                                TestStepDtos.Clear();
                                                foreach (var item2 in list)
                                                {
                                                    TestStepDtos.Add(new TestStepDto
                                                    {
                                                        Conditions = item2.Conditions,
                                                        FeedBacks = item2.FeedBacks,
                                                        Id = item2.Id,
                                                        Inits = item2.Inits,
                                                        JudgmentContent = item2.JudgmentContent,
                                                        ModuleId = item2.ModuleId,
                                                        Remark = item2.Remark,
                                                        Result = item2.Result,
                                                        TestContent = item2.TestContent,
                                                        TestProcess = item2.TestProcess
                                                    });
                                                }
                                            }
                                            else
                                            {
                                                testStep.Result = "通过";
                                                await sqlSugarClient.Updateable(testStep).ExecuteCommandAsync();
                                                var list = await sqlSugarClient.Queryable<TestStep>().Mapper(it => it.Inits, it => it.Inits.First().TestStepId).Mapper(it => it.FeedBacks, it => it.FeedBacks.First().TestStepId).Where(x => x.ModuleId == CurrentDto.Id).ToListAsync();
                                                TestStepDtos.Clear();
                                                foreach (var item2 in list)
                                                {
                                                    TestStepDtos.Add(new TestStepDto
                                                    {
                                                        Conditions = item2.Conditions,
                                                        FeedBacks = item2.FeedBacks,
                                                        Id = item2.Id,
                                                        Inits = item2.Inits,
                                                        JudgmentContent = item2.JudgmentContent,
                                                        ModuleId = item2.ModuleId,
                                                        Remark = item2.Remark,
                                                        Result = item2.Result,
                                                        TestContent = item2.TestContent,
                                                        TestProcess = item2.TestProcess
                                                    });
                                                }
                                            }
                                        }
                                        if (feedback.Operational == "大于")
                                        {
                                            if (Convert.ToUInt16(result) < Convert.ToUInt16(feedback.TagetValue))
                                            {
                                                testStep.Result = $"未通过,当前值为{result}";
                                                Color = new SolidColorBrush(Colors.Red);
                                                await sqlSugarClient.Updateable(testStep).ExecuteCommandAsync();
                                                var list = await sqlSugarClient.Queryable<TestStep>().Mapper(it => it.Inits, it => it.Inits.First().TestStepId).Mapper(it => it.FeedBacks, it => it.FeedBacks.First().TestStepId).Where(x => x.ModuleId == CurrentDto.Id).ToListAsync();
                                                TestStepDtos.Clear();
                                                foreach (var item2 in list)
                                                {
                                                    TestStepDtos.Add(new TestStepDto
                                                    {
                                                        Conditions = item2.Conditions,
                                                        FeedBacks = item2.FeedBacks,
                                                        Id = item2.Id,
                                                        Inits = item2.Inits,
                                                        JudgmentContent = item2.JudgmentContent,
                                                        ModuleId = item2.ModuleId,
                                                        Remark = item2.Remark,
                                                        Result = item2.Result,
                                                        TestContent = item2.TestContent,
                                                        TestProcess = item2.TestProcess
                                                    });
                                                }
                                            }
                                            else
                                            {
                                                testStep.Result = "通过";
                                                await sqlSugarClient.Updateable(testStep).ExecuteCommandAsync();
                                                var list = await sqlSugarClient.Queryable<TestStep>().Mapper(it => it.Inits, it => it.Inits.First().TestStepId).Mapper(it => it.FeedBacks, it => it.FeedBacks.First().TestStepId).Where(x => x.ModuleId == CurrentDto.Id).ToListAsync();
                                                TestStepDtos.Clear();
                                                foreach (var item2 in list)
                                                {
                                                    TestStepDtos.Add(new TestStepDto
                                                    {
                                                        Conditions = item2.Conditions,
                                                        FeedBacks = item2.FeedBacks,
                                                        Id = item2.Id,
                                                        Inits = item2.Inits,
                                                        JudgmentContent = item2.JudgmentContent,
                                                        ModuleId = item2.ModuleId,
                                                        Remark = item2.Remark,
                                                        Result = item2.Result,
                                                        TestContent = item2.TestContent,
                                                        TestProcess = item2.TestProcess
                                                    });
                                                }
                                            }
                                        }
                                        if (feedback.Operational == "小于")
                                        {
                                            if (Convert.ToUInt16(result) > Convert.ToUInt16(feedback.TagetValue))
                                            {
                                                testStep.Result = $"未通过,当前值为{result}";
                                                Color = new SolidColorBrush(Colors.Red);
                                                await sqlSugarClient.Updateable(testStep).ExecuteCommandAsync();
                                                var list = await sqlSugarClient.Queryable<TestStep>().Mapper(it => it.Inits, it => it.Inits.First().TestStepId).Mapper(it => it.FeedBacks, it => it.FeedBacks.First().TestStepId).Where(x => x.ModuleId == CurrentDto.Id).ToListAsync();
                                                TestStepDtos.Clear();
                                                foreach (var item2 in list)
                                                {
                                                    TestStepDtos.Add(new TestStepDto
                                                    {
                                                        Conditions = item2.Conditions,
                                                        FeedBacks = item2.FeedBacks,
                                                        Id = item2.Id,
                                                        Inits = item2.Inits,
                                                        JudgmentContent = item2.JudgmentContent,
                                                        ModuleId = item2.ModuleId,
                                                        Remark = item2.Remark,
                                                        Result = item2.Result,
                                                        TestContent = item2.TestContent,
                                                        TestProcess = item2.TestProcess
                                                    });
                                                }
                                            }
                                            else
                                            {
                                                testStep.Result = "通过";
                                                await sqlSugarClient.Updateable(testStep).ExecuteCommandAsync();
                                                var list = await sqlSugarClient.Queryable<TestStep>().Mapper(it => it.Inits, it => it.Inits.First().TestStepId).Mapper(it => it.FeedBacks, it => it.FeedBacks.First().TestStepId).Where(x => x.ModuleId == CurrentDto.Id).ToListAsync();
                                                TestStepDtos.Clear();
                                                foreach (var item2 in list)
                                                {
                                                    TestStepDtos.Add(new TestStepDto
                                                    {
                                                        Conditions = item2.Conditions,
                                                        FeedBacks = item2.FeedBacks,
                                                        Id = item2.Id,
                                                        Inits = item2.Inits,
                                                        JudgmentContent = item2.JudgmentContent,
                                                        ModuleId = item2.ModuleId,
                                                        Remark = item2.Remark,
                                                        Result = item2.Result,
                                                        TestContent = item2.TestContent,
                                                        TestProcess = item2.TestProcess
                                                    });
                                                }
                                            }
                                        }
                                    }
                                }
                                if (feedback.DelayModeId == 3)
                                {
                                    await Task.Delay(feedback.DelayTime * 1000);
                                    if (register.RegisterType == "基础参数" || register.RegisterType == "内机控制参数" || register.RegisterType == "模拟量输出")
                                    {
                                        if (register.Name.Contains("温度"))
                                        {
                                            result = ((await ModbusSerialMaster.ReadHoldingRegistersAsync(register.StationNum, register.Address, 1))[0] / 10).ToString();
                                        }
                                        else
                                        {
                                            result = (await ModbusSerialMaster.ReadHoldingRegistersAsync(register.StationNum, register.Address, 1))[0].ToString();
                                        }
                                    }
                                    if (register.RegisterType == "模拟量输入")
                                    {
                                        if (register.Name.Contains("温度"))
                                        {
                                            result = ((await ModbusSerialMaster.ReadInputRegistersAsync(register.StationNum, register.Address, 1))[0] / 10).ToString();
                                        }
                                        else
                                        {
                                            result = (await ModbusSerialMaster.ReadInputRegistersAsync(register.StationNum, register.Address, 1))[0].ToString();
                                        }
                                    }
                                    if (register.RegisterType == "数字量输出")
                                    {
                                        result = Convert.ToInt16((await ModbusSerialMaster.ReadCoilsAsync(register.StationNum, register.Address, 1))[0]).ToString();
                                    }
                                    if (register.RegisterType == "数字量输入")
                                    {
                                        result = Convert.ToInt16((await ModbusSerialMaster.ReadInputsAsync(register.StationNum, register.Address, 1))[0]).ToString();

                                    }
                                    if (register.RegisterType == "内机数据" || register.RegisterType == "外机数据" )
                                    {
                                        if (register.Name.Contains("温度") || register.Name.Contains("频率") || register.Name.Contains("电流") || register.Name.Contains("功率") || register.Name.Contains("容量"))
                                        {
                                            result = ((await ModbusSerialMaster.ReadInputRegistersAsync(register.StationNum, register.Address, 1))[0] / 10.0).ToString();
                                        }
                                        else
                                        {
                                            result = (await ModbusSerialMaster.ReadInputRegistersAsync(register.StationNum, register.Address, 1))[0].ToString();
                                        }
                                    }
                                    if (register.RegisterType == "步进电机脉冲检测")
                                    {
                                        result= (await ModbusSerialMaster.ReadInputRegistersAsync(register.StationNum, register.Address, 1))[0].ToString();
                                    }
                                    if (register.RegisterType == "20个温度设置")
                                    {
                                        result = ((await ModbusSerialMaster.ReadHoldingRegistersAsync(register.StationNum, register.Address, 1))[0] / 10.0).ToString();
                                    }
                                    if (feedback.Operational == "等于")
                                    {
                                        if (result != feedback.TagetValue)
                                        {
                                            testStep.Result = $"未通过,当前值为{result}";
                                            Color = new SolidColorBrush(Colors.Red);
                                            await sqlSugarClient.Updateable(testStep).ExecuteCommandAsync();
                                            var list = await sqlSugarClient.Queryable<TestStep>().Mapper(it => it.Inits, it => it.Inits.First().TestStepId).Mapper(it => it.FeedBacks, it => it.FeedBacks.First().TestStepId).Where(x => x.ModuleId == CurrentDto.Id).ToListAsync();
                                            TestStepDtos.Clear();
                                            foreach (var item2 in list)
                                            {
                                                TestStepDtos.Add(new TestStepDto
                                                {
                                                    Conditions = item2.Conditions,
                                                    FeedBacks = item2.FeedBacks,
                                                    Id = item2.Id,
                                                    Inits = item2.Inits,
                                                    JudgmentContent = item2.JudgmentContent,
                                                    ModuleId = item2.ModuleId,
                                                    Remark = item2.Remark,
                                                    Result = item2.Result,
                                                    TestContent = item2.TestContent,
                                                    TestProcess = item2.TestProcess
                                                });
                                            }
                                        }
                                        else
                                        {
                                            testStep.Result = "通过";
                                            await sqlSugarClient.Updateable(testStep).ExecuteCommandAsync();
                                            var list = await sqlSugarClient.Queryable<TestStep>().Mapper(it => it.Inits, it => it.Inits.First().TestStepId).Mapper(it => it.FeedBacks, it => it.FeedBacks.First().TestStepId).Where(x => x.ModuleId == CurrentDto.Id).ToListAsync();
                                            TestStepDtos.Clear();
                                            foreach (var item2 in list)
                                            {
                                                TestStepDtos.Add(new TestStepDto
                                                {
                                                    Conditions = item2.Conditions,
                                                    FeedBacks = item2.FeedBacks,
                                                    Id = item2.Id,
                                                    Inits = item2.Inits,
                                                    JudgmentContent = item2.JudgmentContent,
                                                    ModuleId = item2.ModuleId,
                                                    Remark = item2.Remark,
                                                    Result = item2.Result,
                                                    TestContent = item2.TestContent,
                                                    TestProcess = item2.TestProcess
                                                });
                                            }
                                        }
                                    }
                                    if (feedback.Operational == "大于")
                                    {
                                        if (Convert.ToUInt16(result) < Convert.ToUInt16(feedback.TagetValue))
                                        {
                                            testStep.Result = $"未通过,当前值为{result}";
                                            Color = new SolidColorBrush(Colors.Red);
                                            await sqlSugarClient.Updateable(testStep).ExecuteCommandAsync();
                                            var list = await sqlSugarClient.Queryable<TestStep>().Mapper(it => it.Inits, it => it.Inits.First().TestStepId).Mapper(it => it.FeedBacks, it => it.FeedBacks.First().TestStepId).Where(x => x.ModuleId == CurrentDto.Id).ToListAsync();
                                            TestStepDtos.Clear();
                                            foreach (var item2 in list)
                                            {
                                                TestStepDtos.Add(new TestStepDto
                                                {
                                                    Conditions = item2.Conditions,
                                                    FeedBacks = item2.FeedBacks,
                                                    Id = item2.Id,
                                                    Inits = item2.Inits,
                                                    JudgmentContent = item2.JudgmentContent,
                                                    ModuleId = item2.ModuleId,
                                                    Remark = item2.Remark,
                                                    Result = item2.Result,
                                                    TestContent = item2.TestContent,
                                                    TestProcess = item2.TestProcess
                                                });
                                            }
                                        }
                                        else
                                        {
                                            testStep.Result = "通过";
                                            await sqlSugarClient.Updateable(testStep).ExecuteCommandAsync();
                                            var list = await sqlSugarClient.Queryable<TestStep>().Mapper(it => it.Inits, it => it.Inits.First().TestStepId).Mapper(it => it.FeedBacks, it => it.FeedBacks.First().TestStepId).Where(x => x.ModuleId == CurrentDto.Id).ToListAsync();
                                            TestStepDtos.Clear();
                                            foreach (var item2 in list)
                                            {
                                                TestStepDtos.Add(new TestStepDto
                                                {
                                                    Conditions = item2.Conditions,
                                                    FeedBacks = item2.FeedBacks,
                                                    Id = item2.Id,
                                                    Inits = item2.Inits,
                                                    JudgmentContent = item2.JudgmentContent,
                                                    ModuleId = item2.ModuleId,
                                                    Remark = item2.Remark,
                                                    Result = item2.Result,
                                                    TestContent = item2.TestContent,
                                                    TestProcess = item2.TestProcess
                                                });
                                            }
                                        }
                                    }
                                    if (feedback.Operational == "小于")
                                    {
                                        if (Convert.ToUInt16(result) > Convert.ToUInt16(feedback.TagetValue))
                                        {
                                            testStep.Result = $"未通过,当前值为{result}";
                                            Color = new SolidColorBrush(Colors.Red);
                                            await sqlSugarClient.Updateable(testStep).ExecuteCommandAsync();
                                            var list = await sqlSugarClient.Queryable<TestStep>().Mapper(it => it.Inits, it => it.Inits.First().TestStepId).Mapper(it => it.FeedBacks, it => it.FeedBacks.First().TestStepId).Where(x => x.ModuleId == CurrentDto.Id).ToListAsync();
                                            TestStepDtos.Clear();
                                            foreach (var item2 in list)
                                            {
                                                TestStepDtos.Add(new TestStepDto
                                                {
                                                    Conditions = item2.Conditions,
                                                    FeedBacks = item2.FeedBacks,
                                                    Id = item2.Id,
                                                    Inits = item2.Inits,
                                                    JudgmentContent = item2.JudgmentContent,
                                                    ModuleId = item2.ModuleId,
                                                    Remark = item2.Remark,
                                                    Result = item2.Result,
                                                    TestContent = item2.TestContent,
                                                    TestProcess = item2.TestProcess
                                                });
                                            }
                                        }
                                        else
                                        {
                                            testStep.Result = "通过";
                                            await sqlSugarClient.Updateable(testStep).ExecuteCommandAsync();
                                            var list = await sqlSugarClient.Queryable<TestStep>().Mapper(it => it.Inits, it => it.Inits.First().TestStepId).Mapper(it => it.FeedBacks, it => it.FeedBacks.First().TestStepId).Where(x => x.ModuleId == CurrentDto.Id).ToListAsync();
                                            TestStepDtos.Clear();
                                            foreach (var item2 in list)
                                            {
                                                TestStepDtos.Add(new TestStepDto
                                                {
                                                    Conditions = item2.Conditions,
                                                    FeedBacks = item2.FeedBacks,
                                                    Id = item2.Id,
                                                    Inits = item2.Inits,
                                                    JudgmentContent = item2.JudgmentContent,
                                                    ModuleId = item2.ModuleId,
                                                    Remark = item2.Remark,
                                                    Result = item2.Result,
                                                    TestContent = item2.TestContent,
                                                    TestProcess = item2.TestProcess
                                                });
                                            }
                                        }
                                    }
                                }
                                if (testStep.FeedBacks.Count == 0)
                                {
                                    testStep.Result = "通过";
                                    await sqlSugarClient.Updateable(testStep).ExecuteCommandAsync();
                                    var list = await sqlSugarClient.Queryable<TestStep>().Mapper(it => it.Inits, it => it.Inits.First().TestStepId).Mapper(it => it.FeedBacks, it => it.FeedBacks.First().TestStepId).Where(x => x.ModuleId == CurrentDto.Id).ToListAsync();
                                    TestStepDtos.Clear();
                                    foreach (var item2 in list)
                                    {
                                        TestStepDtos.Add(new TestStepDto
                                        {
                                            Conditions = item2.Conditions,
                                            FeedBacks = item2.FeedBacks,
                                            Id = item2.Id,
                                            Inits = item2.Inits,
                                            JudgmentContent = item2.JudgmentContent,
                                            ModuleId = item2.ModuleId,
                                            Remark = item2.Remark,
                                            Result = item2.Result,
                                            TestContent = item2.TestContent,
                                            TestProcess = item2.TestProcess
                                        });
                                    }
                                }
                            }
                            
                            int index = testSteps.IndexOf(testStep);
                            ProValue = Math.Round((double)(index + 1) / testSteps.Count * 100, 2);
                        }
                        DataTable x = await sqlSugarClient.Queryable<TestStep>().Where(x => x.ModuleId == item.Id).ToDataTableAsync();
                        sheets.Add($"{item.ModuleName}", x);
                    }
                    try
                    {
                        using (var stream = File.Create($"{name}.xlsx"))
                            MiniExcel.SaveAs(stream, sheets, true);
                    }
                    catch (Exception ex)
                    {
                       Growl.Error(ex.Message);
                    }

                });
                

            });
           
          
        }
    }
}
