using AutoMapper;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using Prism.Regions;
using Prism.Services.Dialogs;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TheLast.Dtos;
using TheLast.Entities;

namespace TheLast.ViewModels
{
    public class RealTimeCurveViewModel : NavigationViewModel,IDialogAware
    {
        private List<int> inDoors;
        public List<int> Indoors
        {
            get { return inDoors; }
            set { SetProperty(ref inDoors, value); }
        }
        private List<int> outDoors;
        public List<int> Outdoors
        {
            get { return outDoors; }
            set { SetProperty(ref outDoors, value); }
        }
        byte currentStation;

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
        private PlotModel myModel;
        private readonly ISqlSugarClient sqlSugarClient;
        private readonly IMapper mapper;
        private byte[] stationNums;
        public byte[] StationNums
        {
            get { return stationNums; }
            set { SetProperty(ref stationNums, value); }
        }
        private string[] registerTypes;
        public string[] RegisterTypes
        {
            get { return registerTypes; }
            set { SetProperty(ref registerTypes, value); }
        }
        private int index=-1;
        public int Index
        {
            get { return index; }
            set { SetProperty(ref index, value); }
        }
        public PlotModel MyModel
        {
            get { return myModel; }
            set { SetProperty(ref myModel, value); }
        }
        private ObservableCollection<RegisterDto> registerDtos;
        public ObservableCollection<RegisterDto> RegisterDtos
        {
            get { return registerDtos; }
            set { SetProperty(ref registerDtos, value); }
        }
        private ObservableCollection<RegisterDto> registerDtosDisplay;
        public ObservableCollection<RegisterDto> RegisterDtosDisplay
        {
            get { return registerDtosDisplay; }
            set { SetProperty(ref registerDtosDisplay, value); }
        }
        public RealTimeCurveViewModel(ISqlSugarClient sqlSugarClient, IContainerProvider containerProvider, IMapper mapper) : base(containerProvider)
        {
            this.sqlSugarClient = sqlSugarClient;
            this.mapper = mapper;
            RegisterDtos = new ObservableCollection<RegisterDto>();
            RegisterDtosDisplay = new ObservableCollection<RegisterDto>();
            MyModel = new PlotModel();
            MyModel.Title = "实时状态曲线";
            var dateTimeAxis = new DateTimeAxis();
            dateTimeAxis.Title = "时间";
            MyModel.Axes.Add(dateTimeAxis);
            Indoors = new List<int>();
            Outdoors = new List<int>();
        }
        private DelegateCommand<RegisterDto> check;

        public event Action<IDialogResult> RequestClose;

        public DelegateCommand<RegisterDto> Check =>
            check ?? (check = new DelegateCommand<RegisterDto>(ExecuteCheck));

        public string Title { get; set; }

        void ExecuteCheck(RegisterDto parameter)
        {
            if (parameter.IsVisible)
            {
                MyModel.Series.FirstOrDefault(x => x.Title == parameter.Name).IsVisible = true;
            }
            else
            {
                MyModel.Series.FirstOrDefault(x => x.Title == parameter.Name).IsVisible = false;
            }

        }

        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {
            
        }

        public async void OnDialogOpened(IDialogParameters parameters)
        {
            IsIndoor = Visibility.Collapsed;
            IsOutdoor = Visibility.Collapsed;
            
            if (App.ModbusSerialMaster == null)
            {
                HandyControl.Controls.Growl.Info("串口未设置");
                return;
            }
            var registerList = await sqlSugarClient.Queryable<Register>().Where(x => x.IsEnable == true && x.IsDisplay == true).ToListAsync();
            RegisterDtos = mapper.Map<ObservableCollection<RegisterDto>>(registerList);
            StationNums= RegisterDtos.Select(x => x.StationNum).Distinct().ToArray();
            
            MyModel.Series.Clear();
            MyModel.IsLegendVisible = true;
            var l = new Legend
            {
                LegendBorder = OxyColors.Black,
                LegendBackground = OxyColor.FromAColor(200, OxyColors.White),
                LegendPosition = LegendPosition.TopCenter,
                LegendPlacement = LegendPlacement.Outside,
                LegendOrientation = LegendOrientation.Horizontal,
                LegendItemOrder = LegendItemOrder.Normal,
                LegendItemAlignment = OxyPlot.HorizontalAlignment.Center,
                LegendSymbolPlacement = LegendSymbolPlacement.Right,
            };
            MyModel.Legends.Add(l);

            
        }
        private DelegateCommand<int?> selectedIndoors;
        public DelegateCommand<int?> SelectedIndoors =>
            selectedIndoors ?? (selectedIndoors = new DelegateCommand<int?>(ExecuteSelectedIndoors));

