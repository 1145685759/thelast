using System;
using System.Collections.Generic;
using System.Text;

namespace TheLast.Dtos
{
    /// <summary>
    /// 写入寄存器模型
    /// </summary>
    public class WriteRegister
    {
        public RegisterDto RegisterDto { get; set; }
        public ushort Value { get; set; }
    }
}
