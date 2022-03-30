using AutoMapper;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Legends;
using OxyPlot.Series;
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
using System.Threading.Tasks;
using TheLast.Dtos;
using TheLast.Entities;

namespace TheLast.ViewModels
{
    public class RealTimeCurveViewModel: NavigationViewModel
    {
        private PlotModel myModel;
        private readonly ISqlSugarClient sqlSugarClient;
        private readonly IMapper mapper;

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
        public RealTimeCurveViewModel(ISqlSugarClient sqlSugarClient, IContainerProvider containerProvider,IMapper mapper):base(containerProvider)
        {
            this.sqlSugarClient = sqlSugarClient;
            this.mapper = mapper;
            RegisterDtos = new ObservableCollection<RegisterDto>();
            MyModel = new PlotModel();
            MyModel.Title = "实时状态曲线";
            var dateTimeAxis = new DateTimeAxis();
            dateTimeAxis.Title = "时间";
            MyModel.Axes.Add(dateTimeAxis);
        }
        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
            if (App.ModbusSerialMaster==null)
            {
                HandyControl.Controls.Growl.Info("串口未设置");
                return;
            }
            var registerList= await sqlSugarClient.Queryable<Register>().Where(x => x.IsEnable == true&&x.IsDisplay==true).ToListAsync();
            RegisterDtos = mapper.Map<ObservableCollection<RegisterDto>>(registerList);
            MyModel.Series.Clear();
            MyModel.IsLegendVisible = true;
            var l = new Legend
            {
                LegendBorder = OxyColors.Black,
                LegendBackground = OxyColor.FromAColor(200, OxyColors.White),
                LegendPosition = LegendPosition.RightTop,
                LegendPlacement = LegendPlacement.Outside,
                LegendOrientation =LegendOrientation.Vertical,
                LegendItemOrder = LegendItemOrder.Normal,
                LegendItemAlignment = HorizontalAlignment.Right,
                LegendSymbolPlacement = LegendSymbolPlacement.Right,
            };
            MyModel.Legends.Add(l);

            foreach (var item in registerList)
            {
               
                MyModel.Series.Add(new LineSeries {Title=item.Name,MarkerType=MarkerType.None,IsVisible=false}) ;
                
            }
            await Task.Run(async () =>
            {
                while (true)
                {
                    await App.Current.Dispatcher.BeginInvoke(new Action(async () =>
                    {
                        foreach (LineSeries item in MyModel.Series)
                        {
                            if (item==null)
                            {
                                continue;
                            }
                            var getvaddress = await sqlSugarClient.Queryable<Register>().FirstAsync(x => x.Name == item.Title);
                            var result= await App.ModbusSerialMaster.ReadHoldingRegistersAsync(1,getvaddress.Address, 1);
                            item.Points.Add(DateTimeAxis.CreateDataPoint(DateTime.Now, result[0]));
                            if (item.Points.Count>3600)
                            {
                                item.Points.RemoveAt(0);
                            }
                        }
                    }));
                    MyModel.InvalidatePlot(true);
                    await Task.Delay(1000);
                }
            });
        }
        private DelegateCommand<RegisterDto> check;
        public DelegateCommand<RegisterDto> Check =>
            check ?? (check = new DelegateCommand<RegisterDto>(ExecuteCheck));

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
    }
}
