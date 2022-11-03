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
    public class DAL_User
    {
        public DataTable GetAllUser()
        {
            using (var con = new SqlConnection(Common.constr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select * from RENYUAN";
                cmd.Connection = con;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return dt;
            }
        }

        public bool IsUserAdmnin(long UserId)
        {
            // quan=1 -> ADMIN
            using (var con = new SqlConnection(Common.constr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select QUAN from RENYUAN where RENYUANID=@RENYUANID";
                cmd.Parameters.AddWithValue("@RENYUANID", UserId);
                cmd.Connection = con;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return int.Parse(dt.Rows[0][0].ToString()) == 1 ? true : false;
            }
        }

        public bool UserNameExisted(string userName)
        {
            using (var con = new SqlConnection(Common.constr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select count(*) from RENYUAN where YONGHU = @YONGHU";
                cmd.Parameters.AddWithValue("@YONGHU", userName);
                cmd.Connection = con;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                return int.Parse(dt.Rows[0][0].ToString()) == 0 ? false : true;
            }
        }

        public void AddNewUser(LiuShuiZhang2._0.BLL.BLL_User u)
        {
            using (var con = new SqlConnection(Common.constr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = string.Format("insert into RENYUAN(YONGHU,MIMA,QUAN,TINGYONG) values (@YONGHU,@MIMA,@QUAN,@TINGYONG)");
                cmd.Parameters.AddWithValue("@YONGHU", u.UserName);
                cmd.Parameters.AddWithValue("@MIMA", u.PassWord);
                cmd.Parameters.AddWithValue("@QUAN", u.Permission);
                cmd.Parameters.AddWithValue("@TINGYONG", u.Disable);
                cmd.Connection = con;
                cmd.ExecuteNonQuery();
            }
        }

        public void EditNewUser(LiuShuiZhang2._0.BLL.BLL_User u)
        {
            using (var con = new SqlConnection(Common.constr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = string.Format("update RENYUAN set MIMA=@MIMA , QUAN=@QUAN, TINGYONG=@TINGYONG where RENYUANID=@RENYUANID");
                cmd.Parameters.AddWithValue("@RENYUANID", u.UserID);
                cmd.Parameters.AddWithValue("@MIMA", u.PassWord);
                cmd.Parameters.AddWithValue("@QUAN", u.Permission);
                cmd.Parameters.AddWithValue("@TINGYONG", u.Disable);
                cmd.Connection = con;
                cmd.ExecuteNonQuery();
            }
        }

        public LiuShuiZhang2._0.BLL.BLL_User CheckUserLoginValid(LiuShuiZhang2._0.BLL.BLL_User u)
        {
            using (var con = new SqlConnection(Common.constr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = "select * from RENYUAN where YONGHU=@YONGHU and MIMA=@MIMA";
                cmd.Parameters.AddWithValue("@YONGHU", u.UserName);
                cmd.Parameters.AddWithValue("@MIMA", u.PassWord);
                cmd.Connection = con;
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                DataTable dt = new DataTable();
                da.Fill(dt);
                if (dt.Rows.Count > 0)
                    return new LiuShuiZhang2._0.BLL.BLL_User()
                        {
                            UserID = long.Parse(dt.Rows[0]["RENYUANID"].ToString()),
                            UserName = dt.Rows[0]["YONGHU"].ToString(),
                            PassWord = dt.Rows[0]["MIMA"].ToString(),
                            Permission = int.Parse(dt.Rows[0]["QUAN"].ToString()),
                            Disable = bool.Parse(dt.Rows[0]["TINGYONG"].ToString())
                        };
                else
                    return null;
            }
        }
    }
}
