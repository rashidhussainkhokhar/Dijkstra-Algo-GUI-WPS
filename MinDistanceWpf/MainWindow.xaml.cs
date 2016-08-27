using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Threading.Tasks;
using MinDistanceWpf.Classes;

namespace MinDistanceWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        private const double diameter = 30;
        private const double edgeLabelSize = 20;

        private const int fontSize = 12;
        private const int edgeFontSize = 10;

        private int count;
        private bool findMinDistance;

        private List<Node> cloud;
        private ReachableNodeList reachableNodes;

        private List<Node> nodes;
        private List<Edge> edges;

        private Node edgeNode1, edgeNode2;
        private SolidColorBrush unvisitedBrush, visitedBrush;
        private bool isGraphConnected;

        public MainWindow()
        {
            InitializeComponent();
            drawingCanvas.SetValue(Canvas.ZIndexProperty, 0);

            cloud = new List<Node>();
            reachableNodes = new ReachableNodeList();

            nodes = new List<Node>();
            edges = new List<Edge>();
            
            count = 1;
            findMinDistance = false;
            isGraphConnected = true;

            unvisitedBrush = new SolidColorBrush(Colors.Black);
            visitedBrush = new SolidColorBrush(Colors.DarkViolet);
        }

        /// <summary>
        /// The event establishes the all the GUI interactions with the user:
        ///     -the creation of nodes
        ///     -the creation of edges
        ///     -invoke the min distance 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void drawingCanvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                Point clickPoint = e.GetPosition(drawingCanvas);

                if (HasClickedOnNode(clickPoint.X, clickPoint.Y))
                {
                    AssignEndNodes(clickPoint.X, clickPoint.Y);
                    if (edgeNode1 != null && edgeNode2 != null)
                    {
                        if (findMinDistance)
                        {
                            statusLabel.Content = "Calculating...";
                            FindMinDistancePath(edgeNode1, edgeNode2);
                            PaintMinDistancePath(edgeNode1, edgeNode2);
                        }
                        else
                        {
                            //build an edge
                            double distance = GetEdgeDistance();
                            if (distance != 0.0)
                            {
                                Edge edge = CreateEdge(edgeNode1, edgeNode2, distance);
                                edges.Add(edge);

                                edgeNode1.Edges.Add(edge);
                                edgeNode2.Edges.Add(edge);

                                PaintEdge(edge);
                            }
                            ClearEdgeNodes();
                        }
                    }
                }
                else
                {
                    if (!OverlapsNode(clickPoint))
                    {
                        Node n = CreateNode(clickPoint);
                        nodes.Add(n);
                        PaintNode(n);
                        count++;
                        ClearEdgeNodes();
                    }
                }
            }
        }

       

        private void ClearEdgeNodes()
        {
            edgeNode1 = edgeNode2 = null;
        }

        /// <summary>
        /// A method to detect whether the user has clicked on a node
        /// used either for edge creation or for indicating the end-points for which to find
        /// the minimum distance
        /// </summary>
        /// <param name="x">x-coordinate</param>
        /// <param name="y">y-coordinate</param>
        /// <returns>Whether a user is clicked on a existing node</returns>
        private bool HasClickedOnNode(double x, double y)
        {
            bool rez = false;
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].HasPoint(new Point(x, y)))
                {
                    rez = true;
                    break;
                }
            }
            return rez;
        }

        /// <summary>
        /// Get a node at a specific coordinate
        /// </summary>
        /// <param name="x">x-coordinate</param>
        /// <param name="y">y-coordinate</param>
        /// <returns>The node that has been found or null if there is no node at the speicied coordinates</returns>
        private Node GetNodeAt(double x, double y)
        {
            Node rez = null;
            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i].HasPoint(new Point(x, y)))
                {
                    rez = nodes[i];
                    break;
                }
            }
            return rez;
        }
        /// <summary>
        /// Upon the creation of a new node,
        /// make sure that it is not overlapping an existing node
        /// </summary>
        /// <param name="p">A x,y point</param>
        /// <returns>Whether there is an overlap with an existing node</returns>
        private bool OverlapsNode(Point p)
        {
            bool rez = false;
            double distance;
            for (int i = 0; i < nodes.Count; i++)
            {
                distance = GetDistance(p, nodes[i].Center);
                if (distance < diameter)
                {
                    rez = true;
                    break;
                }
            }
            return rez;
        }

        /// <summary>
        /// Use an additional dialog window to get the distance
        /// for an edge as specified by the user
        /// </summary>
        /// <returns>The distance value specified by the user</returns>
        private double GetEdgeDistance()
        {
            double distance = 0.0;
            DistanceDialog dd = new DistanceDialog();
            dd.Owner = this;

            dd.ShowDialog();
            distance = dd.Distance;

            return distance;
        }
        /// <summary>
        /// Calculate the Eucledean distance between two points
        /// </summary>
        /// <param name="p1">Point 1</param>
        /// <param name="p2">Point 2</param>
        /// <returns>The distance between the two points</returns>
        private double GetDistance(Point p1, Point p2)
        {
            double xSq = Math.Pow(p1.X - p2.X, 2);
            double ySq = Math.Pow(p1.Y - p2.Y, 2);
            double dist = Math.Sqrt(xSq + ySq);

            return dist;
        }

        private void AssignEndNodes(double x, double y)
        {
            Node currentNode = GetNodeAt(x, y);
            if (currentNode != null)
            {
                if (edgeNode1 == null)
                {
                    edgeNode1 = currentNode;
                    statusLabel.Content = "You have selected node " + currentNode.Label + ". Please select another node.";
                }
                else
                {
                    if (currentNode != edgeNode1)
                    {
                        edgeNode2 = currentNode;
                        statusLabel.Content = "Click on the canvas to create a node.";
                    }
                }
            }
        }

        /// <summary>
        /// Create a new node using the coordinates specified by a point
        /// </summary>
        /// <param name="p">A Point object that carries the coordinates for Node creation</param>
        /// <returns></returns>
        private Node CreateNode(Point p)
        {
            double nodeCenterX = p.X - diameter / 2;
            double nodeCenterY = p.Y - diameter / 2;
            Node newNode = new Node(new Point(nodeCenterX, nodeCenterY), p, count.ToString(), diameter);
            return newNode;
        }

        /// <summary>
        /// Paint a single node on the canvas
        /// </summary>
        /// <param name="node">A node object carrying the coordinates</param>
        private void PaintNode(Node node)
        {
            //paint the node
            Ellipse ellipse = new Ellipse();
            if (node.Visited)
                ellipse.Fill = visitedBrush;
            else
                ellipse.Fill = unvisitedBrush;

            ellipse.Width = diameter;
            ellipse.Height = diameter;

            ellipse.SetValue(Canvas.LeftProperty, node.Location.X);
            ellipse.SetValue(Canvas.TopProperty, node.Location.Y);
            ellipse.SetValue(Canvas.ZIndexProperty, 2);
            //add to the canvas
            drawingCanvas.Children.Add(ellipse);

            //paint the node label 
            TextBlock tb = new TextBlock();
            tb.Text = node.Label;
            tb.Foreground = Brushes.White;
            tb.FontSize = fontSize;
            tb.TextAlignment = TextAlignment.Center;
            tb.SetValue(Canvas.LeftProperty, node.Center.X - (fontSize / 4 * node.Label.Length));
            tb.SetValue(Canvas.TopProperty, node.Center.Y - fontSize / 2);
            tb.SetValue(Canvas.ZIndexProperty, 3);

            //add to canvas on top of the cirle
            drawingCanvas.Children.Add(tb);
        }

        private Edge CreateEdge(Node node1, Node node2, double distance)
        {
            return new Edge(node1, node2, distance);
        }

        private void PaintEdge(Edge edge)
        {
            //draw the edge
            Line line = new Line();
            line.X1 = edge.FirstNode.Center.X;
            line.X2 = edge.SecondNode.Center.X;

            line.Y1 = edge.FirstNode.Center.Y;
            line.Y2 = edge.SecondNode.Center.Y;

            if (edge.Visited)
                line.Stroke = visitedBrush;
            else
                line.Stroke = unvisitedBrush;

            line.StrokeThickness = 1;
            line.SetValue(Canvas.ZIndexProperty, 1);
            drawingCanvas.Children.Add(line);

            //draw the distance label
            Point edgeLabelPoint = GetEdgeLabelCoordinate(edge);
            TextBlock tb = new TextBlock();
            tb.Text = edge.Length.ToString();
            tb.Foreground = Brushes.White;

            if (edge.Visited)
                tb.Background = visitedBrush;
            else
                tb.Background = unvisitedBrush;

            tb.Padding = new Thickness(5);
            tb.FontSize = edgeFontSize;

            tb.MinWidth = edgeLabelSize;
            tb.MinHeight = edgeLabelSize;

            tb.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
            tb.VerticalAlignment = System.Windows.VerticalAlignment.Center;
            tb.TextAlignment = TextAlignment.Center;

            tb.SetValue(Canvas.LeftProperty, edgeLabelPoint.X);
            tb.SetValue(Canvas.TopProperty, edgeLabelPoint.Y);
            tb.SetValue(Canvas.ZIndexProperty, 2);
            drawingCanvas.Children.Add(tb);
        }

        /// <summary>
        /// Calculate the coordinates where an edge label is to be drawn
        /// </summary>
        /// <param name="n1"></param>
        /// <param name="n2"></param>
        /// <param name="label"></param>
        /// <returns></returns>
        private Point GetEdgeLabelCoordinate(Edge edge)
        {

            double x = Math.Abs(edge.FirstNode.Location.X - edge.SecondNode.Location.X) / 2;
            double y = Math.Abs(edge.FirstNode.Location.Y - edge.SecondNode.Location.Y) / 2;

            if (edge.FirstNode.Location.X > edge.SecondNode.Location.X)
                x += edge.SecondNode.Location.X;
            else
                x += edge.FirstNode.Location.X;

            if (edge.FirstNode.Location.Y > edge.SecondNode.Location.Y)
                y += edge.SecondNode.Location.Y;
            else
                y += edge.FirstNode.Location.Y;

            return new Point(x, y);
        }
        /// <summary>
        /// The implementation of the Djikstra algorithm
        /// </summary>
        /// <param name="start">Starting Node</param>
        /// <param name="end">Ending Node</param>
        private void FindMinDistancePath(Node start, Node end)
        {
            cloud.Clear();
            reachableNodes.Clear();
            Node currentNode = start;
            currentNode.Visited = true;
            start.TotalCost = 0;
            cloud.Add(currentNode);
            ReachableNode currentReachableNode;

            while (currentNode != end)
            {
                AddReachableNodes(currentNode);
                UpdateReachableNodesTotalCost(currentNode);

                //if we cannot reach any other node, the graph is not connected
                if (reachableNodes.ReachableNodes.Count == 0)
                {
                    isGraphConnected = false;
                    break;
                }

                //get the closest reachable node
                currentReachableNode = reachableNodes.ReachableNodes[0];
                //remove if from the reachable nodes list
                reachableNodes.RemoveReachableNode(currentReachableNode);
                //mark the current node as visited
                currentNode.Visited = true;
                //set the current node to the closest one from the cloud
                currentNode = currentReachableNode.Node;
                //set a pointer to the edge from where we came from
                currentNode.EdgeCameFrom = currentReachableNode.Edge;
                //mark the edge as visited
                currentReachableNode.Edge.Visited = true;

                cloud.Add(currentNode);
            }
        }

        /// <summary>
        /// The method paints the the minimum distance path starting from the end node
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        private void PaintMinDistancePath(Node start, Node end)
        {
            if (isGraphConnected)
            {
                Node currentNode = end;
                double totalCost = 0;
                while (currentNode != start)
                {
                    currentNode.Visited = true;
                    currentNode.EdgeCameFrom.Visited = true;
                    totalCost += currentNode.EdgeCameFrom.Length;

                    PaintNode(currentNode);
                    PaintEdge(currentNode.EdgeCameFrom);

                    currentNode = GetNeighbour(currentNode, currentNode.EdgeCameFrom);
                }
                //paint the current node -> this is the start node
                if(currentNode != null)
                    PaintNode(currentNode);

                start.Visited = true;
                statusLabel.Content = "Total cost: " + totalCost.ToString();
            }
            else
            {
                ClearEdgeNodes();
                isGraphConnected = true;
                findMinDistance = false;
                statusLabel.Content = "Click on the canvas to create a node.";
                MessageBox.Show("The graph is not connected. Cannot find a path", "Error", MessageBoxButton.OK, MessageBoxImage.Hand);
            }
        }

        /// <summary>
        /// On each iteration we need to discover the reachable nodes by going through all the edges that are incident on the node
        /// </summary>
        /// <param name="node"></param>
        private void AddReachableNodes(Node node)
        {
            Node neighbour;
            ReachableNode rn;
            foreach (Edge edge in node.Edges)
            {
                neighbour = GetNeighbour(node, edge);
                //make sure we don't add the node we came from
                if (node.EdgeCameFrom == null || neighbour != GetNeighbour(node, node.EdgeCameFrom))
                {
                    //make sure we don't add a node already in the cloud
                    if (!cloud.Contains(neighbour))
                    {
                        //if the node is already reachable
                        if (reachableNodes.HasNode(neighbour))
                        {
                            //if the distance from this edge is smaller than the current total cost
                            //amend the reachable node using the current edge
                            if (node.TotalCost + edge.Length < neighbour.TotalCost)
                            {
                                rn = reachableNodes.GetReachableNodeFromNode(neighbour);
                                rn.Edge = edge;
                            }
                        }
                        else
                        {
                            rn = new ReachableNode(neighbour, edge);
                            reachableNodes.AddReachableNode(rn);
                        }
                    }
                }
            }

        }

        /// <summary>
        /// On each iteration the total cost of all reachable nodes needs to be recalculated
        /// </summary>
        /// <param name="node">The current node</param>
        private void UpdateReachableNodesTotalCost(Node node)
        {
            double currentCost = node.TotalCost;
            foreach (ReachableNode rn in reachableNodes.ReachableNodes)
            {
                if (currentCost + rn.Edge.Length < rn.Node.TotalCost || rn.Node.TotalCost == -1)
                    rn.Node.TotalCost = currentCost + rn.Edge.Length;
            }

            reachableNodes.SortReachableNodes();
        }

        /// <summary>
        /// Get the node on the other side of the edge
        /// </summary>
        /// <param name="node">The node from which we look for the neighbour</param>
        /// <param name="edge">The edge that we are currently on</param>
        /// <returns></returns>
        private Node GetNeighbour(Node node, Edge edge)
        {
            if (edge.FirstNode == node)
                return edge.SecondNode;
            else
                return edge.FirstNode;
        }

        private void findMinDistanceBtn_Click(object sender, RoutedEventArgs e)
        {
            this.findMinDistance = true;
            statusLabel.Content = "Select two nodes for which you want to find the minimum distance path";
        }

        private void clearBtn_Click(object sender, RoutedEventArgs e)
        {
            Clear();
            drawingCanvas.Children.Clear();
        }

        private void Clear()
        {
            this.nodes.Clear();
            this.edges.Clear();
            this.cloud.Clear();
            this.reachableNodes.Clear();
            this.findMinDistance = false;
            this.count = 1;
        }

        private void Restart()
        {
            this.findMinDistance = false;
            
            this.cloud.Clear();
            this.reachableNodes.Clear();

            foreach (Node n in nodes)
                n.Visited = false;

            foreach (Edge e in edges)
                e.Visited = false;
        }

        private void PaintAllNodes()
        {
            foreach (Node n in nodes)
                PaintNode(n);
        }

        private void PaintAllEdges()
        {
            foreach (Edge e in edges)
                PaintEdge(e);
        }

        private void restartBtn_Click(object sender, RoutedEventArgs e)
        {
            Restart();
            PaintAllNodes();
            PaintAllEdges();
        }
    }
}
