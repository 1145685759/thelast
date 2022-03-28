using MaterialDesignThemes.Wpf;
using Modbus.Device;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using Quartz;
using Quartz.Impl;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using TheLast.Common;
using TheLast.Events;
using TheLast.QuartzJobs;

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
        private string[] comPorts=SerialPort.GetPortNames();
        private readonly IEventAggregator eventAggregator;

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
        public ComSettingViewModel(IEventAggregator eventAggregator)
        {
            this.eventAggregator = eventAggregator;
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
