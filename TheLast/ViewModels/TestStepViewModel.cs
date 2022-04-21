using AutoMapper;
using MiniExcelLibs;
using Prism.Commands;
using Prism.Ioc;
using Prism.Regions;
using Prism.Services.Dialogs;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using TheLast.Common;
using TheLast.Dtos;
using TheLast.Entities;
using unvell.ReoGrid;

namespace TheLast.ViewModels
{
    public class TestStepViewModel : NavigationViewModel
    {

        public TestStepViewModel(IContainerProvider provider, ISqlSugarClient sqlSugarClient, IDialogHostService dialog, IMapper mapper, IRegionManager regionManager)
          : base(provider)
        {
            this.provider = provider;
            this.sqlSugarClient = sqlSugarClient;
            this.dialog = dialog;
            this.mapper = mapper;
            this.regionManager = regionManager;
            TestStepDtos = new ObservableCollection<TestStepDto>();
        }
        private string path;
        private TestStepDto currentItem;
        public TestStepDto CurrentItem
        {
            get { return currentItem; }
            set { SetProperty(ref currentItem, value); }
        }
        private ModuleDto currentDto;
        public ModuleDto CurrentDto
        {
            get { return currentDto; }
            set { SetProperty(ref currentDto, value); }
        }
        private ObservableCollection<TestStepDto> testStepDtos;
        public ObservableCollection<TestStepDto> TestStepDtos
        {
            get { return testStepDtos; }
            set { SetProperty(ref testStepDtos, value); }
        }
        private readonly IContainerProvider provider;
        private readonly ISqlSugarClient sqlSugarClient;
        private readonly IDialogHostService dialog;
        private readonly IMapper mapper;
        private readonly IRegionManager regionManager;
        
        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
            CurrentDto = navigationContext.Parameters.GetValue<ModuleDto>("Module");
            var list = await sqlSugarClient.Queryable<TestStep>().Where(x => x.ModuleId == CurrentDto.Id).ToListAsync();
            TestStepDtos.Clear();
            foreach (var item in list)
            {
                string initStr = string.Empty;
                string feedbackStr = string.Empty;
                var listInit = await sqlSugarClient.Queryable<Init>().Where(x => x.TestStepId == item.Id).ToListAsync();
                foreach (var init in listInit)
                {
                    var register = await sqlSugarClient.Queryable<Register>().FirstAsync(x => x.Id == init.RegisterId);
                    initStr += $"向站号【{register.StationNum}】寄存器【{register.Name}】写入值【{init.WriteValue}】\r\n";
                }
                var feedbackList = await sqlSugarClient.Queryable<FeedBack>().Where(x => x.TestStepId == item.Id).ToListAsync();
                foreach (var feedBack in feedbackList)
                {
                    var register = await sqlSugarClient.Queryable<Register>().FirstAsync(x => x.Id == feedBack.RegisterId);
                    feedbackStr += $"站号【{register.StationNum}】 寄存器【{register.Name}】{feedBack.Operational}【{feedBack.TagetValue}】\r\n";
                }
                TestStepDtos.Add(new TestStepDto
                {
                    Conditions = item.Conditions,
                    Inits = item.Inits,
                    ModuleId = item.ModuleId,
                    Remark = item.Remark,
                    Result = item.Result,
                    TestContent = item.TestContent,
                    TestProcess = initStr,
                    JudgmentContent = feedbackStr,
                    Id = item.Id
                });
            }
        }
        private DelegateCommand checkRegisters;
        public DelegateCommand CheckRegisters =>
            checkRegisters ?? (checkRegisters = new DelegateCommand(ExecuteCheckRegisters));

