using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LiuShuiZhang2._0.BLL;

namespace LiuShuiZhang2._0.DAL
{
    public class DAL_BiZhong
    {
        public DataTable GetAllBiZhong()
        {
            using (var con = new SqlConnection(Common.constr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select * from BIZHONG";
                cmd.Connection = con;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public bool BiZhongExisted(string biZhong)
        {
            using (var con = new SqlConnection(Common.constr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select count(*) from BIZHONG where BIZHONG = @BIZHONG";
                cmd.Parameters.AddWithValue("@BIZHONG", biZhong);
                cmd.Connection = con;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return int.Parse(dt.Rows[0][0].ToString()) == 0 ? false : true;
            }
        }

        public void AddBiZhong(BLL_BiZhong b)
        {
            using (var con = new SqlConnection(Common.constr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = string.Format("insert into BIZHONG(BIZHONG,LIANG,PINGJUNJIA,YIGONG,LEI,TINGYONG) values (@BIZHONG,@LIANG,@PINGJUNJIA,@YIGONG,@LEI,@TINGYONG)");
                
                cmd.Parameters.AddWithValue("@BIZHONG", b.BiZhong);
                cmd.Parameters.AddWithValue("@LIANG", b.Quantity);
                cmd.Parameters.AddWithValue("@PINGJUNJIA", b.AveragePrice);
                cmd.Parameters.AddWithValue("@YIGONG", b.TotalValue);
                cmd.Parameters.AddWithValue("@LEI", b.Type);
                cmd.Parameters.AddWithValue("@TINGYONG", b.Disable);
                cmd.Connection = con;
                cmd.ExecuteNonQuery();
            }
        }

        public void EditBiZhong(BLL_BiZhong b)
        {
            using (var con = new SqlConnection(Common.constr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = string.Format("update BIZHONG set BIZHONG=@BIZHONG , LEI=@LEI, TINGYONG=@TINGYONG where BIZHONGID=@BIZHONGID");
                cmd.Parameters.AddWithValue("@BIZHONGID", b.BiZhongID);
                cmd.Parameters.AddWithValue("@BIZHONG", b.BiZhong);
                cmd.Parameters.AddWithValue("@LEI", b.Type);
                cmd.Parameters.AddWithValue("@TINGYONG", b.Disable);
                cmd.Connection = con;
                cmd.ExecuteNonQuery();
            }
        }

        public int GetLeiOfBiZhong(int biZhongId)
        {
            using (var con = new SqlConnection(Common.constr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select LEI from BIZHONG where BIZHONGID = @BIZHONGID";
                cmd.Parameters.AddWithValue("@BIZHONGID",biZhongId);
                cmd.Connection = con;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return int.Parse(dt.Rows[0][0].ToString());
            }
        }
    }
}
