using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
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
    public partial class Login : Form
    {

        DAL_User DAL_user;
        public Login()
        {
            InitializeComponent();
            
        }
        private void Login_Load(object sender, EventArgs e)
        {
            DAL_user = new DAL_User();
            if (DAL_user.GetAllUser().Rows.Count == 0)
            {
                try
                {
                    DAL_user.AddNewUser(new BLL_User
                    {
                        UserName = ConfigurationManager.AppSettings["DefaultAdminUser"].ToString(),
                        PassWord = ConfigurationManager.AppSettings["DefaultAdminUserPassword"].ToString(),
                        Permission = 1,
                        Disable = false
                    });
                    MessageBox.Show("管理员账户已创建", "温卿提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    textBox_Username.Focus();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "温卿提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void EnterToNextControl(object sender, KeyEventArgs e)
        {
            if ((e.KeyCode == Keys.Enter) || (e.KeyCode == Keys.Return))
                this.SelectNextControl((Control)sender, true, true, true, true);
        }

        private void btn_Exit_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void Login_FormClosing(object sender, FormClosingEventArgs e)
        {
            if(MessageBox.Show("你是否真的想要退出吗？","温卿提示",MessageBoxButtons.YesNo,MessageBoxIcon.Question) != DialogResult.Yes)
                e.Cancel = true;
        }

        private void btn_Login_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(textBox_Username.Text) && !string.IsNullOrEmpty(textBox_PassWord.Text))
            {
                try
                {
                    BLL_User u = DAL_user.CheckUserLoginValid(new BLL_User() { UserName = textBox_Username.Text, PassWord = Common.EncodePasswordToBase64(textBox_PassWord.Text) });
                    if (u != null)
                    {
                        this.Hide();
                        Main m = new Main();
                        m.BLL_User = u;
                        m.ShowDialog();
                        this.Show();
                    }
                    else
                        MessageBox.Show("账户或密码不匹配，请重新输入", "温卿提示", MessageBoxButtons.OK, MessageBoxIcon.Information);

                    textBox_Username.Clear();
                    textBox_PassWord.Clear();
                    textBox_Username.Focus();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "温卿提示", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
                textBox_Username.Focus();
        }
    }
}
