using AutoMapper;
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
    public class AddRegisterViewModel : BindableBase, IDialogHostAware
    {
        private readonly ISqlSugarClient sqlSugarClient;
        private readonly IMapper mapper;
        public AddRegisterViewModel(ISqlSugarClient sqlSugarClient, IMapper mapper)
        {
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new DelegateCommand(Cancel);
            this.sqlSugarClient = sqlSugarClient;
            this.mapper = mapper;
        }
        private string[] type= { "15K传感器", "20K传感器" };
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
        private RegisterDto model;

        public RegisterDto Model
        {
            get { return model; }
            set { model = value; RaisePropertyChanged(); }
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
                param.Add("Value", Model);
                DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.OK, param));
            }
        }
        private Visibility visibility;
        public Visibility Visibility
        {
            get { return visibility; }
            set { SetProperty(ref visibility, value); }
        }
        public string DialogHostName { get; set; }
        public DelegateCommand SaveCommand { get ; set ; }
        public DelegateCommand CancelCommand { get ; set ; }

        public void OnDialogOpend(IDialogParameters parameters)
        {
            if (parameters.ContainsKey("Value"))
            {
                Model = parameters.GetValue<RegisterDto>("Value");
                if (Model.RegisterType=="模拟量输出")
                {
                    Visibility = Visibility.Visible;
                }
                else
                {
                    Visibility = Visibility.Collapsed;
                }
            }
            else
                Model = new RegisterDto();
        }
    }
}
