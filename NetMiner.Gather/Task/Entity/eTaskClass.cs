using System;
using System.Collections.Generic;
using System.Text;

namespace NetMiner.Gather.Task.Entity
{
    public class eTaskClass
    {
        public eTaskClass()
        {
            Children = new List<eTaskClass>();
        }
        ~eTaskClass()
        {
            Children = null;
        }
        public int ID { get; set; }
        public string Name { get; set; }
        public string tPath { get; set; }
        public List<eTaskClass> Children { get; set; }
    }


}
