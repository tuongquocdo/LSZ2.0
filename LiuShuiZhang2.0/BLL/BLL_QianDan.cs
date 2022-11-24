using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiuShuiZhang2._0.BLL
{
    public class BLL_QianDan
    {
        private long qianDanID;
        private string content;
        private DateTime time;
        private decimal qianDanValue;
        private bool finish;

        public long QianDanID { get => qianDanID; set => qianDanID = value; }
        public string Content { get => content; set => content = value; }
        public DateTime Time { get => time; set => time = value; }
        public decimal QianDanValue { get => qianDanValue; set => qianDanValue = value; }
        public bool Finish { get => finish; set => finish = value; }
    }
}
