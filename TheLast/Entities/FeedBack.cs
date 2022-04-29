using SqlSugar;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheLast.Entities
{
    /// <summary>
    /// 反馈表
    /// </summary>
    public class FeedBack
    {
        [SugarColumn(IsPrimaryKey = true, IsIdentity = true)]
        public int Id { get; set; }
        public int RegisterId { get; set; }
        [SugarColumn(IsIgnore = true)]
        public Register Register { get; set; }
        public string TagetValue { get; set; }
        public int TestStepId { get; set; }
        public int DelayTime { get; set; }
        public int DelayModeId { get; set; }
        [SugarColumn(IsIgnore = true)]
        public DelayModel DelayModel { get; set; }
        public byte StationNum { get; set; } = 1;
        public string DisplayTagetValue { get; set; }
        public bool IsJump { get; set; } = false;
        public string Operational { get; set; }
    }
}
