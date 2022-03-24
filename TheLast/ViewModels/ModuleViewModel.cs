using AutoMapper;
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
    public class ModuleViewModel: NavigationViewModel
    {
        private readonly IDialogHostService dialogHost;
        private readonly ISqlSugarClient sqlSugarClient;
        private readonly IMapper mapper;
        private readonly IRegionManager regionManager;
        private IRegionNavigationJournal journal;
        private ProjectDto projectDto;
        public ProjectDto ProjectDto
        {
            get { return projectDto; }
            set { SetProperty(ref projectDto, value); }
        }
        public ModuleViewModel(IContainerProvider provider, ISqlSugarClient sqlSugarClient, IMapper mapper, IRegionManager regionManager):base(provider)
        {
            ModuleDtos = new ObservableCollection<ModuleDto>();
            ExecuteCommand = new DelegateCommand<string>(Execute);
            CheckTestStepCommand = new DelegateCommand<ModuleDto>(CheckTestStep);
            SelectedCommand = new DelegateCommand<ModuleDto>(Selected);
            DeleteCommand = new DelegateCommand<ModuleDto>(Delete);
            dialogHost = provider.Resolve<IDialogHostService>();
            this.sqlSugarClient = sqlSugarClient;
            this.mapper = mapper;
            this.regionManager = regionManager;
        }

        private async void Delete(ModuleDto obj)
        {
            try
            {
                var dialogResult = await dialogHost.Question("温馨提示", $"确认删除待办事项:{obj.ModuleName} ?");
                if (dialogResult.Result != Prism.Services.Dialogs.ButtonResult.OK) return;

                UpdateLoading(true);
                var deleteResult = await sqlSugarClient.Deleteable<Module>().In(obj.Id).ExecuteCommandAsync();
                if (deleteResult > 0)
                {
                    var model = ModuleDtos.FirstOrDefault(t => t.Id.Equals(obj.Id));
                    if (model != null)
                        ModuleDtos.Remove(model);
                }
            }
            finally
            {
                UpdateLoading(false);
            }
        }

        private async void Selected(ModuleDto obj)
        {
            try
            {
                UpdateLoading(true);
                var todoResult = await sqlSugarClient.Queryable<Module>().InSingleAsync(obj.Id);
                if (todoResult != null)
                {
                    CurrentDto = mapper.Map<ModuleDto>(todoResult);
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

        private void CheckTestStep(ModuleDto obj)
        {
            NavigationParameters keyValuePairs = new NavigationParameters();
            keyValuePairs.Add("Module", obj);
            regionManager.Regions[PrismManager.MainViewRegionName].RequestNavigate("TestStep", back =>
            {
                journal = back.Context.NavigationService.Journal;
            }, keyValuePairs);
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
        private ModuleDto currentDto;

        /// <summary>
        /// 编辑选中/新增时对象
        /// </summary>
        public ModuleDto CurrentDto
        {
            get { return currentDto; }
            set { currentDto = value; RaisePropertyChanged(); }
        }
        /// <summary>
        /// 添加待办
        /// </summary>

        private async void Save()
        {
            if (string.IsNullOrWhiteSpace(CurrentDto.ModuleName))return;

            UpdateLoading(true);

            try
            {
                if (CurrentDto.Id > 0)
                {
                    var updateResult = await sqlSugarClient.Updateable(mapper.Map<Module>(CurrentDto)).ExecuteCommandAsync();
                    if (updateResult > 0)
                    {
                        var moduleDto = ModuleDtos.FirstOrDefault(t => t.Id == CurrentDto.Id);
                        if (moduleDto != null)
                        {

                            moduleDto.ModuleName = CurrentDto.ModuleName;
                            moduleDto.SpecificationRequirements = CurrentDto.SpecificationRequirements;
                            moduleDto.ProjectId = CurrentDto.Id;
                        }
                    }
                    IsRightDrawerOpen = false;
                }
                else
                {
                    CurrentDto.ProjectId = ProjectDto.Id;
                    var addResult = await sqlSugarClient.Insertable(mapper.Map<Module>(CurrentDto)).ExecuteReturnEntityAsync();
                    if (addResult != null)
                    {
                        ModuleDtos.Add(mapper.Map<ModuleDto>(addResult));
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
        public DelegateCommand<ModuleDto> SelectedCommand { get; private set; }
        public DelegateCommand<ModuleDto> CheckTestStepCommand { get; private set; }
        public DelegateCommand<ModuleDto> DeleteCommand { get; private set; }

        private ObservableCollection<ModuleDto> moduleDtos;
        public ObservableCollection<ModuleDto> ModuleDtos
        {
            get { return moduleDtos; }
            set { moduleDtos = value; RaisePropertyChanged(); }
        }
        

        private async void GetDataAsync()
        {
            UpdateLoading(true);
            if (string.IsNullOrEmpty(Search))
            {
                var list = await sqlSugarClient.Queryable<Module>().Where(x=>x.ProjectId==ProjectDto.Id).ToListAsync();
                if (list.Count == 0)
                {
                    ModuleDtos.Clear();
                }
                if (list.Count > 0)
                {
                    ModuleDtos.Clear();
                    foreach (var item in list)
                    {
                        ModuleDtos.Add(mapper.Map<ModuleDto>(item));
                    }
                }
            }
            else
            {
                var todoResult = await sqlSugarClient.Queryable<Module>().Where(x => x.ModuleName == Search&&x.ProjectId == ProjectDto.Id).ToListAsync();
                if (todoResult.Count == 0)
                {
                    ModuleDtos.Clear();
                }
                if (todoResult.Count > 0)
                {
                    ModuleDtos.Clear();
                    foreach (var item in todoResult)
                    {
                        ModuleDtos.Add(mapper.Map<ModuleDto>(item));
                    }
                }
            }
            UpdateLoading(false);

        }

        private void Add()
        {
            CurrentDto = new ModuleDto();
            IsRightDrawerOpen = true;
        }
        public override void OnNavigatedTo(NavigationContext navigationContext)
        {
            base.OnNavigatedTo(navigationContext);
            if (navigationContext.Parameters.ContainsKey("Project"))
                ProjectDto = navigationContext.Parameters.GetValue<ProjectDto>("Project");
            GetDataAsync();
        }
    }
}
