using Prism.Events;
using System;
using System.Collections.Generic;
using System.Text;

namespace TheLast.Events
{
    public class ModbusRtuEvent:PubSubEvent<Modbus.Device.ModbusSerialMaster>
    {

    }
}
