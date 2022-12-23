using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LiuShuiZhang2._0.BLL;
using LiuShuiZhang2._0.DAL;

namespace LiuShuiZhang2._0
{
    public partial class JiaoYiDanViewing : Form
    {
        private BLL_JiaoYi jiaoYi;
        public BLL_JiaoYi JiaoYi { get => jiaoYi; set => jiaoYi = value; }

        private DateTime date;
        public DateTime Date { get => date; set => date = value; }

        DAL_JiaoYiDan DAL_jiaoYiDan;
        private void JiaoYiDanViewing_Load(object sender, EventArgs e)
        {
            DAL_jiaoYiDan = new DAL_JiaoYiDan();
            LoadData(date, jiaoYi);
        }

        public JiaoYiDanViewing()
        {
            InitializeComponent();
        }

        private void LoadData(DateTime date, BLL_JiaoYi jiaoYi)
        {
            DataTable dt_jiaoYiDan = DAL_jiaoYiDan.GetJiaoYiDanByTime(date);
            dataGridView_JiaoYiDan.DataSource = dt_jiaoYiDan;
            dataGridView_JiaoYiDan.Columns["JIAOYIDANID"].HeaderText = "交易单码";
            dataGridView_JiaoYiDan.Columns["SHIJIAN"].HeaderText = "时间";
            dataGridView_JiaoYiDan.Columns["ZONGE"].HeaderText = "总额";
            dataGridView_JiaoYiDan.Columns["_500000"].Visible = false;
            dataGridView_JiaoYiDan.Columns["_200000"].Visible = false;
            dataGridView_JiaoYiDan.Columns["_100000"].Visible = false;
            dataGridView_JiaoYiDan.Columns["_50000"].Visible = false;
            dataGridView_JiaoYiDan.Columns["_20000"].Visible = false;
            dataGridView_JiaoYiDan.Columns["_10000"].Visible = false;
            dataGridView_JiaoYiDan.Columns["_5000"].Visible = false;
            dataGridView_JiaoYiDan.Columns["_2000"].Visible = false;
            dataGridView_JiaoYiDan.Columns["_1000"].Visible = false;

            foreach (DataGridViewRow row in dataGridView_JiaoYiDan.Rows)
            {
                if ((decimal)row.Cells["ZONGE"].Value < 0)
                {
                    row.DefaultCellStyle.BackColor = Color.Yellow;
                }
            }
        }

        private void dataGridView_JiaoYiDan_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView d = sender as DataGridView;
            label_JiaoYiDanID.Text = d.Rows[e.RowIndex].Cells["JIAOYIDANID"].Value.ToString();
            label_Time.Text = string.Format("{0:yyyy/MM/dd}", (DateTime)d.Rows[e.RowIndex].Cells["SHIJIAN"].Value);
            label_ToltalValue.Text = string.Format("{0:n0}", (decimal)d.Rows[e.RowIndex].Cells["ZONGE"].Value);

            DataTable dt = new DataTable();
            dt.Columns.AddRange(new DataColumn[]{
                new DataColumn("_500000"),
                new DataColumn("_200000"),
                new DataColumn("_100000"),
                new DataColumn("_50000"),
                new DataColumn("_20000"),
                new DataColumn("_10000"),
                new DataColumn("_5000"),
                new DataColumn("_2000"),
                new DataColumn("_1000")
            });
            DataRow row = dt.NewRow();
            foreach (DataGridViewColumn cl in d.Columns)
            {
                if (cl.Name.StartsWith("_"))
                    row[cl.Name] = int.Parse(d.Rows[e.RowIndex].Cells[cl.Name].Value.ToString());
            }
            dt.Rows.Add(row);
            dataGridView_CashDetails.DataSource = dt;
            dataGridView_CashDetails.Columns["_500000"].HeaderText = "500K";
            dataGridView_CashDetails.Columns["_200000"].HeaderText = "200K";
            dataGridView_CashDetails.Columns["_100000"].HeaderText = "100K";
            dataGridView_CashDetails.Columns["_50000"].HeaderText = "50K";
            dataGridView_CashDetails.Columns["_20000"].HeaderText = "20K";
            dataGridView_CashDetails.Columns["_10000"].HeaderText = "10K";
            dataGridView_CashDetails.Columns["_5000"].HeaderText = "5K";
            dataGridView_CashDetails.Columns["_2000"].HeaderText = "2K";
            dataGridView_CashDetails.Columns["_1000"].HeaderText = "1K";


        }
    }
}
