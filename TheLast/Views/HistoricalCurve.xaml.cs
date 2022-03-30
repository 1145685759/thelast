using AutoMapper;
using MaterialDesignThemes.Wpf;
using Prism.Ioc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using TheLast.ViewModels;

namespace TheLast.Views
{
    /// <summary>
    /// HistoricalCurve.xaml 的交互逻辑
    /// </summary>
    public partial class HistoricalCurve : UserControl
    {
        private readonly ISqlSugarClient isqlsuagerclient;
        private readonly IContainerProvider containerProvider;
        HistoricalCurveViewModel historicalCurveViewModel;
        public HistoricalCurve(ISqlSugarClient isqlsuagerclient,IContainerProvider containerProvider,IMapper mapper)
        {
            InitializeComponent();
            DataContext = historicalCurveViewModel = new HistoricalCurveViewModel(isqlsuagerclient,containerProvider);
            this.isqlsuagerclient = isqlsuagerclient;
            this.containerProvider = containerProvider;
        }
        public void CombinedDialogOpenedEventHandler(object sender, DialogOpenedEventArgs eventArgs)
        {
            CombinedCalendar.SelectedDate = historicalCurveViewModel.StartDate;
            CombinedClock.Time = historicalCurveViewModel.StartTime;
        }
        public void CombinedDialogClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            if (Convert.ToBoolean(Convert.ToUInt16(eventArgs.Parameter)) && CombinedCalendar.SelectedDate is DateTime selectedDate)
            {
                DateTime combined = selectedDate.AddSeconds(CombinedClock.Time.TimeOfDay.TotalSeconds);
                historicalCurveViewModel.StartDate = combined;
                historicalCurveViewModel.StartTime = combined;

            }
        }
        public void Combined1DialogOpenedEventHandler(object sender, DialogOpenedEventArgs eventArgs)
        {
            CombinedCalendar.SelectedDate = historicalCurveViewModel.EndDate;
            CombinedClock.Time = historicalCurveViewModel.EndTime;
        }
        public void Combined1DialogClosingEventHandler(object sender, DialogClosingEventArgs eventArgs)
        {
            if (Convert.ToBoolean(Convert.ToUInt16(eventArgs.Parameter)) && CombinedCalendar.SelectedDate is DateTime selectedDate)
            {
                DateTime combined = selectedDate.AddSeconds(CombinedClock.Time.TimeOfDay.TotalSeconds);
                historicalCurveViewModel.EndDate = combined;
                historicalCurveViewModel.EndTime = combined;

            }
        }
    }
}