        void ExecuteSelectedIndoors(int? par)
        {
            if (par==null)
            {
                return;
            }
           

            if (isIndoor==Visibility.Visible)
            {
                var result = RegisterDtos.Where(x => x.StationNum == currentStation && x.RegisterType =="内机数据"&&x.Name.Contains($"-{par}")).ToList();
                RegisterDtosDisplay.Clear();
                RegisterDtosDisplay.AddRange(result);
                MyModel.Series.Clear();
                foreach (var item in RegisterDtosDisplay)
                {
                    MyModel.Series.Add(new LineSeries { Title = item.Name, MarkerType = MarkerType.None, IsVisible = item.IsVisible });
                }
            }
        }
        private DelegateCommand<int?> selectedOutdoors;
        public DelegateCommand<int?> SelectedOutdoors =>
            selectedOutdoors ?? (selectedOutdoors = new DelegateCommand<int?>(ExecuteSelectedOutdoors));

        void ExecuteSelectedOutdoors(int? par)
        {
            if (par == null)
            {
                return;
            }
            if(IsOutdoor==Visibility.Visible)
            {
                var result = RegisterDtos.Where(x => x.StationNum == currentStation && x.RegisterType == "外机数据" && x.Name.Contains($"-{par}")).ToList();
                RegisterDtosDisplay.Clear();
                RegisterDtosDisplay.AddRange(result);
                MyModel.Series.Clear();
                foreach (var item in RegisterDtosDisplay)
                {
                    MyModel.Series.Add(new LineSeries { Title = item.Name, MarkerType = MarkerType.None, IsVisible = item.IsVisible });
                }
            }
        }
        private DelegateCommand<byte?> stationSelected;
        public DelegateCommand<byte?> StationSelected =>
            stationSelected ?? (stationSelected = new DelegateCommand<byte?>(ExecuteStationSelected));

        void ExecuteStationSelected(byte? parameter)
        {
            if (parameter==null)
            {
                return;
            }
            currentStation = (byte)parameter;
            RegisterTypes =RegisterDtos.Where(x=>x.StationNum==parameter).Select(x => x.RegisterType).Distinct().ToArray();
        }
        private DelegateCommand<string?> typesSelected;
        public DelegateCommand<string?> TypesSelected =>
            typesSelected ?? (typesSelected = new DelegateCommand<string?>(ExecuteTypesSelected));

