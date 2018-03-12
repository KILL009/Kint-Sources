using OpenNos.Domain;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;

namespace OpenNos.GameObject
{
    public class EventSchedule : IConfigurationSectionHandler
    {
        #region Methods

        public object Create(object parent, object configContext, XmlNode section)
        {
            List<Schedule> list = new List<Schedule>();
            foreach (XmlNode aSchedule in section.ChildNodes)
                list.Add(GetSchedule(aSchedule));

            return list;
        }

        private static Schedule GetSchedule(XmlNode str)
        {
            if (str.Attributes != null)
            {
                var result = new Schedule
                {
                    Event = (EventType)Enum.Parse(typeof(EventType), str.Attributes["event"].Value),
                    Time = TimeSpan.Parse(str.Attributes["time"].Value)
                };
                return result;
            }

            return null;
        }

        #endregion
    }
}