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

namespace LiuShuiZhang2._0
{
    public partial class BiZhong : Form
    {
        
        DAL.DAL_BiZhong DAL_BiZhong;
       
        public BiZhong()
        {
            InitializeComponent();
            DAL_BiZhong = new DAL.DAL_BiZhong();
        }

        private void BiZhong_Load(object sender, EventArgs e)
        {
            dataGridView_BiZhong.DataSource = DAL_BiZhong.GetAllBiZhong();
            dataGridView_BiZhong.Columns["BIZHONGID"].HeaderText = "币种码";
            dataGridView_BiZhong.Columns["BIZHONG"].HeaderText = "币种";
            dataGridView_BiZhong.Columns["LEI"].HeaderText = "類";
            dataGridView_BiZhong.Columns["TINGYONG"].HeaderText = "停用";

            dataGridView_BiZhong.Tag = false;
            ChangeMode((bool)dataGridView_BiZhong.Tag);

        }

        private void button_Add_Click(object sender, EventArgs e)
        {
            if (CheckData(false))
            {
                if (MessageBox.Show("请确认是否要添加币种？","温卿提示",MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        DAL_BiZhong.AddBiZhong(new BLL_BiZhong
                        {
                            BiZhong = textBox_BiZhong.Text,
                            Quantity = 0,
                            AveragePrice = 0,
                            TotalValue = 0,
                            Type = int.Parse(((RadioButton)panel_BiZhong.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked)).Tag.ToString()),
                            Disable = checkBox_Disable.Checked
                        }) ;
                        MessageBox.Show("添加币种成功", "温卿提示",MessageBoxButtons.OK,MessageBoxIcon.Information);
                        ClearForm();
                        BiZhong_Load(sender, e);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "温卿提示",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void button_Update_Click(object sender, EventArgs e)
        {
            if (CheckData(true))
            {
                if (MessageBox.Show("请确认是否要更改币种信息？", "温卿提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        DAL_BiZhong.EditBiZhong(new BLL_BiZhong
                        {
                            BiZhongID = long.Parse(textBox_BiZhong.Tag.ToString()),
                            BiZhong = textBox_BiZhong.Text,
                            Type = int.Parse(panel_BiZhong.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked).Tag.ToString()),
                            Disable = checkBox_Disable.Checked
                        });

                        MessageBox.Show("更改人员信息成功", "温卿提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearForm();
                        BiZhong_Load(sender, e);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message, "温卿提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
        }

        private void button_Clear_Click(object sender, EventArgs e)
        {
            BiZhong_Load(sender, e);
            ClearForm();
        }

        private void dataGridView_User_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView_BiZhong.Tag = true;
            ChangeMode((bool)dataGridView_BiZhong.Tag);

            textBox_BiZhong.Tag = dataGridView_BiZhong.CurrentRow.Cells["BIZHONGID"].Value.ToString();
            textBox_BiZhong.Text = dataGridView_BiZhong.CurrentRow.Cells["BIZHONG"].Value.ToString();
            panel_BiZhong.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Tag.ToString() == dataGridView_BiZhong.CurrentRow.Cells["LEI"].Value.ToString()).Checked = true;
            checkBox_Disable.Checked = ((bool)dataGridView_BiZhong.CurrentRow.Cells["TINGYONG"].Value);
        }

        private bool CheckData(bool isEdit)
        {
            bool rs = false;
            if (!(textBox_BiZhong.Text == string.Empty) &&
                textBox_BiZhong.Text.Length >= 3 &&
                textBox_BiZhong.Text.Length <=12)
                rs = true;
            else
            {
                MessageBox.Show("币种不能留空，长度要在3-12码内，请重新输入");
                textBox_BiZhong.Focus();
                textBox_BiZhong.SelectAll();
                return false;
            }

            if (!isEdit)
            {
                if (!DAL_BiZhong.BiZhongExisted(textBox_BiZhong.Text))
                    rs = true;
                else
                {
                    MessageBox.Show("币种已经存在，请选择别的");
                    textBox_BiZhong.Focus();
                    textBox_BiZhong.SelectAll();
                    return false;
                }
            }
            return rs;
        }

        private void ClearForm()
        {
            textBox_BiZhong.Clear();
            textBox_BiZhong.Tag = string.Empty;
            textBox_BiZhong.Focus();
        }

        private void ChangeMode(bool isEdit)
        {
            if (isEdit)
            {
                button_Add.Enabled = false;
                button_Update.Enabled = true;
                
            }
            else
            {
                button_Add.Enabled = true;
                button_Update.Enabled = false;
                
            }
        }
    }
}
