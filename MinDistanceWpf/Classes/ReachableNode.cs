using System;
using System.Collections.Generic;
using System.Text;

namespace MinDistanceWpf.Classes
{
    public class ReachableNode : IComparable<ReachableNode>
    {
        private Node n;
        private Edge e;
        public ReachableNode(Node n, Edge e)
        {
            this.n = n;
            this.e = e;
        }

        public double TotalCost
        {
            get { return n.TotalCost; }
        }

        public Node Node
        {
            get { return this.n; }
            set { this.n = value; }
        }

        public Edge Edge
        {
            get { return this.e; }
            set { this.e = value; }
        }

        public int CompareTo(ReachableNode rn)
        {
            return this.Node.TotalCost.CompareTo(rn.Node.TotalCost);
        }
    }
}
