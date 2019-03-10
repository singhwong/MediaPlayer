using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VideoPlayer.Modle
{
    public class Progress
    {
        public string Path { get; set; }
        public double Value { get; set; }
    }

    public class ProgressList
    {
        public List<Progress> progress_list = new List<Progress>();      
    }
}
