using OpenNos.Domain;
using System;
using System.ComponentModel.DataAnnotations;
using OpenNos.Data;

namespace OpenNos.DAL.EF
{
    public class Mall
    {
        #region Properties

        public Mall()
        {

        }

        public int Id { get; set; }

        public int ItemVnum { get; set; }

        public int Amount { get; set; }

        public int Price { get; set; }

        public static implicit operator Mall(MallDTO v)
        {
            Mall mall = new Mall();
            mall.Id = v.Id;
            mall.ItemVnum = v.ItemVnum;
            mall.Amount = v.Amount;
            mall.Price = v.Price;
            return mall;
        }

        #endregion
    }
}
