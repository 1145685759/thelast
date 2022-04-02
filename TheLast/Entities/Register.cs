using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheLast.Entities
{
    public class Register
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public ushort Address { get; set; }
        public string Name { get; set; }
        public bool IsEnable { get; set; }
        public string RegisterType { get; set; }
        public bool IsDisplay { get; set; } = false;
        public byte StationNum { get; set; } = 1;
        public bool IsHsData { get; set; } = false;
        public ushort? Type { get; set; }
        public ushort? Caste { get; set; }
        public ushort? StartAddress { get; set; }
    }
}
