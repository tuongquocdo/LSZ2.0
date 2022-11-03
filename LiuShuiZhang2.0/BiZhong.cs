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
        
        DAL.DAL_User DAL_User;
       
        public BiZhong()
        {
            InitializeComponent();
            DAL_User = new DAL.DAL_User();
        }

        private void User_Load(object sender, EventArgs e)
        {
            dataGridView_User.DataSource = DAL_User.GetAllUser();
            dataGridView_User.Columns["RENYUANID"].HeaderText = "人员码";
            dataGridView_User.Columns["YONGHU"].HeaderText = "用户";
            dataGridView_User.Columns["MIMA"].HeaderText = "密码";
            dataGridView_User.Columns["QUAN"].HeaderText = "职位";
            dataGridView_User.Columns["TINGYONG"].HeaderText = "停用";

            dataGridView_User.Tag = false;
            ChangeMode((bool)dataGridView_User.Tag);

        }

        private void button_Add_Click(object sender, EventArgs e)
        {
            if (CheckData(false))
            {
                if (MessageBox.Show("请确认是否要添加人员？","温卿提示",MessageBoxButtons.YesNo,MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        DAL_User.AddNewUser(new BLL_User
                        {
                            UserName = textBox_UserName.Text,
                            PassWord = Common.EncodePasswordToBase64(textBox_PassWord.Text),
                            Permission = int.Parse(((RadioButton)panel_User.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked)).Tag.ToString()),
                            Disable = checkBox_Disable.Checked
                        }) ;
                        MessageBox.Show("添加人员成功", "温卿提示",MessageBoxButtons.OK,MessageBoxIcon.Information);
                        ClearForm();
                        User_Load(sender, e);
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
                if (MessageBox.Show("请确认是否要更改人员信息？", "温卿提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
                {
                    try
                    {
                        DAL_User.EditNewUser(new BLL_User
                        {
                            UserID = long.Parse(textBox_UserName.Tag.ToString()),
                            PassWord = Common.EncodePasswordToBase64(textBox_PassWord.Text),
                            Permission = int.Parse(((RadioButton)panel_User.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Checked)).Tag.ToString()),
                            Disable = checkBox_Disable.Checked
                        });

                        MessageBox.Show("更改人员信息成功", "温卿提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        ClearForm();
                        User_Load(sender, e);
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
            User_Load(sender, e);
            ClearForm();
        }

        private void dataGridView_User_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            dataGridView_User.Tag = true;
            ChangeMode((bool)dataGridView_User.Tag);

            textBox_UserName.Tag = dataGridView_User.CurrentRow.Cells["RENYUANID"].Value.ToString();
            textBox_UserName.Text = dataGridView_User.CurrentRow.Cells["YONGHU"].Value.ToString();
            textBox_PassWord.Text = dataGridView_User.CurrentRow.Cells["MIMA"].Value.ToString();
            textBox_RePassWord.Text = dataGridView_User.CurrentRow.Cells["MIMA"].Value.ToString();
            ((RadioButton)panel_User.Controls.OfType<RadioButton>().FirstOrDefault(r => r.Tag.ToString()==dataGridView_User.CurrentRow.Cells["QUAN"].Value.ToString())).Checked = true;
            checkBox_Disable.Checked = ((Boolean)dataGridView_User.CurrentRow.Cells["TINGYONG"].Value);
        }

        private bool CheckData(bool isEdit)
        {
            bool rs = false;
            if (!Common.IsNumber(textBox_UserName.Text) && 
                Common.NotInludeSpecialChar(textBox_UserName.Text) &&
                textBox_UserName.Text.Length >= 4 && textBox_UserName.Text.Length <= 16)
                rs = true;
            else
            {
                MessageBox.Show("用户名不能是数字或存在特别码或存在大写，长度要在4-16码之间，请重新输入");
                textBox_UserName.Focus();
                textBox_UserName.SelectAll();
                return false;
            }

            if (!isEdit)
            {
                if (!DAL_User.UserNameExisted(textBox_UserName.Text))
                    rs = true;
                else
                {
                    MessageBox.Show("用户名已经存在，请选择别的");
                    textBox_UserName.Focus();
                    textBox_UserName.SelectAll();
                    return false;
                }
            }

            if (textBox_PassWord.Text == textBox_RePassWord.Text &&
                textBox_PassWord.Text.Length >= 8 && textBox_PassWord.Text.Length <= 16)
                rs = true;
            else
            {
                MessageBox.Show("密码输入不一致，长度要在8-16码之间，请重新输入");
                textBox_PassWord.Clear();
                textBox_RePassWord.Clear();
                textBox_PassWord.Focus();
                return false;
            }

            return rs;
        }

        private void ClearForm()
        {
            textBox_UserName.Clear();
            textBox_UserName.Tag = string.Empty;
            textBox_PassWord.Clear();
            textBox_RePassWord.Clear();
            textBox_UserName.Focus();
        }

        private void ChangeMode(bool isEdit)
        {
            if (isEdit)
            {
                button_Add.Enabled = false;
                button_Update.Enabled = true;
                textBox_UserName.Enabled = false;
            }
            else
            {
                button_Add.Enabled = true;
                button_Update.Enabled = false;
                textBox_UserName.Enabled = true;
            }
        }
    }
}
