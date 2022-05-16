using AutoMapper;
using MaterialDesignThemes.Wpf;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using SqlSugar;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using TheLast.Common;
using TheLast.Dtos;
using TheLast.Entities;

namespace TheLast.ViewModels
{
    public class JumpConfigViewModel : BindableBase, IDialogHostAware
    {

        private int moduleId;
        public List<Register> Registers { get; set; }
        public string DialogHostName { get; set; }
        public DelegateCommand SaveCommand { get; set; }
        public DelegateCommand CancelCommand { get; set; }
        private List<RegisterDto> feedBackRegisterDto;
        private readonly ISqlSugarClient sqlSugarClient;
        private List<string> registerNames;
        public List<string> RegisterNames
        {
            get { return registerNames; }
            set { SetProperty(ref registerNames, value); }
        }
        private ObservableCollection<Register> jumpRegister;
        public ObservableCollection<Register> JumpRegister
        {
            get { return jumpRegister; }
            set { SetProperty(ref jumpRegister, value); }
        }
        private List<string> registerTypes;
        public List<string> RegisterTypes
        {
            get { return registerTypes; }
            set { SetProperty(ref registerTypes, value); }
        }
        private List<byte> stationNum;
        public List<byte> StationNum
        {
            get { return stationNum; }
            set { SetProperty(ref stationNum, value); }
        }
        public List<RegisterDto> FeedBackRegisterDto
        {
            get { return feedBackRegisterDto; }
            set { SetProperty(ref feedBackRegisterDto, value); }
        }

        public JumpConfigViewModel(IMapper mapper, ISqlSugarClient sqlSugarClient)
        {
            StationNum = new List<byte>();
            RegisterTypes = new List<string>();
            JumpRegister = new ObservableCollection<Register>();
            RegisterNames = new List<string>();
            Registers = new List<Register>();
            SaveCommand = new DelegateCommand(Save);
            CancelCommand = new DelegateCommand(Cancel);
            this.sqlSugarClient = sqlSugarClient;
        }

