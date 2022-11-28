using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiuShuiZhang2._0.BLL
{
    public class BLL_LiuShui
    {
        long liuShuiID;
        DateTime liuShuiDate;
        decimal preValue;
        decimal curValue;
        decimal deltaValue;
        decimal countValue;
        long _500;
        long _200;
        long _100;
        long _50;
        long _20;
        long _10;
        long _5;
        long _2;
        long _1;

        public long LiuShuiID { get => liuShuiID; set => liuShuiID = value; }
        public DateTime LiuShuiDate { get => liuShuiDate; set => liuShuiDate = value; }
        public decimal PreValue { get => preValue; set => preValue = value; }
        public decimal CurValue { get => curValue; set => curValue = value; }
        public decimal DeltaValue { get => deltaValue; set => deltaValue = value; }
        public decimal CountValue { get => countValue; set => countValue = value; }
        public long __500 { get => _500; set => _500 = value; }
        public long __200 { get => _200; set => _200 = value; }
        public long __100 { get => _100; set => _100 = value; }
        public long __50 { get => _50; set => _50 = value; }
        public long __20 { get => _20; set => _20 = value; }
        public long __10 { get => _10; set => _10 = value; }
        public long __5 { get => _5; set => _5 = value; }
        public long __2 { get => _2; set => _2 = value; }
        public long __1 { get => _1; set => _1 = value; }
    }
}
