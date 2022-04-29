using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheLast.Entities
{
    /// <summary>
    /// 寄存器数量限制表
    /// </summary>
    public class RegisterCount
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string Type { get; set; }
        public int Count { get; set; }
    }
}
