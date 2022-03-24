using AutoMapper;
using Modbus.Device;
using Prism.Commands;
using Prism.Ioc;
using Prism.Regions;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using TheLast.Common;
using TheLast.Dtos;
using TheLast.Entities;
using TheLast.Extensions;

namespace TheLast.ViewModels
{
    public class ProjectManagerViewModel: NavigationViewModel
    {
        private readonly IDialogHostService dialogHost;
        private readonly ISqlSugarClient sqlSugarClient;
        private readonly IMapper mapper;
        private readonly IRegionManager regionManager;
        public ModbusSerialMaster ModbusSerialMaster { get; set; }
        private IRegionNavigationJournal journal;
        public ProjectManagerViewModel( IContainerProvider provider,ISqlSugarClient sqlSugarClient,IMapper mapper,IRegionManager regionManager)
           : base(provider)
        {
            ProjectDtos = new ObservableCollection<ProjectDto>();
            ExecuteCommand = new DelegateCommand<string>(Execute);
            CheckModuleCommand = new DelegateCommand<ProjectDto>(CheckModule);
            SelectedCommand = new DelegateCommand<ProjectDto>(Selected);
            DeleteCommand = new DelegateCommand<ProjectDto>(Delete);
            AutomaticTestingCommand = new DelegateCommand<ProjectDto>(AutomaticTesting);
            dialogHost = provider.Resolve<IDialogHostService>();
            this.sqlSugarClient = sqlSugarClient;
            this.mapper = mapper;
            this.regionManager = regionManager;
        }

        private void AutomaticTesting(ProjectDto obj)
        {
            NavigationParameters keyValuePairs = new NavigationParameters();
            keyValuePairs.Add("Project", obj);
            keyValuePairs.Add("Master", ModbusSerialMaster);
            regionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate("AutomaticTestingView", back =>
            {
                journal = back.Context.NavigationService.Journal;
            }, keyValuePairs);
        }

        private void CheckModule(ProjectDto obj)
        {
            NavigationParameters keyValuePairs = new NavigationParameters();
            keyValuePairs.Add("Project", obj);
            regionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate("ModuleView", back =>
            {
                journal = back.Context.NavigationService.Journal;
            }, keyValuePairs);
        }

        private async void Delete(ProjectDto obj)
        {
            try
            {
                var dialogResult = await dialogHost.Question("温馨提示", $"确认删除待办事项:{obj.ProjectName} ?");
                if (dialogResult.Result != Prism.Services.Dialogs.ButtonResult.OK) return;

                UpdateLoading(true);
                var deleteResult=await sqlSugarClient.Deleteable<Project>().In(obj.Id).ExecuteCommandAsync();
                if (deleteResult>0)
                {
                    var model = ProjectDtos.FirstOrDefault(t => t.Id.Equals(obj.Id));
                    if (model != null)
                        ProjectDtos.Remove(model);
                }
            }
            finally
            {
                UpdateLoading(false);
            }
        }

        private async void Selected(ProjectDto obj)
        {
            try
            {
                UpdateLoading(true);
                var todoResult=await sqlSugarClient.Queryable<Project>().InSingleAsync(obj.Id);
                if (todoResult!=null)
                {
                    CurrentDto = mapper.Map<ProjectDto>(todoResult);
                    IsRightDrawerOpen = true;
                }
            }
            catch (Exception )
            {

            }
            finally
            {
                UpdateLoading(false);
            }
        }

        private void Execute(string obj)
        {
            switch (obj)
            {
                case "新增": Add(); break;
                case "查询": GetDataAsync(); break;
                case "保存": Save(); break;
            }
        }
        private int selectedIndex;

        /// <summary>
        /// 下拉列表选中状态值
        /// </summary>
        public int SelectedIndex
        {
            get { return selectedIndex; }
            set { selectedIndex = value; RaisePropertyChanged(); }
        }
        private string search;

        /// <summary>
        /// 搜索条件
        /// </summary>
        public string Search
        {
            get { return search; }
            set { search = value; RaisePropertyChanged(); }
        }
        private bool isRightDrawerOpen;

        /// <summary>
        /// 右侧编辑窗口是否展开
        /// </summary>
        public bool IsRightDrawerOpen
        {
            get { return isRightDrawerOpen; }
            set { isRightDrawerOpen = value; RaisePropertyChanged(); }
        }
        private ProjectDto currentDto;

