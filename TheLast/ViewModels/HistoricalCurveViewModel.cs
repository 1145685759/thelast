using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
using Prism.Commands;
using Prism.Ioc;
using Prism.Regions;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using TheLast.Entities;

namespace TheLast.ViewModels
{
    public class HistoricalCurveViewModel: NavigationViewModel
    {
        private PlotModel myModel;
        private readonly ISqlSugarClient sqlSugarClient;

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
        public HistoricalCurveViewModel(ISqlSugarClient sqlSugarClient, IContainerProvider containerProvider) : base(containerProvider)
        {
            this.sqlSugarClient = sqlSugarClient;
            MyModel = new PlotModel();
            MyModel.Title = "历史数据曲线";
            var dateTimeAxis = new DateTimeAxis();
            dateTimeAxis.Title = "时间";
            MyModel.Axes.Add(dateTimeAxis);
            StartDate = DateTime.Now.Date.AddDays(-1);
            EndDate = DateTime.Now.Date;
        }
        private ObservableCollection<string> registerName;
        public ObservableCollection<string> RegisterName
        {
            get { return registerName; }
            set { SetProperty(ref registerName, value); }
        }
        private DelegateCommand load;
        public DelegateCommand Load =>
            load ?? (load = new DelegateCommand(ExecuteLoad));

        async void ExecuteLoad()
        {
            if (RegisterName!=null&&RegisterName.Count>0)
            {
                RegisterName.Clear();
            }
            var s = await sqlSugarClient.Queryable<HsData>().Mapper(it=>it.Register,it=>it.RegisterId).Where(x => x.DateTime >= StartDate && x.DateTime <= EndDate).ToListAsync();
            List<int> registerid =await sqlSugarClient.Queryable<HsData>().Mapper(it => it.Register, it => it.RegisterId).Where(x => x.DateTime >= StartDate && x.DateTime <= EndDate).Select(it=>it.RegisterId).ToListAsync();
            registerid= registerid.Distinct().ToList();
            foreach (var item in registerid)
            {
                var title=(await sqlSugarClient.Queryable<Register>().FirstAsync(x => x.Id == item)).Name;
                RegisterName.Add(title);
                MyModel.Series.Add(new LineSeries { Title = title, MarkerType = MarkerType.None, IsVisible = false });
            }
            MyModel.IsLegendVisible = true;
            var l = new Legend
            {
                LegendBorder = OxyColors.Black,
                LegendBackground = OxyColor.FromAColor(200, OxyColors.White),
                LegendPosition = LegendPosition.RightTop,
                LegendPlacement = LegendPlacement.Outside,
                LegendOrientation = LegendOrientation.Vertical,
                LegendItemOrder = LegendItemOrder.Normal,
                LegendItemAlignment = HorizontalAlignment.Right,
                LegendSymbolPlacement = LegendSymbolPlacement.Right,
            };
            MyModel.Legends.Add(l);
            foreach (LineSeries item in MyModel.Series)
            {
                if (item==null)
                {
                    continue;
                }
                foreach (var point in s.Where(x=>x.Register.Name==item.Title))
                {
                    if (item.Title.Contains("温度"))
                    {
                        item.Points.Add(DateTimeAxis.CreateDataPoint(point.DateTime, point.RealValue/10.0));
                    }
                    else
                    {
                        item.Points.Add(DateTimeAxis.CreateDataPoint(point.DateTime, point.RealValue));
                    }
                }
            }
            MyModel.InvalidatePlot(true);
        }
        public override  void OnNavigatedTo(NavigationContext navigationContext)
        {
            MyModel.Series.Clear();
            RegisterName = new ObservableCollection<string>();
        }
        private DelegateCommand<string> check;
        public DelegateCommand<string> Check =>
            check ?? (check = new DelegateCommand<string>(ExecuteCheck));

        void ExecuteCheck(string parameter)
        {
           MyModel.Series.FirstOrDefault(x => x.Title == parameter).IsVisible = !MyModel.Series.FirstOrDefault(x => x.Title == parameter).IsVisible;
           MyModel.InvalidatePlot(true);
        }
    }
}