        async void ExecuteCheckRegisters()
        {
            var testSteps= await sqlSugarClient.Queryable<TestStep>().Where(x => x.ModuleId == CurrentDto.Id).ToListAsync();
            foreach (var testStep in testSteps)
            {
                var inits = await sqlSugarClient.Queryable<Init>().Where(x => x.TestStepId == testStep.Id).ToListAsync();
                foreach (var init in inits)
                {
                    var re = await sqlSugarClient.Queryable<Register>().FirstAsync(x => x.Id == init.RegisterId);
                    if (!re.IsEnable)
                    {
                        HandyControl.Controls.Growl.Error($"{re.Name}未启用，请检查");
                        return;
                    } 
                }
                var feedbacks = await sqlSugarClient.Queryable<FeedBack>().Where(x => x.TestStepId == testStep.Id).ToListAsync();
                foreach (var feedback in feedbacks)
                {
                    var re = await sqlSugarClient.Queryable<Register>().FirstAsync(x => x.Id == feedback.RegisterId);
                    if (!re.IsEnable)
                    {
                        HandyControl.Controls.Growl.Error($"{re.Name}未启用，请检查");
                        return;
                    }
                }
                HandyControl.Controls.Growl.Success("检查完毕，无异常！");
            }
        }
        private DelegateCommand<TestStepDto> deleteTestStep;
        public DelegateCommand<TestStepDto> DeleteTestStep =>
            deleteTestStep ?? (deleteTestStep = new DelegateCommand<TestStepDto>(ExecuteDeleteTestStep));

        async void ExecuteDeleteTestStep(TestStepDto parameter)
        {
            await sqlSugarClient.Deleteable<TestStep>().In(parameter.Id).ExecuteCommandAsync();
            TestStepDtos.Remove(parameter);
        }
        private DelegateCommand loadTestStep;
        public DelegateCommand LoadTestStep =>
            loadTestStep ?? (loadTestStep = new DelegateCommand(ExecuteLoadTestStep));

        async void ExecuteLoadTestStep()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "ExceL文件|*.xlsx";
            openFileDialog.RestoreDirectory = true;
            openFileDialog.FilterIndex = 1;
            if (openFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                path = openFileDialog.FileName;
            }
            else
            {
                return;
            }

