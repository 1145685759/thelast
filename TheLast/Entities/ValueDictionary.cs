using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheLast.Entities
{
    /// <summary>
    /// 值字典
    /// </summary>
    public class ValueDictionary
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string DisplayValue { get; set; }
        public string RealValue { get; set; }
        public int RegisterId { get; set; }
        [SugarColumn(IsIgnore = true)]
        public Register Register { get; set; }
    }
}
