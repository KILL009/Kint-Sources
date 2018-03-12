using System;
using System.Linq;
using System.Reactive.Linq;

namespace OpenNos.GameObject
{
    public class Buff
    {
        #region Members

        public int Level;

        #endregion

        #region Instantiation

        public Buff(int id)
        {
            Card = ServerManager.Instance.Cards.FirstOrDefault(s => s.CardId == id);
        }

        public Buff(int id, byte level)
        {
            Card = ServerManager.Instance.Cards.FirstOrDefault(s => s.CardId == id);
            Level = level;
        }

        #endregion

        #region Properties

        public Card Card { get; set; }

        public int RemainingTime { get; set; }

        public DateTime Start { get; set; }

        public bool StaticBuff { get; set; }

        #endregion
    }
}