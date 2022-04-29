using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;
using TheLast.Entities;

namespace TheLast.Dtos
{
    /// <summary>
    /// 测试步骤DTO
    /// </summary>
    public class TestStepDto: BindableBase
    {
        private int id;
        public int Id
        {
            get { return id; }
            set { SetProperty(ref id, value); }
        }
        private int moduleId;
        public int ModuleId
        {
            get { return moduleId; }
            set { SetProperty(ref moduleId, value); }
        }
        private List<Init> inits;
        public List<Init>  Inits
        {
            get { return inits; }
            set { SetProperty(ref inits, value); }
        }
        private List<FeedBack> feedBacks;
        public List<FeedBack> FeedBacks
        {
            get { return feedBacks; }
            set { SetProperty(ref feedBacks, value); }
        }
        private string testProcess;
        public string TestProcess
        {
            get { return testProcess; }
            set { SetProperty(ref testProcess, value); }
        }
        private string judgmentContent;
        public string JudgmentContent
        {
            get { return judgmentContent; }
            set { SetProperty(ref judgmentContent, value); }
        }
        private string conditions;
        public string Conditions
        {
            get { return conditions; }
            set { SetProperty(ref conditions, value); }
        }
        private string testContent;
        public string TestContent
        {
            get { return testContent; }
            set { SetProperty(ref testContent, value); }
        }
        private string result;
        public string Result
        {
            get { return result; }
            set { SetProperty(ref result, value); }
        }
        private string remark;
        public string Remark
        {
            get { return remark; }
            set { SetProperty(ref remark, value); }
        }
    }
}
