using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheLast.Entities
{
    public class TestStep
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public int ModuleId { get; set; }
        [SugarColumn(IsIgnore =true)]
        public List<Init> Inits { get; set; }
        [SugarColumn(IsIgnore = true)]
        public List<FeedBack> FeedBacks { get; set; }
        public string Conditions { get; set; }
        public string TestContent { get; set; }
        public string Result { get; set; }
        public string Remark { get; set; }
        public string TestProcess { get; set; }
        public string JudgmentContent { get; set; }

    }
}
