using MaterialDesignThemes.Wpf;
using Modbus.Device;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Threading.Tasks;
using TheLast.Common;
using TheLast.Entities;
using TheLast.Events;
using TheLast.Extensions;

namespace TheLast.ViewModels
{
    public class ComSettingViewModel : BindableBase
    {
        SerialPort serialPort;
        private string currentComPort;
        public string CurrentComPort
        {
            get { return currentComPort; }
            set { SetProperty(ref currentComPort, value); }
        }
        List<HsData> hsDatas;
        private string[] comPorts=SerialPort.GetPortNames();
        private readonly IEventAggregator eventAggregator;
        private readonly ISqlSugarClient sqlSugarClient;
        private readonly IRegionManager regionManager;

        public string[] ComPorts
        {
            get { return comPorts; }
            set { SetProperty(ref comPorts, value); }
        }
        public string DialogHostName { get ; set ; }
        public DelegateCommand SaveCommand { get ; set ; }
        public DelegateCommand CancelCommand { get ; set; }

        private void Cancel()
        {
            if (DialogHost.IsDialogOpen(DialogHostName))
                DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.No)); //取消返回NO告诉操作结束
        }
        public ComSettingViewModel(IEventAggregator eventAggregator,ISqlSugarClient sqlSugarClient,IRegionManager regionManager)
        {
            hsDatas = new List<HsData>();
            this.eventAggregator = eventAggregator;
            this.sqlSugarClient = sqlSugarClient;
            this.regionManager = regionManager;
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new DelegateCommand(Cancel);
        }

        private  void Save()
        {
            if (string.IsNullOrEmpty(CurrentComPort))
            {
                HandyControl.Controls.Growl.Error("请先选择串口！");
                return;
            }
            if (serialPort==null)
            {
                serialPort = new SerialPort();
                serialPort.PortName = CurrentComPort;
                serialPort.BaudRate = 115200;
                serialPort.DataBits = 8;
                serialPort.Parity = Parity.Even;
                serialPort.StopBits = StopBits.One;
                try
                {
                    if (!serialPort.IsOpen)
                    {
                        serialPort.Open();
                        App.ModbusSerialMaster = ModbusSerialMaster.CreateRtu(serialPort);
                        HandyControl.Controls.Growl.Success("串口连接成功");
                    }
                    else
                    {
                        App.ModbusSerialMaster = ModbusSerialMaster.CreateRtu(serialPort);
                        HandyControl.Controls.Growl.Success("串口连接成功");
                    }
                    Task.Run(async () =>
                    {
                        while (true)
                        {
                            hsDatas.Clear();
                            var registers= await sqlSugarClient.Queryable<Register>().Where(x => x.IsEnable == true&&x.IsHsData==true).ToListAsync();
                            foreach (var item in registers)
                            {
                                hsDatas.Add(new HsData { DateTime = DateTime.Now, RealValue = (await App.ModbusSerialMaster.ReadHoldingRegistersAsync(item.StationNum, item.Address, 1))[0], Register = item, RegisterId = item.Id });
                            }
                            await sqlSugarClient.Insertable(hsDatas).ExecuteCommandAsync();
                            await Task.Delay(1000);
                        }

                    });
                    regionManager.RequestNavigate(PrismManager.MainViewRegionName, "ProjectManager");

                }
                catch (Exception ex)
                {
                    HandyControl.Controls.Growl.Error(ex.Message);
                }
            }
            else
            {
                HandyControl.Controls.Growl.Error("串口已打开");
            }
        }
    }
}
