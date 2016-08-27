using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Media;

namespace MinDistanceWpf.Classes
{
    public class Node:IComparable<Node>
    {
        private Point drawingLocation;
        private Point center;
        private string label;
        private double diameter;
        private bool visited;
        private double totalCost;

        private List<Edge> edges;
        private Edge edgeCameFrom;

        public Node(Point location, string label, double diameter)
        {
            this.drawingLocation = location;
            this.label = label;
            this.diameter = diameter;

            visited = false;

            edges = new List<Edge>();
            totalCost = -1;
        }

        public Node(Point location, Point center, string label, double diameter)
            :this(location,label,diameter)
        {
            this.center = center;
        }

        public Point Location
        {
            get { return drawingLocation; }
        }

        public Point Center
        {
            get { return center; }
            set { this.center = value; }
        }

        public double Diameter
        {
            get { return diameter; }
        }

        public string Label
        {
            get { return label; }
        }

        public double TotalCost
        {
            get { return totalCost; }
            set { this.totalCost = value; }
        }

        public List<Edge> Edges
        {
            get { return this.edges; }
            set { this.edges = value; }
        }

        public Edge EdgeCameFrom
        {
            get { return this.edgeCameFrom; }
            set { this.edgeCameFrom = value; }
        }

        /// <summary>
        /// Calculates whether the node contains a specific point.
        /// </summary>
        /// <param name="p">The point for which we are checking</param>
        /// <returns>true or false</returns>
        public bool HasPoint(Point p)
        {
            double xSq = Math.Pow(p.X - center.X,2);
            double ySq = Math.Pow(p.Y - center.Y,2);
            double dist = Math.Sqrt(xSq + ySq);

            return (dist <= (diameter/2));
        }

        public bool Visited
        {
            get { return visited; }
            set { this.visited = value; }
        }

        public int CompareTo(Node n)
        {
            return this.totalCost.CompareTo(n.TotalCost);
        }

        public void Reset()
        {
            visited = false;
            edgeCameFrom = null;
            totalCost = -1;
        }
    }
}
