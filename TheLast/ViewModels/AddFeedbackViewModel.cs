using AutoMapper;
using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using SqlSugar;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using TheLast.Common;
using TheLast.Dtos;
using TheLast.Entities;

namespace TheLast.ViewModels
{
    public class AddFeedbackViewModel : BindableBase, IDialogHostAware
    {
        private int currentDelayMode;
        public int CurrentDelayMode
        {
            get { return currentDelayMode; }
            set { SetProperty(ref currentDelayMode, value); }
        }
        private int delatTime;
        public int DelatTime
        {
            get { return delatTime; }
            set { SetProperty(ref delatTime, value); }
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

        private async void Save()
        {
            foreach (var item in AddFeedbackDtoList)
            {
                var entity = await sqlSugarClient.Queryable<FeedBack>().FirstAsync(x => x.Id== item.Id);
                if (entity == null)
                {
                    await sqlSugarClient.Insertable(new FeedBack
                    {
                        RegisterId = item.RegisterId,
                        TestStepId = item.TestStepId,
                        TagetValue = item.TagetValue,
                        DisplayTagetValue = item.DisplayTagetValue,
                        DelayModeId = item.DelayModeId,
                        DelayTime = item.DelayTime,
                    }).ExecuteCommandAsync();
                    var testStep = await sqlSugarClient.Queryable<TestStep>().FirstAsync(x => x.Id == item.TestStepId);
                    var registerName = (await sqlSugarClient.Queryable<Register>().FirstAsync(x => x.Id == item.RegisterId)).Name;
                    testStep.JudgmentContent += $"寄存器【{registerName}】=【{item.DisplayTagetValue}】\r\n";
                    await sqlSugarClient.Updateable(testStep).ExecuteCommandAsync();
                }
                else
                {
                    await sqlSugarClient.Updateable(new FeedBack
                    {
                        Id = item.Id,
                        RegisterId = item.RegisterId,
                        TestStepId = item.TestStepId,
                        TagetValue = item.TagetValue,
                        DelayModeId = item.DelayModeId,
                        DelayTime = item.DelayTime,
                        DisplayTagetValue=item.DisplayTagetValue
                    }).ExecuteCommandAsync();
                    var testStep = await sqlSugarClient.Queryable<TestStep>().FirstAsync(x => x.Id == item.TestStepId);
                    var registerName = (await sqlSugarClient.Queryable<Register>().FirstAsync(x => x.Id == item.RegisterId)).Name;
                    testStep.JudgmentContent += $"寄存器【{registerName}】=【{item.DisplayTagetValue}】\r\n";
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
            CurrentTestStepDto = parameters.GetValue<TestStepDto>("Value");
            var delayList = await sqlSugarClient.Queryable<DelayModel>().ToListAsync();
            DelayModeList = mapper.Map<List<DelayModelDto>>(delayList);
            var list = await sqlSugarClient.Queryable<FeedBack>().Where(x => x.TestStepId ==CurrentTestStepDto.Id).ToListAsync();
            AddFeedbackDtoList.Clear();
            foreach (var item in list)
            {
                FeedBackDto feedBackDto = new FeedBackDto();
                feedBackDto.Address = sqlSugarClient.Queryable<Register>().First(x => x.Id == item.RegisterId).Name;
                feedBackDto.Id = item.Id;
                feedBackDto.RegisterId = item.RegisterId;
                feedBackDto.TestStepId = item.TestStepId;
                feedBackDto.TagetValue = item.TagetValue;
                feedBackDto.DisplayTagetValue = item.DisplayTagetValue;
                feedBackDto.DelayModeId = item.DelayModeId;
                feedBackDto.DelayTime = item.DelayTime;
                var s= sqlSugarClient.Queryable<DelayModel>().First(x => x.Id == item.DelayModeId);
                feedBackDto.DisplayDelayMode = s.Mode;
                AddFeedbackDtoList.Add(feedBackDto);
            }
            List<string> registerTypes = await sqlSugarClient.Queryable<Register>().Select(it => it.RegisterType).ToListAsync();
            RegisterTypeDtos = registerTypes.Distinct().ToList();
        }
        private DelegateCommand<string> selectedCommand;
        public DelegateCommand<string> SelectedCommand =>
            selectedCommand ?? (selectedCommand = new DelegateCommand<string>(ExecuteSelectedCommand));

        async void ExecuteSelectedCommand(string parameter)
        {
            var resulet = await sqlSugarClient.Queryable<Register>().Where(x => x.IsEnable == true && x.RegisterType == parameter).ToListAsync();

            RegisterDtos = mapper.Map<List<RegisterDto>>(resulet);
        }
        private DelegateCommand<RegisterDto> selectedRegisterCommand;
        public DelegateCommand<RegisterDto> SelectedRegisterCommand =>
            selectedRegisterCommand ?? (selectedRegisterCommand = new DelegateCommand<RegisterDto>(ExecuteSelectedRegisterCommand));

        async void ExecuteSelectedRegisterCommand(RegisterDto parameter)
        {

            var result = await sqlSugarClient.Queryable<ValueDictionary>().Where(x => x.RegisterId == parameter.Id).ToListAsync();
            if (result.Count == 0)
            {
                IsEditable = true;
            }
            else
            {
                IsEditable = false;
                ValueDictionaryDtos = mapper.Map<List<ValueDictionaryDto>>(result);
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
                feedBackDto.Address = (await sqlSugarClient.Queryable<Register>().FirstAsync(x => x.Id == CurrentRegister.Id)).Name;
                feedBackDto.RegisterId = CurrentRegister.Id;
                feedBackDto.DisplayDelayMode = (await sqlSugarClient.Queryable<DelayModel>().FirstAsync(x => x.Id == CurrentDelayMode + 1)).Mode;
                feedBackDto.DelayTime = DelatTime;
                feedBackDto.TestStepId = CurrentTestStepDto.Id;
                feedBackDto.TagetValue = inputValue;
                feedBackDto.DelayModeId = CurrentDelayMode + 1;
                feedBackDto.DisplayTagetValue = inputValue;
                AddFeedbackDtoList.Add(feedBackDto);
            }
            else
            {
                FeedBackDto feedBackDto = new FeedBackDto();
                feedBackDto.Address = (await sqlSugarClient.Queryable<Register>().FirstAsync(x => x.Id == CurrentRegister.Id)).Name;
                feedBackDto.RegisterId = CurrentRegister.Id;
                feedBackDto.DisplayDelayMode = (await sqlSugarClient.Queryable<DelayModel>().FirstAsync(x => x.Id == CurrentDelayMode + 1)).Mode;
                feedBackDto.DelayTime = DelatTime;
                feedBackDto.TestStepId = CurrentTestStepDto.Id;
                feedBackDto.TagetValue = CurrentValueDictionary.RealValue;
                feedBackDto.DelayModeId = CurrentDelayMode + 1;
                feedBackDto.DisplayTagetValue = inputValue;
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