        /// <summary>
        /// 编辑选中/新增时对象
        /// </summary>
        public ProjectDto CurrentDto
        {
            get { return currentDto; }
            set { currentDto = value; RaisePropertyChanged(); }
        }
        /// <summary>
        /// 添加待办
        /// </summary>
        private void Add()
        {
            CurrentDto = new ProjectDto();
            IsRightDrawerOpen = true;
        }
        private async void Save()
        {
            if (string.IsNullOrWhiteSpace(CurrentDto.ProjectName))
                return;

            UpdateLoading(true);

            try
            {
                if (CurrentDto.Id > 0)
                {
                    var updateResult=await sqlSugarClient.Updateable(mapper.Map<Project>(CurrentDto)).ExecuteCommandAsync();
                    if (updateResult>0)
                    {
                        var project = ProjectDtos.FirstOrDefault(t => t.Id == CurrentDto.Id);
                        if (project != null)
                        {
                            project.ProjectName = CurrentDto.ProjectName;
                            project.ProjectDescribe = CurrentDto.ProjectDescribe;
                            project.IsComplete = CurrentDto.IsComplete;
                        }
                    }
                    IsRightDrawerOpen = false;
                }
                else
                {
                    var addResult = await sqlSugarClient.Insertable(mapper.Map<Project>(CurrentDto)).ExecuteReturnEntityAsync();
                    if (addResult!=null)
                    {
                        ProjectDtos.Add(mapper.Map<ProjectDto>(addResult));
                        IsRightDrawerOpen = false;
                    }
                }
            }
            catch (Exception)
            {

            }
            finally
            {
                UpdateLoading(false);
            }
        }
        public DelegateCommand<string> ExecuteCommand { get; private set; }
        public DelegateCommand<ProjectDto> SelectedCommand { get; private set; }
        public DelegateCommand<ProjectDto> CheckModuleCommand { get; private set; }
        public DelegateCommand<ProjectDto> AutomaticTestingCommand { get; private set; }
        public DelegateCommand<ProjectDto> DeleteCommand { get; private set; }

        private ObservableCollection<ProjectDto> projectDtos;
        public ObservableCollection<ProjectDto> ProjectDtos
        {
            get { return projectDtos; }
            set { projectDtos = value; RaisePropertyChanged(); }
        }
        private async void GetDataAsync()
        {
            UpdateLoading(true);
            int? Status=null;
            if (SelectedIndex==0)
            {
                Status = null;
            }
            if (SelectedIndex==2)
            {
                Status = 1;
            }
            if (SelectedIndex == 1)
            {
                Status = 0;
            }
            if (string.IsNullOrEmpty(Search)&&Status==0)
            {
                var list= await sqlSugarClient.Queryable<Project>().Where(x=>x.IsComplete==Status).ToListAsync();
                ProjectDtos.Clear();
                foreach (var item in list)
                {
                    ProjectDtos.Add(mapper.Map<ProjectDto>(item));
                }
            }
            if (string.IsNullOrEmpty(Search) && Status==1)
            {
                var list = await sqlSugarClient.Queryable<Project>().Where(x=>x.IsComplete==Status).ToListAsync();
                ProjectDtos.Clear();
                foreach (var item in list)
                {
                    ProjectDtos.Add(mapper.Map<ProjectDto>(item));
                }
            }
            if (!string.IsNullOrEmpty(Search)&&Status==null)
            {
                var list = await sqlSugarClient.Queryable<Project>().Where(x => x.ProjectName.Contains(Search)).ToListAsync();
                ProjectDtos.Clear();
                foreach (var item in list)
                {
                    ProjectDtos.Add(mapper.Map<ProjectDto>(item));
                }
            }
            if (string.IsNullOrEmpty(Search)&&Status==null)
            {
                var todoResult = await sqlSugarClient.Queryable<Project>().ToListAsync();

                if (todoResult.Count > 0)
                {
                    ProjectDtos.Clear();
                    foreach (var item in todoResult)
                    {
                        ProjectDtos.Add(mapper.Map<ProjectDto>(item));
                    }
                }
            }
            if (!string.IsNullOrEmpty(Search)&&Status!=null)
            {
                var list = await sqlSugarClient.Queryable<Project>().Where(x => x.ProjectName.Contains(Search)&&x.IsComplete==Status).ToListAsync();
                ProjectDtos.Clear();
                foreach (var item in list)
                {
                    ProjectDtos.Add(mapper.Map<ProjectDto>(item));
                }
            }
            
            UpdateLoading(false);
        }
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
            if (navigationContext.Parameters.GetValue<ModbusSerialMaster>("Master")!=null)
            {
                ModbusSerialMaster = navigationContext.Parameters.GetValue<ModbusSerialMaster>("Master");
            }
            if (navigationContext.Parameters.ContainsKey("Value"))
                SelectedIndex = navigationContext.Parameters.GetValue<int>("Value");
            else
                SelectedIndex = 0;
            GetDataAsync();
        }

    }
}
