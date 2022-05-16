using AutoMapper;
using MaterialDesignExtensions.Controls;
using Prism.Commands;
using Prism.Ioc;
using Prism.Mvvm;
using System.Linq;
using Prism.Regions;
using Prism.Services.Dialogs;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using TheLast.Common;
using TheLast.Dtos;
using TheLast.Entities;
using TheLast.Extensions;

namespace TheLast.ViewModels
{
    public class BasicParametersViewModel: NavigationViewModel
    {

        private readonly IDialogHostService dialog;
        private string GetType;
        private ObservableCollection<RegisterDto> registerDtos;
        public ObservableCollection<RegisterDto> RegisterDtos
        {
            get { return registerDtos; }
            set { SetProperty(ref registerDtos, value); }
        }
        private string searchString;
        public string SearchString
        {
            get { return searchString; }
            set { SetProperty(ref searchString, value); }
        }
        private Visibility isIndoor;
        public Visibility IsIndoor
        {
            get { return isIndoor; }
            set { SetProperty(ref isIndoor, value); }
        }
        private Visibility isOutdoor;
        public Visibility IsOutdoor
        {
            get { return isOutdoor; }
            set { SetProperty(ref isOutdoor, value); }
        }
        private bool open;
        public bool Open
        {
            get { return open; }
            set { SetProperty(ref open, value); }
        }
        private List<int> indoors;
        public List<int> Indoors
        {
            get { return indoors; }
            set { SetProperty(ref indoors, value); }
        }
        private List<int> outdoors;
        public List<int> Outdoors
        {
            get { return outdoors; }
            set { SetProperty(ref outdoors, value); }
        }
        public BasicParametersViewModel(ISqlSugarClient sqlSugarClient,IMapper mapper, IDialogHostService dialog, IContainerProvider provider) : base(provider)
        {
            RegisterDtos = new ObservableCollection<RegisterDto>();
            this.sqlSugarClient = sqlSugarClient;
            this.mapper = mapper;
            this.dialog = dialog;
            Indoors=new List<int>();
            Outdoors = new List<int>();


        }
        
        private DelegateCommand search;
        private readonly ISqlSugarClient sqlSugarClient;
        private readonly IMapper mapper;

        public DelegateCommand Search =>
            search ?? (search = new DelegateCommand(ExecuteSearch));


        public override async void OnNavigatedTo(NavigationContext navigationContext)
        {
            Indoors.Clear();
            Outdoors.Clear();
            for (int i = 1; i <= App.IndoorCount; i++)
            {
                Indoors.Add(i);
            }
            for (int i = 1; i <= App.OutdoorCount; i++)
            {
                Outdoors.Add(i);
            }
            GetType =navigationContext.Parameters.GetValue<string>("Type");
            if (GetType.Contains("配置"))
            {
                GetType = GetType.Replace("配置", "");
            }
            if (GetType=="内机数据")
            {
                
                IsOutdoor = Visibility.Collapsed;
                IsIndoor = Visibility.Visible;
            }
            else if (GetType == "外机数据")
            {
                IsIndoor = Visibility.Collapsed;
                IsOutdoor = Visibility.Visible;
            }
            else
            {
                IsOutdoor=Visibility.Collapsed;
                IsIndoor = Visibility.Collapsed;
                var registers = await sqlSugarClient.Queryable<Register>().Where(x => x.RegisterType == GetType).ToListAsync();
                RegisterDtos.Clear();
                RegisterDtos.AddRange(mapper.Map<List<RegisterDto>>(registers));
            }
           
        }

        async void ExecuteSearch( )
        {
            UpdateLoading(true);
            if (!string.IsNullOrEmpty(SearchString))
            {
                var list= await sqlSugarClient.Queryable<Register>().Where(x => x.Name.Contains(searchString)&&x.RegisterType==GetType).ToListAsync();
                if (list.Count>0)
                {
                    RegisterDtos.Clear();
                    foreach (var item in list)
                    {
                        RegisterDtos.Add(mapper.Map<RegisterDto>(item));
                    }
                } 
            }
            else
            {
                var registers = await sqlSugarClient.Queryable<Register>().Where(x => x.RegisterType == GetType).ToListAsync();
                RegisterDtos.Clear();
                RegisterDtos.AddRange(mapper.Map<List<RegisterDto>>(registers));
            }
            UpdateLoading(false);
        }
        private DelegateCommand<int?> selectedIndoorCommand;
        public DelegateCommand<int?> SelectedIndoorCommand =>
            selectedIndoorCommand ?? (selectedIndoorCommand = new DelegateCommand<int?>(ExecuteSelectedIndoorCommand));

