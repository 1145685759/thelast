using System;
using System.Collections.Generic;
using System.Text;

namespace TheLast.Dtos
{
    public class WriteRegister
    {
        public RegisterDto RegisterDto { get; set; }
        public ushort Value { get; set; }
    }
}
