using SqlSugar;
using System;
using System.Collections.Generic;
using System.Security.Principal;
using System.Text;

namespace TheLast.Entities
{
public class Init
{
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public int RegisterId { get; set; }
        [SugarColumn(IsIgnore = true)]
        public Register Register { get; set; }
        public string WriteValue { get; set; }
        public int TestStepId { get; set; }
        public string DisplayValue { get; set; }
    }
}
