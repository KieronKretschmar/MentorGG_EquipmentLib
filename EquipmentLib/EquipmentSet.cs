using RabbitCommunicationLib.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace EquipmentLibrary
{
    public class EquipmentSet
    {
        public Source Source { get; set; }
        public DateTime ValidFrom { get; set; }
        public Dictionary<short, EquipmentInfo> EquipmentDict { get; set; }
    }
}
