using OpenNos.Data;
using System;
using System.Collections.Generic;

namespace OpenNos.GameObject
{
    public class Portal : PortalDTO
    {
        #region Members

        private Guid destinationMapInstanceId;
        private Guid sourceMapInstanceId;

        #endregion

        #region Instantiation

        public Portal()
        {
            OnTraversalEvents = new List<EventContainer>();
        }

        #endregion

        #region Properties

        public Guid DestinationMapInstanceId
        {
            get
            {
                if (destinationMapInstanceId == default(Guid) && DestinationMapId != -1)
                {
                    destinationMapInstanceId = ServerManager.Instance.GetBaseMapInstanceIdByMapId(DestinationMapId);
                }

                return destinationMapInstanceId;
            }
            set { destinationMapInstanceId = value; }
        }

        public List<EventContainer> OnTraversalEvents { get; set; }

        public Guid SourceMapInstanceId
        {
            get
            {
                if (sourceMapInstanceId == default(Guid))
                {
                    sourceMapInstanceId = ServerManager.Instance.GetBaseMapInstanceIdByMapId(SourceMapId);
                }

                return sourceMapInstanceId;
            }
            set { sourceMapInstanceId = value; }
        }

        #endregion

        #region Methods

        public string GenerateGp()
        {
            return $"gp {SourceX} {SourceY} {ServerManager.Instance.GetMapInstance(DestinationMapInstanceId)?.Map.MapId ?? 0} {Type} {PortalId} {(IsDisabled ? 1 : 0)}";
        }

        #endregion
    }
}