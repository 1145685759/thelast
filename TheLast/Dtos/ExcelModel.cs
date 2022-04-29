using System;
using System.Collections.Generic;
using System.Text;

namespace TheLast.Dtos
{
    /// <summary>
    /// EXCEL模型
    /// </summary>
    public class ExcelModel
    {
        public string 应用条件 { get; set; }
        public string 寄存器写入 { get; set; }
        public string 写入值 { get; set; }
        public string 寄存器判断 { get; set; }
        public string 运算符 { get; set; }
        public string 目标值 { get; set; }
        public string 备注 { get; set; }
        public string 站号 { get; set; }
        public string 延时模式 { get; set; }
        public string 测试内容 { get; set; }
        public string 延时时间 { get; set; }
    }
}
