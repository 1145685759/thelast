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
        public ObservableCollection<WriteRegister> WriteRegisters
        {
            get { return writeRegisters; }
            set { SetProperty(ref writeRegisters, value); }
        }
        public ManualTestViewModel(IContainerProvider containerProvider,ISqlSugarClient sqlSugarClient,IMapper mapper) : base(containerProvider)
        {

            WriteRegisters = new ObservableCollection<WriteRegister>();
            this.sqlSugarClient = sqlSugarClient;
            this.mapper = mapper;
        }
        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
            modbusSerialMaster = App.ModbusSerialMaster;
            WriteRegisters.Clear();
            var registers= await sqlSugarClient.Queryable<Register>().Where(x => x.IsEnable == true).ToListAsync();
            foreach (var item in registers)
            {
                WriteRegisters.Add(new WriteRegister
                {
                    RegisterDto = mapper.Map<RegisterDto>(item)
                }) ;
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
    }
}
