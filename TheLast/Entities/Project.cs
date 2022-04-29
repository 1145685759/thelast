using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheLast.Entities
{
    /// <summary>
    /// 项目表
    /// </summary>
    public class Project
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string ProjectName { get; set; }
        public string ProjectDescribe { get; set; }
        [SugarColumn(IsIgnore = true)]
        public List<Module> Modules { get; set; }
    }
}
