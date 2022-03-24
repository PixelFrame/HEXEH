using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEXEH.Core
{
    public class DataTree
    {
        public DataTreeNode Head { get; set; } = new DataTreeNode();

        public DataTree(string Name, string Description)
        {
            Head = new DataTreeNode
            {
                Label = Name,
                Value = Description
            };
        }
    }

    public class DataTreeNode
    {
        public string Label { get; set; } = "";
        public string Value { get; set; } = "";
        public List<DataTreeNode> Childs { get; set; } = new();
    }
}
