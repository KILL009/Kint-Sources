using OpenNos.XMLModel.Events.Quest;
using OpenNos.XMLModel.Models.Quest;
using OpenNos.XMLModel.Objects.Quest;
using System.IO;
using System.Xml.Serialization;

namespace OpenNos.QuestGenerator.CLITest
{
    internal static class Program
    {
        #region Methods

        internal static void Main()
        {
            QuestModel questModel = new QuestModel
            {
                QuestGiver = new QuestGiver()
                {
                    Type = Domain.QuestGiverType.InitialQuest,
                    QuestGiverId = -1,
                    MinimumLevel = 1,
                    MaximumLevel = 255
                },

                WalkObjective = new WalkObjective()
                {
                    MapId = 1,
                    MapX = 53,
                    MapY = 154
                },

                Reward = new Reward()
                {
                    DisplayRewardWindow = true,
                    ForceLevelUp = 5,
                    QuestId = -1,
                    TeleportPosition = new TeleportTo()
                    {
                        MapId = 1,
                        MapX = 75,
                        MapY = 75
                    },
                    Buff = 116
                }
            };

            using (StreamWriter sw = new StreamWriter("output.xml"))
            {
                new XmlSerializer(typeof(QuestModel)).Serialize(sw, questModel);
                sw.Flush();
            }
        }

        #endregion
    }
}