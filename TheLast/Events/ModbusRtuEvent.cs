using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheLast.Events
{
    /// <summary>
    /// ModbusRtu事件
    /// </summary>
    public class ModbusRtuEvent:PubSubEvent<Modbus.Device.ModbusSerialMaster>
    {

    }
}
