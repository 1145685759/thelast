using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using TheLast.Common;
using TheLast.Dtos;
using TheLast.Entities;

namespace TheLast.ViewModels
{
    public class CheckRegisterViewModel : BindableBase, IDialogHostAware
    {
        private int indoorIndex;
        public int IndoorIndex
        {
            get { return indoorIndex; }
            set { SetProperty(ref indoorIndex, value); }
        }
        private string name;
        public string Name
        {
            get { return name; }
            set { SetProperty(ref name, value); }
        }
        private Visibility visibility;
        public Visibility Visibility
        {
            get { return visibility; }
            set { SetProperty(ref visibility, value); }
        }
        private List<int> indoorCount;
        public List<int> IndoorCount
        {
            get { return indoorCount; }
            set { SetProperty(ref indoorCount, value); }
        }
        private List<int> outdoorCount;
        public List<int> OutdoorCount
        {
            get { return outdoorCount; }
            set { SetProperty(ref outdoorCount, value); }
        }
        private List<Register> indoorPrarm;
        public List<Register> IndoorPrarm
        {
            get { return indoorPrarm; }
            set { SetProperty(ref indoorPrarm, value); }
        }
        private List<Register> outdoorPrarm;
        public List<Register> OutdoorPrarm
        {
            get { return outdoorPrarm; }
            set { SetProperty(ref outdoorPrarm, value); }
        }
        public string DialogHostName { get ; set ; }
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand CancelCommand { get ; set ; }
        private RegisterDto model;
        public RegisterDto Model
        {
            get { return model; }
            set { SetProperty(ref model, value); }
        }
        private int selectedIndex;
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set { SetProperty(ref selectedIndex, value); }
        }
        private double allowableRangeDeviation;
        public double AllowableRangeDeviation
        {
            get { return allowableRangeDeviation; }
            set { SetProperty(ref allowableRangeDeviation, value); }
        }
        private string[] type = { "15K传感器10K串联电阻", "20K传感器10K串联电阻", "50K传感器5.1K串联电阻", "15K传感器20K串联电阻", "20K传感器20K串联电阻", "50K传感器20K串联电阻" };
        public string[] Types
        {
            get { return type; }
            set { SetProperty(ref type, value); }
        }
        private int indoorPrarmIndex;
        public int IndoorPrarmIndex
        {
            get { return indoorPrarmIndex; }
            set { SetProperty(ref indoorPrarmIndex, value); }
        }
        private string[] castes = { "5V电源", "3.3V电源" };
        public string[] Castes
        {
            get { return castes; }
            set { SetProperty(ref castes, value); }
        }
        private int outdoorIndex;
        public int OutdoorIndex
        {
            get { return outdoorIndex; }
            set { SetProperty(ref outdoorIndex, value); }
        }
        private int outdoorPrarmIndex;
        public int OutdoorPrarmIndex
        {
            get { return outdoorPrarmIndex; }
            set { SetProperty(ref outdoorPrarmIndex, value); }
        }
        public CheckRegisterViewModel(ISqlSugarClient sqlSugarClient)
        {
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new DelegateCommand(Cancel);
            IndoorCount = new List<int>();
            OutdoorCount=new List<int>();
            this.sqlSugarClient = sqlSugarClient;
        }

