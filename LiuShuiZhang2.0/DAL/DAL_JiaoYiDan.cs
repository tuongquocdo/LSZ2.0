using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiuShuiZhang2._0.DAL
{
    public class DAL_JiaoYiDan
    {
        public long AddNewJiaoYiDan(LiuShuiZhang2._0.BLL.BLL_JiaoYiDan jyd)
        {
            using (var con = new SqlConnection(Common.constr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = string.Format("insert into JIAOYIDAN(SHIJIAN,ZONGE,_500000,_200000,_100000,_50000,_20000,_10000,_5000,_2000,_1000) output inserted.JIAOYIDANID" +
                    " values (@SHIJIAN,@ZONGE,@_500000,@_200000,@_100000,@50000,@_20000,@_10000,@_5000,@_2000,@_1000)");
                cmd.Parameters.AddWithValue("@SHIJIAN", jyd.Time);
                cmd.Parameters.AddWithValue("@ZONGE", jyd.TotalPrice);
                cmd.Parameters.AddWithValue("@_500000", jyd.__500);
                cmd.Parameters.AddWithValue("@_200000", jyd.__200);
                cmd.Parameters.AddWithValue("@_100000", jyd.__100);
                cmd.Parameters.AddWithValue("@_50000", jyd.__50);
                cmd.Parameters.AddWithValue("@_20000", jyd.__20);
                cmd.Parameters.AddWithValue("@_10000", jyd.__10);
                cmd.Parameters.AddWithValue("@_5000", jyd.__5);
                cmd.Parameters.AddWithValue("@_2000", jyd.__2);
                cmd.Parameters.AddWithValue("@_100", jyd.__1);
                cmd.Connection = con;
                return (long) cmd.ExecuteScalar();
            }
        }

        public DataTable GetJiaoYiDanByTime(DateTime date)
        {
            using (var con = new SqlConnection(Common.constr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = string.Format("select * from JIAOYIDAN where SHIJIAN = '{0}' and  ZONGE != 0",date);
                cmd.Connection = con;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public DataTable GetJiaoYiDanByJiaoYiDanId(long JiaoYiDanId)
        {
            using (var con = new SqlConnection(Common.constr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = string.Format("select * from JIAOYIDAN where JIAOYIDANID = '{0}' and  ZONGE != 0", JiaoYiDanId);
                cmd.Connection = con;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }
    }
}
