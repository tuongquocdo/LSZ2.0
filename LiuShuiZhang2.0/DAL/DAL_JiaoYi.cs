using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LiuShuiZhang2._0.DAL
{
    public class DAL_JiaoYi
    {
        public void AddNewJiaoYi(LiuShuiZhang2._0.BLL.BLL_JiaoYi jy)
        {
            using (var con = new SqlConnection(Common.constr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = string.Format("insert into JIAOYI(JIAOYIDANID,REYUANID,LIUSHUIID,BIZHONGID,QIANDANID,SHIJIAN,LIANG,JIA,YIGONG,BENQIAN,LIRUN,BEIZHU,QUEREN) " +
                    "values (@JIAOYIDANID,@REYUANID,@LIUSHUIID,@BIZHONGID,@QIANDANID,@SHIJIAN,@LIANG,@JIA,@YIGONG,@BENQIAN,@LIRUN,@BEIZHU,QUEREN)");
                cmd.Parameters.AddWithValue("@JIAOYIDANID", jy.JiaoYiDanID);
                cmd.Parameters.AddWithValue("@REYUANID", jy.UserID);
                cmd.Parameters.AddWithValue("@LIUSHUIID", jy.LiuShuiID);
                cmd.Parameters.AddWithValue("@BIZHONGID", jy.BiZhongID);
                cmd.Parameters.AddWithValue("@QIANDANID", jy.QianDanID);
                cmd.Parameters.AddWithValue("@SHIJIAN", jy.Time);
                cmd.Parameters.AddWithValue("@LIANG", jy.Quantity);
                cmd.Parameters.AddWithValue("@JIA", jy.Value);
                cmd.Parameters.AddWithValue("@YIGONG", jy.Price);
                cmd.Parameters.AddWithValue("@BENQIAN", jy.Cogs);
                cmd.Parameters.AddWithValue("@LIRUN", jy.Profit);
                cmd.Parameters.AddWithValue("@BEIZHU", jy.Note);
                cmd.Parameters.AddWithValue("@QUEREN", jy.Confirmed);
                cmd.Connection = con;
                cmd.ExecuteNonQuery();
            }
        }


        public void AddNewJiaoYis(BLL.BLL_JiaoYiDan jyd, BLL.BLL_LiuShui ls,  List<BLL.BLL_JiaoYi> jys)
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
                    cmd.Parameters.AddWithValue("@JIAOYIDAN_SHIJIAN", jyd.Time);
                    cmd.Parameters.AddWithValue("@JIAOYIDAN_ZONGE", jyd.TotalPrice);
                    cmd.Parameters.AddWithValue("@JIAOYIDAN_500000", jyd.__500);
                    cmd.Parameters.AddWithValue("@JIAOYIDAN_200000", jyd.__200);
                    cmd.Parameters.AddWithValue("@JIAOYIDAN_100000", jyd.__100);
                    cmd.Parameters.AddWithValue("@JIAOYIDAN_50000", jyd.__50);
                    cmd.Parameters.AddWithValue("@JIAOYIDAN_20000", jyd.__20);
                    cmd.Parameters.AddWithValue("@JIAOYIDAN_10000", jyd.__10);
                    cmd.Parameters.AddWithValue("@JIAOYIDAN_5000", jyd.__5);
                    cmd.Parameters.AddWithValue("@JIAOYIDAN_2000", jyd.__2);
                    cmd.Parameters.AddWithValue("@JIAOYIDAN_1000", jyd.__1);
                    jiaoYiDanId = (long) cmd.ExecuteScalar();

                    cmd.CommandText = string.Format("update LIUSHUI set XIANE=@LIUSHUI_XIANE, XIANGCHA=@LIUSHUI_XIANGCHA, DIANSUANJIEGUO=@LIUSHUI_DIANSUANJIEGUO, _500000=@LIUSHUI_500000," +
                        "_200000=@LIUSHUI_200000,_100000=@LIUSHUI_100000,_50000=@LIUSHUI_50000,_20000=@LIUSHUI_20000,_10000=@LIUSHUI_10000,_5000=@LIUSHUI_5000,_2000=@LIUSHUI_2000,_1000=@LIUSHUI_1000 Where LIUSHUIID = @LIUSHUI_LIUSHUIID");
                    cmd.Parameters.AddWithValue("@LIUSHUI_XIANE", ls.CurValue);
                    cmd.Parameters.AddWithValue("@LIUSHUI_XIANGCHA", ls.DeltaValue);
                    cmd.Parameters.AddWithValue("@LIUSHUI_DIANSUANJIEGUO", ls.CountValue);
                    cmd.Parameters.AddWithValue("@LIUSHUI_500000", ls.__500);
                    cmd.Parameters.AddWithValue("@LIUSHUI_200000", ls.__200);
                    cmd.Parameters.AddWithValue("@LIUSHUI_100000", ls.__100);
                    cmd.Parameters.AddWithValue("@LIUSHUI_50000", ls.__50);
                    cmd.Parameters.AddWithValue("@LIUSHUI_20000", ls.__20);
                    cmd.Parameters.AddWithValue("@LIUSHUI_10000", ls.__10);
                    cmd.Parameters.AddWithValue("@LIUSHUI_5000", ls.__5);
                    cmd.Parameters.AddWithValue("@LIUSHUI_2000", ls.__2);
                    cmd.Parameters.AddWithValue("@LIUSHUI_1000", ls.__1);
                    cmd.Parameters.AddWithValue("@LIUSHUI_LIUSHUIID", ls.LiuShuiID);
                    cmd.ExecuteNonQuery();

                    foreach (BLL.BLL_JiaoYi jy in jys)
                    {
                        cmd.CommandText = string.Format("insert into JIAOYI(JIAOYIDANID,RENYUANID,LIUSHUIID,BIZHONGID,QIANDANID,SHIJIAN,LIANG,JIA,YIGONG,BENQIAN,LIRUN,BEIZHU,QUEREN) " +
                        "values ({0},{1},{2},{3},{4},'{5}',{6},{7},{8},{9},{10}, N'{11}','{12}')", jiaoYiDanId, jy.UserID,jy.LiuShuiID, jy.BiZhongID, jy.QianDanID,
                        jy.Time, jy.Quantity, jy.Value, jy.Price, jy.Cogs, jy.Profit, jy.Note, jy.Confirmed);
                        //cmd.Parameters.AddWithValue("@JIAOYI_JIAOYIDANID", jiaoYiDanId);
                        //cmd.Parameters.AddWithValue("@JIAOYI_RENYUANID", jy.UserID);
                        //cmd.Parameters.AddWithValue("@JIAOYI_LIUSHUIID", jy.LiuShuiID);
                        //cmd.Parameters.AddWithValue("@JIAOYI_BIZHONGID", jy.BiZhongID);
                        //cmd.Parameters.AddWithValue("@JIAOYI_QIANDANID", jy.QianDanID);
                        //cmd.Parameters.AddWithValue("@JIAOYI_SHIJIAN", jy.Time);
                        //cmd.Parameters.AddWithValue("@JIAOYI_LIANG", jy.Quantity);
                        //cmd.Parameters.AddWithValue("@JIAOYI_JIA", jy.Value);
                        //cmd.Parameters.AddWithValue("@JIAOYI_YIGONG", jy.Price);
                        //cmd.Parameters.AddWithValue("@JIAOYI_BENQIAN", jy.Cogs);
                        //cmd.Parameters.AddWithValue("@JIAOYI_LIRUN", jy.Profit);
                        //cmd.Parameters.AddWithValue("@JIAOYI_BEIZHU", jy.Note);
                        //cmd.Parameters.AddWithValue("@JIAOYI_QUEREN", jy.Confirmed);
                        cmd.ExecuteNonQuery();
                    }
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
