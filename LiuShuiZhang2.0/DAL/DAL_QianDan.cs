using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiuShuiZhang2._0.DAL
{
    public class DAL_QianDan
    {
        public void AddNewQianDan(LiuShuiZhang2._0.BLL.BLL_QianDan q)
        {
            using (var con = new SqlConnection(Common.constr))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.CommandText = string.Format("insert into QIANDAN(SHIJIAN,QIANDANE,WANCHENG) values (@SHIJIAN,@QIANDANE,@WANCHENG)");
                cmd.Parameters.AddWithValue("@SHIJIAN", q.Time);
                cmd.Parameters.AddWithValue("@QIANDANE", q.QianDanValue);
                cmd.Parameters.AddWithValue("@WANCHENG", q.Finish);
                cmd.Connection = con;
                cmd.ExecuteNonQuery();
            }
        }
    }
}
