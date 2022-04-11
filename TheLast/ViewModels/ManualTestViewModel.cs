using AutoMapper;
using Modbus.Device;
using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using TheLast.Common;
using TheLast.Dtos;
using TheLast.Entities;
using TheLast.Extensions;

namespace TheLast.ViewModels
{
    public class ManualTestViewModel: NavigationViewModel
    {
        private ObservableCollection<WriteRegister> writeRegisters;
        private readonly ISqlSugarClient sqlSugarClient;
        private readonly IMapper mapper;
        private ModbusSerialMaster modbusSerialMaster;
        private ObservableCollection<Register> registers;
        public ObservableCollection<Register> Registers
        {
            get { return registers; }
            set { SetProperty(ref registers, value); }
        }
        private List<string> registerTypeDtos;
        public List<string> RegisterTypeDtos
        {
            get { return registerTypeDtos; }
            set { SetProperty(ref registerTypeDtos, value); }
        }
        private int typeIndex;
        public int TypeIndex
        {
            get { return typeIndex; }
            set { SetProperty(ref typeIndex, value); }
        }
        private int stationIndex;
        public int StationIndex
        {
            get { return stationIndex; }
            set { SetProperty(ref stationIndex, value); }
        }
        private List<byte> stationNumList;
        public List<byte> StationNumList
        {
            get { return stationNumList; }
            set { SetProperty(ref stationNumList, value); }
        }
        public ObservableCollection<WriteRegister> WriteRegisters
        {
            get { return writeRegisters; }
            set { SetProperty(ref writeRegisters, value); }
        }
        public ManualTestViewModel(IContainerProvider containerProvider,ISqlSugarClient sqlSugarClient,IMapper mapper) : base(containerProvider)
        {
            RegisterTypeDtos = new List<string>();
            Registers = new ObservableCollection<Register>();
            WriteRegisters = new ObservableCollection<WriteRegister>();
            this.sqlSugarClient = sqlSugarClient;
            this.mapper = mapper;
        }
        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
            StationIndex = -1;
            TypeIndex = -1;
            modbusSerialMaster = App.ModbusSerialMaster;
            Registers.Clear();
            WriteRegisters.Clear();
            Registers = (ObservableCollection<Register>)Registers.AddRange(await sqlSugarClient.Queryable<Register>().Where(x => x.IsEnable == true).ToListAsync());
            StationNumList = Registers.Select(x => x.StationNum).Distinct().ToList();
            
        }
        private DelegateCommand<byte?> stationSelectedCommand;
        public DelegateCommand<byte?> StationSelectedCommand =>
            stationSelectedCommand ?? (stationSelectedCommand = new DelegateCommand<byte?>(ExecuteStationSelectedCommand));

        void ExecuteStationSelectedCommand(byte? parameter)
        {
            WriteRegisters.Clear();
            if (parameter!=null)
            {
                RegisterTypeDtos = Registers.Where(x => x.StationNum == parameter).Select(x => x.RegisterType).Distinct().ToList();
            }
        }
        private DelegateCommand<WriteRegister> lostFocusCommand;
        public DelegateCommand<WriteRegister> LostFocusCommand =>
            lostFocusCommand ?? (lostFocusCommand = new DelegateCommand<WriteRegister>(ExecuteLostFocusCommand));

        async void ExecuteLostFocusCommand(WriteRegister parameter)
        {
            if (modbusSerialMaster == null)
            {
                HandyControl.Controls.Growl.Info("写入失败：未设置串口");
                return;
            }
            await modbusSerialMaster.WriteSingleRegisterAsync(parameter.RegisterDto.StationNum, parameter.RegisterDto.Address, parameter.Value);
        }
        private DelegateCommand<string?> selectedCommand;
        public DelegateCommand<string?> SelectedCommand =>
            selectedCommand ?? (selectedCommand = new DelegateCommand<string?>(ExecuteSelectedCommand));

        void ExecuteSelectedCommand(string? parameter)
        {
            if (!string.IsNullOrEmpty(parameter))
            {
                WriteRegisters.Clear();
                var s = Registers.Where(x => x.RegisterType == parameter).ToList();
                foreach (var item in s)
                {
                    WriteRegisters.Add(new WriteRegister { RegisterDto = mapper.Map<RegisterDto>(item) });
                }
            }
           
        }
        private DelegateCommand writeBatch;
        public DelegateCommand WriteBatch =>
            writeBatch ?? (writeBatch = new DelegateCommand(ExecuteWriteBatch));

        async void ExecuteWriteBatch()
        {
            if (modbusSerialMaster == null)
            {
                HandyControl.Controls.Growl.Info("写入失败：未设置串口");
                return;
            }
            foreach (var item in WriteRegisters)
            {
                await modbusSerialMaster.WriteSingleRegisterAsync(item.RegisterDto.StationNum, item.RegisterDto.Address, item.Value);

            }
        }
    }
}