            var rows = MiniExcel.Query<ExcelModel>(path).ToList();
            foreach (var item in rows)
            {
                List<string> xieruzhi = new List<string>();
                List<string> jicunqixieru = new List<string>();
                List<string> jicunqipanduan = new List<string>();
                List<string> mubiaozhi = new List<string>();
                List<string> zhanhao = new List<string>();
                List<string> yanshimoshi = new List<string>();
                List<string> yanshishijian = new List<string>();
                List<string> yunsuanfu = new List<string>();
                if (!string.IsNullOrEmpty(item.写入值))
                {
                    xieruzhi = item.写入值.Split("\n").ToList();
                }
                if (!string.IsNullOrEmpty(item.寄存器写入))
                {
                    jicunqixieru = item.寄存器写入.Split("\n").ToList();
                }
                if (!string.IsNullOrEmpty(item.寄存器判断))
                {
                    jicunqipanduan = item.寄存器判断.Split("\n").ToList();
                }
                if (!string.IsNullOrEmpty(item.运算符))
                {
                    yunsuanfu = item.运算符.Split("\n").ToList();
                }
                if (!string.IsNullOrEmpty(item.目标值))
                {
                    mubiaozhi = item.目标值.Split("\n").ToList();
                }
                if (!string.IsNullOrEmpty(item.站号))
                {
                    zhanhao = item.站号.Split("\n").ToList();
                }
                if (!string.IsNullOrEmpty(item.延时模式))
                {
                    yanshimoshi = item.延时模式.Split("\n").ToList();
                }
                if (!string.IsNullOrEmpty(item.延时时间))
                {
                    yanshishijian = item.延时时间.Split("\n").ToList();
                }

                string TestProcess = string.Empty;
                string JudgmentContent = string.Empty;
                List<Init> inits = new List<Init>();
                List<FeedBack> feedBacks = new List<FeedBack>();
                for (int i = 0; i < jicunqixieru.Count; i++)
                {
                    TestProcess += $"向 站号【{zhanhao[i].Remove(0,2)}】寄存器【{jicunqixieru[i].Remove(0, 2)}】写入值【{xieruzhi[i].Remove(0, 2)}】\r\n";
                }
                for (int i = 0; i < jicunqipanduan.Count; i++)
                {
                    JudgmentContent += $"站号【{zhanhao[i].Remove(0, 2)}】 寄存器【{jicunqipanduan[i].Remove(0, 2)}】{yunsuanfu[i].Remove(0, 2)}【{mubiaozhi[i].Remove(0, 2)}】\r\n";
                }

                TestStep testStep = new TestStep
                {
                    ModuleId = CurrentDto.Id,
                    Conditions = item.应用条件,
                    Remark = item.备注,
                    JudgmentContent = JudgmentContent,
                    TestProcess = TestProcess,
                    TestContent=item.测试内容
                };
                int id = await sqlSugarClient.Insertable(testStep).ExecuteReturnIdentityAsync();
                for (int i = 0; i < jicunqixieru.Count; i++)
                {
                    string registerName = jicunqixieru[i].Remove(0, 2);
                    string writevalue = xieruzhi[i].Remove(0, 2);
                    int registerid = (await sqlSugarClient.Queryable<Register>().FirstAsync(x => x.Name == registerName)).Id;
                    var valuedic = await sqlSugarClient.Queryable<ValueDictionary>().Where(x => x.RegisterId == registerid).ToListAsync();
                    if (valuedic.Count != 0)
                    {
                        Init init = new Init();
                        init.DisplayValue = writevalue;
                        init.RegisterId = (await sqlSugarClient.Queryable<Register>().FirstAsync(x => x.Name == registerName)).Id;
                        init.TestStepId = id;
                        var ValueDictionary = await sqlSugarClient.Queryable<ValueDictionary>().FirstAsync(x => x.DisplayValue == writevalue && x.RegisterId == registerid);
                        if (ValueDictionary!=null)
                        {
                            init.WriteValue = ValueDictionary.RealValue;
                        }
                        else
                        {
                            HandyControl.Controls.Growl.Error($"Excel配置格式错误:{rows.IndexOf(item)+2}行写入值错误");
                            await sqlSugarClient.Deleteable<TestStep>(id).ExecuteCommandAsync();
                            return;
                        }
                        
                        inits.Add(init);
                    }
                    else
                    {
                        inits.Add(new Init
                        {
                            DisplayValue = writevalue,
                            RegisterId = (await sqlSugarClient.Queryable<Register>().FirstAsync(x => x.Name == registerName)).Id,
                            TestStepId = id,
                            WriteValue = writevalue
                        });
                    }

                }
                for (int i = 0; i < jicunqipanduan.Count; i++)
                {
                    string registerName = jicunqipanduan[i].Remove(0, 2);
                    string tagetvalue = mubiaozhi[i].Remove(0, 2);
                    string delayMode = yanshimoshi[i].Remove(0, 2);
                    string yunsuanfuStr = yunsuanfu[i].Remove(0, 2);
                    int registerid = (await sqlSugarClient.Queryable<Register>().FirstAsync(x => x.Name == registerName)).Id;
                    if (sqlSugarClient.Queryable<ValueDictionary>().Where(x => x.RegisterId == registerid).ToList().Count != 0)
                    {
                        try
                        {
                            var tagetvalues= await sqlSugarClient.Queryable<ValueDictionary>().FirstAsync(x => x.DisplayValue == tagetvalue && x.RegisterId == registerid);
                            FeedBack feedBack = new FeedBack();
                            feedBack.Operational = yunsuanfuStr;
                            feedBack.DelayModeId = (await sqlSugarClient.Queryable<DelayModel>().FirstAsync(x => x.Mode == delayMode)).Id;
                            feedBack.DelayTime = Convert.ToInt32(yanshishijian[i].Remove(0, 2));
                            feedBack.StationNum = Convert.ToByte(zhanhao[i].Remove(0, 2));
                            feedBack.DisplayTagetValue = tagetvalue;
                            feedBack.RegisterId = (await sqlSugarClient.Queryable<Register>().FirstAsync(x => x.Name == registerName)).Id;
                            feedBack.TestStepId = id;
                            if (tagetvalues!=null)
                            {
                                feedBack.TagetValue = tagetvalues.RealValue;
                            }
                            else
                            {
                                HandyControl.Controls.Growl.Error($"Excel配置格式错误:{rows.IndexOf(item)+2}行目标值错误");
                                await sqlSugarClient.Deleteable<TestStep>(id).ExecuteCommandAsync();
                                return;
                            }
                            feedBacks.Add(feedBack);
                        }
                        catch (Exception ex)
                        {
                            HandyControl.Controls.Growl.Error($"Excel配置格式错误{ex.Message}");
                        }
                       

                    }
                    else
                    {
                        feedBacks.Add(new FeedBack
                        {
                            Operational=yunsuanfuStr,
                            DelayModeId = (await sqlSugarClient.Queryable<DelayModel>().FirstAsync(x => x.Mode == delayMode)).Id,
                            DelayTime = Convert.ToInt32(yanshishijian[i].Remove(0, 2)),
                            StationNum = Convert.ToByte(zhanhao[i].Remove(0, 2)),
                            DisplayTagetValue = tagetvalue,
                            RegisterId = (await sqlSugarClient.Queryable<Register>().FirstAsync(x => x.Name == registerName)).Id,
                            TestStepId = id,
                            TagetValue = tagetvalue,
                        });
                    }
                }
                await sqlSugarClient.Insertable(inits).ExecuteCommandAsync();
                await sqlSugarClient.Insertable(feedBacks).ExecuteCommandAsync();
            }
            var list = await sqlSugarClient.Queryable<TestStep>().Where(x => x.ModuleId == CurrentDto.Id).ToListAsync();
            TestStepDtos.Clear();
            foreach (var item in list)
            {
                string initStr = string.Empty;
                string feedbackStr = string.Empty;
                var listInit = await sqlSugarClient.Queryable<Init>().Where(x => x.TestStepId == item.Id).ToListAsync();
                foreach (var init in listInit)
                {
                    var register = await sqlSugarClient.Queryable<Register>().FirstAsync(x => x.Id == init.RegisterId);
                    initStr += $"向站号【{register.StationNum}】寄存器【{register.Name}】写入值【{init.WriteValue}】\r\n";
                }
                var feedbackList = await sqlSugarClient.Queryable<FeedBack>().Where(x => x.TestStepId == item.Id).ToListAsync();
                foreach (var feedBack in feedbackList)
                {
                    var register = await sqlSugarClient.Queryable<Register>().FirstAsync(x => x.Id == feedBack.RegisterId);
                    feedbackStr += $"站号【{register.StationNum}】 寄存器【{register.Name}】{feedBack.Operational}【{feedBack.TagetValue}】\r\n";
                }
                TestStepDtos.Add(new TestStepDto
                {
                    Conditions = item.Conditions,
                    Inits = item.Inits,
                    ModuleId = item.ModuleId,
                    Remark = item.Remark,
                    Result = item.Result,
                    FeedBacks = item.FeedBacks,
                    TestContent = item.TestContent,
                    TestProcess = initStr,
                    JudgmentContent = feedbackStr,
                    Id = item.Id
                });

            }
        }
        private DelegateCommand jumpConfig;
        public DelegateCommand JumpConfig =>
            jumpConfig ?? (jumpConfig = new DelegateCommand(ExecuteJumpConfig));

        async void ExecuteJumpConfig()
        {
            DialogParameters param = new DialogParameters();
            param.Add("ModuleId", CurrentDto.Id);
            var dialogResult = await dialog.ShowDialog("JumpConfig", param);
        }
        private DelegateCommand<TestStepDto> configInit;
        public DelegateCommand<TestStepDto> ConfigInit =>
            configInit ?? (configInit = new DelegateCommand<TestStepDto>(ExecuteConfigInit));

        async void ExecuteConfigInit(TestStepDto testStepDto)
        {
            DialogParameters param = new DialogParameters();
            param.Add("Value", testStepDto);
            var dialogResult = await dialog.ShowDialog("AddInit", param);
            if (dialogResult.Result == ButtonResult.OK)
            {
                try
                {
                    UpdateLoading(true);
                    var list = await sqlSugarClient.Queryable<TestStep>().Where(x => x.ModuleId == CurrentDto.Id).ToListAsync();
                    TestStepDtos.Clear();
                    foreach (var item in list)
                    {
                        string initStr = string.Empty;
                        string feedbackStr = string.Empty;
                        var listInit = await sqlSugarClient.Queryable<Init>().Where(x => x.TestStepId == item.Id).ToListAsync();
                        foreach (var init in listInit)
                        {
                            var register = await sqlSugarClient.Queryable<Register>().FirstAsync(x => x.Id == init.RegisterId);
                            initStr += $"向站号【{register.StationNum}】寄存器【{register.Name}】写入值【{init.WriteValue}】\r\n";
                        }
                        var feedbackList = await sqlSugarClient.Queryable<FeedBack>().Where(x => x.TestStepId == item.Id).ToListAsync();
                        foreach (var feedBack in feedbackList)
                        {
                            var register = await sqlSugarClient.Queryable<Register>().FirstAsync(x => x.Id == feedBack.RegisterId);
                            feedbackStr += $"站号【{register.StationNum}】 寄存器【{register.Name}】{feedBack.Operational}【{feedBack.TagetValue}】\r\n";
                        }
                        TestStepDtos.Add(new TestStepDto
                        {
                            Conditions = item.Conditions,
                            Inits = item.Inits,
                            ModuleId = item.ModuleId,
                            Remark = item.Remark,
                            Result = item.Result,
                            TestContent = item.TestContent,
                            TestProcess = initStr,
                            JudgmentContent = feedbackStr,
                            Id = item.Id
                        });
                    }

                }
                finally
                {
                    UpdateLoading(false);
                }
            }
        }
        private DelegateCommand<TestStepDto> configFeedback;
        public DelegateCommand<TestStepDto> ConfigFeedback =>
            configFeedback ?? (configFeedback = new DelegateCommand<TestStepDto>(ExecuteConfigFeedback));

        async void ExecuteConfigFeedback(TestStepDto testStepDto)
        {
            DialogParameters param = new DialogParameters();
            param.Add("Value", testStepDto);
            var dialogResult = await dialog.ShowDialog("AddFeedback", param);
            if (dialogResult.Result == ButtonResult.OK)
            {
                try
                {
                    UpdateLoading(true);
                    var list = await sqlSugarClient.Queryable<TestStep>().Where(x => x.ModuleId == CurrentDto.Id).ToListAsync();
                    TestStepDtos.Clear();
                    foreach (var item in list)
                    {
                        string initStr = string.Empty;
                        string feedbackStr = string.Empty;
                        var listInit = await sqlSugarClient.Queryable<Init>().Where(x => x.TestStepId == item.Id).ToListAsync();
                        var listFeedback = await sqlSugarClient.Queryable<FeedBack>().Where(x => x.TestStepId == item.Id).ToListAsync();
                        foreach (var init in listInit)
                        {
                            var register = await sqlSugarClient.Queryable<Register>().FirstAsync(x => x.Id == init.RegisterId);
                            initStr += $"向站号【{register.StationNum}】寄存器【{register.Name}】写入值【{init.WriteValue}】\r\n";
                        }
                        var feedbackList = await sqlSugarClient.Queryable<FeedBack>().Where(x => x.TestStepId == item.Id).ToListAsync();
                        foreach (var feedBack in feedbackList)
                        {
                            var register = await sqlSugarClient.Queryable<Register>().FirstAsync(x => x.Id == feedBack.RegisterId);
                            feedbackStr += $"站号【{register.StationNum}】 寄存器【{register.Name}】{feedBack.Operational}【{feedBack.TagetValue}】\r\n";
                        }
                        TestStepDtos.Add(new TestStepDto
                        {
                            Conditions = item.Conditions,
                            Inits = item.Inits,
                            ModuleId = item.ModuleId,
                            Remark = item.Remark,
                            Result = item.Result,
                            TestContent = item.TestContent,
                            TestProcess = initStr,
                            JudgmentContent = feedbackStr,
                            Id = item.Id
                        });
                    }
                }
                finally
                {
                    UpdateLoading(false);
                }
            }
        }
        private DelegateCommand addTestStep;
        public DelegateCommand AddTestStep =>
            addTestStep ?? (addTestStep = new DelegateCommand(ExecuteAddTestStep));

        async void ExecuteAddTestStep()
        {
            await sqlSugarClient.Insertable(new TestStep { ModuleId = CurrentDto.Id }).ExecuteCommandAsync();
            var list = await sqlSugarClient.Queryable<TestStep>().Where(x => x.ModuleId == CurrentDto.Id).ToListAsync();
            TestStepDtos.Clear();
            foreach (var item in list)
            {
                string initStr = string.Empty;
                string feedbackStr = string.Empty;
                var listInit = await sqlSugarClient.Queryable<Init>().Where(x => x.TestStepId == item.Id).ToListAsync();
                foreach (var init in listInit)
                {
                    var register = await sqlSugarClient.Queryable<Register>().FirstAsync(x => x.Id == init.RegisterId);
                    initStr += $"向站号【{register.StationNum}】寄存器【{register.Name}】写入值【{init.WriteValue}】\r\n";
                }
                var feedbackList = await sqlSugarClient.Queryable<FeedBack>().Where(x => x.TestStepId == item.Id).ToListAsync();
                foreach (var feedBack in feedbackList)
                {
                    var register = await sqlSugarClient.Queryable<Register>().FirstAsync(x => x.Id == feedBack.RegisterId);
                    feedbackStr += $"站号【{register.StationNum}】 寄存器【{register.Name}】{feedBack.Operational}【{feedBack.TagetValue}】\r\n";
                }
                TestStepDtos.Add(new TestStepDto
                {
                    Conditions = item.Conditions,
                    Inits = item.Inits,
                    ModuleId = item.ModuleId,
                    Remark = item.Remark,
                    Result = item.Result,
                    TestContent = item.TestContent,
                    TestProcess = initStr,
                    JudgmentContent = feedbackStr,
                    Id = item.Id
                });
            }
        }
        private DelegateCommand<string> conditionsLostFocus;
        public DelegateCommand<string> ConditionsLostFocus =>
            conditionsLostFocus ?? (conditionsLostFocus = new DelegateCommand<string>(ExecuteConditionsLostFocus));

        async void ExecuteConditionsLostFocus(string parameter)
        {
            var entity = await sqlSugarClient.Queryable<TestStep>().InSingleAsync(CurrentItem.Id);
            entity.Conditions = parameter;
            await sqlSugarClient.Updateable(entity).ExecuteCommandAsync();
        }
        private DelegateCommand<string> testContentLostFocus;
        public DelegateCommand<string> TestContentLostFocus =>
            testContentLostFocus ?? (testContentLostFocus = new DelegateCommand<string>(ExecuteTestContentLostFocus));

        async void ExecuteTestContentLostFocus(string parameter)
        {
            var entity = await sqlSugarClient.Queryable<TestStep>().InSingleAsync(CurrentItem.Id);
            entity.TestContent = parameter;
            await sqlSugarClient.Updateable(entity).ExecuteCommandAsync();
        }
        private DelegateCommand<string> remarkLostFocus;
        public DelegateCommand<string> RemarkLostFocus =>
            remarkLostFocus ?? (remarkLostFocus = new DelegateCommand<string>(ExecuteRemarkLostFocus));

        async void ExecuteRemarkLostFocus(string parameter)
        {
            var entity = await sqlSugarClient.Queryable<TestStep>().InSingleAsync(CurrentItem.Id);
            entity.Remark = parameter;
            await sqlSugarClient.Updateable(entity).ExecuteCommandAsync();
        }
        
    }
}
