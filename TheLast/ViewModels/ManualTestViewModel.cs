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
        private ObservableCollection<string> registerTypeDtos;
        public ObservableCollection<string> RegisterTypeDtos
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
            RegisterTypeDtos = new ObservableCollection<string>();
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
            if (modbusSerialMaster==null)
            {
                HandyControl.Controls.Growl.Error("串口未连接");
            }
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
            RegisterTypeDtos.Clear();
            WriteRegisters.Clear();
            if (parameter!=null)
            {
                RegisterTypeDtos.AddRange(Registers.Where(x => x.StationNum == parameter).Select(x => x.RegisterType).Distinct().ToList());
                if (RegisterTypeDtos.Contains("外机数据"))
                {
                    RegisterTypeDtos.Remove("外机数据");
                }
                if (RegisterTypeDtos.Contains("内机数据"))
                {
                    RegisterTypeDtos.Remove("内机数据");
                }
                if (RegisterTypeDtos.Contains("步进电机脉冲检测"))
                {
                    RegisterTypeDtos.Remove("步进电机脉冲检测");
                }
                if (RegisterTypeDtos.Contains("数字量输入"))
                {
                    RegisterTypeDtos.Remove("数字量输入");
                }
                if (RegisterTypeDtos.Contains("反馈脉冲数"))
                {
                    RegisterTypeDtos.Remove("反馈脉冲数");
                }
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
            if (parameter.RegisterDto.RegisterType=="数字量输出")
            {
                bool value=false;
                if (parameter.Value=="1")
                {
                    value = true;
                }
                else if (parameter.Value=="0")
                {
                    value = false;
                }
                else
                {
                    HandyControl.Controls.Growl.Error("写入失败：写入值错误");
                    return;
                }
                try
                {
                    await modbusSerialMaster.WriteSingleCoilAsync(parameter.RegisterDto.StationNum, parameter.RegisterDto.Address, value);

                }
                catch (Exception ex)
                {

                    HandyControl.Controls.Growl.Error($"写入失败：{ex.Message}");
                }

            }
            if (parameter.RegisterDto.RegisterType=="20个温度设置")
            {
                try
                {
                    ushort[] data = new ushort[5] { (ushort)(Convert.ToDouble(parameter.Value) * 10), (ushort)parameter.RegisterDto.Type, (ushort)parameter.RegisterDto.FineTuning, (ushort)parameter.RegisterDto.AccessAddress,parameter.RegisterDto.AllowableRangeDeviation };
                    for (int i = 0; i < data.Length; i++)
                    {
                        try
                        {
                            await modbusSerialMaster.WriteSingleRegisterAsync(parameter.RegisterDto.StationNum, (ushort)(parameter.RegisterDto.Address + i), data[i]);

                        }
                        catch (Exception ex)
                        {

                            HandyControl.Controls.Growl.Error($"写入失败：{ex.Message}");
                        }
                    }
                }
                catch (Exception ex)
                {

                    HandyControl.Controls.Growl.Error($"写入失败：{ex.Message}");
                }
                
            }
            else
            {
                if (parameter.RegisterDto.Name.Contains("温度")|| parameter.RegisterDto.Name.Contains("感温包"))
                {
                    try
                    {
                        await modbusSerialMaster.WriteSingleRegisterAsync(parameter.RegisterDto.StationNum, parameter.RegisterDto.Address, (ushort)(Convert.ToDouble(parameter.Value) * 10));

                    }
                    catch (Exception ex)
                    {

                        HandyControl.Controls.Growl.Error($"写入失败：{ex.Message}");
                    }
                }
                else
                {
                    try
                    {
                        await modbusSerialMaster.WriteSingleRegisterAsync(parameter.RegisterDto.StationNum, parameter.RegisterDto.Address, Convert.ToUInt16(parameter.Value));

                    }
                    catch (Exception ex)
                    {

                        HandyControl.Controls.Growl.Error($"写入失败：{ex.Message}");
                    }
                }
            }
        }
        private DelegateCommand<string?> selectedCommand;
        public DelegateCommand<string?> SelectedCommand =>
            selectedCommand ?? (selectedCommand = new DelegateCommand<string?>(ExecuteSelectedCommand));

        async void ExecuteSelectedCommand(string? parameter)
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
            if (parameter=="数字量输出")
            {
                for (int i = 0; i < WriteRegisters.Count; i++)
                {
                    WriteRegisters[i].Value = (((await modbusSerialMaster.ReadCoilsAsync(WriteRegisters[i].RegisterDto.StationNum, WriteRegisters[i].RegisterDto.Address, 1))[0]) ? 1 : 0).ToString();
                }
            }
            else
            {
                for (int i = 0; i < WriteRegisters.Count; i++)
                {
                    if (WriteRegisters[i].RegisterDto.Name.Contains("感温包") || WriteRegisters[i].RegisterDto.Name.Contains("温度"))
                    {
                        WriteRegisters[i].Value = ((await modbusSerialMaster.ReadHoldingRegistersAsync(WriteRegisters[i].RegisterDto.StationNum, WriteRegisters[i].RegisterDto.Address, 1))[0] / 10.0).ToString();
                    }
                    else
                    {
                        WriteRegisters[i].Value = (await modbusSerialMaster.ReadHoldingRegistersAsync(WriteRegisters[i].RegisterDto.StationNum, WriteRegisters[i].RegisterDto.Address, 1))[0].ToString();
                    }
                }
            }
        }
        private DelegateCommand<string> writeBatch;
        public DelegateCommand<string> WriteBatch =>
            writeBatch ?? (writeBatch = new DelegateCommand<string>(ExecuteWriteBatch));

        async void ExecuteWriteBatch(string d)
        {
            if (modbusSerialMaster == null)
            {
                HandyControl.Controls.Growl.Info("写入失败：未设置串口");
                return;
            }
            if (d == "数字量输出")
            {
                foreach (var item in WriteRegisters)
                {
                    bool value = false;
                    if (item.Value == "1")
                    {
                        value = true;
                    }
                    else if (item.Value == "0")
                    {
                        value = false;
                    }
                    else
                    {
                        HandyControl.Controls.Growl.Error("写入失败：写入值不正确");
                        return ;
                    }
                    await modbusSerialMaster.WriteSingleCoilAsync(item.RegisterDto.StationNum, item.RegisterDto.Address, value);
                }
            }
            else
            {
                foreach (var item in WriteRegisters)
                {
                    if (item.RegisterDto.Name.Contains("温度")|| item.RegisterDto.Name.Contains("感温包"))
                    {
                        try
                        {
                            await modbusSerialMaster.WriteSingleRegisterAsync(item.RegisterDto.StationNum, item.RegisterDto.Address, (ushort)(Convert.ToDouble(item.Value) * 10));

                        }
                        catch (Exception ex)
                        {

                            HandyControl.Controls.Growl.Error($"写入失败：{ex.Message}");
                        }
                    }
                    else
                    {
                        try
                        {
                            await modbusSerialMaster.WriteSingleRegisterAsync(item.RegisterDto.StationNum, item.RegisterDto.Address, Convert.ToUInt16(item.Value));

                        }
                        catch (Exception ex)
                        {

                            HandyControl.Controls.Growl.Error($"写入失败：{ex.Message}");
                        }
                    }
                }
            }
            
        }
    }
}
