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
    public partial class JiaoYiDanViewing : Form
    {
        private BLL_JiaoYi jiaoYi;

        public BLL_JiaoYi JiaoYi { get => jiaoYi; set => jiaoYi = value; }

        private DateTime date;
        public DateTime Date { get => date; set => date = value; }

        public JiaoYiDanViewing()
        {
            InitializeComponent();
        }

        public JiaoYiDanViewing(BLL_JiaoYi jiaoYi)
        {
            this.jiaoYi = jiaoYi;
            InitializeComponent();
        }

        public JiaoYiDanViewing(DateTime date)
        {
            this.date = date;
            InitializeComponent();
        }
    }
}
