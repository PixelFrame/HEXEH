using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HEXEH.Core
{
    public class DataTree
    {
        public DataTreeNode Head { get; set; } = new DataTreeNode();

        public DataTree(string name, string description)
        {
            Head = new DataTreeNode
            {
                Label = name,
                Value = description
            };
        }
    }

    public class DataTreeNode
    {
        public string Label { get; set; } = "";
        public string Value { get; set; } = "";
        public List<DataTreeNode> Childs { get; set; } = new();

        public DataTreeNode() { }
        public DataTreeNode(string lable, string value)
        {
            Label = lable;
            Value = value;
        }
        public DataTreeNode(string lable, string value, DataTreeNode child)
        {
            Label = lable;
            Value = value;
            Childs = new List<DataTreeNode>
            {
                child
            };
        }
        public DataTreeNode(string lable, string value, DataTreeNode[] childs)
        {
            Label = lable;
            Value = value;
            Childs = new List<DataTreeNode>();
            Childs.AddRange(childs);
        }

        public override string ToString()
        {
            var sbResult = new StringBuilder();
            printNode("", true, true, ref sbResult);

            return sbResult.ToString();
        }

        private void printNode(string indent, bool root, bool last, ref StringBuilder sbResult)
        {
            sbResult.Append(indent);
            if (root) { }
            else if (last)
            {
                sbResult.Append(@"└─");
                indent += "  ";
            }
            else
            {
                sbResult.Append(@"├─");
                indent += "│ ";
            }
            sbResult.AppendLine($"{Label}: {Value}");
            for (int i = 0; i < Childs.Count; i++)
                Childs[i].printNode(indent, false, i == Childs.Count - 1, ref sbResult);
        }
    }
}
