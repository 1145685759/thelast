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
using System.Windows;
using TheLast.Common;
using TheLast.Dtos;
using TheLast.Entities;

namespace TheLast.ViewModels
{
    public class AddInitViewModel : BindableBase, IDialogHostAware
    {
        public string DialogHostName { get; set; }
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }
        private List<ValueDictionaryDto> valueDictionaryDtos;
        public List<ValueDictionaryDto> ValueDictionaryDtos
        {
            get { return valueDictionaryDtos; }
            set { SetProperty(ref valueDictionaryDtos, value); }
        }
        private byte currentStationNum;
        public byte CurrentStationNum
        {
            get { return currentStationNum; }
            set { SetProperty(ref currentStationNum, value); }
        }
        private List<byte> stationNumList;
        public List<byte> StationNumList
        {
            get { return stationNumList; }
            set { SetProperty(ref stationNumList, value); }
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
        private List<Register> allRegisters;
        public List<Register> AllRegisters
        {
            get { return allRegisters; }
            set { SetProperty(ref allRegisters, value); }
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
        private Visibility isTemperature;
        public Visibility IsTemperature
        {
            get { return isTemperature; }
            set { SetProperty(ref isTemperature, value); }
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
        private int currentId;
        public int CurrentId
        {
            get { return currentId; }
            set { SetProperty(ref currentId, value); }
        }
      
      
        
        private ObservableCollection<InitDto> addInitDtoList;
        public ObservableCollection<InitDto> AddInitDtoList
        {
            get { return addInitDtoList; }
            set { SetProperty(ref addInitDtoList, value); }
        }
        private int currentSensorType;
        public int CurrentSensorType
        {
            get { return currentSensorType; }
            set { SetProperty(ref currentSensorType, value); }
        }
        private int circuitPowerlevel;
        public int CircuitPowerlevel
        {
            get { return circuitPowerlevel; }
            set { SetProperty(ref circuitPowerlevel, value); }
        }
        private byte stationNum;
        public byte StationNum
        {
            get { return stationNum; }
            set { SetProperty(ref stationNum, value); }
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
        private string[] sensorTypes=new string[2] { "15K传感器", "20K传感器" };
        public string[] SensorTypes
        {
            get { return sensorTypes; }
            set { SetProperty(ref sensorTypes, value); }
        }
        private string[] circuitPowerlevels=new string[2] { "5V电源", "3.3V电源"};
        public string[] CircuitPowerlevels
        {
            get { return circuitPowerlevels; }
            set { SetProperty(ref circuitPowerlevels, value); }
        }
        private ValueDictionaryDto currentValueDictionary;
        public ValueDictionaryDto CurrentValueDictionary
        {
            get { return currentValueDictionary; }
            set { SetProperty(ref currentValueDictionary, value); }
        }
        public AddInitViewModel(IMapper mapper, ISqlSugarClient sqlSugarClient)
        {
            RegisterTypeDtos = new List<string>();
            ValueDictionaryDtos = new List<ValueDictionaryDto>();
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new DelegateCommand(Cancel);
            AllRegisters = new List<Register>();
           
            AddInitDtoList = new ObservableCollection<InitDto>();
            RegisterDtos = new List<RegisterDto>();
            this.mapper = mapper;
            StationNumList = new List<byte>();
            this.sqlSugarClient = sqlSugarClient;
        }

        private void Cancel()
        {
            if (DialogHost.IsDialogOpen(DialogHostName))
                DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.No)); //取消返回NO告诉操作结束
        }

        private async void Save()
        {
            foreach (var item in AddInitDtoList)
            {
                var entity = await sqlSugarClient.Queryable<Init>().FirstAsync(x => x.Id == item.Id);
                var testStep = await sqlSugarClient.Queryable<TestStep>().FirstAsync(x => x.Id == item.TestStepId);
                if (AddInitDtoList.IndexOf(item)==0)
                {
                    testStep.TestProcess = string.Empty;
                }
                
                if (entity == null)
                {
                        await sqlSugarClient.Insertable(new Init
                        {
                            RegisterId = item.RegisterId,
                            TestStepId = item.TestStepId,
                            WriteValue = item.WriteValue,
                            DisplayValue=item.DisplayValue,
                            
                        }).ExecuteCommandAsync();
                    var register = (await sqlSugarClient.Queryable<Register>().FirstAsync(x => x.Id == item.RegisterId));
                    testStep.TestProcess += $"向 站号【{register.StationNum}】寄存器【{register.Name}】写入值【{item.DisplayValue}】\r\n";
                    await sqlSugarClient.Updateable(testStep).ExecuteCommandAsync();
                }
                else
                {
                        await sqlSugarClient.Updateable(new Init
                        {
                            Id=item.Id,
                            RegisterId = item.RegisterId,
                            TestStepId = item.TestStepId,
                            WriteValue = item.WriteValue,
                            DisplayValue=item.DisplayValue
                            
                        }).ExecuteCommandAsync();
                    
                    var register = (await sqlSugarClient.Queryable<Register>().FirstAsync(x => x.Id == item.RegisterId));
                    testStep.TestProcess += $"向 站号【{register.StationNum}】寄存器【{register.Name}】写入值【{item.DisplayValue}】\r\n";
                    await sqlSugarClient.Updateable(testStep).ExecuteCommandAsync();

                }
            }

            if (DialogHost.IsDialogOpen(DialogHostName))
            {
                //确定时,把编辑的实体返回并且返回OK;
                DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.OK));
            }
        }
        private DelegateCommand<object> stationSelectedCommand;
        public DelegateCommand<object> StationSelectedCommand =>
            stationSelectedCommand ?? (stationSelectedCommand = new DelegateCommand<object>(ExecuteStationSelectedCommand));

