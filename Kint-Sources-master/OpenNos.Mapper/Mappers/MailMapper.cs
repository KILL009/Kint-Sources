using OpenNos.DAL.EF;
using OpenNos.Data;
using System;

namespace OpenNos.Mapper.Mappers
{
    public class MailMapper
    {
        public MailMapper()
        {

        }

        public bool ToMailDTO(Mail input, MailDTO output)
        {
            if (input == null)
            {
                output = null;
                return false;
            }
            output.AttachmentAmount = input.AttachmentAmount;
            output.AttachmentLevel = input.AttachmentLevel;
            output.AttachmentRarity = input.AttachmentRarity;
            output.AttachmentUpgrade = input.AttachmentUpgrade;
            output.AttachmentVNum = input.AttachmentVNum;
            output.Date = input.Date;
            output.EqPacket = input.EqPacket;
            output.IsOpened = input.IsOpened;
            output.IsSenderCopy = input.IsSenderCopy;
            output.MailId = input.MailId;
            output.Message = input.Message;
            output.ReceiverId = input.ReceiverId;
            output.SenderClass = input.SenderClass;
            output.SenderGender = input.SenderGender;
            output.SenderHairColor = input.SenderHairColor;
            output.SenderHairStyle = input.SenderHairStyle;
            output.SenderId = input.SenderId;
            output.SenderMorphId = input.SenderMorphId;
            output.Title = input.Title;
            return true;
        }

        public bool ToMail(MailDTO input, Mail output)
        {
            if (input == null)
            {
                output = null;
                return false;
            }
            output.AttachmentAmount = input.AttachmentAmount;
            output.AttachmentLevel = input.AttachmentLevel;
            output.AttachmentRarity = input.AttachmentRarity;
            output.AttachmentUpgrade = input.AttachmentUpgrade;
            output.AttachmentVNum = input.AttachmentVNum;
            output.Date = input.Date;
            output.EqPacket = input.EqPacket;
            output.IsOpened = input.IsOpened;
            output.IsSenderCopy = input.IsSenderCopy;
            output.MailId = input.MailId;
            output.Message = input.Message;
            output.ReceiverId = input.ReceiverId;
            output.SenderClass = input.SenderClass;
            output.SenderGender = input.SenderGender;
            output.SenderHairColor = input.SenderHairColor;
            output.SenderHairStyle = input.SenderHairStyle;
            output.SenderId = input.SenderId;
            output.SenderMorphId = input.SenderMorphId;
            output.Title = input.Title;
            return true;
        }
    }
}