        private void Cancel()
        {
            if (DialogHost.IsDialogOpen(DialogHostName))
                DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.No)); //取消返回NO告诉操作结束
        }

        private async void Save()
        {
            var s= await sqlSugarClient.Queryable<TestStep>().Where(x => x.ModuleId == moduleId).ToListAsync();
            foreach (var item in s)
            {
                var x= await sqlSugarClient.Queryable<FeedBack>().Where(x => x.TestStepId == item.Id).ToListAsync();
                if (x.Count!=0)
                {
                    foreach (var feedBack in x)
                    {
                        Register f = await sqlSugarClient.Queryable<Register>().FirstAsync(x => x.Id == feedBack.RegisterId);
                        foreach (var register in JumpRegister)
                        {
                            if (register.Id==f.Id)
                            {
                                feedBack.IsJump = true;
                                await sqlSugarClient.Updateable(feedBack).ExecuteCommandAsync();
                            }
                            else
                            {
                                feedBack.IsJump = false;
                                await sqlSugarClient.Updateable(feedBack).ExecuteCommandAsync();
                            }
                        }
                        
                    }
                }
            }
            if (DialogHost.IsDialogOpen(DialogHostName))
            {
                //确定时,把编辑的实体返回并且返回OK;
                DialogHost.Close(DialogHostName, new DialogResult(ButtonResult.OK));
            }

        }

        public async void OnDialogOpend(IDialogParameters parameters)
        {
            List<int> s = new List<int>();
            moduleId=parameters.GetValue<int>("ModuleId");
            var teststepsx= await sqlSugarClient.Queryable<TestStep>().Where(x => x.ModuleId == moduleId).ToListAsync();
            foreach (var item in teststepsx)
            {
                var feedbackes= await sqlSugarClient.Queryable<FeedBack>().Where(x => x.TestStepId == item.Id).ToListAsync();
                foreach (var RegistersId in feedbackes)
                {
                    if (s.Count==0)
                    {
                        var id = await sqlSugarClient.Queryable<Register>().FirstAsync(x => x.Id == RegistersId.RegisterId);
                        if (!s.Contains(id.Id))
                        {
                            s.Add(id.Id);
                        }
                        
                    }
                    else
                    {
                        for (int i = 0; i < s.Count; i++)
                        {
                            if (s[i]!= (await sqlSugarClient.Queryable<Register>().FirstAsync(x => x.Id == RegistersId.RegisterId)).Id)
                            {
                                var id = await sqlSugarClient.Queryable<Register>().FirstAsync(x => x.Id == RegistersId.RegisterId);
                                if (!s.Contains(id.Id))
                                {
                                    s.Add(id.Id);
                                }
                            }
                        }
                    }
                }
                
            }
            
            foreach (var item in s)
            {
                Registers.Add(await sqlSugarClient.Queryable<Register>().FirstAsync(x => x.Id == item));
            }
            StationNum =Registers.Where(x=>x!=null).Select(x => x.StationNum).Distinct().ToList();
            var teststeps= await sqlSugarClient.Queryable<TestStep>().Where(x => x.ModuleId == moduleId).ToListAsync();
            foreach (var item in teststeps)
            {
                var feedbacks = await sqlSugarClient.Queryable<FeedBack>().Where(x => x.IsJump == true && x.TestStepId == item.Id).ToListAsync();
                if (feedbacks.Count>0)
                {
                    foreach (var feedBack in feedbacks)
                    {
                        var register = await sqlSugarClient.Queryable<Register>().FirstAsync(x => x.Id == feedBack.RegisterId);
                        if (jumpRegister.Count==0)
                        {
                            JumpRegister.Add(register);
                        }
                        else
                        {
                            foreach (var jump in JumpRegister)
                            {
                                if (jump.Id != register.Id)
                                {
                                    JumpRegister.Add(register);
                                }
                            }
                        }
                        
                        
                    }
                }
            }
            
        }
        private DelegateCommand<Register> delete;
        public DelegateCommand<Register> Delete =>
            delete ?? (delete = new DelegateCommand<Register>(ExecuteDelete));

        async void ExecuteDelete(Register parameter)
        {
            var s = await sqlSugarClient.Queryable<TestStep>().Where(x => x.ModuleId == moduleId).ToListAsync();
            foreach (var item in s)
            {
                var x = await sqlSugarClient.Queryable<FeedBack>().Where(x => x.TestStepId == item.Id).ToListAsync();
                if (x.Count != 0)
                {
                    foreach (var feedBack in x)
                    {
                        if (feedBack.RegisterId == parameter.Id)
                        {
                            feedBack.IsJump = false;
                            
                            await sqlSugarClient.Updateable(feedBack).ExecuteCommandAsync();
                        }
                    }
                }

            }
            JumpRegister.Remove(parameter);
        }
        private DelegateCommand<byte?> stationSelectedCommand;
        public DelegateCommand<byte?> StationSelectedCommand =>
            stationSelectedCommand ?? (stationSelectedCommand = new DelegateCommand<byte?>(ExecuteStationSelectedCommand));

        async void ExecuteStationSelectedCommand(byte? parameter)
        {
            if (parameter!=null)
            {
                var t= await sqlSugarClient.Queryable<TestStep>().Where(x => x.ModuleId == moduleId).ToListAsync();
                foreach (var item in t)
                {
                   var registerid= await sqlSugarClient.Queryable<FeedBack>().Where(x => x.TestStepId == item.Id).Select(x=>x.RegisterId).ToListAsync();
                    foreach (var register in registerid)
                    {
                        if (RegisterTypes.Count==0)
                        {
                            RegisterTypes.Add((await sqlSugarClient.Queryable<Register>().FirstAsync(x => x.Id == register)).RegisterType);
                        }
                        else
                        {
                            for (int i = 0; i < RegisterTypes.Count; i++)
                            {
                                if (RegisterTypes[i]== (await sqlSugarClient.Queryable<Register>().FirstAsync(x => x.Id == register)).RegisterType)
                                {
                                    continue;
                                }
                                RegisterTypes.Add((await sqlSugarClient.Queryable<Register>().FirstAsync(x => x.Id == register)).RegisterType);
                            }
                        }
                        RegisterTypes=RegisterTypes.Distinct().ToList();
                        
                    }
                }
            }
            
            
        }
        private DelegateCommand<string?> selectedCommand;
        public DelegateCommand<string?> SelectedCommand =>
            selectedCommand ?? (selectedCommand = new DelegateCommand<string?>(ExecuteSelectedCommand));

        void ExecuteSelectedCommand(string? parameter)
        {
            if (parameter != null)
            {
                RegisterNames = Registers.Where(s => s != null && s.RegisterType == parameter).Select(x => x.Name).Distinct().ToList();
            }
        }
        private DelegateCommand<string> add;
        public DelegateCommand<string> Add =>
            add ?? (add = new DelegateCommand<string>(ExecuteAdd));

        async void ExecuteAdd(string parameter)
        {
            JumpRegister.Add(await sqlSugarClient.Queryable<Register>().FirstAsync(x => x.Name == parameter));
        }
    }
}