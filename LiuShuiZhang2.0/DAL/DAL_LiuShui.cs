using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using LiuShuiZhang2._0.BLL;

namespace LiuShuiZhang2._0.DAL
{
    public class DAL_LiuShui
    {
        public DataTable GetLastRecord()
        {
            using (var con = new SqlConnection(Common.constr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select top 1 * from LIUSHUI order by LIUSHUIID desc";
                cmd.Connection = con;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public DataTable GetRecordByDate(DateTime date)
        {
            using (var con = new SqlConnection(Common.constr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select * from LIUSHUI where RIZI = @RIZI";
                cmd.Parameters.AddWithValue("@RIZI",date);
                cmd.Connection = con;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public void AddNewLiuShui(BLL_LiuShui l)
        {
            using (var con = new SqlConnection(Common.constr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = string.Format("insert into LIUSHUI(RIZI,QIANE,XIANE,XIANGCHA,DIANSUANJIEGUO,_500000,_200000,_100000,_50000,_20000,_10000,_5000,_2000,_1000)" +
                                                "values (@RIZI,@QIANE,@XIANE,@XIANGCHA,@DIANSUANJIEGUO,@_500000,@_200000,@_100000,@_50000,@_20000,@_10000,@_5000,@_2000,@_1000)");
                cmd.Parameters.AddWithValue("@RIZI", l.LiuShuiDate);
                cmd.Parameters.AddWithValue("@QIANE", l.PreValue);
                cmd.Parameters.AddWithValue("@XIANE", l.CurValue);
                cmd.Parameters.AddWithValue("@XIANGCHA",l.DeltaValue);
                cmd.Parameters.AddWithValue("@DIANSUANJIEGUO", l.CountValue);
                cmd.Parameters.AddWithValue("@_500000", l.__500);
                cmd.Parameters.AddWithValue("@_200000", l.__200);
                cmd.Parameters.AddWithValue("@_100000", l.__100);
                cmd.Parameters.AddWithValue("@_50000", l.__50);
                cmd.Parameters.AddWithValue("@_20000", l.__20);
                cmd.Parameters.AddWithValue("@_10000", l.__10);
                cmd.Parameters.AddWithValue("@_5000", l.__5);
                cmd.Parameters.AddWithValue("@_2000", l.__2);
                cmd.Parameters.AddWithValue("@_1000", l.__1);

                cmd.Connection = con;
                cmd.ExecuteNonQuery();
            }
        }
    }
}
