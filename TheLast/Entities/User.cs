using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheLast.Entities
{
    /// <summary>
    /// 用户表
    /// </summary>
    public class User
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public string Account { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public string RoleName { get; set; } = "普通用户";
    }
}
