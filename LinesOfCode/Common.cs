using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinesOfCode
{
    class Common
    {
        public class ReaderConfig
        {
            public List<string> extList = new List<string>();
            public List<string> ignoreList = new List<string>();

            public ReaderConfig(List<string> extList, List<string> ignoreList)
            {
                this.extList.AddRange(extList);
                this.ignoreList.AddRange(ignoreList);
            }
        }
    }
}
