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
using System.Text.RegularExpressions;

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

        DataTable dt_JiaoYi;

        List<DataGridViewRow> transactionRows;

        DataGridView dgv_Temp;

        int JiaoYiMode;

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
            if (numericUpDown_CashCount_DeltaValue.Value > 0)
            {
                numericUpDown_CashCount_DeltaValue.BackColor = Color.Green;
                numericUpDown_CashCount_DeltaValue.ForeColor = Color.Black;
            }
            else if (numericUpDown_CashCount_DeltaValue.Value < 0)
            {
                numericUpDown_CashCount_DeltaValue.BackColor = Color.Red;
                numericUpDown_CashCount_DeltaValue.ForeColor = Color.Black;
            }
            else
            {
                numericUpDown_CashCount_DeltaValue.BackColor = SystemColors.Info;
                numericUpDown_CashCount_DeltaValue.ForeColor = Color.Red;
            }
            
            foreach (DataGridViewColumn c in dataGridView_CashStatus_CashDetails.Columns)
            {
                if (c.Name.Split('_').Length > 1)
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
            transactionRows = new List<DataGridViewRow>();
            dgv_Temp = new DataGridView();
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

        private void dataGridView_CashStatus_CashDetails_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
        {
            dataGridView_CashStatus_CashDetails.EditingControl.KeyPress -= dataGridView_CashStatus_CashDetails_EditingControl_KeyPress;
            dataGridView_CashStatus_CashDetails.EditingControl.KeyPress += dataGridView_CashStatus_CashDetails_EditingControl_KeyPress;
            
        }

        private void dataGridView_CashStatus_CashDetails_EditingControl_KeyPress(object sender, KeyPressEventArgs e)
        {
            string pattern = "^[0-9]{0,2}$";
            if (!char.IsControl(e.KeyChar))
            {
                Control editingControl = (Control)sender;
                if (!Regex.IsMatch(e.KeyChar.ToString(), pattern))
                    e.Handled = true;
            }
        }

        private void dataGridView_CashDetails_SelectionChanged(object sender, EventArgs e)
        {
            (sender as DataGridView).ClearSelection();
        }

        private void dataGridView_CashDetails_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView_CashStatus_CashDetails.Rows.Count > 0)
            {
                CalcCountValueAndDeltaValue();
            }
        }

        private void numericUpdown_CashDetails_CellValueChanged(object sender, EventArgs e)
        {
            if (dataGridView_CashStatus_CashDetails.Rows.Count > 0)
            {
                CalcCountValueAndDeltaValue();
            }
        }

        bool isCounting = false;
        private void button_CashCouterMode_Click(object sender, EventArgs e)
        {
            isCounting = (isCounting == false);
            if (isCounting)
            {
                groupBox_CashStatus.Enabled = true;
                groupBox_Transaction.Enabled = groupBox_LiuShui.Enabled = groupBox_CashCounting.Enabled = false;
                button_CashStatus_CashCouting.Text = "点算完毕";
                dataGridView_CashStatus_CashDetails.ReadOnly = false;
                dataGridView_CashStatus_CashDetails.CurrentCell = dataGridView_CashStatus_CashDetails.Rows[0].Cells["_500000"];
                dataGridView_CashStatus_CashDetails.BeginEdit(true);
                dataGridView_CashStatus_CashDetails.SelectionChanged -= dataGridView_CashDetails_SelectionChanged;
            }
            else
            {
                try
                {
                    
                    if (MessageBox.Show("是否要保留已点算的结果？", "温卿提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                    {
                        DAL_liuShui.UpdateLiuShuiByLiuShuiID(CreateLiuShui(null));   
                    }
                    button_Transaction_CancelTran.PerformClick();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "温卿提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                groupBox_CashStatus.Enabled = groupBox_Transaction.Enabled = groupBox_LiuShui.Enabled = groupBox_CashCounting.Enabled = true;
                button_CashStatus_CashCouting.Text = "点算现金";
                dataGridView_CashStatus_CashDetails.ReadOnly = true;
                dataGridView_CashStatus_CashDetails.SelectionChanged += dataGridView_CashDetails_SelectionChanged;
            }
        }

        #endregion

        #region Transaction

        private void dataGridView_Transaction_DataGridView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == System.Windows.Forms.MouseButtons.Right)
            {
                contextMenuStrip__LiuShuiZhang.Show(Cursor.Position.X, Cursor.Position.Y);
            }
        }

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
                AddTran((int)BLL_JiaoYi.Enum_JiaoYiMode.InsertJiaoYis);
            }
        }
        private void button_Transaction_SaveTran_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确认要进账？", "温卿提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                JiaoYiMode = (int)BLL_JiaoYi.Enum_JiaoYiMode.InsertJiaoYis;
                //transactionRows.AddRange(dataGridView_Transaction_MainTran.Rows.Cast<DataGridViewRow>());
                dgv_Temp = dataGridView_Transaction_MainTran;

                numericUpDown_CashStatus_CurValue.Value += numericUpDown_Transaction_MainTotalAll.Value;
                numericUpDown_CashCount_MainTotalAll.Value = numericUpDown_Transaction_MainTotalAll.Value;
                // Pass to CashCounting
                groupBox_CashCounting.Enabled = true;
                groupBox_Transaction.Enabled = groupBox_LiuShui.Enabled = groupBox_CashStatus.Enabled = false;
                numericUpDown_CashCounting_500000.Value = 1;
                numericUpDown_CashCounting_500000.Value = 0;
                numericUpDown_CashCounting_500000.Focus();
            }
        }

        private void button_Transaction_CancelTran_Click(object sender, EventArgs e)
        {
            Main_Load(this, null);
            ClearTransactionTable();
            ClearTransactionData();
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

        private void dataGridView_Transaction_Tran_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            CalcTotallAll(sender);
            if (e.Row.Index >=0)
            {
                DataGridView d = sender as DataGridView;
                if ((decimal)d.Rows[e.Row.Index].Cells[7].Value < 0)
                {
                    d.Rows[e.Row.Index].DefaultCellStyle.BackColor = Color.Yellow;
                }
            }
        }

        private void dataGridView_Transaction_Tran_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            CalcTotallAll(sender);
            DataGridView d = sender as DataGridView;
            if (d.Rows.Count > 0)
            {
                if ((decimal)d.Rows[e.RowIndex].Cells[7].Value < 0)
                {
                    d.Rows[e.RowIndex].DefaultCellStyle.BackColor = Color.Yellow;
                }
            }
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

                        if ((decimal)selectedRow.Cells["DataGridViewColumn_YIGONG"].Value /
                        (decimal)selectedRow.Cells["DataGridViewColumn_LIANG"].Value < 0)
                        {
                            selectedRow.Cells["DataGridViewColumn_LIANG"].Value = (decimal)selectedRow.Cells["DataGridViewColumn_LIANG"].Value * -1;
                        }

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

        #region LiuShuiZhang

        List<string> filterCommands = new List<string>() { string.Empty, string.Empty };
        private void comboBox_LiuShuiZhang_Type_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox c = sender as ComboBox;
            if (c.SelectedIndex == 0)
                filterCommands[Convert.ToInt32(c.Tag)] = string.Empty;
            else
            {
                filterCommands[Convert.ToInt32(c.Tag)] = string.Format("BIZHONG = '{0}'", c.Text);
            }
            FilterJiaoYi(filterCommands);
        }

        private void comboBox_LiuShuiZhang_ShouZhi_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBox c = sender as ComboBox;
            if (c.SelectedIndex == 0)
                filterCommands[Convert.ToInt32(c.Tag)] = string.Empty;
            else
            {
                filterCommands[Convert.ToInt32(c.Tag)] = string.Format("LIANG {0}", Convert.ToDecimal(comboBox_LiuShuiZhang_ShouZhi.SelectedValue.ToString()) > 0
                                                                                    ? string.Format("> 0")
                                                                                    : string.Format("< 0")
                                                                      );
            }
            FilterJiaoYi(filterCommands);
        }

        private void FilterJiaoYi(List<string> _filterCommands)
        {
            string filterCommand = string.Empty;
            for (int i = 0; i < _filterCommands.Count; i++)
            {
                filterCommand += _filterCommands[i];
                if (_filterCommands[i] != string.Empty &&
                    i != _filterCommands.Count - 1 &&
                    !string.IsNullOrEmpty(_filterCommands[i + 1]))
                {
                    filterCommand += " And ";
                }
            }
            if (dt_JiaoYi != null)
            {
                dt_JiaoYi.DefaultView.RowFilter = filterCommand;
            }
        }

        private void dataGridView_LiuShuiZhang_Trans_RowStateChanged(object sender, DataGridViewRowStateChangedEventArgs e)
        {
            if (e.Row.Index >= 0)
            {
                DataGridView d = sender as DataGridView;
                if ((decimal)d.Rows[e.Row.Index].Cells["LIANG"].Value < 0)
                {
                    d.Rows[e.Row.Index].DefaultCellStyle.BackColor = Color.Yellow;
                }
            }
        }

        #endregion

        #region Cash Counting
        private void button_CashCounting_SaveCashCounting_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("请确认进账？", "温卿提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    //initialize LiuShui
                    BLL_LiuShui BLL_liuShui = CreateLiuShui(null);

                    ///initialize JiaoYiDan
                    BLL_jiaoYiDan = CreateJiaoYiDan(JiaoYiMode);

                    //initialize JiaoYi List
                    BLL_jiaoYi_biZhong = CreateJiaoYis(dgv_Temp);

                    DAL_jiaoYi.AddNewJiaoYis(BLL_jiaoYiDan, BLL_liuShui, BLL_jiaoYi_biZhong, JiaoYiMode);

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
            groupBox_CashCounting.Enabled = false;
            groupBox_Transaction.Enabled = groupBox_LiuShui.Enabled = groupBox_CashStatus.Enabled = button_CashStatus_CashCouting.Enabled = true;
        }

        #endregion

        #endregion

        #region Private method

        private enum WorkingMode { 
            Working,
            Viewing
        }

        public DataGridView CloneDataGrid(DataGridView mainDataGridView)
        {
            DataGridView cloneDataGridView = new DataGridView();

            if (cloneDataGridView.Columns.Count == 0)
            {
                foreach (DataGridViewColumn datagrid in mainDataGridView.Columns)
                {
                    cloneDataGridView.Columns.Add(datagrid.Clone() as DataGridViewColumn);
                }
            }

            DataGridViewRow dataRow = new DataGridViewRow();

            for (int i = 0; i < mainDataGridView.Rows.Count; i++)
            {
                dataRow = (DataGridViewRow)mainDataGridView.Rows[i].Clone();
                int Index = 0;
                foreach (DataGridViewCell cell in mainDataGridView.Rows[i].Cells)
                {
                    dataRow.Cells[Index].Value = cell.Value;
                    Index++;
                }
                cloneDataGridView.Rows.Add(dataRow);
            }
            cloneDataGridView.AllowUserToAddRows = false;
            cloneDataGridView.Refresh();


            return cloneDataGridView;
        }

        private void AddTran(int jym)
        {
            if (jym == (int)BLL_JiaoYi.Enum_JiaoYiMode.InsertJiaoYis)
            {
                dataGridView_Transaction_MainTran.Rows.Add(
                BLL_user.UserID,
                dt_LastLiuShui.Rows[0]["LIUSHUIID"].ToString(),
                comboBox_Transaction_Type.SelectedValue,
                0,
                comboBox_Transaction_Type.Text,
                numericUpDown_Transaction_Quan.Value,
                Math.Round(Math.Abs(numericUpDownEx_Transaction_AfterFee.Value / numericUpDown_Transaction_Quan.Value), 2),
                numericUpDownEx_Transaction_AfterFee.Value,
                textBox_Transaction_Note.Text
                );
                ClearTransactionData();
            }
            else
            {
                dataGridView_Transaction_MainTran.Rows.Clear();

                dataGridView_Transaction_MainTran.Rows.Add(
                BLL_jiaoYi.UserID,
                BLL_jiaoYi.LiuShuiID.ToString(),
                BLL_jiaoYi.BiZhongID,
                BLL_jiaoYi.QianDanID,
                string.Empty,
                BLL_jiaoYi.Quantity * -1,
                BLL_jiaoYi.Value,
                BLL_jiaoYi.Price * -1,
                BLL_jiaoYi.Note
                );
                dgv_Temp = CloneDataGrid(dataGridView_Transaction_MainTran);
                dgv_Temp.Columns.Add("DataGridViewColumn_JIAOYIID", string.Empty);
                dgv_Temp.Rows[0].Cells["DataGridViewColumn_JIAOYIID"].Value = BLL_jiaoYi.JiaoYiID;
                dataGridView_Transaction_MainTran.Rows.Clear();
            }
        }

        private void CalcCountValueAndDeltaValue()
        {
            long sum = 0;
            foreach (DataGridViewColumn c in dataGridView_CashStatus_CashDetails.Columns)
            {
                if (c.Visible)
                {
                    sum += dataGridView_CashStatus_CashDetails.Rows[0].Cells[c.Name].Value.ToString() == string.Empty ? 0 :
                            Convert.ToInt64(dataGridView_CashStatus_CashDetails.Rows[0].Cells[c.Name].Value.ToString()) *
                           Convert.ToInt64(c.Name.Split('_')[1].ToString());
                }
            }
            numericUpDown_CashStatus_CountValue.Value = sum;
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
                ((NumericUpDownEx)c).Value = 1;
                ((NumericUpDownEx)c).Value = 0;
            }
        }

        private void ChangeWorkingMode(int mode)
        {
            if (mode == 0) // Working mode
            {
                groupBox_CashStatus.Enabled = true;
                groupBox_Transaction.Enabled = true;
                groupBox_CashCounting.Enabled = false;
                groupBox_LiuShui.Enabled = true;
            }
            else if (mode == 1) // viewing mode
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

            dt_LastLiuShui = DAL_liuShui.GetLastRecord();

            if (dt_LastLiuShui.Rows.Count > 0)
            {
                int gg = DateTime.Compare(((DateTime)dt_LastLiuShui.Rows[0]["RIZI"]).Date, dateTimePicker.Value.Date);
                //view data mode
                if (gg > 0)
                {
                    ChangeWorkingMode((int)WorkingMode.Viewing);
                    dt_LastLiuShui = DAL_liuShui.GetRecordByDate(dateTimePicker.Value.Date);

                    if (dt_LastLiuShui.Rows.Count <= 0)
                    {
                        MessageBox.Show("该日子并没有存在流水账", "温卿提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    }
                }
                //Working Mode
                else
                {
                    ChangeWorkingMode((int)WorkingMode.Working);
                    // Create new data
                    if (gg < 0)
                    {
                        try
                        {
                            DataTable newLiuShuiBaseLastRecord = dt_LastLiuShui.Copy();
                            newLiuShuiBaseLastRecord.Rows[0]["QIANE"] = newLiuShuiBaseLastRecord.Rows[0]["XIANE"] = newLiuShuiBaseLastRecord.Rows[0]["DIANSUANJIEGUO"];
                            newLiuShuiBaseLastRecord.Rows[0]["XIANGCHA"] = 0;
                            newLiuShuiBaseLastRecord.Rows[0]["RIZI"] = dateTimePicker.Value.Date;

                            DAL_liuShui.AddNewLiuShui(CreateLiuShui(newLiuShuiBaseLastRecord));
                            MessageBox.Show(string.Format("{0}的流水账已经创新", dateTimePicker.Value.Date), "温卿提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                            this.Refresh();

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show(ex.Message, "温卿提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }
            else // the frist time using
            {
                ChangeWorkingMode((int)WorkingMode.Working);
                try
                {
                    DataTable newLiuShui = new DataTable();
                    newLiuShui.Columns.AddRange(new DataColumn[] {
                        new DataColumn("LIUSHUIID",typeof(long)),
                        new DataColumn("RIZI",typeof(DateTime)),
                        new DataColumn("QIANE",typeof(Decimal)),
                        new DataColumn("XIANE",typeof(Decimal)),
                        new DataColumn("XIANGCHA",typeof(Decimal)),
                        new DataColumn("DIANSUANJIEGUO",typeof(Decimal)),
                        new DataColumn("_500000",typeof(Decimal)),
                        new DataColumn("_200000",typeof(Decimal)),
                        new DataColumn("_100000",typeof(Decimal)),
                        new DataColumn("_50000",typeof(Decimal)),
                        new DataColumn("_20000",typeof(Decimal)),
                        new DataColumn("_10000",typeof(Decimal)),
                        new DataColumn("_5000",typeof(Decimal)),
                        new DataColumn("_2000",typeof(Decimal)),
                        new DataColumn("_1000",typeof(Decimal)),
                    });
                    newLiuShui.Rows.Add(newLiuShui.NewRow());

                    foreach (DataColumn c in newLiuShui.Columns)
                    {
                        if (c.ColumnName == "RIZI")
                        {
                            newLiuShui.Rows[0][c.ColumnName] = dateTimePicker.Value.Date;
                        }
                        else
                        {
                            newLiuShui.Rows[0][c.ColumnName] = 0;
                        }
                    }
                    DAL_liuShui.AddNewLiuShui(CreateLiuShui(newLiuShui));
                    MessageBox.Show(string.Format("{0}的流水账已经创新", dateTimePicker.Value.Date), "温卿提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    Refresh();

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "温卿提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            dt_LastLiuShui = DAL_liuShui.GetLastRecord();
            dataGridView_CashStatus_CashDetails.DataSource = dt_LastLiuShui;
            foreach (DataGridViewColumn c in dataGridView_CashStatus_CashDetails.Columns)
            {
                c.SortMode = DataGridViewColumnSortMode.NotSortable;
            }
            dataGridView_CashStatus_CashDetails.Columns["LIUSHUIID"].Visible = false;
            dataGridView_CashStatus_CashDetails.Columns["RIZI"].Visible = false;
            dataGridView_CashStatus_CashDetails.Columns["QIANE"].Visible = false;
            dataGridView_CashStatus_CashDetails.Columns["XIANE"].Visible = false;
            dataGridView_CashStatus_CashDetails.Columns["XIANGCHA"].Visible = false;
            dataGridView_CashStatus_CashDetails.Columns["DIANSUANJIEGUO"].Visible = false;

            dataGridView_CashStatus_CashDetails.Columns["_500000"].HeaderText = "500K";
            dataGridView_CashStatus_CashDetails.Columns["_200000"].HeaderText = "200K";
            dataGridView_CashStatus_CashDetails.Columns["_100000"].HeaderText = "100K";

            dataGridView_CashStatus_CashDetails.Columns["_50000"].HeaderText = "50K ";
            dataGridView_CashStatus_CashDetails.Columns["_20000"].HeaderText = "20K ";
            dataGridView_CashStatus_CashDetails.Columns["_10000"].HeaderText = "10K ";

            dataGridView_CashStatus_CashDetails.Columns["_5000"].HeaderText = "5K  ";
            dataGridView_CashStatus_CashDetails.Columns["_2000"].HeaderText = "2K  ";
            dataGridView_CashStatus_CashDetails.Columns["_1000"].HeaderText = "1K  ";

            numericUpDown_CashStatus_PreValue.Value = dt_LastLiuShui.Rows.Count==0 ? 0 : decimal.Parse(dt_LastLiuShui.Rows[0]["QIANE"].ToString().Trim());
            numericUpDown_CashStatus_CurValue.Value = dt_LastLiuShui.Rows.Count == 0 ? 0 : decimal.Parse(dt_LastLiuShui.Rows[0]["XIANE"].ToString().Trim());
            numericUpDown_CashStatus_DeltaValue.Value = dt_LastLiuShui.Rows.Count == 0 ? 0 : decimal.Parse(dt_LastLiuShui.Rows[0]["XIANGCHA"].ToString().Trim());
            numericUpDown_CashStatus_CountValue.Value = dt_LastLiuShui.Rows.Count == 0 ? 0 : decimal.Parse(dt_LastLiuShui.Rows[0]["DIANSUANJIEGUO"].ToString().Trim());

            dataGridView_CashStatus_CashDetails.CellValueChanged += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CashDetails_CellValueChanged);
            numericUpDown_CashStatus_CurValue.ValueChanged += new System.EventHandler(this.numericUpdown_CashDetails_CellValueChanged);

            

            #endregion

            #region Load BiZhong

            comboBox_Transaction_Type.DataSource = DAL_biZhong.GetAllBiZhong();
            comboBox_Transaction_Type.DisplayMember = "BIZHONG";
            comboBox_Transaction_Type.ValueMember = "BIZHONGID";
            comboBox_Transaction_Type.SelectedIndex = 0;

            DataTable dt_BiZhongForFilter = DAL_biZhong.GetBiZhongInLiuShuiZhang(dateTimePicker.Value.Date);
            DataRow dtr_SelectedLiuShuiFilterValue = dt_BiZhongForFilter.NewRow();
            dtr_SelectedLiuShuiFilterValue[0] = 0;
            dtr_SelectedLiuShuiFilterValue[1] = "全部";
            dt_BiZhongForFilter.Rows.InsertAt(dtr_SelectedLiuShuiFilterValue, 0);
            comboBox_LiuShuiZhang_Type.DataSource = dt_BiZhongForFilter;
            comboBox_LiuShuiZhang_Type.DisplayMember = "BIZHONG";
            comboBox_LiuShuiZhang_Type.ValueMember = "BIZHONGID";
            comboBox_LiuShuiZhang_Type.SelectedIndex = 0;

            DataTable dt_ShouZhiForFilter = new DataTable();
            dt_ShouZhiForFilter.Columns.AddRange(new DataColumn[] { 
                new DataColumn("SHOUZHIID"),
                new DataColumn("SHOUZHI")
            });
            DataRow dtr_SelectedShouZhiFilterValue0 = dt_ShouZhiForFilter.NewRow();
            dtr_SelectedShouZhiFilterValue0[0] = "0";
            dtr_SelectedShouZhiFilterValue0[1] = "全部";
            dt_ShouZhiForFilter.Rows.Add(dtr_SelectedShouZhiFilterValue0);
            DataRow dtr_SelectedShouZhiFilterValue1 = dt_ShouZhiForFilter.NewRow();
            dtr_SelectedShouZhiFilterValue1[0] = "-1";
            dtr_SelectedShouZhiFilterValue1[1] = "收";
            dt_ShouZhiForFilter.Rows.Add(dtr_SelectedShouZhiFilterValue1);
            DataRow dtr_SelectedShouZhiFilterValue2 = dt_ShouZhiForFilter.NewRow();
            dtr_SelectedShouZhiFilterValue2[0] = "1";
            dtr_SelectedShouZhiFilterValue2[1] = "支";
            dt_ShouZhiForFilter.Rows.Add(dtr_SelectedShouZhiFilterValue2);
            comboBox_LiuShuiZhang_ShouZhi.DataSource = dt_ShouZhiForFilter;
            comboBox_LiuShuiZhang_ShouZhi.DisplayMember = "SHOUZHI";
            comboBox_LiuShuiZhang_ShouZhi.ValueMember = "SHOUZHIID";
            comboBox_LiuShuiZhang_ShouZhi.SelectedIndex = 0;


            #endregion

            #region Initialize Fee Type

            comboBox_Transaction_FeeType.DataSource = BLL_jiaoYi.FeeTypes;
            comboBox_Transaction_FeeType.DisplayMember = "FEETYPE";
            comboBox_Transaction_FeeType.ValueMember = "FEETYPEID";
            comboBox_Transaction_FeeType.SelectedItem = 1;
            comboBox_Transaction_FeeType.SelectedItem = 0;

            #endregion

            #region Load JiaoYi
            dt_JiaoYi = DAL_jiaoYi.GetAllRecordByDate(dateTimePicker.Value);
            dataGridView_LiuShuiZhang_Trans.DataSource = dt_JiaoYi;
            dataGridView_LiuShuiZhang_Trans.ReadOnly = true;
            dataGridView_LiuShuiZhang_Trans.Columns["JIAOYIID"].Visible = false;
            dataGridView_LiuShuiZhang_Trans.Columns["JIAOYIDANID"].Visible = false;
            dataGridView_LiuShuiZhang_Trans.Columns["RENYUANID"].Visible = false;
            dataGridView_LiuShuiZhang_Trans.Columns["LIUSHUIID"].Visible = false;
            dataGridView_LiuShuiZhang_Trans.Columns["BIZHONGID"].Visible = false;
            dataGridView_LiuShuiZhang_Trans.Columns["QIANDANID"].Visible = false;
            dataGridView_LiuShuiZhang_Trans.Columns["BIZHONG"].HeaderText = "币种";

            dataGridView_LiuShuiZhang_Trans.Columns["LIANG"].HeaderText = "数量";
            dataGridView_LiuShuiZhang_Trans.Columns["LIANG"].DefaultCellStyle.Format = "N2";

            dataGridView_LiuShuiZhang_Trans.Columns["JIA"].HeaderText = "价格";
            dataGridView_LiuShuiZhang_Trans.Columns["JIA"].DefaultCellStyle.Format = "N2";
            dataGridView_LiuShuiZhang_Trans.Columns["YIGONG"].HeaderText = "一共";

            dataGridView_LiuShuiZhang_Trans.Columns["YIGONG"].DefaultCellStyle.Format = "N2";
            dataGridView_LiuShuiZhang_Trans.Columns["BEIZHU"].HeaderText = "备注";

            dataGridView_LiuShuiZhang_Trans.Columns["SHIJIAN"].HeaderText = "时间";
            dataGridView_LiuShuiZhang_Trans.Columns["SHIJIAN"].DefaultCellStyle.Format = "yyyy/MM/dd HH:mm:ss";

            #endregion
        }

        private BLL_LiuShui CreateLiuShui(DataTable ls)
        {
            return new BLL_LiuShui()
            {
                LiuShuiID = ls == null ? Convert.ToInt64(dt_LastLiuShui.Rows[0]["LIUSHUIID"].ToString()) : Convert.ToInt64(ls.Rows[0]["LIUSHUIID"].ToString()),
                LiuShuiDate = ls == null ? dateTimePicker.Value.Date : Convert.ToDateTime(ls.Rows[0]["RIZI"].ToString()),
                PreValue = ls == null ? numericUpDown_CashStatus_PreValue.Value : Convert.ToDecimal(ls.Rows[0]["QIANE"].ToString()),
                CurValue = ls == null ? numericUpDown_CashStatus_CurValue.Value : Convert.ToDecimal(ls.Rows[0]["XIANE"].ToString()),
                DeltaValue = ls == null ? numericUpDown_CashStatus_DeltaValue.Value : Convert.ToDecimal(ls.Rows[0]["XIANGCHA"].ToString()),
                CountValue = ls == null ? numericUpDown_CashStatus_CountValue.Value : Convert.ToDecimal(ls.Rows[0]["DIANSUANJIEGUO"].ToString()),
                __500 = ls == null ? long.Parse(dataGridView_CashStatus_CashDetails.Rows[0].Cells["_500000"].Value.ToString()) : Convert.ToInt64(ls.Rows[0]["_500000"].ToString()),
                __200 = ls == null ? long.Parse(dataGridView_CashStatus_CashDetails.Rows[0].Cells["_200000"].Value.ToString()) : Convert.ToInt64(ls.Rows[0]["_200000"].ToString()),
                __100 = ls == null ? long.Parse(dataGridView_CashStatus_CashDetails.Rows[0].Cells["_100000"].Value.ToString()) : Convert.ToInt64(ls.Rows[0]["_100000"].ToString()),
                __50 = ls == null ? long.Parse(dataGridView_CashStatus_CashDetails.Rows[0].Cells["_50000"].Value.ToString()) : Convert.ToInt64(ls.Rows[0]["_50000"].ToString()),
                __20 = ls == null ? long.Parse(dataGridView_CashStatus_CashDetails.Rows[0].Cells["_20000"].Value.ToString()) : Convert.ToInt64(ls.Rows[0]["_20000"].ToString()),
                __10 = ls == null ? long.Parse(dataGridView_CashStatus_CashDetails.Rows[0].Cells["_10000"].Value.ToString()) : Convert.ToInt64(ls.Rows[0]["_10000"].ToString()),
                __5 = ls == null ? long.Parse(dataGridView_CashStatus_CashDetails.Rows[0].Cells["_5000"].Value.ToString()) : Convert.ToInt64(ls.Rows[0]["_5000"].ToString()),
                __2 = ls == null ? long.Parse(dataGridView_CashStatus_CashDetails.Rows[0].Cells["_2000"].Value.ToString()) : Convert.ToInt64(ls.Rows[0]["_2000"].ToString()),
                __1 = ls == null ? long.Parse(dataGridView_CashStatus_CashDetails.Rows[0].Cells["_1000"].Value.ToString()) : Convert.ToInt64(ls.Rows[0]["_1000"].ToString()),
            };
        }

        private BLL_JiaoYiDan CreateJiaoYiDan(int jym)
        {
            if (jym == (int)BLL_JiaoYi.Enum_JiaoYiMode.InsertJiaoYis)
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
            else
            {
                return new BLL_JiaoYiDan()
                {
                    JiaoYiDanID = BLL_jiaoYi.JiaoYiDanID
                };
            }
        }

        private BLL_JiaoYi_BiZhong CreateJiaoYis(DataGridView dgv)
        {
            BLL_JiaoYi_BiZhong rs = new BLL_JiaoYi_BiZhong();
            List<BLL_JiaoYi> lstjy = new List<BLL_JiaoYi>();
            List<BLL_BiZhong> lstbz = new List<BLL_BiZhong>();

            foreach (DataGridViewRow row in dgv.Rows)
            {
                BLL_jiaoYi = new BLL_JiaoYi();
                int lei = DAL_biZhong.GetLeiOfBiZhong(int.Parse(row.Cells["DataGridViewColumn_BIZHONGID"].Value.ToString()));
                long biZhongID = long.Parse(row.Cells["DataGridViewColumn_BIZHONGID"].Value.ToString());

                BLL_jiaoYi.JiaoYiDanID = 0;
                BLL_jiaoYi.UserID = long.Parse(row.Cells["DataGridViewColumn_REYUANID"].Value.ToString());
                BLL_jiaoYi.LiuShuiID = long.Parse(row.Cells["DataGridViewColumn_LIUSHUIID"].Value.ToString());
                BLL_jiaoYi.BiZhongID = long.Parse(row.Cells["DataGridViewColumn_BIZHONGID"].Value.ToString());
                BLL_jiaoYi.QianDanID = long.Parse(row.Cells["DataGridViewColumn_QIANDANID"].Value.ToString());
                BLL_jiaoYi.Time = DateTime.Now;
                BLL_jiaoYi.Quantity = (decimal)row.Cells["DataGridViewColumn_LIANG"].Value;
                BLL_jiaoYi.Value = (decimal)row.Cells["DataGridViewColumn_JIA"].Value;
                BLL_jiaoYi.Price = (decimal)row.Cells["DataGridViewColumn_YIGONG"].Value;
                BLL_jiaoYi.Cogs = 0;
                BLL_jiaoYi.Profit = 0;
                BLL_jiaoYi.Note = row.Cells["DataGridViewColumn_BEIZHU"].Value.ToString();
                BLL_jiaoYi.Confirmed = dgv.Columns.Cast<DataGridViewColumn>().Any(i => i.Name == "DataGridViewColumn_JIAOYIID")
                                      ? false
                                      : true;
                BLL_jiaoYi.JiaoYiID = dgv.Columns.Cast<DataGridViewColumn>().Any(i => i.Name == "DataGridViewColumn_JIAOYIID") 
                                      ? long.Parse(row.Cells["DataGridViewColumn_JIAOYIID"].Value.ToString())
                                      : 0;

                //qian dan
                if (lei == (int)BLL_BiZhong.BiZhongLei.KeRenQian || lei == (int)BLL_BiZhong.BiZhongLei.QianKeRen)
                {
                    string title = lei == (int)BLL_BiZhong.BiZhongLei.KeRenQian ? "客人欠本店" : "本店欠客人";
                    BLL_jiaoYi.QianDan = new BLL_QianDan()
                    {
                        Content = string.Format("{0} - 客户：{1} - 开单时间：{2}",
                        title,
                        row.Cells["DataGridViewColumn_BEIZHU"].Value.ToString(),
                        BLL_jiaoYi.Time),
                        Time = BLL_jiaoYi.Time,
                        QianDanValue = BLL_jiaoYi.Price,
                        Finish = false
                    };
                }


                if (lei == (int)BLL_BiZhong.BiZhongLei.WaiBi)
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
                        index = lstbz.FindIndex(i => i.BiZhongID == biZhongID);
                    }
                    

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

        private void dataGridView_LiuShuiZhang_Trans_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {
            DataGridView drv = sender as DataGridView;
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0 && e.ColumnIndex >= 0)
            {
                try
                {
                    drv.CurrentCell = drv.Rows[e.RowIndex].Cells[e.ColumnIndex];
                    drv.Rows[e.RowIndex].Selected = true;
                    drv.Focus();

                    BLL_jiaoYi.JiaoYiID = Convert.ToInt64(drv.Rows[e.RowIndex].Cells["JIAOYIID"].Value);
                    BLL_jiaoYi.JiaoYiDanID = Convert.ToInt64(drv.Rows[e.RowIndex].Cells["JIAOYIDANID"].Value);
                    BLL_jiaoYi.UserID = Convert.ToInt64(drv.Rows[e.RowIndex].Cells["RENYUANID"].Value);
                    BLL_jiaoYi.LiuShuiID = Convert.ToInt64(drv.Rows[e.RowIndex].Cells["LIUSHUIID"].Value);
                    BLL_jiaoYi.BiZhongID = Convert.ToInt64(drv.Rows[e.RowIndex].Cells["BIZHONGID"].Value);
                    BLL_jiaoYi.Time = DateTime.Now;
                    BLL_jiaoYi.Quantity = (decimal)drv.Rows[e.RowIndex].Cells["LIANG"].Value;
                    BLL_jiaoYi.Value = (decimal)drv.Rows[e.RowIndex].Cells["JIA"].Value;
                    BLL_jiaoYi.Price = (decimal)drv.Rows[e.RowIndex].Cells["YIGONG"].Value;
                    BLL_jiaoYi.Cogs = 0;
                    BLL_jiaoYi.Profit = 0;
                    BLL_jiaoYi.Note = drv.Rows[e.RowIndex].Cells["BEIZHU"].Value.ToString();
                    BLL_jiaoYi.Confirmed = false;
                    AddTran((int)BLL_JiaoYi.Enum_JiaoYiMode.DeleteJiaoYi);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "温卿提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                contextMenuStrip__LiuShuiZhang.Show(Cursor.Position.X, Cursor.Position.Y);
            }
            
        }

        private void toolStripMenuItem_DeleteJiaoYi_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("确认删除交易？", "温卿提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                JiaoYiMode = (int)BLL_JiaoYi.Enum_JiaoYiMode.DeleteJiaoYi;
                numericUpDown_CashStatus_CurValue.Value += (decimal)dgv_Temp.Rows[0].Cells["DataGridViewColumn_YIGONG"].Value *-1;
                numericUpDown_CashCount_MainTotalAll.Value = (decimal)dgv_Temp.Rows[0].Cells["DataGridViewColumn_YIGONG"].Value*-1;
                // Pass to CashCounting
                groupBox_CashCounting.Enabled = true;
                groupBox_Transaction.Enabled = groupBox_LiuShui.Enabled = groupBox_CashStatus.Enabled = false;
                numericUpDown_CashCounting_500000.Value = 1;
                numericUpDown_CashCounting_500000.Value = 0;
                numericUpDown_CashCounting_500000.Focus();
            }
        }
    }
}
