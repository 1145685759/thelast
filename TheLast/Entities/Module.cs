using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheLast.Entities
{
    public class Module
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string ModuleName { get; set; }
        public string SpecificationRequirements { get; set; }
        public int ProjectId { get; set; }
        [SugarColumn(IsIgnore = true)]
        public List<TestStep> TestSteps { get; set; }
    }
}
