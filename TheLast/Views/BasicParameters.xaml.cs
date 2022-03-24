using MaterialDesignExtensions.Controls;
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
    /// BasicParameters.xaml 的交互逻辑
    /// </summary>
    public partial class BasicParameters : UserControl
    {
        public BasicParameters()
        {
            InitializeComponent();
        }

        private void DataGrid_LoadingRow(object sender, DataGridRowEventArgs e)
        {
            e.Row.Header = e.Row.GetIndex() + 1;
        }

    }
}
