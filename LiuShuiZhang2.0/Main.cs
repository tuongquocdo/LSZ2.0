using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LiuShuiZhang2._0.DAL;
using LiuShuiZhang2._0.BLL;

namespace LiuShuiZhang2._0
{
    public partial class Main : Form
    {
        DAL_LiuShui DAL_liuShui;

        DAL_BiZhong DAL_biZhong;

        private BLL_User user;
        public BLL_User User { get => user; set => user = value; }

        private bool cashCountingMode;
        public bool CashCountingMode { get => cashCountingMode; set => cashCountingMode = value; }
        
        public Main()
        {
            InitializeComponent();
        }

        #region Other
        private void Control_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Return))
            {
                this.SelectNextControl((Control)sender, true, true, true, true);
            }
            else if (e.KeyCode == Keys.PageUp)
            {
                this.SelectNextControl((Control)sender, false, true, true, true);
            }
            else if (e.KeyCode == Keys.PageDown)
            {
                this.SelectNextControl((Control)sender, true, true, true, true);
            }
        }

        private void NumericUpDown_Enter(object sender, EventArgs e)
        {
            ((NumericUpDown)sender).Select(0, ((NumericUpDown)sender).Text.Length);
        }

        private void ShortKey(object sender, KeyEventArgs e)
        {
            //For Begin Trans
            if (e.Control && e.KeyCode == Keys.A)
            {
                comboBox_Transaction_Type.Focus();
            }
            else if (e.Control && e.KeyCode == Keys.N)
            {
                button_Transaction_NextTran.PerformClick();
            }
            else if (e.Control && e.KeyCode == Keys.S)
            {
                button_Transaction_SaveTran.PerformClick();
            }
            else if (e.Control && e.KeyCode == Keys.C)
            {
                //button_CancelTran.PerformClick();
            }

        }

        private void CashCountingValueChanged(object sender, EventArgs e)
        {
            NumericUpDownEx s = (NumericUpDownEx)sender;

            numericUpDown_CashCounting_TotalCashCounting.Value =
                Common.GetAllControlByType(panel_CashCountingTable, typeof(NumericUpDownEx)).Cast<NumericUpDownEx>()
                                            .Where(item => item.Tag.ToString() == "text")
                                            .Sum(item => item.Value * decimal.Parse(item.Name.Split('_')[2]));

            foreach (DataGridViewColumn c in dataGridView_CashStatus_CashDetails.Columns)
            {
                if (int.Parse(c.Name.Split('_')[1]) == int.Parse(s.Name.Split('_')[2]))
                {
                    dataGridView_CashStatus_CashDetails.Rows[0].Cells[c.Name].Value =
                        int.Parse(dataGridView_CashStatus_CashDetails.Rows[0].Cells[c.Name].Value.ToString()) -
                        s.Valued + 
                        s.Value;
                }
            }
        }
        #endregion

        #region Menu Strip 

        private void ToolStripMenuItem_User_Click(object sender, EventArgs e)
        {
            User us = new User();
            us.ShowDialog();
        }

        private void ToolStripMenuItem_Type_Click(object sender, EventArgs e)
        {
            BiZhong bz = new BiZhong();
            bz.ShowDialog();
        }

        private void ToolStripMenuItem_Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        #endregion

        #region Main

        #region From Main
        private void Main_Load(object sender, EventArgs e)
        {
            #region Handle User Permision
            ///test
            user = new BLL_User()
            {
                UserID = 2,
                UserName = "admin"
            };
            label1_HandleUser.Text = "管理人员: " + user.UserName;

            DAL_User u = new DAL_User();
            if (u.IsUserAdmnin(user.UserID))
            {
                ToolStripMenuItem_User.Visible = true;
                ToolStripMenuItem_Type.Visible = true;

            }
            #endregion

            dateTimePicker.MaxDate = DateTime.Now;
            DAL_liuShui = new DAL_LiuShui();
            DAL_biZhong = new DAL_BiZhong();
            FillDataToForm();
        }

        private void Main_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (MessageBox.Show("确认退出？", "温卿提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                e.Cancel = true;
        }

        private void dateTimePicker_ValueChanged(object sender, EventArgs e)
        {
            FillDataToForm();
        }

        #endregion

        #region Cash Status
        private void dataGridView_CashDetails_SelectionChanged(object sender, EventArgs e)
        {
            (sender as DataGridView).ClearSelection();
        }

        private void dataGridView_CashDetails_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {

            numericUpDown_CashStatus_CountValue.Value =
                ((IEnumerable<DataGridViewColumn>)dataGridView_CashStatus_CashDetails.Columns.Cast<DataGridViewColumn>())
                                            .Sum(item => int.Parse(dataGridView_CashStatus_CashDetails.Rows[0].Cells[item.Name].Value.ToString()) *
                                                         int.Parse(item.Name.Split('_')[1].ToString()));
            numericUpDown_CashStatus_DeltaValue.Value = numericUpDown_CashStatus_CountValue.Value - numericUpDown_CashStatus_CurValue.Value;
        }
        private void button_CashCouterMode_Click(object sender, EventArgs e)
        {
            groupBox_CashCounting.Enabled = cashCountingMode = true;
            groupBox_Transaction.Enabled = groupBox_LiuShui.Enabled = false;
            ((Control)sender).Enabled = false;
            numericUpDown_CashCounting_500000.Focus();

        }

        #endregion

        #region Transaction

        private void button_CancelCashCounting_Click(object sender, EventArgs e)
        {
            ClearCashCountingTable();
            Main_Load(sender, e);
            groupBox_CashCounting.Enabled = cashCountingMode = false;
            groupBox_Transaction.Enabled = groupBox_LiuShui.Enabled = true;
            button_CashStatus_CashCouterMode.Enabled = true;
        }
        private void numericUpDown_Transaction_ValueChanged(object sender, EventArgs e)
        {
            numericUpDown_Transaction_Total.Value = 
                Math.Round(numericUpDown_Transaction_Quan.Value * numericUpDown_Transaction_Price.Value / 1000)*1000;
        }
        private void button_Fix_Click(object sender, EventArgs e)
        {

        }

        #endregion

        #endregion

        #region Private method

        private void ClearCashCountingTable()
        {
            foreach (var c in Common.GetAllControlByType(panel_CashCountingTable, typeof(NumericUpDown)))
            {
                ((NumericUpDown)c).Value = 0;
            }
        }

        private void ChangeWorkingMode(int mode)
        {
            if (mode == 1) // Working mode
            {
                groupBox_CashStatus.Enabled = true;
                groupBox_Transaction.Enabled = true;
                groupBox_CashCounting.Enabled = false;
                groupBox_LiuShui.Enabled = true;
            }
            else if (mode == 2) // viewing mode
            {
                groupBox_CashStatus.Enabled = false;
                groupBox_Transaction.Enabled = false;
                groupBox_CashCounting.Enabled = false;
                groupBox_LiuShui.Enabled = true;
            }
        }

        private void FillDataToForm()
        {
            #region Load Cash Details

            if (dataGridView_CashStatus_CashDetails.Rows.Count == 0) { dataGridView_CashStatus_CashDetails.Rows.Add(new DataGridViewRow()); }

            DataTable dt_LastLiuShui = DAL_liuShui.GetLastRecord();
            

            if (dt_LastLiuShui.Rows.Count > 0)
            {
                int gg = DateTime.Compare(((DateTime)dt_LastLiuShui.Rows[0]["RIZI"]).Date, dateTimePicker.Value.Date);

                //Working mode
                if (gg == 0)
                {
                    ChangeWorkingMode(1);
                    numericUpDown_CashStatus_PreValue.Value = decimal.Parse(dt_LastLiuShui.Rows[0]["QIANE"].ToString().Trim());
                    numericUpDown_CashStatus_CurValue.Value = decimal.Parse(dt_LastLiuShui.Rows[0]["XIANE"].ToString().Trim());
                    numericUpDown_CashStatus_DeltaValue.Value = decimal.Parse(dt_LastLiuShui.Rows[0]["XIANGCHA"].ToString().Trim());
                    numericUpDown_CashStatus_CountValue.Value = decimal.Parse(dt_LastLiuShui.Rows[0]["DIANSUANJIEGUO"].ToString().Trim());
                    foreach (DataGridViewColumn i in dataGridView_CashStatus_CashDetails.Columns)
                    {
                        dataGridView_CashStatus_CashDetails.Rows[0].Cells[i.Name].Value = dt_LastLiuShui.Rows[0][i.Name];
                        dataGridView_CashStatus_CashDetails.Rows[0].Cells[i.Name].ValueType = typeof(int);
                    }
                }
                //view data mode
                else if (gg > 0)
                {
                    ChangeWorkingMode(2);
                    DataTable dt_PreviousLiuShui = DAL_liuShui.GetRecordByDate(dateTimePicker.Value.Date);

                    if (dt_PreviousLiuShui.Rows.Count > 0)
                    {
                        MessageBox.Show("View data mode");
                    }
                    else
                    {
                        MessageBox.Show("该日子并没有存在流水账", "温卿提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                //create new day data for using
                else
                {
                    ChangeWorkingMode(1);
                    numericUpDown_CashStatus_PreValue.Value = 
                    numericUpDown_CashStatus_CurValue.Value = 
                    numericUpDown_CashStatus_CountValue.Value = 
                    decimal.Parse(dt_LastLiuShui.Rows[0]["DIANSUANJIEGUO"].ToString().Trim());
                    numericUpDown_CashStatus_DeltaValue.Value = 0;
                    foreach (DataGridViewColumn i in dataGridView_CashStatus_CashDetails.Columns)
                    {
                        dataGridView_CashStatus_CashDetails.Rows[0].Cells[i.Name].Value = dt_LastLiuShui.Rows[0][i.Name];
                        dataGridView_CashStatus_CashDetails.Rows[0].Cells[i.Name].ValueType = typeof(int);
                    }

                    try
                    {
                        DAL_liuShui.AddNewLiuShui(CreateLiuShui());
                        MessageBox.Show(string.Format("{0}的流水账已经创新", dateTimePicker.Value.Date), "温卿提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        this.Refresh();

                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "温卿提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            else // the frist time using
            {
                ChangeWorkingMode(1);
                foreach (DataGridViewColumn i in dataGridView_CashStatus_CashDetails.Columns)
                {
                    dataGridView_CashStatus_CashDetails.Rows[0].Cells[i.Name].Value = 0;
                    dataGridView_CashStatus_CashDetails.Rows[0].Cells[i.Name].ValueType = typeof(int);
                }
                try
                {
                    DAL_liuShui.AddNewLiuShui(CreateLiuShui());
                    MessageBox.Show(string.Format("{0}的流水账已经创新", dateTimePicker.Value.Date), "温卿提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    this.Refresh();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "温卿提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            dataGridView_CashStatus_CashDetails.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CashDetails_CellValueChanged);
            #endregion

            #region Load BiZhong

            comboBox_Transaction_Type.DataSource = DAL_biZhong.GetAllBiZhong();
            comboBox_Transaction_Type.DisplayMember = "BIZHONG";
            comboBox_Transaction_Type.ValueMember = "BIZHONGID";

            #endregion
        }

        private BLL_LiuShui CreateLiuShui()
        {
            return new BLL_LiuShui()
            {
                LiuShuiDate = dateTimePicker.Value.Date,
                PreValue = numericUpDown_CashStatus_PreValue.Value,
                CurValue = numericUpDown_CashStatus_CurValue.Value,
                DeltaValue = numericUpDown_CashStatus_DeltaValue.Value,
                CountValue = numericUpDown_CashStatus_CountValue.Value,
                __500 = int.Parse(dataGridView_CashStatus_CashDetails.Rows[0].Cells["_500000"].Value.ToString()),
                __200 = int.Parse(dataGridView_CashStatus_CashDetails.Rows[0].Cells["_200000"].Value.ToString()),
                __100 = int.Parse(dataGridView_CashStatus_CashDetails.Rows[0].Cells["_100000"].Value.ToString()),
                __50 = int.Parse(dataGridView_CashStatus_CashDetails.Rows[0].Cells["_50000"].Value.ToString()),
                __20 = int.Parse(dataGridView_CashStatus_CashDetails.Rows[0].Cells["_20000"].Value.ToString()),
                __10 = int.Parse(dataGridView_CashStatus_CashDetails.Rows[0].Cells["_10000"].Value.ToString()),
                __5 = int.Parse(dataGridView_CashStatus_CashDetails.Rows[0].Cells["_5000"].Value.ToString()),
                __2 = int.Parse(dataGridView_CashStatus_CashDetails.Rows[0].Cells["_2000"].Value.ToString()),
                __1 = int.Parse(dataGridView_CashStatus_CashDetails.Rows[0].Cells["_1000"].Value.ToString()),
            };
        }



        #endregion

        
    }
    
}
