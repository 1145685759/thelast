using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Text;
using TheLast.Common;
using TheLast.Dtos;

namespace TheLast.ViewModels
{
    public class CheckRegisterViewModel : BindableBase, IDialogHostAware
    {
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
