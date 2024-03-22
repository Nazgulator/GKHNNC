using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GKHNNC.Models
{
    public class WordComplete
    {
        public string WorkType = "";
        public List<MKDCompleteWork> CompleteWorks = new List<MKDCompleteWork>();
        public List<string> AllWorks = new List<string>();
    }
}