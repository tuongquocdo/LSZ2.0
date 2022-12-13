using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiuShuiZhang2._0.BLL
{
    public class BLL_JiaoYi
    {
        private long jiaoYiID;
        private long jiaoYiDanID;
        private long userID;
        private long liuShuiID;
        private long biZhongID;
        private long qianDanID;
        private DateTime time;
        private decimal quantity;
        private decimal value;
        private decimal price;
        private decimal cogs;
        private decimal profit;
        private string note;
        private bool confirmed;
        private DataTable feeTypes;
        private BLL_QianDan qianDan;
        private string biZhong;

        public long JiaoYiID { get => jiaoYiID; set => jiaoYiID = value; }
        public long JiaoYiDanID { get => jiaoYiDanID; set => jiaoYiDanID = value; }
        public long UserID { get => userID; set => userID = value; }
        public long LiuShuiID { get => liuShuiID; set => liuShuiID = value; }
        public long BiZhongID { get => biZhongID; set => biZhongID = value; }
        public long QianDanID { get => qianDanID; set => qianDanID = value; }
        public DateTime Time { get => time; set => time = value; }
        public decimal Quantity { get => quantity; set => quantity = value; }
        public decimal Value { get => value; set => this.value = value; }
        public decimal Price { get => price; set => price = value; }
        public decimal Cogs { get => cogs; set => cogs = value; }
        public decimal Profit { get => profit; set => profit = value; }
        public string Note { get => note; set => note = value; }
        public bool Confirmed { get => confirmed; set => confirmed = value; }
        public DataTable FeeTypes { get => feeTypes; set => feeTypes = value; }
        public BLL_QianDan QianDan { get => qianDan; set => qianDan = value; }
        public string BiZhong { get => biZhong; set => biZhong = value; }

        public BLL_JiaoYi()
        {
            DataTable tb = new DataTable("FeeTypes");
            tb.Columns.Add("FEETYPEID", typeof(int));
            tb.Columns.Add("FEETYPE", typeof(string));
            tb.Rows.Add(new object[] { 0, "无费" });
            tb.Rows.Add(new object[] { 1,"%"});
            tb.Rows.Add(new object[] { 2, "该币种" });
            tb.Rows.Add(new object[] { 3, "现金" });
            feeTypes = tb;
        }

        public enum Enum_FeeTypes
        { 
            Free,
            Percent,
            ByBiZhong,
            ByXianJin
        }

        public enum Enum_JiaoYiMode
        { 
            InsertJiaoYis,
            DeleteJiaoYi,
            CloneJiaoYi
        }

    }
}
