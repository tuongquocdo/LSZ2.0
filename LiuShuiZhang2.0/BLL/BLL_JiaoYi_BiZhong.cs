using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiuShuiZhang2._0.BLL
{
    public class BLL_JiaoYi_BiZhong
    {
        private List<BLL_JiaoYi> jys;

        private List<BLL_BiZhong> bzs;

        public List<BLL_JiaoYi> Jys { get => jys; set => jys = value; }
        public List<BLL_BiZhong> Bzs { get => bzs; set => bzs = value; }
    }
}
