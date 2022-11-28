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

        private void Test_Load(object sender, EventArgs e)
        {
            DAL.DAL_LiuShui ls = new DAL.DAL_LiuShui();
            DataTable tb = ls.GetLastRecord();

            BindingList<BLL.BLL_LiuShui> liushuis = new BindingList<BLL.BLL_LiuShui>();
            foreach (DataRow l in tb.Rows)
            {
                BLL.BLL_LiuShui liushui = new BLL.BLL_LiuShui {
                    __500 = Convert.ToInt64(l["_500000"].ToString()),
                    __200 = Convert.ToInt64(l["_200000"].ToString()),
                    __100 = Convert.ToInt64(l["_100000"].ToString()),
                    __50 = Convert.ToInt64(l["_50000"].ToString()),
                    __20 = Convert.ToInt64(l["_20000"].ToString()),
                    __10 = Convert.ToInt64(l["_10000"].ToString()),
                    __5 = Convert.ToInt64(l["_5000"].ToString()),
                    __2 = Convert.ToInt64(l["_2000"].ToString()),
                    __1 = Convert.ToInt64(l["_1000"].ToString()),
                };
                liushuis.Add(liushui);
            }
            dataGridView1.DataSource = liushuis;
            
        }

        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (e.FormattedValue.GetType() != dataGridView1.CurrentCell.ValueType.UnderlyingSystemType)
                MessageBox.Show("Input type is wrong");
        }
    }
}
