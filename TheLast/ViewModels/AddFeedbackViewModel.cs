using AutoMapper;
using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using SqlSugar;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using TheLast.Common;
using TheLast.Dtos;
using TheLast.Entities;

namespace TheLast.ViewModels
{
    public class AddFeedbackViewModel : BindableBase, IDialogHostAware
    {
        private int offsetValue;
        public int OffsetValue
        {
            get { return offsetValue; }
            set { SetProperty(ref offsetValue, value); }
        }
        private List<int> indoorNums;
        public List<int> IndoorNums
        {
            get { return indoorNums; }
            set { SetProperty(ref indoorNums, value); }
        }
        private List<int> outdoorNums;
        public List<int> OutdoorNums
        {
            get { return outdoorNums; }
            set { SetProperty(ref outdoorNums, value); }
        }
        private int indoorNum;
        public int IndoorNum
        {
            get { return indoorNum; }
            set { SetProperty(ref indoorNum, value); }
        }
        private int outdoorNum;
        public int OutdoorNum
        {
            get { return outdoorNum; }
            set { SetProperty(ref outdoorNum, value); }
        }
        private Visibility isIndoor;
        public Visibility IsIndoor
        {
            get { return isIndoor; }
            set { SetProperty(ref isIndoor, value); }
        }
        private Visibility isOutdoor;
        public Visibility IsOutdoor
        {
            get { return isOutdoor; }
            set { SetProperty(ref isOutdoor, value); }
        }
        /// <summary>
        /// 站号列表
        /// </summary>
        private List<byte> stationNumList;
        public List<byte> StationNumList
        {
            get { return stationNumList; }
            set { SetProperty(ref stationNumList, value); }
        }

        /// <summary>
        /// 站号
        /// </summary>
        private byte stationNum;
        public byte StationNum
        {
            get { return stationNum; }
            set { SetProperty(ref stationNum, value); }
        }

        /// <summary>
        /// 当前延时模型
        /// </summary>
        private int currentDelayMode;
        public int CurrentDelayMode
        {
            get { return currentDelayMode; }
            set { SetProperty(ref currentDelayMode, value); }
        }

        /// <summary>
        /// 延时时间
        /// </summary>
        private int delatTime;
        public int DelatTime
        {
            get { return delatTime; }
            set { SetProperty(ref delatTime, value); }
        }
        
        /// <summary>
        /// 
        /// </summary>
        private byte currentStationNum;
        public byte CurrentStationNum
        {
            get { return currentStationNum; }
            set { SetProperty(ref currentStationNum, value); }
        }
        private List<DelayModelDto> delayModeList;
        public List<DelayModelDto> DelayModeList
        {
            get { return delayModeList; }
            set { SetProperty(ref delayModeList, value); }
        }
        public string DialogHostName { get; set; }
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }
        private List<ValueDictionaryDto> valueDictionaryDtos;
        public List<ValueDictionaryDto> ValueDictionaryDtos
        {
            get { return valueDictionaryDtos; }
            set { SetProperty(ref valueDictionaryDtos, value); }
        }
        private List<string> registerTypeDtos;
        private readonly IMapper mapper;
        private readonly ISqlSugarClient sqlSugarClient;
        private List<RegisterDto> registerDtos;
        public List<RegisterDto> RegisterDtos
        {
            get { return registerDtos; }
            set { SetProperty(ref registerDtos, value); }
        }
        private TestStepDto currentTestStepDto;
        public TestStepDto CurrentTestStepDto
        {
            get { return currentTestStepDto; }
            set { SetProperty(ref currentTestStepDto, value); }
        }
        public List<string> RegisterTypeDtos
        {
            get { return registerTypeDtos; }
            set { SetProperty(ref registerTypeDtos, value); }
        }
        private string currentRegisterType;
        public string CurrentRegisterType
        {
            get { return currentRegisterType; }
            set { SetProperty(ref currentRegisterType, value); }
        }
        private string inputValue;
        public string InputValue
        {
            get { return inputValue; }
            set { SetProperty(ref inputValue, value); }
        }
        private ObservableCollection<FeedBackDto> addFeedbackDtoList;
        public ObservableCollection<FeedBackDto> AddFeedbackDtoList
        {
            get { return addFeedbackDtoList; }
            set { SetProperty(ref addFeedbackDtoList, value); }
        }
        private string[] operationals=new string[] { "等于","大于","小于"};
        public string[] Operationals
        {
            get { return operationals; }
            set { SetProperty(ref operationals, value); }
        }
        private string currentOperational;
        public string CurrentOperational
        {
            get { return currentOperational; }
            set { SetProperty(ref currentOperational, value); }
        }
        private bool isEditable = false;
        public bool IsEditable
        {
            get { return isEditable; }
            set { SetProperty(ref isEditable, value); }
        }
        private RegisterDto currentRegister;
        public RegisterDto CurrentRegister
        {
            get { return currentRegister; }
            set { SetProperty(ref currentRegister, value); }
        }
        private ValueDictionaryDto currentValueDictionary;
        public ValueDictionaryDto CurrentValueDictionary
        {
            get { return currentValueDictionary; }
            set { SetProperty(ref currentValueDictionary, value); }
        }
        public AddFeedbackViewModel(IMapper mapper, ISqlSugarClient sqlSugarClient)
        {
            IndoorNums = new List<int>();
            OutdoorNums = new List<int>();
            StationNumList = new List<byte>();
            DelayModeList = new List<DelayModelDto>();
            RegisterTypeDtos = new List<string>();
            ValueDictionaryDtos = new List<ValueDictionaryDto>();
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new DelegateCommand(Cancel);
            AddFeedbackDtoList = new ObservableCollection<FeedBackDto>();
            RegisterDtos = new List<RegisterDto>();
            this.mapper = mapper;
            this.sqlSugarClient = sqlSugarClient;
        }

