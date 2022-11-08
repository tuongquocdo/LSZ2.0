using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiuShuiZhang2._0.BLL
{
    public class BLL_BiZhong
    {
        private long biZhongID;

        private string biZhong;

        private decimal quantity;

        private decimal averagePrice;

        private decimal totalValue;

        private int type;

        private bool disable;
       
        public long BiZhongID { get => biZhongID; set => biZhongID = value; }
        public string BiZhong { get => biZhong; set => biZhong = value; }
        public decimal Quantity { get => quantity; set => quantity = value; }
        public decimal AveragePrice { get => averagePrice; set => averagePrice = value; }
        public decimal TotalValue { get => totalValue; set => totalValue = value; }
        public int Type { get => type; set => type = value; }
        public bool Disable { get => disable; set => disable = value; }

        public enum BiZhongLei
        {
            None,
            WaiBi,
            KeRenQian,
            QianKeRen,
            DianZiZhang,
            XianJin
        }
    }
}
