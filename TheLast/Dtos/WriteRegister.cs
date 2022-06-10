using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheLast.Dtos
{
    /// <summary>
    /// 写入寄存器模型
    /// </summary>
    public class WriteRegister: BindableBase
    {
        private RegisterDto registerDto;
        public RegisterDto RegisterDto
        {
            get { return registerDto; }
            set { SetProperty(ref registerDto, value); }
        }
        private string value1;
        public string Value
        {
            get { return value1; }
            set { SetProperty(ref value1, value); }
        }
    }
}
