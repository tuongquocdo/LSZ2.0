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
        
        DAL_JiaoYiDan DAL_jiaoYiDan;

        DAL_JiaoYi DAL_jiaoYi;

        BLL_JiaoYi BLL_jiaoYi;

        BLL_JiaoYiDan BLL_jiaoYiDan;

        BLL_JiaoYi_BiZhong BLL_jiaoYi_biZhong;
        public BLL_JiaoYiDan BLL_JiaoYiDan { get => BLL_jiaoYiDan; set => BLL_jiaoYiDan = value; }

        private BLL_User BLL_user;
        public BLL_User BLL_User { get => BLL_user; set => BLL_user = value; }

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
                SelectNextControl((Control)sender, true, true, true, true);
            }
            else if (e.KeyCode == Keys.PageUp)
            {
                SelectNextControl((Control)sender, false, true, true, true);
            }
            else if (e.KeyCode == Keys.PageDown)
            {
                SelectNextControl((Control)sender, true, true, true, true);
            }
        }

        private void NumericUpDown_Leave(object sender, EventArgs e)
        {
            if ((sender as NumericUpDownEx).Text == string.Empty)
            {
                (sender as NumericUpDownEx).Text = "0.00";
                (sender as NumericUpDownEx).Value = 0;
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

        private void dataGridView_SelectionChanged_QuickSumValue(object sender, EventArgs e)
        {

            DataGridViewSelectedCellCollection cells = (sender as DataGridView).SelectedCells;
            decimal rs = 0;
            foreach (DataGridViewCell cell in cells)
            {
                if (Common.IsDecimal(cell.Value.ToString()))
                {
                    rs += decimal.Parse(cell.Value.ToString());
                }
            }

            toolStripStatusLabel_QuickSum.Text = rs.ToString("N0");
        }

        private void CashCountingValueChanged(object sender, EventArgs e)
        {
            NumericUpDownEx s = (NumericUpDownEx)sender;

            numericUpDown_CashCounting_TotalCashCounting.Value =
                Common.GetAllControlByType(panel_CashCountingTable, typeof(NumericUpDownEx)).Cast<NumericUpDownEx>()
                                            .Where(item => item.Tag.ToString() == "text")
                                            .Sum(item => item.Value * decimal.Parse(item.Name.Split('_')[2]));
            numericUpDown_CashCount_DeltaValue.Value = numericUpDown_CashCounting_TotalCashCounting.Value - numericUpDown_CashCount_MainTotalAll.Value;

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
            BLL_user = new BLL_User()
            {
                UserID = 2,
                UserName = "admin"
            };
            label1_HandleUser.Text = "管理人员: " + BLL_user.UserName;

            DAL_User u = new DAL_User();
            if (u.IsUserAdmnin(BLL_user.UserID))
            {
                ToolStripMenuItem_User.Visible = true;
                ToolStripMenuItem_Type.Visible = true;

            }
            #endregion

            dateTimePicker.MaxDate = DateTime.Now;
            DAL_liuShui = new DAL_LiuShui();
            DAL_biZhong = new DAL_BiZhong();
            DAL_jiaoYiDan = new DAL_JiaoYiDan();
            DAL_jiaoYi = new DAL_JiaoYi();
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
            CalcCountValueAndDeltaValue();
        }

        private void numericUpdown_CashDetails_CellValueChanged(object sender, EventArgs e)
        {
            CalcCountValueAndDeltaValue();
        }

        private void button_CashCouterMode_Click(object sender, EventArgs e)
        {
            groupBox_CashCounting.Enabled = cashCountingMode = true;
            groupBox_Transaction.Enabled = groupBox_LiuShui.Enabled = groupBox_CashStatus.Enabled  = ((Control)sender).Enabled = false;
            numericUpDown_CashCounting_500000.Focus();

        }

        #endregion

        #region Transaction

        private void numericUpDown_Transaction_ValueChanged(object sender, EventArgs e)
        {
            try
            {
                numericUpDown_Transaction_Total.Value =
                   Math.Round(numericUpDown_Transaction_Quan.Value * numericUpDown_Transaction_Price.Value / 1000) * 1000;
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
                BLL_user.UserID,
                dt_LastLiuShui.Rows[0]["LIUSHUIID"].ToString(),
                comboBox_Transaction_Type.SelectedValue,
                0,
                comboBox_Transaction_Type.Text,
                numericUpDown_Transaction_Quan.Value,
                Math.Round(Math.Abs(numericUpDownEx_Transaction_AfterFee.Value / numericUpDown_Transaction_Quan.Value),2),
                numericUpDownEx_Transaction_AfterFee.Value,
                textBox_Transaction_Note.Text
                );
                ClearTransactionData();
            }
        }
        private void button_Transaction_SaveTran_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确认要进账？", "温卿提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                numericUpDown_CashStatus_CurValue.Value += numericUpDown_Transaction_MainTotalAll.Value;
                numericUpDown_CashCount_MainTotalAll.Value = numericUpDown_Transaction_MainTotalAll.Value;
                #region Handle CashCounting

                groupBox_CashCounting.Enabled = cashCountingMode = true;
                groupBox_Transaction.Enabled = groupBox_LiuShui.Enabled = groupBox_CashStatus.Enabled = false;
                numericUpDown_CashCounting_500000.Focus();

                #endregion
            }
        }

        private void button_Transaction_CancelTran_Click(object sender, EventArgs e)
        {
            Main_Load(this, null);
            ClearTransactionData();
            ClearTransactionTable();
        }

        private void button_Transaction_Temp_Click(object sender, EventArgs e)
        {
            DataGridView dataGridView_Transaction_from =
                Common.GetAllControlByName(this, ((string[])(sender as Button).Tag)[0]).ToList()[0] as DataGridView;

            DataGridView dataGridView_Transaction_to =
                Common.GetAllControlByName(this, ((string[])(sender as Button).Tag)[1]).ToList()[0] as DataGridView;

            if (dataGridView_Transaction_from.RowCount > 0)
            {
                if (dataGridView_Transaction_from.SelectedRows.Count != 0)
                {
                    foreach (DataGridViewRow selectedRow in dataGridView_Transaction_from.SelectedRows)
                    {
                        dataGridView_Transaction_to.Rows.Add(
                        selectedRow.Cells[0].Value,
                        selectedRow.Cells[1].Value,
                        selectedRow.Cells[2].Value,
                        selectedRow.Cells[3].Value,
                        selectedRow.Cells[4].Value,
                        selectedRow.Cells[5].Value,
                        selectedRow.Cells[6].Value,
                        selectedRow.Cells[7].Value,
                        selectedRow.Cells[8].Value
                        );

                        dataGridView_Transaction_from.Rows.RemoveAt(selectedRow.Index);
                    }

                }
                else
                {
                    MessageBox.Show("请选择要搁置的交易", "温卿提示", MessageBoxButtons.OK, MessageBoxIcon.Question);
                }
            }
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

        private void dataGridView_Transaction_Tran_RowsAdded(object sender, DataGridViewRowsAddedEventArgs e)
        {
            DataGridView d = sender as DataGridView;

            if ((decimal)d.Rows[e.RowIndex].Cells[7].Value < 0)
            {
                d.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Yellow;
            }
        }

        private void dataGridView_Transaction_Tran_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            CalcTotallAll(sender);
        }

        private void dataGridView_Transaction_Tran_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            CalcTotallAll(sender);
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
                        selectedRow.Cells["DataGridViewColumn_YIGONG"].Value =
                        (decimal)selectedRow.Cells["DataGridViewColumn_YIGONG"].Value +
                        numericUpDown_Transaction_MainTotalAll.Value -
                        numericUpDown_Transaction_FixValue.Value;

                        selectedRow.Cells["DataGridViewColumn_JIA"].Value = Math.Abs(
                        (decimal)selectedRow.Cells["DataGridViewColumn_YIGONG"].Value /
                        (decimal)selectedRow.Cells["DataGridViewColumn_LIANG"].Value);
                    }
                }
                else
                {
                    MessageBox.Show("请选择一个要修改的交易", "温卿提示", MessageBoxButtons.OK, MessageBoxIcon.Question);
                }
            }

            numericUpDown_Transaction_FixValue.Value = 0;
        }

        private void button_Transaction_ClearTemp_Click(object sender, EventArgs e)
        {
            if (dataGridView_Transaction_TempTran.RowCount > 0)
            {
                if (dataGridView_Transaction_TempTran.SelectedRows.Count > 0)
                {
                    if(MessageBox.Show("是否要删除该草稿？", "温卿提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    foreach (DataGridViewRow r in dataGridView_Transaction_TempTran.SelectedRows)
                    {
                        dataGridView_Transaction_TempTran.Rows.Remove(r);
                    }
                }
                else
                {
                    MessageBox.Show("请选择要清除的交易草稿", "温卿提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }

            }
        }

        #endregion

        #region LiuShui
       
        #endregion

        #region Cash Counting
        private void button_CashCounting_SaveCashCounting_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("请确认进账？", "温卿提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    #region initialize LiuShui
                    BLL_LiuShui BLL_liuShui = CreateLiuShui();
                    BLL_liuShui.LiuShuiID = (long)dt_LastLiuShui.Rows[0]["LIUSHUIID"];
                    #endregion

                    #region initialize JiaoYiDan
                    BLL_jiaoYiDan = CreateJiaoYiDan();
                    #endregion

                    #region initialize JiaoYi List
                    BLL_jiaoYi_biZhong = CreateJiaoYis();
                    #endregion

                    DAL_jiaoYi.AddNewJiaoYis(BLL_jiaoYiDan, BLL_liuShui, BLL_jiaoYi_biZhong);

                    MessageBox.Show("进账成功", "温卿提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    button_CashCounting_CancelCashCounting.PerformClick();
                    button_Transaction_CancelTran.PerformClick();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "温卿提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void button_CancelCashCounting_Click(object sender, EventArgs e)
        {
            ClearCashCountingTable();
            Main_Load(sender, e);
            groupBox_CashCounting.Enabled = cashCountingMode = false;
            groupBox_Transaction.Enabled = groupBox_LiuShui.Enabled = groupBox_CashStatus.Enabled = button_CashStatus_CashCouterMode.Enabled = true;
        }

        #endregion

        #endregion

        #region Private method

        private void CalcCountValueAndDeltaValue()
        {
            numericUpDown_CashStatus_CountValue.Value =
                dataGridView_CashStatus_CashDetails.Columns.Cast<DataGridViewColumn>()
                                            .Sum(item => int.Parse(dataGridView_CashStatus_CashDetails.Rows[0].Cells[item.Name].Value.ToString()) *
                                                         int.Parse(item.Name.Split('_')[1].ToString()));
            numericUpDown_CashStatus_DeltaValue.Value = numericUpDown_CashStatus_CountValue.Value - numericUpDown_CashStatus_CurValue.Value;

        }

        private void ClearTransactionTable()
        {
            dataGridView_Transaction_MainTran.Rows.Clear();
            numericUpDown_Transaction_MainTotalAll.Value = 0;
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

        private void CalcTotallAll(object sender)
        {
            decimal rs = 0;
            foreach (DataGridViewRow r in (sender as DataGridView).Rows)
            {
                rs += (decimal)r.Cells[7].Value;
            }

            List<Control> ls = Common.GetAllControlByName(this, (sender as DataGridView).Tag.ToString()).ToList();
            if (ls != null && ls.Count > 0)
                (ls[0] as NumericUpDownEx).Value = rs * -1;
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
                MessageBox.Show("请输入数量，价格", "温倾提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
            foreach (var c in Common.GetAllControlByType(panel_CashCountingTable, typeof(NumericUpDownEx)))
            {
                ((NumericUpDownEx)c).Value = 0;
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
            numericUpDown_CashStatus_CurValue.ValueChanged += new System.EventHandler(this.numericUpdown_CashDetails_CellValueChanged);
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

            #region Load LiuShui
            dataGridView_LiuShui_Trans.DataSource = DAL_jiaoYi.GetAllRecordByDate(dateTimePicker.Value);
            dataGridView_LiuShui_Trans.Columns["JIAOYIID"].Visible = false;
            dataGridView_LiuShui_Trans.Columns["JIAOYIDANID"].Visible = false;
            dataGridView_LiuShui_Trans.Columns["RENYUANID"].Visible = false;
            dataGridView_LiuShui_Trans.Columns["LIUSHUIID"].Visible = false;
            dataGridView_LiuShui_Trans.Columns["BIZHONGID"].Visible = false;
            dataGridView_LiuShui_Trans.Columns["QIANDANID"].Visible = false;
            dataGridView_LiuShui_Trans.Columns["BIZHONG"].HeaderText = "币种";

            dataGridView_LiuShui_Trans.Columns["LIANG"].HeaderText = "数量";
            dataGridView_LiuShui_Trans.Columns["LIANG"].DefaultCellStyle.Format = "N2";
            dataGridView_LiuShui_Trans.Columns["JIA"].HeaderText = "价格";

            dataGridView_LiuShui_Trans.Columns["JIA"].DefaultCellStyle.Format = "N2";
            dataGridView_LiuShui_Trans.Columns["YIGONG"].HeaderText = "一共";

            dataGridView_LiuShui_Trans.Columns["YIGONG"].DefaultCellStyle.Format = "N2";
            dataGridView_LiuShui_Trans.Columns["BEIZHU"].HeaderText = "备注";

            dataGridView_LiuShui_Trans.Columns["SHIJIAN"].HeaderText = "时间";
            dataGridView_LiuShui_Trans.Columns["SHIJIAN"].DefaultCellStyle.Format = "yyyy/MM/dd HH:mm:ss";

            foreach (DataGridViewRow row in dataGridView_LiuShui_Trans.Rows)
            {
                if ((decimal)row.Cells["LIANG"].Value < 0)
                {
                    row.DefaultCellStyle.BackColor = Color.Yellow;
                }
            }
            

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

        private BLL_JiaoYiDan CreateJiaoYiDan()
        {
            return new BLL_JiaoYiDan()
            {
                Time = dateTimePicker.Value,
                TotalPrice = numericUpDown_Transaction_MainTotalAll.Value,
                __500 = int.Parse(numericUpDown_CashCounting_500000.Value.ToString()),
                __200 = int.Parse(numericUpDown_CashCounting_200000.Value.ToString()),
                __100 = int.Parse(numericUpDown_CashCounting_100000.Value.ToString()),
                __50 = int.Parse(numericUpDown_CashCounting_50000.Value.ToString()),
                __20 = int.Parse(numericUpDown_CashCounting_20000.Value.ToString()),
                __10 = int.Parse(numericUpDown_CashCounting_10000.Value.ToString()),
                __5 = int.Parse(numericUpDown_CashCounting_5000.Value.ToString()),
                __2 = int.Parse(numericUpDown_CashCounting_2000.Value.ToString()),
                __1 = int.Parse(numericUpDown_CashCounting_1000.Value.ToString())
            };
        }

        private BLL_JiaoYi_BiZhong CreateJiaoYis()
        {
            BLL_JiaoYi_BiZhong rs = new BLL_JiaoYi_BiZhong();
            List<BLL_JiaoYi> lstjy = new List<BLL_JiaoYi>();
            List<BLL_BiZhong> lstbz = new List<BLL_BiZhong>();

            foreach (DataGridViewRow row in dataGridView_Transaction_MainTran.Rows)
            {
                BLL_jiaoYi = new BLL_JiaoYi();
                int lei = DAL_biZhong.GetLeiOfBiZhong(int.Parse(row.Cells["DataGridViewColumn_BIZHONGID"].Value.ToString()));
                long biZhongID = long.Parse(row.Cells["DataGridViewColumn_BIZHONGID"].Value.ToString());

                BLL_jiaoYi.JiaoYiDanID = 0;
                BLL_jiaoYi.UserID = int.Parse(row.Cells["DataGridViewColumn_REYUANID"].Value.ToString());
                BLL_jiaoYi.LiuShuiID = int.Parse(row.Cells["DataGridViewColumn_LIUSHUIID"].Value.ToString());
                BLL_jiaoYi.BiZhongID = int.Parse(row.Cells["DataGridViewColumn_BIZHONGID"].Value.ToString());
                BLL_jiaoYi.QianDanID = int.Parse(row.Cells["DataGridViewColumn_QIANDANID"].Value.ToString());
                BLL_jiaoYi.Time = DateTime.Now;
                BLL_jiaoYi.Quantity = (decimal)row.Cells["DataGridViewColumn_LIANG"].Value;
                BLL_jiaoYi.Value = (decimal)row.Cells["DataGridViewColumn_JIA"].Value;
                BLL_jiaoYi.Price = (decimal)row.Cells["DataGridViewColumn_YIGONG"].Value;
                BLL_jiaoYi.Cogs = 0;
                BLL_jiaoYi.Profit = 0;
                BLL_jiaoYi.Note = row.Cells["DataGridViewColumn_BEIZHU"].Value.ToString();
                BLL_jiaoYi.Confirmed = true;

                //qian dan
                if (lei == 2 || lei == 3)
                {

                }

                //update cogs & profit for sell

                if (lei == 1)
                {
                    DataTable tb = DAL_biZhong.GetAllBiZhongByBiZhongID(biZhongID);

                    BLL_BiZhong bz;
                    int index = lstbz.FindIndex(i => i.BiZhongID == biZhongID);
                    if (index == -1)
                    {
                        bz = new BLL_BiZhong();
                        bz.BiZhongID = biZhongID;
                        bz.Quantity = (decimal)tb.Rows[0]["LIANG"];
                        bz.AveragePrice = (decimal)tb.Rows[0]["PINGJUNJIA"];
                        bz.TotalValue = (decimal)tb.Rows[0]["YIGONG"];
                        lstbz.Add(bz);
                    }
                    index = lstbz.FindIndex(i => i.BiZhongID == biZhongID);

                    if (BLL_jiaoYi.Quantity < 0)
                    {
                        BLL_jiaoYi.Cogs = (decimal)tb.Rows[0]["PINGJUNJIA"];
                        BLL_jiaoYi.Profit = (Math.Abs((decimal)row.Cells["DataGridViewColumn_JIA"].Value) - BLL_jiaoYi.Cogs) *
                                            Math.Abs((decimal)row.Cells["DataGridViewColumn_LIANG"].Value);
                    }
                    else
                    {
                        lstbz[index].AveragePrice = (lstbz[index].TotalValue + Math.Abs((decimal)row.Cells["DataGridViewColumn_YIGONG"].Value)) /
                                                    (lstbz[index].Quantity + Math.Abs((decimal)row.Cells["DataGridViewColumn_LIANG"].Value));
                    }
                    lstbz[index].Quantity += Math.Abs((decimal)row.Cells["DataGridViewColumn_LIANG"].Value);
                    lstbz[index].TotalValue = lstbz[index].Quantity * lstbz[index].AveragePrice;
                }

                lstjy.Add(BLL_jiaoYi);
            }
            rs.Jys = lstjy;
            rs.Bzs = lstbz;
            return rs;
        }
        #endregion

        
    }
}
