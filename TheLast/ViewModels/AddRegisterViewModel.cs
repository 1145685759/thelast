using AutoMapper;
using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;
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
        private RegisterDto model;
        

        //public event Action<IDialogResult> RequestClose;

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

        public string DialogHostName { get; set; }
        public DelegateCommand SaveCommand { get ; set ; }
        public DelegateCommand CancelCommand { get ; set ; }

        public void OnDialogOpend(IDialogParameters parameters)
        {
            if (parameters.ContainsKey("Value"))
            {
                Model = parameters.GetValue<RegisterDto>("Value");
            }
            else
                Model = new RegisterDto();
        }
    }
}
