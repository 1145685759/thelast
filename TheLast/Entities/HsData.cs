using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheLast.Entities
{
    /// <summary>
    /// 历史数据表
    /// </summary>
    public class HsData
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public DateTime DateTime { get; set; }
        public int RegisterId { get; set; }
        [SugarColumn(IsIgnore = true)]
        public Register Register { get; set; }
        public ushort RealValue { get; set; }
    }
}