        private void Cancel()
        {
            if (DialogHost.IsDialogOpen(DialogHostName))
                DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.No)); //取消返回NO告诉操作结束
        }

        private void Save()
        {
            Model.AllowableRangeDeviation =Convert.ToUInt16(AllowableRangeDeviation*10);
            Model.Name = Name;
            if (DialogHost.IsDialogOpen(DialogHostName))
            {
                //确定时,把编辑的实体返回并且返回OK
                DialogParameters param = new DialogParameters();
                if (SelectedIndex==0)
                {
                    Model.IsEnable = false;
                }
                else
                {
                    Model.IsEnable = true;
                }
                param.Add("Value", Model);
                DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.OK, param));
            }
        }
        private DelegateCommand<int?> selectedindoorCommand;
        private readonly ISqlSugarClient sqlSugarClient;

        public DelegateCommand<int?> SelectedindoorCommand =>
            selectedindoorCommand ?? (selectedindoorCommand = new DelegateCommand<int?>(ExecuteSelectedindoorCommand));

        async void ExecuteSelectedindoorCommand(int? parameter)
        {
            if (parameter == null) return;
            IndoorPrarm =await sqlSugarClient.Queryable<Register>().Where(x => x.StationNum == Model.StationNum && x.RegisterType == "内机数据" && x.Name.Contains($"-{parameter}") && x.Name.Contains("温")).ToListAsync();
        }
        private DelegateCommand<int?> selectedoutdoorCommand;
        public DelegateCommand<int?> SelectedoutdoorCommand =>
            selectedoutdoorCommand ?? (selectedoutdoorCommand = new DelegateCommand<int?>(ExecuteSelectedoutdoorCommand));

        async void ExecuteSelectedoutdoorCommand(int? parameter)
        {
            if (parameter == null) return;
            OutdoorPrarm = await sqlSugarClient.Queryable<Register>().Where(x => x.StationNum == Model.StationNum && x.RegisterType == "外机数据" && x.Name.Contains($"-{parameter}") && x.Name.Contains("温")).ToListAsync();

        }
        private DelegateCommand<Register> selectedindoorPrarmCommand;
        public DelegateCommand<Register> SelectedindoorPrarmCommand =>
            selectedindoorPrarmCommand ?? (selectedindoorPrarmCommand = new DelegateCommand<Register>(ExecuteSelectedindoorPrarmCommand));

        void ExecuteSelectedindoorPrarmCommand(Register parameter)
        {
            if (parameter == null) return;
            Model.AccessAddress = parameter.Address;
            Name="工装板"+parameter.Name;
        }
        private DelegateCommand<Register> selectedoutdoorPrarmCommand;
        public DelegateCommand<Register> SelectedoutdoorPrarmCommand =>
            selectedoutdoorPrarmCommand ?? (selectedoutdoorPrarmCommand = new DelegateCommand<Register>(ExecuteSelectedoutdoorPrarmCommand));

        void ExecuteSelectedoutdoorPrarmCommand(Register parameter)
        {
            if (parameter == null) return;
            Model.AccessAddress = parameter.Address;
            Name= "工装板"+parameter.Name;
        }
        public async void OnDialogOpend(IDialogParameters parameters)
        {
            IndoorIndex = -1;
            OutdoorIndex = -1;
            IndoorCount.Clear();
            OutdoorCount.Clear();
            for (int i = 1; i <= App.IndoorCount; i++)
            {
                IndoorCount.Add(i);
            }
            for (int i = 1; i <= App.OutdoorCount; i++)
            {
                OutdoorCount.Add(i);
            }
            Model = parameters.GetValue<RegisterDto>("Value");
            AllowableRangeDeviation = Model.AllowableRangeDeviation / 10.0;
            Name = Model.Name;
            if (Model.RegisterType == "20个温度设置")
            {
                Visibility = Visibility.Visible;
                var register = await sqlSugarClient.Queryable<Register>().FirstAsync(x => x.StationNum == Model.StationNum && x.Address == Model.AccessAddress&&x.RegisterType.Contains("数据"));
                if (register.RegisterType == "内机数据")
                {
                    var s = register.Name.Substring(register.Name.IndexOf("-") + 1, register.Name.Length - register.Name.IndexOf("-") - 1);
                    IndoorIndex = Convert.ToInt16(s) - 1;
                    IndoorPrarm = await sqlSugarClient.Queryable<Register>().Where(x => x.StationNum == Model.StationNum && x.RegisterType == "内机数据" && x.Name.Contains($"-{s}") && x.Name.Contains("温")).ToListAsync();
                    for (int i = 0; i < IndoorPrarm.Count; i++)
                    {
                        if (IndoorPrarm[i].Name == register.Name)
                        {
                            IndoorPrarmIndex = i;
                        }
                    }
                }
                if (register.RegisterType == "外机数据")
                {
                    var s = register.Name.Substring(register.Name.IndexOf("-") + 1, register.Name.Length - register.Name.IndexOf("-") - 1);
                    OutdoorIndex = Convert.ToInt16(s) - 1;
                    OutdoorPrarm = await sqlSugarClient.Queryable<Register>().Where(x => x.StationNum == Model.StationNum && x.RegisterType == "外机数据" && x.Name.Contains($"-{s}") && x.Name.Contains("温")).ToListAsync();
                    for (int i = 0; i < OutdoorPrarm.Count; i++)
                    {
                        if (OutdoorPrarm[i].Name == register.Name)
                        {
                            OutdoorPrarmIndex = i;
                        }
                    }
                }
            }
            else
            {
                Visibility = Visibility.Collapsed;
            }
            if (Model.IsEnable)
            {
                SelectedIndex = 1;
            }
            else
            {
                SelectedIndex = 0;
            }
        }
    }
}