        async void ExecuteTypesSelected(string? parameter)
        {
            if (parameter == null)
            {
                return;
            }
            Index = -1;
            if (parameter=="内机数据")
            {
                IsIndoor = Visibility.Visible;
                IsOutdoor = Visibility.Collapsed;
                if (App.IndoorCount == 0)
                {
                    return;
                }
                for (int i = 1; i <= App.IndoorCount; i++)
                {
                    Indoors.Add(i);
                }
                
            }
            else if (parameter=="外机数据")
            {
                IsIndoor = Visibility.Collapsed;
                IsOutdoor = Visibility.Visible;
                if ( App.OutdoorCount == 0)
                {
                    return;
                }
                for (int i = 1; i <= App.OutdoorCount; i++)
                {
                    Outdoors.Add(i);
                }
               
            }
            else  
            {
                IsIndoor = Visibility.Collapsed;
                IsOutdoor = Visibility.Collapsed;
                var result = RegisterDtos.Where(x => x.StationNum == currentStation && x.RegisterType == parameter).ToList();
                RegisterDtosDisplay.Clear();
                RegisterDtosDisplay.AddRange(result);
                MyModel.Series.Clear();
            }
           
            foreach (var item in RegisterDtosDisplay)
            {
                MyModel.Series.Add(new LineSeries { Title = item.Name, MarkerType = MarkerType.None, IsVisible = item.IsVisible });
            }
            await Task.Run(async () =>
            {
                while (true)
                {
                    await App.Current.Dispatcher.BeginInvoke(new Action(async () =>
                    {
                        for (int i = 0; i < RegisterDtosDisplay.Count; i++)
                        {
                            try
                            {
                                LineSeries lineSeries = (LineSeries)MyModel.Series[i];
                                if (RegisterDtosDisplay[i].RegisterType == "内机数据" || RegisterDtosDisplay[i].RegisterType == "步进电机脉冲检测" || RegisterDtosDisplay[i].RegisterType == "外机数据")
                                {

                                    var result = await App.ModbusSerialMaster.ReadInputRegistersAsync(1, RegisterDtosDisplay[i].Address, 1);
                                    if (RegisterDtosDisplay[i].Name.Contains("温度")|| RegisterDtosDisplay[i].Name.Contains("感温包"))
                                    {
                                        lineSeries.Points.Add(DateTimeAxis.CreateDataPoint(DateTime.Now, result[0] / 10.0));
                                    }
                                    else
                                    {
                                        lineSeries.Points.Add(DateTimeAxis.CreateDataPoint(DateTime.Now, result[0]));
                                    }
                                    if (lineSeries.Points.Count > 3600)
                                    {
                                        lineSeries.Points.RemoveAt(0);
                                    }
                                }
                                else if (RegisterDtosDisplay[i].RegisterType == "基础参数" || RegisterDtosDisplay[i].RegisterType == "内机控制参数" || RegisterDtosDisplay[i].RegisterType == "20个温度设置")
                                {
                                    if (i > RegisterDtosDisplay.Count)
                                    {
                                        continue;
                                    }
                                    var result = await App.ModbusSerialMaster.ReadHoldingRegistersAsync(1, RegisterDtosDisplay[i].Address, 1);
                                    if (RegisterDtosDisplay[i].Name.Contains("温度")|| RegisterDtosDisplay[i].Name.Contains("感温包"))
                                    {
                                        lineSeries.Points.Add(DateTimeAxis.CreateDataPoint(DateTime.Now, result[0] / 10.0));
                                    }
                                    else
                                    {
                                        lineSeries.Points.Add(DateTimeAxis.CreateDataPoint(DateTime.Now, result[0]));
                                    }
                                    if (lineSeries.Points.Count > 3600)
                                    {
                                        lineSeries.Points.RemoveAt(0);
                                    }
                                }
                                else if (RegisterDtosDisplay[i].RegisterType == "数字量输入")
                                {
                                    if (i > RegisterDtosDisplay.Count)
                                    {
                                        continue;
                                    }
                                    var result = await App.ModbusSerialMaster.ReadInputsAsync(1, RegisterDtosDisplay[i].Address, 1);
                                    lineSeries.Points.Add(DateTimeAxis.CreateDataPoint(DateTime.Now, Convert.ToUInt16(result[0])));
                                    if (lineSeries.Points.Count > 3600)
                                    {
                                        lineSeries.Points.RemoveAt(0);
                                    }
                                }
                                else if (RegisterDtosDisplay[i].RegisterType == "数字量输出")
                                {
                                    if (i > RegisterDtosDisplay.Count)
                                    {
                                        continue;
                                    }
                                    var result = await App.ModbusSerialMaster.ReadCoilsAsync(1, RegisterDtosDisplay[i].Address, 1);
                                    lineSeries.Points.Add(DateTimeAxis.CreateDataPoint(DateTime.Now, Convert.ToUInt16(result[0])));
                                    if (lineSeries.Points.Count > 3600)
                                    {
                                        lineSeries.Points.RemoveAt(0);
                                    }
                                }
                            }
                            catch (Exception)
                            {

                                continue;
                            }


                        }
                    }));
                    MyModel.InvalidatePlot(true);
                    await Task.Delay(1000);
                }
            });
        }
    }
}