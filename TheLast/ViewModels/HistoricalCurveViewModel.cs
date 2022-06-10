using AutoMapper;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using Prism.Commands;
using Prism.Ioc;
using Prism.Services.Dialogs;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using TheLast.Dtos;
using TheLast.Entities;

namespace TheLast.ViewModels
{
    public class HistoricalCurveViewModel : NavigationViewModel, IDialogAware
    {
        public List<HsData> s { get; set; }
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
        private PlotModel myModel;
        private readonly ISqlSugarClient sqlSugarClient;
        private readonly IMapper mapper;

        public PlotModel MyModel
        {
            get { return myModel; }
            set { SetProperty(ref myModel, value); }
        }
        private DateTime startDate;
        public DateTime StartDate
        {
            get { return startDate; }
            set { SetProperty(ref startDate, value); }
        }
        private DateTime startTime;
        public DateTime StartTime
        {
            get { return startTime; }
            set { SetProperty(ref startTime, value); }
        }
        private DateTime endDate;
        public DateTime EndDate
        {
            get { return endDate; }
            set { SetProperty(ref endDate, value); }
        }
        private DateTime endTime;
        public DateTime EndTime
        {
            get { return endTime; }
            set { SetProperty(ref endTime, value); }
        }
        public HistoricalCurveViewModel(ISqlSugarClient sqlSugarClient,IMapper mapper, IContainerProvider containerProvider) : base(containerProvider)
        {
            Indoors = new List<int>();
            Outdoors = new List<int>();
            this.sqlSugarClient = sqlSugarClient;
            this.mapper = mapper;
            MyModel = new PlotModel();
            MyModel.Title = "历史数据曲线";
            var dateTimeAxis = new DateTimeAxis();
            dateTimeAxis.Title = "时间";
            MyModel.Axes.Add(dateTimeAxis);
            StartDate = DateTime.Now.Date.AddDays(-1);
            EndDate = DateTime.Now.Date;
            RegisterNameDisplay = new ObservableCollection<RegisterDto>();
        }
        private ObservableCollection<RegisterDto> registerName;
        public ObservableCollection<RegisterDto> RegisterName
        {
            get { return registerName; }
            set { SetProperty(ref registerName, value); }
        }
        private ObservableCollection<RegisterDto> registerNameDisplay;
        public ObservableCollection<RegisterDto> RegisterNameDisplay
        {
            get { return registerNameDisplay; }
            set { SetProperty(ref registerNameDisplay, value); }
        }
        private int index;
        public int Index
        {
            get { return index; }
            set { SetProperty(ref index, value); }
        }
        private DelegateCommand load;
        public DelegateCommand Load =>
            load ?? (load = new DelegateCommand(ExecuteLoad));


        async void ExecuteLoad()
        {
            if (RegisterName != null && RegisterName.Count > 0)
            {
                RegisterName.Clear();
            }
            s = await sqlSugarClient.Queryable<HsData>().Mapper(it => it.Register, it => it.RegisterId).Where(x => x.DateTime >= StartDate && x.DateTime <= EndDate).ToListAsync();
            var x = s.Select(x => x.Register).Distinct().ToList();
            RegisterNameDisplay = mapper.Map<ObservableCollection<RegisterDto>>(x);
            
            StationNums = s.Select(x => x.Register.StationNum).Distinct().ToArray();
            //List<int> registerid = await sqlSugarClient.Queryable<HsData>().Mapper(it => it.Register, it => it.RegisterId).Where(x => x.DateTime >= StartDate && x.DateTime <= EndDate).Select(it => it.RegisterId).ToListAsync();
            //registerid = registerid.Distinct().ToList();

        }
        private DelegateCommand<byte?> stationSelected;
        public DelegateCommand<byte?> StationSelected =>
            stationSelected ?? (stationSelected = new DelegateCommand<byte?>(ExecuteStationSelected));

        void ExecuteStationSelected(byte? parameter)
        {
            if (parameter == null)
            {
                return;
            }
            currentStation = (byte)parameter;
            RegisterTypes = s.Where(x => x.Register.StationNum == currentStation).Select(x => x.Register.RegisterType).Distinct().ToArray();
        }
        private DelegateCommand<RegisterDto> check;

        public event Action<IDialogResult> RequestClose;

        public DelegateCommand<RegisterDto> Check =>
            check ?? (check = new DelegateCommand<RegisterDto>(ExecuteCheck));

        public string Title { get; set; }

