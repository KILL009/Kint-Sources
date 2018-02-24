using System;
using System.Xml.Serialization;

namespace OpenNos.XMLModel.Events
{
    [Serializable]
    public class OnLockerOpen
    {
        #region Properties

        [XmlElement]
        public ChangePortalType ChangePortalType { get; set; }

        [XmlElement]
        public object RefreshMapItems { get; set; }

        [XmlElement]
        public SendMessage SendMessage { get; set; }

        #endregion
    }
}