using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using TheLast.Common;
using TheLast.Dtos;

namespace TheLast.ViewModels
{
    public class CheckRegisterViewModel : BindableBase, IDialogHostAware
    {
        private Visibility visibility;
        public Visibility Visibility
        {
            get { return visibility; }
            set { SetProperty(ref visibility, value); }
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
        private string[] type = { "15K传感器", "20K传感器" };
        public string[] Types
        {
            get { return type; }
            set { SetProperty(ref type, value); }
        }
        private string[] castes = { "5V电源", "3.3V电源" };
        public string[] Castes
        {
            get { return castes; }
            set { SetProperty(ref castes, value); }
        }
        public CheckRegisterViewModel()
        {
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new DelegateCommand(Cancel);
        }

        private void Cancel()
        {
            if (DialogHost.IsDialogOpen(DialogHostName))
                DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.No)); //取消返回NO告诉操作结束
        }

        private void Save()
        {
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

        public void OnDialogOpend(IDialogParameters parameters)
        {
            Model = parameters.GetValue<RegisterDto>("Value");
            if (Model.RegisterType == "20个温度设置")
            {
                Visibility = Visibility.Visible;
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
