using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiuShuiZhang2._0.BLL
{
    public class BLL_User
    {
        private long userID;

        private string userName;

        private string passWord;

        private int permission;

        private bool disable;
        public long UserID { get => userID; set => userID = value; }
        public string UserName { get => userName; set => userName = value; }
        public string PassWord { get => passWord; set => passWord = value; }
        public int Permission { get => permission; set => permission = value; }
        public bool Disable { get => disable; set => disable = value; }
    }
}
