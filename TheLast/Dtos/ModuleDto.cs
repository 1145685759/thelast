using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheLast.Dtos
{
    /// <summary>
    /// 模块DTO
    /// </summary>
    public class ModuleDto: BindableBase
    {
        private int id;
        public int Id
        {
            get { return id; }
            set { SetProperty(ref id, value); }
        }
        private string moduleName;
        public string ModuleName
        {
            get { return moduleName; }
            set { SetProperty(ref moduleName, value); }
        }
        private string specificationRequirements;
        public string SpecificationRequirements
        {
            get { return specificationRequirements; }
            set { SetProperty(ref specificationRequirements, value); }
        }
        private int projectId;
        public int ProjectId
        {
            get { return projectId; }
            set { SetProperty(ref projectId, value); }
        }
        private ProjectDto projectDto;
        public ProjectDto ProjectDto
        {
            get { return projectDto; }
            set { SetProperty(ref projectDto, value); }
        }
        private int testStepCount;
        public int TestStepCount
        {
            get { return testStepCount; }
            set { SetProperty(ref testStepCount, value); }
        }
        private int abnormalTestStep;
        public int AbnormalTestStep
        {
            get { return abnormalTestStep; }
            set { SetProperty(ref abnormalTestStep, value); }
        }
    }
}