        async void ExecuteStationSelectedCommand(object parameter)
        {
            CurrentStationNum = (byte)parameter;
            List<string> registerTypes = await sqlSugarClient.Queryable<Register>().Where(it=>it.StationNum==CurrentStationNum).Select(it => it.RegisterType).ToListAsync();
            RegisterTypeDtos = registerTypes.Distinct().ToList();
            RegisterTypeDtos.Remove("步进电机脉冲检测");
            RegisterTypeDtos.Remove("外机数据");
            RegisterTypeDtos.Remove("内机数据");
            RegisterTypeDtos.Remove("数字量输入");
            RegisterTypeDtos.Remove("模拟量输入");
        }
        public async void OnDialogOpend(IDialogParameters parameters)
        {
            
            CurrentTestStepDto = parameters.GetValue<TestStepDto>("Value");
            AllRegisters = await sqlSugarClient.Queryable<Register>().Where(x => x.Name.Contains("温度")&&x.RegisterType=="内机控制参数"&&x.IsEnable==true).ToListAsync();
            StationNumList = (await sqlSugarClient.Queryable<Register>().Select(it => it.StationNum).ToListAsync()).Distinct().ToList();
            var list = await sqlSugarClient.Queryable<Init>().Where(x => x.TestStepId ==CurrentTestStepDto.Id).ToListAsync();
            IsTemperature = Visibility.Collapsed;
           
            AddInitDtoList.Clear();
            foreach (var item in list)
            {
                    AddInitDtoList.Add(new InitDto
                    {
                        StationNum= sqlSugarClient.Queryable<Register>().First(x => x.Id == item.RegisterId).StationNum,
                        Address = sqlSugarClient.Queryable<Register>().First(x => x.Id == item.RegisterId).Name,
                        Id = item.Id,
                        RegisterId = item.RegisterId,
                        TestStepId = item.TestStepId,
                        WriteValue = item.WriteValue,
                        DisplayValue=item.DisplayValue,
                    });
            }
            //List<string> registerTypes = await sqlSugarClient.Queryable<Register>().Where(it=>it.StationNum==CurrentStationNum).Select(it => it.RegisterType).ToListAsync();
            //RegisterTypeDtos = registerTypes.Distinct().ToList();
        }
        private DelegateCommand<string> selectedCommand;
        public DelegateCommand<string> SelectedCommand =>
            selectedCommand ?? (selectedCommand = new DelegateCommand<string>(ExecuteSelectedCommand));

        async void ExecuteSelectedCommand(string parameter)
        {
            List<Register> registers = new List<Register>();
            if (parameter== "20个温度设置")
            {
                IsTemperature = Visibility.Visible;
            }
           
            else
            {
                
                IsTemperature = Visibility.Collapsed;
                registers = await sqlSugarClient.Queryable<Register>().Where(x => x.IsEnable == true && x.RegisterType == parameter && x.StationNum == CurrentStationNum).ToListAsync();
            }
           

            RegisterDtos = mapper.Map<List<RegisterDto>>(registers);
        }
        private DelegateCommand selectedIndoorCommand;
        public DelegateCommand SelectedIndoorCommand =>
            selectedIndoorCommand ?? (selectedIndoorCommand = new DelegateCommand(ExecuteSelectedIndoorCommand));

