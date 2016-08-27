using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Media;

namespace MinDistanceWpf.Classes
{
    public class Edge:IComparable<Edge>
    {
        private double length;
        private Node firstNode;
        private Node secondNode;

        private const int fontSize = 10;
        private bool visited;

        public Edge(Node firstNode, Node secondNode)
        {
            this.firstNode = firstNode;
            this.secondNode = secondNode;

            visited = false;
        }

        public Edge(Node firstNode, Node secondNode, double length)
            : this(firstNode, secondNode)
        {
            this.length = length;
        }

        public Node FirstNode
        {
            get { return this.firstNode; }
            set { this.firstNode = value; }
        }

        public Node SecondNode
        {
            get { return this.secondNode; }
            set { this.secondNode = value; }
        }

        public double Length
        {
            get { return this.length; }
            set { this.length = value; }
        }


        public void Reset()
        {
            visited = false;
        }

        public int CompareTo(Edge otherEdge)
        {
            return this.Length.CompareTo(otherEdge.Length);
        }

        public bool Visited
        {
            get { return visited; }
            set { this.visited = value; }
        }
    }
}
