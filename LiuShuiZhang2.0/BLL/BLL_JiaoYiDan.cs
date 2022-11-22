using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiuShuiZhang2._0.BLL
{
    public class BLL_JiaoYiDan
    {
        private long jiaoYiDanID;
        private DateTime time;
        private decimal totalPrice;
        int _500;
        int _200;
        int _100;
        int _50;
        int _20;
        int _10;
        int _5;
        int _2;
        int _1;

        public long JiaoYiDanID { get => jiaoYiDanID; set => jiaoYiDanID = value; }
        public DateTime Time { get => time; set => time = value; }
        public decimal TotalPrice { get => totalPrice; set => totalPrice = value; }
        public int __500 { get => _500; set => _500 = value; }
        public int __200 { get => _200; set => _200 = value; }
        public int __100 { get => _100; set => _100 = value; }
        public int __50 { get => _50; set => _50 = value; }
        public int __20 { get => _20; set => _20 = value; }
        public int __10 { get => _10; set => _10 = value; }
        public int __5 { get => _5; set => _5 = value; }
        public int __2 { get => _2; set => _2 = value; }
        public int __1 { get => _1; set => _1 = value; }
    }
}
