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
                        for (int i = 0; i < registerList.Count; i++)
                        {
                            LineSeries lineSeries = (LineSeries)MyModel.Series[i];
                            if (lineSeries == null)
                            {
                                continue;
                            }
                            if (registerList[i].RegisterType == "内机数据" || registerList[i].RegisterType == "步进电机脉冲检测" || registerList[i].RegisterType == "外机数据")
                            {
                                var result = await App.ModbusSerialMaster.ReadInputRegistersAsync(1, registerList[i].Address, 1);
                                if (registerList[i].Name.Contains("温度"))
                                {
                                    lineSeries.Points.Add(DateTimeAxis.CreateDataPoint(DateTime.Now, result[0]/10));
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
                            else if (registerList[i].RegisterType == "基础参数"|| registerList[i].RegisterType == "内机控制参数" || registerList[i].RegisterType == "20个温度设置")
                            {
                                var result = await App.ModbusSerialMaster.ReadHoldingRegistersAsync(1, registerList[i].Address, 1);
                                if (registerList[i].Name.Contains("温度"))
                                {
                                    lineSeries.Points.Add(DateTimeAxis.CreateDataPoint(DateTime.Now, result[0]/10));
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
                            else if (registerList[i].RegisterType == "数字量输入")
                            {
                                var result = await App.ModbusSerialMaster.ReadInputsAsync(1, registerList[i].Address, 1);
                                lineSeries.Points.Add(DateTimeAxis.CreateDataPoint(DateTime.Now,Convert.ToUInt16(result[0])));
                                if (lineSeries.Points.Count > 3600)
                                {
                                    lineSeries.Points.RemoveAt(0);
                                }
                            }
                            else if (registerList[i].RegisterType == "数字量输出")
                            {
                                var result = await App.ModbusSerialMaster.ReadCoilsAsync(1, registerList[i].Address, 1);
                                lineSeries.Points.Add(DateTimeAxis.CreateDataPoint(DateTime.Now, Convert.ToUInt16(result[0])));
                                if (lineSeries.Points.Count > 3600)
                                {
                                    lineSeries.Points.RemoveAt(0);
                                }
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