        private void ExecuteSelectedIndoorCommand()
        {

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
            var result = await sqlSugarClient.Queryable<ValueDictionary>().Where(x => x.RegisterId == parameter.Id).ToListAsync();
            if (result.Count == 0&& !parameter.RegisterType.Contains("数字量"))
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
        private DelegateCommand<int?> selectedSensorTypeComboxCommand;
        public DelegateCommand<int?> SelectedSensorTypeComboxCommand =>
            selectedSensorTypeComboxCommand ?? (selectedSensorTypeComboxCommand = new DelegateCommand<int?>(ExecuteSelectedSensorTypeComboxCommand));

        async void ExecuteSelectedSensorTypeComboxCommand(int? parameter)
        {
            if (parameter!=null)
            {
                var entity= await sqlSugarClient.Queryable<Register>().FirstAsync(x => x.Id == CurrentRegister.Id);
                entity.Type = parameter;
                await sqlSugarClient.Updateable(entity).ExecuteCommandAsync();
            }
        }
        private DelegateCommand<int?> selectedCurrentRegisterCommand;
        public DelegateCommand<int?> SelectedCurrentRegisterCommand =>
            selectedCurrentRegisterCommand ?? (selectedCurrentRegisterCommand = new DelegateCommand<int?>(ExecuteSelectedCurrentRegisterCommand));

        async void ExecuteSelectedCurrentRegisterCommand(int? parameter)
        {
            if (parameter!=null)
            {
                var entity = await sqlSugarClient.Queryable<Register>().FirstAsync(x => x.Id == CurrentRegister.Id);
                entity.Caste = parameter;
                await sqlSugarClient.Updateable(entity).ExecuteCommandAsync();
            }
        }
        private DelegateCommand<Register> selectedAllRegistersCommand;
        public DelegateCommand<Register> SelectedAllRegistersCommand =>
            selectedAllRegistersCommand ?? (selectedAllRegistersCommand = new DelegateCommand<Register>(ExecuteSelectedAllRegistersCommand));

        async void ExecuteSelectedAllRegistersCommand(Register parameter)
        {
            if (parameter!=null)
            {
                var entity = await sqlSugarClient.Queryable<Register>().FirstAsync(x => x.Id == CurrentRegister.Id);
                entity.AccessAddress = parameter.Address;
                await sqlSugarClient.Updateable(entity).ExecuteCommandAsync();
            }
        }   
        private DelegateCommand addInit;
        public DelegateCommand AddInit =>
            addInit ?? (addInit = new DelegateCommand(ExecuteAddInit));

        async void ExecuteAddInit()
        {
            if (IsEditable)
            {
                InitDto initDto = new InitDto
                {
                    Address = (await sqlSugarClient.Queryable<Register>().FirstAsync(x => x.Id == CurrentRegister.Id)).Name,
                    StationNum= (await sqlSugarClient.Queryable<Register>().FirstAsync(x => x.Id == CurrentRegister.Id)).StationNum,
                    RegisterId = CurrentRegister.Id,
                    TestStepId = CurrentTestStepDto.Id,
                    WriteValue = inputValue,
                    DisplayValue = inputValue
                };
                AddInitDtoList.Add(initDto);
            }
                
            else
            {
                InitDto initDto = new InitDto
                {
                    Address = (await sqlSugarClient.Queryable<Register>().FirstAsync(x => x.Id == CurrentRegister.Id)).Name,
                    StationNum = (await sqlSugarClient.Queryable<Register>().FirstAsync(x => x.Id == CurrentRegister.Id)).StationNum,
                    RegisterId = CurrentRegister.Id,
                    TestStepId = CurrentTestStepDto.Id,
                    WriteValue = CurrentValueDictionary.RealValue,
                    DisplayValue = CurrentValueDictionary.DisplayValue,
                };
                AddInitDtoList.Add(initDto);
            }

        }

        private DelegateCommand<InitDto> deleteInitCommand;
        public DelegateCommand<InitDto> DeleteInitCommand =>
            deleteInitCommand ?? (deleteInitCommand = new DelegateCommand<InitDto>(ExecuteDeleteInitCommand));

        async void ExecuteDeleteInitCommand(InitDto parameter)
        {
            await sqlSugarClient.Deleteable(mapper.Map<Init>(parameter)).ExecuteCommandAsync();
            AddInitDtoList.Remove(parameter);
        }
    }
}