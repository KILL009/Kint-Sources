using System;
using System.Xml.Serialization;

namespace OpenNos.XMLModel.Events
{
    [Serializable]
    public class OnTraversal
    {
        #region Properties

        [XmlElement]
        public End End { get; set; }

        #endregion
    }
}