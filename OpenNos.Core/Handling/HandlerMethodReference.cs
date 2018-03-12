using OpenNos.Domain;
using System;
using System.Linq;

namespace OpenNos.Core.Handling
{
    public class HandlerMethodReference
    {
        #region Instantiation

        public HandlerMethodReference(Action<object, object> handlerMethod, IPacketHandler parentHandler, PacketAttribute handlerMethodAttribute)
        {
            HandlerMethod = handlerMethod;
            ParentHandler = parentHandler;
            HandlerMethodAttribute = handlerMethodAttribute;
            Identification = HandlerMethodAttribute.Header;
            PassNonParseablePacket = false;
            Authority = AuthorityType.User;
        }

        public HandlerMethodReference(Action<object, object> handlerMethod, IPacketHandler parentHandler, Type packetBaseParameterType)
        {
            HandlerMethod = handlerMethod;
            ParentHandler = parentHandler;
            PacketDefinitionParameterType = packetBaseParameterType;
            var headerAttribute = (PacketHeaderAttribute)PacketDefinitionParameterType.GetCustomAttributes(true).FirstOrDefault(ca => ca.GetType().Equals(typeof(PacketHeaderAttribute)));
            Identification = headerAttribute?.Identification;
            PassNonParseablePacket = headerAttribute?.PassNonParseablePacket ?? false;
            Authority = headerAttribute?.Authority ?? AuthorityType.User;
        }

        #endregion

        #region Properties

        public AuthorityType Authority { get; private set; }

        public Action<object, object> HandlerMethod { get; private set; }

        public PacketAttribute HandlerMethodAttribute { get; }

        /// <summary>
        /// Unique identification of the Packet by Header
        /// </summary>
        public string Identification { get; private set; }

        public Type PacketDefinitionParameterType { get; }

        public IPacketHandler ParentHandler { get; private set; }

        public bool PassNonParseablePacket { get; private set; }

        #endregion
    }
}