        private void Cancel()
        {
            if (DialogHost.IsDialogOpen(DialogHostName))
                DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.No)); //取消返回NO告诉操作结束
        }
        private DelegateCommand<object> stationSelectedCommand;
        public DelegateCommand<object> StationSelectedCommand =>
            stationSelectedCommand ?? (stationSelectedCommand = new DelegateCommand<object>(ExecuteStationSelectedCommand));

        async void ExecuteStationSelectedCommand(object parameter)
        {
            CurrentStationNum = (byte)parameter;
            List<string> registerTypes = await sqlSugarClient.Queryable<Register>().Where(it => it.StationNum == CurrentStationNum).Select(it => it.RegisterType).ToListAsync();
            RegisterTypeDtos = registerTypes.Distinct().ToList();
        }
        private async void Save()
        {
            foreach (var item in AddFeedbackDtoList)
            {
                var entity = await sqlSugarClient.Queryable<FeedBack>().FirstAsync(x => x.Id== item.Id);
                if (entity == null)
                {
                    await sqlSugarClient.Insertable(new FeedBack
                    {
                        Operational=CurrentOperational,
                        RegisterId = item.RegisterId,
                        TestStepId = item.TestStepId,
                        TagetValue = item.TagetValue,
                        DisplayTagetValue = item.DisplayTagetValue,
                        Offset=item.Offset,
                        DelayModeId = item.DelayModeId,
                        DelayTime = item.DelayTime,
                        IsJump=item.IsJump
                    }).ExecuteCommandAsync();
                    var testStep = await sqlSugarClient.Queryable<TestStep>().FirstAsync(x => x.Id == item.TestStepId);
                    var register = (await sqlSugarClient.Queryable<Register>().FirstAsync(x => x.Id == item.RegisterId));
                    testStep.JudgmentContent = string.Empty;
                    testStep.JudgmentContent += $"站号【{register.StationNum}】 寄存器【{register.Name}】{item.Operational}【{item.DisplayTagetValue}】\r\n";
                    await sqlSugarClient.Updateable(testStep).ExecuteCommandAsync();
                }
                else
                {
                    await sqlSugarClient.Updateable(new FeedBack
                    {
                        Offset = item.Offset,
                        Operational = CurrentOperational,
                        Id = item.Id,
                        RegisterId = item.RegisterId,
                        TestStepId = item.TestStepId,
                        TagetValue = item.TagetValue,
                        DelayModeId = item.DelayModeId,
                        DelayTime = item.DelayTime,
                        IsJump=item.IsJump,
                        DisplayTagetValue=item.DisplayTagetValue
                        
                    }).ExecuteCommandAsync();
                    var testStep = await sqlSugarClient.Queryable<TestStep>().FirstAsync(x => x.Id == item.TestStepId);
                    var register = (await sqlSugarClient.Queryable<Register>().FirstAsync(x => x.Id == item.RegisterId));
                    testStep.JudgmentContent = string.Empty;
                    testStep.JudgmentContent += $"站号【{register.StationNum}】 寄存器【{register.Name}】{item.Operational}【{item.DisplayTagetValue}】\r\n";
                    await sqlSugarClient.Updateable(testStep).ExecuteCommandAsync();
                }
            }

            if (DialogHost.IsDialogOpen(DialogHostName))
            {
                //确定时,把编辑的实体返回并且返回OK;
                DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.OK));
            }
        }

        public async void OnDialogOpend(IDialogParameters parameters)
        {
            for (int i = 1; i < App.IndoorCount + 1; i++)
            {
                IndoorNums.Add(i);
            }
            for (int i = 1; i < App.OutdoorCount + 1; i++)
            {
                OutdoorNums.Add(i);
            }
            IsIndoor = Visibility.Collapsed;
            IsOutdoor = Visibility.Collapsed;
            StationNumList = (await sqlSugarClient.Queryable<Register>().Select(it => it.StationNum).ToListAsync()).Distinct().ToList();
            CurrentTestStepDto = parameters.GetValue<TestStepDto>("Value");
            var delayList = await sqlSugarClient.Queryable<DelayModel>().ToListAsync();
            DelayModeList = mapper.Map<List<DelayModelDto>>(delayList);
            var list = await sqlSugarClient.Queryable<FeedBack>().Where(x => x.TestStepId ==CurrentTestStepDto.Id).ToListAsync();
            AddFeedbackDtoList.Clear();
            foreach (var item in list)
            {
                FeedBackDto feedBackDto = new FeedBackDto();
                feedBackDto.StationNum = sqlSugarClient.Queryable<Register>().First(x => x.Id == item.RegisterId).StationNum;
                feedBackDto.Address = sqlSugarClient.Queryable<Register>().First(x => x.Id == item.RegisterId).Name;
                feedBackDto.Id = item.Id;
                feedBackDto.Offset=item.Offset;
                feedBackDto.Operational = item.Operational;
                feedBackDto.RegisterId = item.RegisterId;
                feedBackDto.TestStepId = item.TestStepId;
                feedBackDto.TagetValue = item.TagetValue;
                feedBackDto.DisplayTagetValue = item.DisplayTagetValue;
                feedBackDto.DelayModeId = item.DelayModeId;
                feedBackDto.DelayTime = item.DelayTime;
                feedBackDto.IsJump = item.IsJump;
                var s= sqlSugarClient.Queryable<DelayModel>().First(x => x.Id == item.DelayModeId);
                feedBackDto.DisplayDelayMode = s.Mode;
                AddFeedbackDtoList.Add(feedBackDto);
            }
        }
        private DelegateCommand<string> selectedCommand;
        public DelegateCommand<string> SelectedCommand =>
            selectedCommand ?? (selectedCommand = new DelegateCommand<string>(ExecuteSelectedCommand));

        async void ExecuteSelectedCommand(string parameter)
        {
            List<Register> registers = new List<Register>();
            if (parameter == "内机数据")
            {
                IsIndoor = Visibility.Visible;
                IsOutdoor = Visibility.Collapsed;
            }
            else if (parameter == "外机数据")
            {
                IsIndoor = Visibility.Collapsed;
                IsOutdoor = Visibility.Visible;
            }
            else
            {
                IsIndoor = Visibility.Collapsed;
                IsOutdoor = Visibility.Collapsed;
                registers = await sqlSugarClient.Queryable<Register>().Where(x => x.IsEnable == true && x.RegisterType == parameter && x.StationNum == CurrentStationNum).ToListAsync();
                RegisterDtos = mapper.Map<List<RegisterDto>>(registers);
            }

            
        }
        private DelegateCommand<int?> selectedIndoorCommand;
        public DelegateCommand<int?> SelectedIndoorCommand =>
            selectedIndoorCommand ?? (selectedIndoorCommand = new DelegateCommand<int?>(ExecuteSelectedIndoorCommand));

        async void ExecuteSelectedIndoorCommand(int? parameter)
        {
            List<Register> registers = new List<Register>();
            registers = await sqlSugarClient.Queryable<Register>().Where(x => x.IsEnable == true && x.RegisterType == "内机数据" && x.StationNum == CurrentStationNum && x.Name.Contains($"-{parameter}")).ToListAsync();
            for (int i = 0; i < registers.Count; i++)
            {
                var d = int.Parse(registers[i].Name.Substring(registers[i].Name.IndexOf("-") + 1));
                if (d != parameter)
                {
                    registers.Remove(registers[i]);
                }
            }
            RegisterDtos = mapper.Map<List<RegisterDto>>(registers);
        }
        private DelegateCommand<int?> selectedOutdoorCommand;
        public DelegateCommand<int?> SelectedOutdoorCommand =>
            selectedOutdoorCommand ?? (selectedOutdoorCommand = new DelegateCommand<int?>(ExecuteSelectedOutdoorCommand));

        async void ExecuteSelectedOutdoorCommand(int? parameter)
        {
            List<Register> registers = new List<Register>();
            registers = await sqlSugarClient.Queryable<Register>().Where(x => x.IsEnable == true && x.RegisterType == "外机数据" && x.StationNum == CurrentStationNum && x.Name.Contains($"-{parameter}")).ToListAsync();
            for (int i = 0; i < registers.Count; i++)
            {
                var d = int.Parse(registers[i].Name.Substring(registers[i].Name.IndexOf("-") + 1));
                if (d != parameter)
                {
                    registers.Remove(registers[i]);
                }
            }
            RegisterDtos = mapper.Map<List<RegisterDto>>(registers);
        }
        private DelegateCommand<RegisterDto> selectedRegisterCommand;
        public DelegateCommand<RegisterDto> SelectedRegisterCommand =>
            selectedRegisterCommand ?? (selectedRegisterCommand = new DelegateCommand<RegisterDto>(ExecuteSelectedRegisterCommand));

        async void ExecuteSelectedRegisterCommand(RegisterDto parameter)
        {
            if (parameter==null)
            {
                return;
            }
            ValueDictionaryDtos.Clear();
            var result = await sqlSugarClient.Queryable<ValueDictionary>().Where(x => x.RegisterId == parameter.Id).ToListAsync();
            if (result.Count == 0 && !parameter.RegisterType.Contains("数字量"))
            {
                IsEditable = true;
            }
            else if (result[0].DisplayValue == "空"&& !parameter.RegisterType.Contains("数字量"))
            {
                IsEditable = true;
            }
            else
            {
                if (result.Count == 0 && parameter.RegisterType.Contains("数字量"))
                {
                    ValueDictionaryDtos.Clear(); IsEditable = false;
                    ValueDictionaryDtos.Add(new ValueDictionaryDto
                    {
                        DisplayValue = "断开",
                        RealValue = "0",
                    });
                    ValueDictionaryDtos.Add(new ValueDictionaryDto
                    {
                        DisplayValue = "闭合",
                        RealValue = "1",
                    });
                }
                else
                {
                    IsEditable = false;
                    ValueDictionaryDtos = mapper.Map<List<ValueDictionaryDto>>(result);
                }
               
            }
        }
        private DelegateCommand addFeedback;
        public DelegateCommand AddFeedback =>
            addFeedback ?? (addFeedback = new DelegateCommand(ExecuteAddFeedback));

        async void ExecuteAddFeedback()
        {
            if (IsEditable)
            {
                FeedBackDto feedBackDto = new FeedBackDto();
                feedBackDto.StationNum =(await sqlSugarClient.Queryable<Register>().FirstAsync(x => x.Id == CurrentRegister.Id)).StationNum;
                feedBackDto.Address = (await sqlSugarClient.Queryable<Register>().FirstAsync(x => x.Id == CurrentRegister.Id)).Name;
                feedBackDto.RegisterId = CurrentRegister.Id;
                feedBackDto.DisplayDelayMode = (await sqlSugarClient.Queryable<DelayModel>().FirstAsync(x => x.Id == CurrentDelayMode + 1)).Mode;
                feedBackDto.DelayTime = DelatTime;
                feedBackDto.Operational = CurrentOperational;
                feedBackDto.TestStepId = CurrentTestStepDto.Id;
                feedBackDto.TagetValue = inputValue;
                feedBackDto.DelayModeId = CurrentDelayMode + 1;
                feedBackDto.DisplayTagetValue = inputValue;
                feedBackDto.Offset = OffsetValue;
                AddFeedbackDtoList.Add(feedBackDto);
            }
            else
            {
                FeedBackDto feedBackDto = new FeedBackDto();
                feedBackDto.StationNum = (await sqlSugarClient.Queryable<Register>().FirstAsync(x => x.Id == CurrentRegister.Id)).StationNum;
                feedBackDto.Address = (await sqlSugarClient.Queryable<Register>().FirstAsync(x => x.Id == CurrentRegister.Id)).Name;
                feedBackDto.RegisterId = CurrentRegister.Id;
                feedBackDto.Operational = CurrentOperational;
                feedBackDto.DisplayDelayMode = (await sqlSugarClient.Queryable<DelayModel>().FirstAsync(x => x.Id == CurrentDelayMode + 1)).Mode;
                feedBackDto.DelayTime = DelatTime;
                feedBackDto.TestStepId = CurrentTestStepDto.Id;
                feedBackDto.TagetValue = CurrentValueDictionary.RealValue;
                feedBackDto.DelayModeId = CurrentDelayMode + 1;
                feedBackDto.DisplayTagetValue = inputValue;
                feedBackDto.Offset = OffsetValue;
                AddFeedbackDtoList.Add(feedBackDto);
            }  
          
               
        }

        private DelegateCommand<FeedBackDto> deleteFeedbackCommand;
        public DelegateCommand<FeedBackDto> DeleteFeedbackCommand =>
            deleteFeedbackCommand ?? (deleteFeedbackCommand = new DelegateCommand<FeedBackDto>(ExecuteDeleteFeedbackCommand));

        async void ExecuteDeleteFeedbackCommand(FeedBackDto parameter)
        {
            await sqlSugarClient.Deleteable(mapper.Map<FeedBack>(parameter)).ExecuteCommandAsync();
            
            AddFeedbackDtoList.Remove(parameter);
        }
    }
}