using OpenNos.Data;
using OpenNos.Domain;
using System;

namespace OpenNos.GameObject
{
    public class PotionItem : Item
    {
        #region Instantiation

        public PotionItem(ItemDTO item) : base(item)
        {
        }

        #endregion

        #region Methods

        public override void Use(ClientSession session, ref ItemInstance inv, byte option = 0, string[] packetsplit = null)
        {
            if ((DateTime.Now - session.Character.LastPotion).TotalMilliseconds < 750)
            {
                return;
            }

            session.Character.LastPotion = DateTime.Now;
            switch (Effect)
            {
                default:
                    if (session.Character.Hp == session.Character.HPLoad() && session.Character.Mp == session.Character.MPLoad())
                    {
                        return;
                    }

                    if (session.Character.Hp <= 0)
                    {
                        return;
                    }

                    session.Character.Inventory.RemoveItemAmountFromInventory(1, inv.Id);
                    if ((int)session.Character.HPLoad() - session.Character.Hp < Hp)
                    {
                        session.CurrentMapInstance?.Broadcast(session.Character.GenerateRc((int)session.Character.HPLoad() - session.Character.Hp));
                    }
                    else if ((int)session.Character.HPLoad() - session.Character.Hp > Hp)
                    {
                        session.CurrentMapInstance?.Broadcast(session.Character.GenerateRc(Hp));
                    }

                    session.Character.Mp += Mp;
                    session.Character.Hp += Hp;
                    if (session.Character.Mp > session.Character.MPLoad())
                    {
                        session.Character.Mp = (int)session.Character.MPLoad();
                    }

                    if (session.Character.Hp > session.Character.HPLoad())
                    {
                        session.Character.Hp = (int)session.Character.HPLoad();
                    }

                    if (session.CurrentMapInstance?.MapInstanceType == MapInstanceType.Act4Instance || session.CurrentMapInstance?.IsPVP == true)
                    {
                        if (inv.ItemVNum == 1242 || inv.ItemVNum == 5582 || inv.ItemVNum == 1243 || inv.ItemVNum == 5583 || inv.ItemVNum == 1244 || inv.ItemVNum == 5584)
                        {
                            return;
                        }
                    }

                    if (inv.ItemVNum == 1242 || inv.ItemVNum == 5582)
                    {
                        session.CurrentMapInstance?.Broadcast(session.Character.GenerateRc((int)session.Character.HPLoad() - session.Character.Hp));
                        session.Character.Hp = (int)session.Character.HPLoad();
                    }
                    else if (inv.ItemVNum == 1243 || inv.ItemVNum == 5583)
                    {
                        session.Character.Mp = (int)session.Character.MPLoad();
                    }
                    else if (inv.ItemVNum == 1244 || inv.ItemVNum == 5584)
                    {
                        session.CurrentMapInstance?.Broadcast(session.Character.GenerateRc((int)session.Character.HPLoad() - session.Character.Hp));
                        session.Character.Hp = (int)session.Character.HPLoad();
                        session.Character.Mp = (int)session.Character.MPLoad();
                    }

                    session.SendPacket(session.Character.GenerateStat());
                    break;
            }
        }

        #endregion
    }
}