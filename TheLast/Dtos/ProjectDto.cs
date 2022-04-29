using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheLast.Dtos
{
    /// <summary>
    /// 项目DTO
    /// </summary>
    public class ProjectDto: BindableBase
    {
        private int id;
        public int Id
        {
            get { return id; }
            set { SetProperty(ref id, value); }
        }
        private string projectName;
        public string ProjectName
        {
            get { return projectName; }
            set { SetProperty(ref projectName, value); }
        }
        private string projectDescribe;
        public string ProjectDescribe
        {
            get { return projectDescribe; }
            set { SetProperty(ref projectDescribe, value); }
        }
        private int isComplete;
        public int IsComplete
        {
            get { return isComplete; }
            set { SetProperty(ref isComplete, value); }
        }
        private int moduleCount;
        public int ModuleCount
        {
            get { return moduleCount; }
            set { SetProperty(ref moduleCount, value); }
        }
    }
}
