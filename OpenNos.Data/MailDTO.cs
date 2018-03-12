using OpenNos.Domain;
using System;
using System.ComponentModel.DataAnnotations;

namespace OpenNos.Data
{
    [Serializable]
    public class MailDTO : MappingBaseDTO
    {
        #region Properties

        public byte AttachmentAmount { get; set; }

        public byte AttachmentRarity { get; set; }

        public byte AttachmentUpgrade { get; set; }

        public short? AttachmentVNum { get; set; }

        public DateTime Date { get; set; }

        public string EqPacket { get; set; }

        public bool IsOpened { get; set; }

        public bool IsSenderCopy { get; set; }

        [Key]
        public long MailId { get; set; }

        public string Message { get; set; }

        public long ReceiverId { get; set; }

        public ClassType SenderClass { get; set; }

        public GenderType SenderGender { get; set; }

        public HairColorType SenderHairColor { get; set; }

        public HairStyleType SenderHairStyle { get; set; }

        public long SenderId { get; set; }

        public short SenderMorphId { get; set; }

        public string Title { get; set; }

        #endregion
    }
}