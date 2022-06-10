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

namespace TheLast.Views
{
    /// <summary>
    /// AutomaticTestingView.xaml 的交互逻辑
    /// </summary>
    public partial class AutomaticTestingView : UserControl
    {
        public AutomaticTestingView()
        {
            InitializeComponent();
        }
        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

        private void Remark_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (((TextBox)sender).Text.Contains("未通过"))
            {
                ((TextBox)sender).Foreground = new SolidColorBrush(Colors.Red);
            }
            else
            {
                ((TextBox)sender).Foreground = new SolidColorBrush(Colors.Black);
            }
                

        }
    }
}
