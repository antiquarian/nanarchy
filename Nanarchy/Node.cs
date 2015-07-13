using System.Collections.Generic;
using Nanarchy.Core.Interfaces;

namespace Nanarchy
{
    public class Node
    {
        public Node()
        {
            NodeId = 0;
            Children = new List<Node>();
            Name = string.Empty;
        }
        public int NodeId { get; set; }
        public string Name { get; set; }
        public List<Node> Children { get; set; }
        public ITarget Data { get; set; }
    }
}