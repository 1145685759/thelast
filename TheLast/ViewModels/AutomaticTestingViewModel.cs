using AutoMapper;
using Modbus.Device;
using Prism.Commands;
using Prism.Events;
using Prism.Ioc;
using Prism.Regions;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;
using TheLast.Common;
using TheLast.Dtos;
using TheLast.Entities;
using TheLast.Events;
using TheLast.Extensions;

namespace TheLast.ViewModels
{
    public class AutomaticTestingViewModel: NavigationViewModel
    {
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
        private int proValue;
        public int ProValue
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
            if (ModbusSerialMaster==null)
            {
                HandyControl.Controls.Growl.Info("未设置串口");
                return;
            }
            foreach (var item in CurrentModules)
            {
                var testSteps =await sqlSugarClient.Queryable<TestStep>().Mapper(it=>it.Inits,it=>it.Inits.First().TestStepId).Mapper(it=>it.FeedBacks,it=>it.FeedBacks.First().TestStepId).Where(x => x.ModuleId == item.Id).ToListAsync();
                CurrentTestStepDtos = new List<TestStepDto>();
                
                foreach (var item1 in testSteps)
                {
                    CurrentTestStepDtos.Add(new TestStepDto 
                    {
                        Conditions=item1.Conditions,
                        FeedBacks=item1.FeedBacks,
                        Id=item1.Id,
                        Inits=item1.Inits,
                        JudgmentContent=item1.JudgmentContent,
                        ModuleId=item1.ModuleId,
                        Remark=item1.Remark,
                        Result=item1.Result,
                        TestContent=item1.TestContent,
                        TestProcess=item1.TestProcess
                    });
                }
                foreach (var testStep in testSteps)
                {
                    foreach (var init in testStep.Inits)
                    {
                        var register = await sqlSugarClient.Queryable<Register>().FirstAsync(x => x.Id == init.RegisterId);
                        await ModbusSerialMaster.WriteSingleRegisterAsync(register.StationNum, register.Address, Convert.ToUInt16(init.WriteValue));
                    }
                    foreach (var feedback in testStep.FeedBacks)
                    {
                        var now = DateTime.Now;
                        var register = await sqlSugarClient.Queryable<Register>().FirstAsync(x => x.Id == feedback.RegisterId);
                        if (feedback.DelayModeId==1)
                        {
                           
                            var result= await ModbusSerialMaster.ReadHoldingRegistersAsync(register.StationNum, register.Address, 1);
                            if (result[0].ToString()!=feedback.TagetValue)
                            {
                                testStep.Result="未通过";
                                Color =new SolidColorBrush(Colors.Red);
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
                            await Task.Delay(feedback.DelayTime * 1000);
                        }
                        if (feedback.DelayModeId==2)
                        {
                            while ((DateTime.Now-now).TotalSeconds<feedback.DelayTime)
                            {
                                var result= await ModbusSerialMaster.ReadHoldingRegistersAsync(register.StationNum, register.Address, 1);
                                if (result[0].ToString() != feedback.TagetValue)
                                {
                                    testStep.Result = "未通过";
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
                        if (feedback.DelayModeId==3)
                        {
                            await Task.Delay(feedback.DelayTime * 1000);
                            var result = await ModbusSerialMaster.ReadHoldingRegistersAsync(register.StationNum, register.Address, 1);
                            if (result[0].ToString() != feedback.TagetValue)
                            {
                                testStep.Result = "未通过";
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
                    ProValue = (testSteps.IndexOf(testStep) + 1 / testSteps.Count) * 100;

                }
            }
        }
    }
}
