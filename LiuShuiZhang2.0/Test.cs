using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiuShuiZhang2._0
{
    public partial class Test : Form
    {
        public Test()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            using (var con = new SqlConnection(Common.constr))
            {
                long jiaoYiDanId;
                SqlTransaction sqltran = null;
                SqlCommand cmd = new SqlCommand();
                try
                {
                    con.Open();
                    sqltran = con.BeginTransaction();
                    cmd.Connection = con;
                    cmd.Transaction = sqltran;

                    cmd.CommandText = string.Format("insert into JIAOYIDAN(SHIJIAN,ZONGE,_500000,_200000,_100000,_50000,_20000,_10000,_5000,_2000,_1000) output inserted.JIAOYIDANID" +
                    " values (@JIAOYIDAN_SHIJIAN,@JIAOYIDAN_ZONGE,@JIAOYIDAN_500000,@JIAOYIDAN_200000,@JIAOYIDAN_100000,@JIAOYIDAN_50000,@JIAOYIDAN_20000,@JIAOYIDAN_10000,@JIAOYIDAN_5000,@JIAOYIDAN_2000,@JIAOYIDAN_1000)");
                    cmd.Parameters.AddWithValue("@JIAOYIDAN_SHIJIAN", DateTime.Now);
                    cmd.Parameters.AddWithValue("@JIAOYIDAN_ZONGE", 1000);
                    cmd.Parameters.AddWithValue("@JIAOYIDAN_500000", 0);
                    cmd.Parameters.AddWithValue("@JIAOYIDAN_200000", 0);
                    cmd.Parameters.AddWithValue("@JIAOYIDAN_100000", 0);
                    cmd.Parameters.AddWithValue("@JIAOYIDAN_50000", 0);
                    cmd.Parameters.AddWithValue("@JIAOYIDAN_20000", 0);
                    cmd.Parameters.AddWithValue("@JIAOYIDAN_10000", 0);
                    cmd.Parameters.AddWithValue("@JIAOYIDAN_5000", 0);
                    cmd.Parameters.AddWithValue("@JIAOYIDAN_2000", 0);
                    cmd.Parameters.AddWithValue("@JIAOYIDAN_1000", 0);
                    jiaoYiDanId = (long)cmd.ExecuteScalar();

                    cmd.CommandText = string.Format("update LIUSHUI set XIANE=@LIUSHUI_XIANE, XIANGCHA=@LIUSHUI_XIANGCHA, DIANSUANJIEGUO=@LIUSHUI_DIANSUANJIEGUO, _500000=@LIUSHUI_500000," +
                        "_200000=@LIUSHUI_200000,_100000=@LIUSHUI_100000,_50000=@LIUSHUI_50000,_20000=@LIUSHUI_20000,_10000=@LIUSHUI_10000,_5000=@LIUSHUI_5000,_2000=@LIUSHUI_2000,_1000=@LIUSHUI_1000 Where LIUSHUIID = @LIUSHUI_LIUSHUIID");
                    cmd.Parameters.AddWithValue("@LIUSHUI_XIANE", 100);
                    cmd.Parameters.AddWithValue("@LIUSHUI_XIANGCHA", -100);
                    cmd.Parameters.AddWithValue("@LIUSHUI_DIANSUANJIEGUO", 100);
                    cmd.Parameters.AddWithValue("@LIUSHUI_500000", 0);
                    cmd.Parameters.AddWithValue("@LIUSHUI_200000", 0);
                    cmd.Parameters.AddWithValue("@LIUSHUI_100000", 0);
                    cmd.Parameters.AddWithValue("@LIUSHUI_50000", 0);
                    cmd.Parameters.AddWithValue("@LIUSHUI_20000", 0);
                    cmd.Parameters.AddWithValue("@LIUSHUI_10000", 0);
                    cmd.Parameters.AddWithValue("@LIUSHUI_5000", 0);
                    cmd.Parameters.AddWithValue("@LIUSHUI_2000", 0);
                    cmd.Parameters.AddWithValue("@LIUSHUI_1000", 0);
                    cmd.Parameters.AddWithValue("@LIUSHUI_LIUSHUIID", 1);
                    cmd.ExecuteNonQuery();

                    
                    cmd.CommandText = string.Format("insert into JIAOYI(JIAOYIDANID,RENYUANID,LIUSHUIID,BIZHONGID,QIANDANID,SHIJIAN,LIANG,JIA,YIGONG,BENQIAN,LIRUN,BEIZHU,QUEREN) " +
                        "values (@JIAOYI_JIAOYIDANID,@JIAOYI_RENYUANID,@JIAOYI_LIUSHUIID,@JIAOYI_BIZHONGID,@JIAOYI_QIANDANID,@JIAOYI_SHIJIAN,@JIAOYI_LIANG,@JIAOYI_JIA,@JIAOYI_YIGONG,@JIAOYI_BENQIAN,@JIAOYI_LIRUN,@JIAOYI_BEIZHU,@JIAOYI_QUEREN)");
                    cmd.Parameters.AddWithValue("@JIAOYI_JIAOYIDANID", jiaoYiDanId);
                    cmd.Parameters.AddWithValue("@JIAOYI_RENYUANID", 2);
                    cmd.Parameters.AddWithValue("@JIAOYI_LIUSHUIID", 1);
                    cmd.Parameters.AddWithValue("@JIAOYI_BIZHONGID", 1);
                    cmd.Parameters.AddWithValue("@JIAOYI_QIANDANID", 0);
                    cmd.Parameters.AddWithValue("@JIAOYI_SHIJIAN", DateTime.Now);
                    cmd.Parameters.AddWithValue("@JIAOYI_LIANG", 100);
                    cmd.Parameters.AddWithValue("@JIAOYI_JIA", 100);
                    cmd.Parameters.AddWithValue("@JIAOYI_YIGONG", 10000);
                    cmd.Parameters.AddWithValue("@JIAOYI_BENQIAN", 0);
                    cmd.Parameters.AddWithValue("@JIAOYI_LIRUN", 0);
                    cmd.Parameters.AddWithValue("@JIAOYI_BEIZHU", "jkj");
                    cmd.Parameters.AddWithValue("@JIAOYI_QUEREN", false);
                    cmd.ExecuteNonQuery();
                    
                    sqltran.Commit();
                }
                catch (Exception ex)
                {
                    sqltran.Rollback();
                    MessageBox.Show(ex.Message, "温卿提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

            }
        }
    }
}
