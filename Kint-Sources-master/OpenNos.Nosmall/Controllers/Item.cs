using ShopApplicationByFizo.Models;

namespace ShopApplicationByFizo.Controllers
{
    public class Item
    {
        private shopping sh = new shopping();

        public shopping Sh
        {
            get { return sh; }
            set { sh = value; }
        }

        private int quantity;

        public int Quantity
        {
            get { return quantity; }
            set { quantity = value; }
        }

        public Item()
        { }

        public Item(shopping shopping, int quantity)
        {
            this.sh = shopping;
            this.quantity = quantity;
        }
    }
}