using MaterialDesignThemes.Wpf;
using Modbus.Device;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using TheLast.Common;
using TheLast.Events;

namespace TheLast.ViewModels
{
    public class ComSettingViewModel : BindableBase, IDialogHostAware
    {
        private ModbusSerialMaster ModbusSerialMaster;
        private string currentComPort;
        public string CurrentComPort
        {
            get { return currentComPort; }
            set { SetProperty(ref currentComPort, value); }
        }
        private string[] comPorts=SerialPort.GetPortNames();

        public string[] ComPorts
        {
            get { return comPorts; }
            set { SetProperty(ref comPorts, value); }
        }
        public string DialogHostName { get ; set ; }
        public DelegateCommand SaveCommand { get ; set ; }
        public DelegateCommand CancelCommand { get ; set; }

        public void OnDialogOpend(IDialogParameters parameters)
        {
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new DelegateCommand(Cancel);
        }

        private void Cancel()
        {
            if (DialogHost.IsDialogOpen(DialogHostName))
                DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.No)); //取消返回NO告诉操作结束
        }

        private void Save()
        {
            SerialPort serialPort = new SerialPort();
            serialPort.PortName = CurrentComPort;
            serialPort.BaudRate = 115200;
            serialPort.DataBits = 8;
            serialPort.Parity = Parity.Even;
            serialPort.StopBits = StopBits.One;
            if (!serialPort.IsOpen)
            {
                serialPort.Open();
            }
            ModbusSerialMaster = ModbusSerialMaster.CreateRtu(serialPort);
            DialogParameters param = new DialogParameters();
            param.Add("Master", ModbusSerialMaster);
            if (DialogHost.IsDialogOpen(DialogHostName))
            {
                //确定时,把编辑的实体返回并且返回OK;
                DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.OK, param));
            }
        }
    }
}
