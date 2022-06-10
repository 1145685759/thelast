using MaterialDesignThemes.Wpf;
using Modbus.Device;
using Newtonsoft.Json;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Ports;
using System.Text;
using System.Threading.Tasks;
using TheLast.Common;
using TheLast.Entities;
using TheLast.Events;
using TheLast.Extensions;
using UtilsSharp;

namespace TheLast.ViewModels
{
    public class ComSettingViewModel : BindableBase, INavigationAware
    {
        private int comPortIndex;
        public int ComPortIndex
        {
            get { return comPortIndex; }
            set { SetProperty(ref comPortIndex, value); }
        }
        SerialPort serialPort;
        private int indoorCount;
        public int IndoorCount
        {
            get { return indoorCount; }
            set { SetProperty(ref indoorCount, value); }
        }
        private float vREF;
        public float VREF
        {
            get { return vREF; }
            set { SetProperty(ref vREF, value); }
        }
        private List<ValueDictionary> iMCMS;
        public List<ValueDictionary> IMCMS
        {
            get { return iMCMS; }
            set { SetProperty(ref iMCMS, value); }
        }
        private int iMCM;
        public int IMCM
        {
            get { return iMCM; }
            set { SetProperty(ref iMCM, value); }
        }
        private List<ValueDictionary> uCMS;
        public List<ValueDictionary> UCMS
        {
            get { return uCMS; }
            set { SetProperty(ref uCMS, value); }
        }
        private int uCM;
        public int UCM
        {
            get { return uCM; }
            set { SetProperty(ref uCM, value); }
        }
        private List<ValueDictionary> wSGS;
        public List<ValueDictionary> WSGS
        {
            get { return wSGS; }
            set { SetProperty(ref wSGS, value); }
        }
        private int wSG;
        public int WSG
        {
            get { return wSG; }
            set { SetProperty(ref wSG, value); }
        }
        private int outdoorCount;
        public int OutdoorCount
        {
            get { return outdoorCount; }
            set { SetProperty(ref outdoorCount, value); }
        }
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
            if (DialogHostName==null)
            {
                return;
            }
            if (DialogHost.IsDialogOpen(DialogHostName))
                DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.No)); //取消返回NO告诉操作结束
        }
        public ComSettingViewModel(IEventAggregator eventAggregator,ISqlSugarClient sqlSugarClient,IRegionManager regionManager)
        {
            hsDatas = new List<HsData>();
            IMCMS = new List<ValueDictionary>();
            UCMS = new List<ValueDictionary>();
            WSGS = new List<ValueDictionary>();
            this.eventAggregator = eventAggregator;
            this.sqlSugarClient = sqlSugarClient;
            this.regionManager = regionManager;
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new DelegateCommand(Cancel);
            IMCMS=sqlSugarClient.Queryable<ValueDictionary>().Where(x => x.RegisterId == 2).ToList();
            UCMS = sqlSugarClient.Queryable<ValueDictionary>().Where(x => x.RegisterId == 3).ToList();
            WSGS = sqlSugarClient.Queryable<ValueDictionary>().Where(x => x.RegisterId == 4).ToList();
        }

        private async void Save()
        {
            if (string.IsNullOrEmpty(CurrentComPort))
            {
                HandyControl.Controls.Growl.Error("请先选择串口！");
                return;
            }
           
            App.IndoorCount = IndoorCount;
            App.OutdoorCount = OutdoorCount;
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
                        //App.ModbusSerialMaster.Transport.ReadTimeout = 100;
                        //App.ModbusSerialMaster.Transport.WriteTimeout = 100;//写入数据超时100ms
                        //App.ModbusSerialMaster.Transport.Retries = 3;//重试次数
                        //App.ModbusSerialMaster.Transport.WaitToRetryMilliseconds = 10;//重试间隔

                        HandyControl.Controls.Growl.Success("串口连接成功");
                        await App.ModbusSerialMaster.WriteSingleRegisterAsync(1, 0, Convert.ToUInt16(VREF * 10));
                        await App.ModbusSerialMaster.WriteSingleRegisterAsync(1, 1, Convert.ToUInt16(IMCM));
                        await App.ModbusSerialMaster.WriteSingleRegisterAsync(1, 2, Convert.ToUInt16(UCM));
                        await App.ModbusSerialMaster.WriteSingleRegisterAsync(1, 3, Convert.ToUInt16(WSG));
                    }
                    else
                    {
                        App.ModbusSerialMaster = ModbusSerialMaster.CreateRtu(serialPort);
                        HandyControl.Controls.Growl.Success("串口连接成功");
                        await App.ModbusSerialMaster.WriteSingleRegisterAsync(1, 0, Convert.ToUInt16(VREF * 10));
                        await App.ModbusSerialMaster.WriteSingleRegisterAsync(1, 1, Convert.ToUInt16(IMCM));
                        await App.ModbusSerialMaster.WriteSingleRegisterAsync(1, 2, Convert.ToUInt16(UCM));
                        await App.ModbusSerialMaster.WriteSingleRegisterAsync(1, 3, Convert.ToUInt16(WSG));
                    }
                   await Task.Run(async () =>
                    {
                        while (true)
                        {
                            hsDatas.Clear();
                            var registers= await sqlSugarClient.Queryable<Register>().Where(x => x.IsEnable == true&&x.IsHsData==true).ToListAsync();
                            foreach (var item in registers)
                            {
                                try
                                {
                                    if (item.RegisterType == "步进电机脉冲检测" || item.RegisterType == "外机数据" || item.RegisterType == "内机数据"||item.RegisterType== "反馈脉冲数")
                                    {
                                        hsDatas.Add(new HsData { DateTime = DateTime.Now, RealValue = (await App.ModbusSerialMaster.ReadInputRegistersAsync(item.StationNum, item.Address, 1))[0], Register = item, RegisterId = item.Id });
                                    }
                                    else if (item.RegisterType == "数字量输入")
                                    {
                                        hsDatas.Add(new HsData { DateTime = DateTime.Now, RealValue = Convert.ToUInt16((await App.ModbusSerialMaster.ReadInputsAsync(item.StationNum, item.Address, 1))[0]), Register = item, RegisterId = item.Id });
                                    }
                                    else if (item.RegisterType == "数字量输出")
                                    {
                                        hsDatas.Add(new HsData { DateTime = DateTime.Now, RealValue = Convert.ToUInt16((await App.ModbusSerialMaster.ReadCoilsAsync(item.StationNum, item.Address, 1))[0]), Register = item, RegisterId = item.Id });
                                    }
                                    else
                                    {
                                        hsDatas.Add(new HsData { DateTime = DateTime.Now, RealValue = (await App.ModbusSerialMaster.ReadHoldingRegistersAsync(item.StationNum, item.Address, 1))[0], Register = item, RegisterId = item.Id });

                                    }
                                }
                                catch (Exception ex)
                                {
                                    HandyControl.Controls.Growl.Error(item.Name);
                                }
                                
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

        public void OnNavigatedTo(NavigationContext navigationContext)
        {
            ComPortIndex = comPorts.Length - 1;
            try
            {
                IndoorCount = Convert.ToInt32(AppsettingsHelper.GetValue("IndoorCount"));
                OutdoorCount = Convert.ToInt32(AppsettingsHelper.GetValue("OutdoorCount"));
                IMCM = Convert.ToInt32(AppsettingsHelper.GetValue("IndoorControl"));
                UCM = Convert.ToInt32(AppsettingsHelper.GetValue("UnitControl"));
                WSG = Convert.ToInt32(AppsettingsHelper.GetValue("Gears"));
            }
            catch (Exception ex)
            {
                HandyControl.Controls.Growl.Error("配置文件异常");
            }
           
        }

        public bool IsNavigationTarget(NavigationContext navigationContext)
        {
            return true;
        }

        public async void OnNavigatedFrom(NavigationContext navigationContext)
        {
            var setting = new Settings { Gears = WSG, IndoorControl = IMCM, UnitControl = UCM, IndoorCount = IndoorCount, OutdoorCount = OutdoorCount };
            var json= JsonConvert.SerializeObject(setting);
            string path = Directory.GetCurrentDirectory() + "\\appsettings.json";
            await File.WriteAllTextAsync(path,json);
        }
    }
}