        async void ExecuteSelectedIndoorCommand(int? num)
        {
            var registers= await sqlSugarClient.Queryable<Register>().Where(x => x.IsEnable == true && x.RegisterType == "内机数据" && x.Name.Contains($"-{num}")).ToListAsync();
            for (int i = 0; i < registers.Count; i++)
            {
                var d = int.Parse(registers[i].Name.Substring(registers[i].Name.IndexOf("-") + 1));
                if (d != num)
                {
                    registers.Remove(registers[i]);
                }

            }
            RegisterDtos.Clear();
            RegisterDtos.AddRange(mapper.Map<List<RegisterDto>>(registers));
        }

        private DelegateCommand<int?> selectedOutdoorCommand;
        public DelegateCommand<int?> SelectedOutdoorCommand =>
            selectedOutdoorCommand ?? (selectedOutdoorCommand = new DelegateCommand<int?>(ExecuteSelectedOutdoorCommand));

        async void ExecuteSelectedOutdoorCommand(int? num)
        {
            var registers = await sqlSugarClient.Queryable<Register>().Where(x => x.IsEnable == true && x.RegisterType == "外机数据" && x.Name.Contains($"-{num}")).ToListAsync();
            for (int i = 0; i < registers.Count; i++)
            {
                var d = int.Parse(registers[i].Name.Substring(registers[i].Name.IndexOf("-") + 1));
                if (d != num)
                {
                    registers.Remove(registers[i]);
                }

            }
            RegisterDtos.Clear();
            RegisterDtos.AddRange(mapper.Map<List<RegisterDto>>(registers));
        }


        private DelegateCommand addRegister;
        public DelegateCommand AddRegister =>
            addRegister ?? (addRegister = new DelegateCommand(ExecuteAddRegisterAsync));

        async void ExecuteAddRegisterAsync()
        {
            DialogParameters param = new DialogParameters();
            var model = new RegisterDto();
            model.RegisterType =GetType;
            int count= await sqlSugarClient.Queryable<Register>().CountAsync(x => x.RegisterType == GetType);
            int maxCount= (await sqlSugarClient.Queryable<RegisterCount>().FirstAsync(x => x.Type == GetType)).Count;
            if (count>=maxCount)
            {
                HandyControl.Controls.Growl.Error("添加失败，该类型寄存器已达上限");
                return;
            }
            param.Add("Value", model);
            var dialogResult = await dialog.ShowDialog("AddRegister", param);
            if (dialogResult.Result == ButtonResult.OK)
            {
                try
                {
                    UpdateLoading(true);
                    var registerdto = dialogResult.Parameters.GetValue<RegisterDto>("Value");
                    if (registerdto!=null)
                    {
                        int id= await sqlSugarClient.Insertable(mapper.Map<Register>(registerdto)).ExecuteReturnIdentityAsync();
                        registerdto.Id = id;
                        RegisterDtos.Add(registerdto);
                    }
                }
                finally
                {
                    UpdateLoading(false);
                }
            }
        }
        private DelegateCommand<RegisterDto> check;
        public DelegateCommand<RegisterDto> Check =>
            check ?? (check = new DelegateCommand<RegisterDto>(ExecuteCheck));

        async void ExecuteCheck(RegisterDto registerDto)
        {
            DialogParameters param = new DialogParameters();
            param.Add("Value", registerDto);
            var dialogResult = await dialog.ShowDialog("CheckRegister", param);
            if (dialogResult.Result == ButtonResult.OK)
            {
                try
                {
                    UpdateLoading(true);
                    var registerdto = dialogResult.Parameters.GetValue<RegisterDto>("Value");
                    if (registerdto != null)
                    {
                        await sqlSugarClient.Updateable(mapper.Map<Register>(registerdto)).ExecuteCommandAsync();
                        registerDto.IsEnable=registerdto.IsEnable;
                        registerDto.Name = registerdto.Name;
                        registerDto.Address = registerdto.Address;
                    }
                }
                finally
                {
                    UpdateLoading(false);
                }
            }
        }
        private DelegateCommand<RegisterDto> deleted;
        public DelegateCommand<RegisterDto> Deleted =>
            deleted ?? (deleted = new DelegateCommand<RegisterDto>(ExecuteDeleted));

        async void ExecuteDeleted(RegisterDto parameter)
        {
            try
            {
                var dialogResult = await dialog.Question("温馨提示", $"确认删除寄存器:{parameter.Name} ?");
                if (dialogResult.Result != ButtonResult.OK) return;
                UpdateLoading(true);
                var deleteResult= await sqlSugarClient.Deleteable<Register>().In(parameter.Id).ExecuteCommandAsync();
                if (deleteResult==1)
                {
                    var model = RegisterDtos.FirstOrDefault(t => t.Id.Equals(parameter.Id));
                    if (model != null)
                        RegisterDtos.Remove(model);
                }
            }
            finally
            {
                UpdateLoading(false);
            }
        }

    }
}
