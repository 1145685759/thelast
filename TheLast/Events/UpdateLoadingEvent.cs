using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheLast.Events
{
    public class UpdateModel
    {
        public bool IsOpen { get; set; }
    }

    public class UpdateLoadingEvent : PubSubEvent<UpdateModel>
    {

    }
}
