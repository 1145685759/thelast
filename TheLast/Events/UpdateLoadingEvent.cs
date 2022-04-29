using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheLast.Events
{
    /// <summary>
    /// 加载模型
    /// </summary>
    public class UpdateModel
    {
        public bool IsOpen { get; set; }
    }
    /// <summary>
    /// 加载事件
    /// </summary>
    public class UpdateLoadingEvent : PubSubEvent<UpdateModel>
    {

    }
}
