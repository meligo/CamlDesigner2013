// ----------------------------------------------------------------------
// <copyright file="Node.cs" company="Biwug">
//     Copyright statement. All right reserved
// </copyright>
//
// ------------------------------------------------------------------------

namespace CamlDesigner
{
    using System.Collections.Generic;

    public class Node
    {
        private List<Node> childNodes;
        private string text;

        public Node()
        {
        }

        public Node(string text)
        {
            this.text = text;
        }

        public List<Node> ChildNodes
        {
            get
            {
                if (this.childNodes == null)
                    this.childNodes = new List<Node>();
                return this.childNodes;
            }
        }

        public string Text
        {
            get { return this.text; }
            set { this.text = value; }
        }
    }
}
