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
        DataTable mainTrans = new DataTable("DataTable_MainTransaction");

        DAL_LiuShui DAL_liuShui;

        DAL_BiZhong DAL_biZhong;

        BLL_JiaoYi BLL_jiaoYi;

        private BLL_User user;
        public BLL_User User { get => user; set => user = value; }

        DataTable dt_LastLiuShui;

        private bool cashCountingMode;
        public bool CashCountingMode { get => cashCountingMode; set => cashCountingMode = value; }

        public Main()
        {
            InitializeComponent();
        }

        #region Other
        private void Control_KeyDown(object sender, KeyEventArgs e)
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
            BLL_jiaoYi = new BLL_JiaoYi();
            
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
            try
            {
                numericUpDown_Transaction_Total.Value =
                   Math.Round(numericUpDown_Transaction_Quan.Value * numericUpDown_Transaction_Price.Value / 1000) * 1000 * -1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "温卿提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            CalcAfterFee();
        }

        private void button_Transaction_NextTran_Click(object sender, EventArgs e)
        {
            if (CheckTranInfo())
            {
                dataGridView_Transaction_MainTran.Rows.Add(
                user.UserID,
                dt_LastLiuShui.Rows[0]["LIUSHUIID"].ToString(),
                comboBox_Transaction_Type.SelectedValue,
                0,
                comboBox_Transaction_Type.Text,
                numericUpDown_Transaction_Quan.Value,
                Math.Abs(numericUpDownEx_Transaction_AfterFee.Value / numericUpDown_Transaction_Quan.Value),
                numericUpDownEx_Transaction_AfterFee.Value,
                textBox_Transaction_Note.Text
                ) ;
                ClearTransactionData();
            }
        }

        private void button_Transaction_SaveToTemp_Click(object sender, EventArgs e)
        {
            if (dataGridView_Transaction_MainTran.RowCount > 0)
            {
                if (dataGridView_Transaction_MainTran.SelectedRows.Count != 0)
                {
                    foreach (DataGridViewRow selectedRow in dataGridView_Transaction_MainTran.SelectedRows)
                    {
                        dataGridView_Transaction_TempTran.Rows.Add(
                        selectedRow.Cells["DataGridViewColumn_REYUANID"].Value,
                        selectedRow.Cells["DataGridViewColumn_LIUSHUIID"].Value,
                        selectedRow.Cells["DataGridViewColumn_BIZHONGID"].Value,
                        selectedRow.Cells["DataGridViewColumn_QIANDANID"].Value,
                        selectedRow.Cells["DataGridViewColumn_BIZHONG"].Value,
                        selectedRow.Cells["DataGridViewColumn_LIANG_N2"].Value,
                        selectedRow.Cells["DataGridViewColumn_JIA_N2"].Value,
                        selectedRow.Cells["DataGridViewColumn_YIGONG_N2"].Value,
                        selectedRow.Cells["DataGridViewColumn_BEIZHU"].Value
                        );

                        dataGridView_Transaction_MainTran.Rows.RemoveAt(selectedRow.Index);
                    }

                }
                else
                {
                    MessageBox.Show("请选择要搁置的交易", "温卿提示", MessageBoxButtons.OK, MessageBoxIcon.Question);
                }


            }
        }

        private void button_Transaction_CancelTran_Click(object sender, EventArgs e)
        {
            Main_Load(this, null);
            ClearTransactionData();
            ClearTransactionTable();
        }


        private void comboBox_Transaction_Type_SelectedIndexChanged(object sender, EventArgs e)
        {
            string temp = (sender as ComboBox).SelectedValue.ToString();
            if (Common.IsNumber(temp))
            {
                int biZhongID = int.Parse(temp);
                try
                {
                    if (DAL_biZhong.GetLeiOfBiZhong(biZhongID) == (int)BLL_BiZhong.BiZhongLei.KeRenQian)
                    {
                        numericUpDown_Transaction_Quan.Minimum = numericUpDown_Transaction_Quan.Maximum = 1;
                        numericUpDown_Transaction_Quan.ReadOnly = true;
                        comboBox_Transaction_FeeType.Enabled = false;
                        numericUpDownEx_Transaction_Fee.Enabled = false;
                    }
                    else if (DAL_biZhong.GetLeiOfBiZhong(biZhongID) == (int)BLL_BiZhong.BiZhongLei.QianKeRen)
                    {
                        numericUpDown_Transaction_Quan.Minimum = numericUpDown_Transaction_Quan.Maximum = -1;
                        numericUpDown_Transaction_Quan.ReadOnly = true;
                        comboBox_Transaction_FeeType.Enabled = false;
                        numericUpDownEx_Transaction_Fee.Enabled = false;
                    }
                    else if (DAL_biZhong.GetLeiOfBiZhong(biZhongID) == (int)BLL_BiZhong.BiZhongLei.DianZiZhang ||
                            DAL_biZhong.GetLeiOfBiZhong(biZhongID) == (int)BLL_BiZhong.BiZhongLei.XianJin)
                    {
                        numericUpDown_Transaction_Quan.Minimum = -1;
                        numericUpDown_Transaction_Quan.Maximum = 1;
                        numericUpDown_Transaction_Quan.ReadOnly = false;
                        comboBox_Transaction_FeeType.Enabled = false;
                        numericUpDownEx_Transaction_Fee.Enabled = false;
                    }
                    else if (DAL_biZhong.GetLeiOfBiZhong(biZhongID) == (int)BLL_BiZhong.BiZhongLei.WaiBi)
                    {
                        numericUpDown_Transaction_Quan.Minimum = -999999999999999;
                        numericUpDown_Transaction_Quan.Maximum = 999999999999999;
                        numericUpDown_Transaction_Quan.ReadOnly = false;
                        comboBox_Transaction_FeeType.Enabled = true;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "温卿提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void comboBox_Transaction_FeeType_SelectedIndexChanged(object sender, EventArgs e)
        {
            string temp = (sender as ComboBox).SelectedValue.ToString();

            if (Common.IsNumber(temp))
            {
                if (int.Parse(temp) == (int)BLL_JiaoYi.Enum_FeeTypes.Free)
                {
                    numericUpDownEx_Transaction_Fee.Enabled = false;
                }
                else
                {
                    numericUpDownEx_Transaction_Fee.Enabled = true;
                }
                numericUpDownEx_Transaction_Fee.Value = 0;
                CalcAfterFee();
            }
        }

        private void dataGridView_Transaction_MainTran_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            DataGridView d = sender as DataGridView;

            foreach (DataGridViewColumn columnn in d.Columns)
            {
                columnn.DefaultCellStyle.Format =
                    columnn.Name.Split('_').Length == 3 ? columnn.Name.Split('_')[2] : string.Empty;
            }

            if((decimal)d.Rows[e.RowIndex].Cells["DataGridViewColumn_YIGONG_N2"].Value > 0 )
            {
                d.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Yellow;
            }
            CalcTotallAll();
        }

        private void dataGridView_Transaction_MainTran_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            CalcTotallAll();
        }

        private void button_Fix_Click(object sender, EventArgs e)
        {
            if (dataGridView_Transaction_MainTran.RowCount > 0)
            {
                if (dataGridView_Transaction_MainTran.SelectedRows.Count == 1)
                {
                    DataGridViewRow selectedRow = dataGridView_Transaction_MainTran.SelectedRows[0];
                    if (MessageBox.Show("是否要修改合共额，修改后你在交易单上所选择的交易价格会自动地调整至对应的修改额", "温卿提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        selectedRow.Cells["DataGridViewColumn_YIGONG_N2"].Value =
                        (decimal)selectedRow.Cells["DataGridViewColumn_YIGONG_N2"].Value -
                        numericUpDown_Transaction_TotalAll.Value +
                        numericUpDown_Transaction_FixValue.Value;

                        selectedRow.Cells["DataGridViewColumn_JIA_N2"].Value = Math.Abs(
                        (decimal)selectedRow.Cells["DataGridViewColumn_YIGONG_N2"].Value /
                        (decimal)selectedRow.Cells["DataGridViewColumn_LIANG_N2"].Value);
                    }
                }
                else
                {
                    MessageBox.Show("请选择一个要修改的交易", "温卿提示", MessageBoxButtons.OK, MessageBoxIcon.Question);
                }
            }

            numericUpDown_Transaction_FixValue.Value = 0;
        }

        #endregion

        #endregion

        #region Private method

        private void ClearTransactionTable()
        {
            dataGridView_Transaction_MainTran.Rows.Clear();
            numericUpDown_Transaction_TotalAll.Value = 0;
        }

        private void ClearTransactionData()
        {
            comboBox_Transaction_Type.SelectedIndex = 0;
            numericUpDown_Transaction_Quan.Value = 0;
            numericUpDown_Transaction_Price.Value = 0;
            textBox_Transaction_Note.Text = string.Empty;
            numericUpDown_Transaction_Total.Value = 0;
            numericUpDownEx_Transaction_AfterFee.Value = 0;
            numericUpDown_Transaction_FixValue.Value = 0;
            comboBox_Transaction_Type.Focus();
        }

        private void CalcAfterFee()
        {
            if ((int)comboBox_Transaction_FeeType.SelectedValue == (int)BLL_JiaoYi.Enum_FeeTypes.Free)
            {
                numericUpDownEx_Transaction_AfterFee.Value = numericUpDown_Transaction_Total.Value;
            }
            else if ((int)comboBox_Transaction_FeeType.SelectedValue == (int)BLL_JiaoYi.Enum_FeeTypes.Percent)
            {
                numericUpDownEx_Transaction_AfterFee.Value = numericUpDown_Transaction_Total.Value +
                                                               Math.Abs(numericUpDown_Transaction_Total.Value) *
                                                                numericUpDownEx_Transaction_Fee.Value /
                                                                100;
            }
            else if ((int)comboBox_Transaction_FeeType.SelectedValue == (int)BLL_JiaoYi.Enum_FeeTypes.ByBiZhong)
            {
                numericUpDownEx_Transaction_AfterFee.Value = numericUpDown_Transaction_Total.Value +
                                                            numericUpDownEx_Transaction_Fee.Value *
                                                            numericUpDown_Transaction_Price.Value;
            }
            else if ((int)comboBox_Transaction_FeeType.SelectedValue == (int)BLL_JiaoYi.Enum_FeeTypes.ByXianJin)
            {
                numericUpDownEx_Transaction_AfterFee.Value = numericUpDown_Transaction_Total.Value +
                                                            numericUpDownEx_Transaction_Fee.Value;
            }
        }

        private void CalcTotallAll()
        {
            decimal rs = 0;
            foreach (DataGridViewRow r in dataGridView_Transaction_MainTran.Rows)
            {
                rs += (decimal)r.Cells["DataGridViewColumn_YIGONG_N2"].Value;
            }
            numericUpDown_Transaction_TotalAll.Value = rs;
        }

        private bool CheckTranInfo()
        {
            bool checkingResult;
            if (numericUpDown_Transaction_Quan.Value != 0 &&
                numericUpDown_Transaction_Price.Value != 0 &&
                numericUpDown_Transaction_Total.Value != 0)
                checkingResult = true;
            else
            {
                MessageBox.Show("请输入数量，价格","温倾提示",MessageBoxButtons.OK,MessageBoxIcon.Information);
                return false;
            }

            try
            {
                if ((DAL_biZhong.GetLeiOfBiZhong(int.Parse(comboBox_Transaction_Type.SelectedValue.ToString())) == (int)BLL_BiZhong.BiZhongLei.KeRenQian ||
                    DAL_biZhong.GetLeiOfBiZhong(int.Parse(comboBox_Transaction_Type.SelectedValue.ToString())) == (int)BLL_BiZhong.BiZhongLei.QianKeRen ||
                    DAL_biZhong.GetLeiOfBiZhong(int.Parse(comboBox_Transaction_Type.SelectedValue.ToString())) == (int)BLL_BiZhong.BiZhongLei.XianJin))
                {
                    if (textBox_Transaction_Note.Text != string.Empty)
                        checkingResult = true;
                    else
                    {
                        MessageBox.Show("此币种需要输入备注", "温倾提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "温卿提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            return checkingResult;
        }

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

            dt_LastLiuShui = DAL_liuShui.GetLastRecord();
            
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
            dt_LastLiuShui = DAL_liuShui.GetLastRecord();

            #endregion

            #region Load BiZhong

            comboBox_Transaction_Type.DataSource = DAL_biZhong.GetAllBiZhong();
            comboBox_Transaction_Type.DisplayMember = "BIZHONG";
            comboBox_Transaction_Type.ValueMember = "BIZHONGID";
            comboBox_Transaction_Type.SelectedIndex = 1;
            comboBox_Transaction_Type.SelectedIndex = 0;

            #endregion

            #region Initialize Fee Type

            comboBox_Transaction_FeeType.DataSource = BLL_jiaoYi.FeeTypes;
            comboBox_Transaction_FeeType.DisplayMember = "FEETYPE";
            comboBox_Transaction_FeeType.ValueMember = "FEETYPEID";
            comboBox_Transaction_FeeType.SelectedItem = 1;
            comboBox_Transaction_FeeType.SelectedItem = 0;

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

        private void tabControl__Transaction_Tran_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }

}
