using System;
using System.Xml.Serialization;

namespace OpenNos.XMLModel.Events
{
    [Serializable]
    public class OnTimeout
    {
        [XmlElement]
        public End End { get; set; }
    }
}