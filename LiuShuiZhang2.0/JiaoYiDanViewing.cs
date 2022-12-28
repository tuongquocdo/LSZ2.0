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

        DAL_JiaoYiDan DAL_jiaoYiDan;
        DAL_JiaoYi DAL_jiaoYi;
        private void JiaoYiDanViewing_Load(object sender, EventArgs e)
        {
            DAL_jiaoYiDan = new DAL_JiaoYiDan();
            DAL_jiaoYi = new DAL_JiaoYi();
            LoadData(jiaoYi);
        }

        public JiaoYiDanViewing()
        {
            InitializeComponent();
        }

        private void LoadData(BLL_JiaoYi jiaoYi)
        {
            dateTimePicker.Value = jiaoYi.Time;

            DataTable dt_jiaoYiDan;
            if (jiaoYi.JiaoYiDanID == 0)
            {
                dt_jiaoYiDan = DAL_jiaoYiDan.GetJiaoYiDanByTime(jiaoYi.Time);
            }
            else
            {
                dt_jiaoYiDan = DAL_jiaoYiDan.GetJiaoYiDanByJiaoYiDanId(jiaoYi.JiaoYiDanID);
            }

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

            if (jiaoYi.JiaoYiDanID != 0)
            {
                ShowJiaoYi(0);
            }
        }

        private void dataGridView_JiaoYiDan_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            ShowJiaoYi(e.RowIndex);
        }

        private void ShowJiaoYi(int index)
        {
            label_JiaoYiDanID.Text = dataGridView_JiaoYiDan.Rows[index].Cells["JIAOYIDANID"].Value.ToString();
            label_Time.Text = string.Format("{0:yyyy/MM/dd}", (DateTime)dataGridView_JiaoYiDan.Rows[index].Cells["SHIJIAN"].Value);
            label_ToltalValue.Text = string.Format("{0:n0}", (decimal)dataGridView_JiaoYiDan.Rows[index].Cells["ZONGE"].Value);

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
            foreach (DataGridViewColumn cl in dataGridView_JiaoYiDan.Columns)
            {
                if (cl.Name.StartsWith("_"))
                    row[cl.Name] = int.Parse(dataGridView_JiaoYiDan.Rows[index].Cells[cl.Name].Value.ToString());
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

            dataGridView_JiaoYi.DataSource = DAL_jiaoYi.GetAllRecordByJiaoYiDanId((long)dataGridView_JiaoYiDan.Rows[index].Cells["JIAOYIDANID"].Value);
            dataGridView_JiaoYi.ReadOnly = true;
            dataGridView_JiaoYi.Columns["JIAOYIID"].Visible = false;
            dataGridView_JiaoYi.Columns["JIAOYIDANID"].Visible = false;
            dataGridView_JiaoYi.Columns["RENYUANID"].Visible = false;
            dataGridView_JiaoYi.Columns["LIUSHUIID"].Visible = false;
            dataGridView_JiaoYi.Columns["BIZHONGID"].Visible = false;
            dataGridView_JiaoYi.Columns["QIANDANID"].Visible = false;
            dataGridView_JiaoYi.Columns["BIZHONG"].HeaderText = "币种";

            dataGridView_JiaoYi.Columns["LIANG"].HeaderText = "数量";
            dataGridView_JiaoYi.Columns["LIANG"].DefaultCellStyle.Format = "N2";

            dataGridView_JiaoYi.Columns["JIA"].HeaderText = "价格";
            dataGridView_JiaoYi.Columns["JIA"].DefaultCellStyle.Format = "N2";
            dataGridView_JiaoYi.Columns["YIGONG"].HeaderText = "一共";

            dataGridView_JiaoYi.Columns["YIGONG"].DefaultCellStyle.Format = "N2";
            dataGridView_JiaoYi.Columns["BEIZHU"].HeaderText = "备注";

            dataGridView_JiaoYi.Columns["SHIJIAN"].HeaderText = "时间";
            dataGridView_JiaoYi.Columns["SHIJIAN"].DefaultCellStyle.Format = "yyyy/MM/dd HH:mm:ss";

        }
    }
}