        void ExecuteCheck(RegisterDto parameter)
        {
            MyModel.Series.FirstOrDefault(x => x.Title == parameter.Name).IsVisible = !MyModel.Series.FirstOrDefault(x => x.Title == parameter.Name).IsVisible;
            MyModel.InvalidatePlot(true);
        }
        private DelegateCommand<int?> selected;
        public DelegateCommand<int?> Selected =>
            selected ?? (selected = new DelegateCommand<int?>(ExecuteSelected));

        void ExecuteSelected(int? par)
        {
            if (par == null)
            {
                return;
            }
            if (isIndoor == Visibility.Visible)
            {
                var result = RegisterNameDisplay.Where(x => x.StationNum == currentStation && x.RegisterType == "内机数据" && x.Name.Contains($"-{par}")).ToList();
                RegisterName.AddRange(result);
                MyModel.Series.Clear();
                foreach (var item in RegisterName)
                {
                    MyModel.Series.Add(new LineSeries { Title = item.Name, MarkerType = MarkerType.None, IsVisible =item.IsVisible});
                }
                foreach (LineSeries item in MyModel.Series)
                {
                    if (item == null)
                    {
                        continue;
                    }
                    foreach (var point in s.Where(x => x.Register.Name == item.Title))
                    {
                        if (item.Title.Contains("温度") || item.Title.Contains("感温包")) 
                        {
                            item.Points.Add(DateTimeAxis.CreateDataPoint(point.DateTime, point.RealValue / 10.0));
                        }
                        else
                        {
                            item.Points.Add(DateTimeAxis.CreateDataPoint(point.DateTime, point.RealValue));
                        }
                    }
                }
                MyModel.InvalidatePlot(true);
            }
            else
            {
                var result = s.Where(x => x.Register.StationNum == currentStation && x.Register.RegisterType == "外机数据" && x.Register.Name.Contains($"-{par}")).ToList();
                MyModel.Series.Clear();
                foreach (var item in result)
                {
                    MyModel.Series.Add(new LineSeries { Title = item.Register.Name, MarkerType = MarkerType.None, IsVisible = false });
                }
                foreach (LineSeries item in MyModel.Series)
                {
                    if (item == null)
                    {
                        continue;
                    }
                    foreach (var point in s.Where(x => x.Register.Name == item.Title))
                    {
                        if (item.Title.Contains("温度"))
                        {
                            item.Points.Add(DateTimeAxis.CreateDataPoint(point.DateTime, point.RealValue / 10.0));
                        }
                        else
                        {
                            item.Points.Add(DateTimeAxis.CreateDataPoint(point.DateTime, point.RealValue));
                        }
                    }
                }
                MyModel.InvalidatePlot(true);
            }
        }
        private DelegateCommand<string?> typesSelected;
        public DelegateCommand<string?> TypesSelected =>
            typesSelected ?? (typesSelected = new DelegateCommand<string?>(ExecuteTypesSelected));

        void ExecuteTypesSelected(string? parameter)
        {
            if (parameter == null)
            {
                return;
            }
            Index = -1;
            RegisterName.Clear();
            MyModel.Series.Clear();
            if (parameter == "内机数据")
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
            else if (parameter == "外机数据")
            {
                IsIndoor = Visibility.Collapsed;
                IsOutdoor = Visibility.Visible;
                if (App.OutdoorCount == 0)
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
                var register = RegisterNameDisplay.Where(x => x.StationNum == currentStation && x.RegisterType == parameter).ToList();
                RegisterName.AddRange(register);
                foreach (var item in RegisterName)
                {
                    var title = item.Name;
                    MyModel.Series.Add(new LineSeries { Title = title, MarkerType = MarkerType.None, IsVisible = item.IsVisible});
                }
                foreach (LineSeries item in MyModel.Series)
                {
                    if (item == null)
                    {
                        continue;
                    }
                    foreach (var point in s.Where(x => x.Register.Name == item.Title))
                    {
                        if (item.Title.Contains("温度"))
                        {
                            item.Points.Add(DateTimeAxis.CreateDataPoint(point.DateTime, point.RealValue / 10.0));
                        }
                        else
                        {
                            item.Points.Add(DateTimeAxis.CreateDataPoint(point.DateTime, point.RealValue));
                        }
                    }
                }
                MyModel.InvalidatePlot(true);
            }
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
        public bool CanCloseDialog()
        {
            return true;
        }

        public void OnDialogClosed()
        {

        }

        public void OnDialogOpened(IDialogParameters parameters)
        {
            IsIndoor = Visibility.Collapsed;
            IsOutdoor = Visibility.Collapsed;
            MyModel.Series.Clear();
            RegisterName = new ObservableCollection<RegisterDto>();
        }
    }
}
