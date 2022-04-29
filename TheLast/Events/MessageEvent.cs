using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheLast.Events
{
    /// <summary>
    /// 消息模型
    /// </summary>
    public class MessageModel
    {
        public string Filter { get; set; }
        public string Message { get; set; }
    }
    /// <summary>
    /// 消息事件
    /// </summary>
    public class MessageEvent : PubSubEvent<MessageModel>
    {
    }
}
