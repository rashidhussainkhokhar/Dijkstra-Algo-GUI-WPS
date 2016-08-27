using System;
using System.Collections.Generic;
using System.Text;

namespace MinDistanceWpf.Classes
{
    public class ReachableNodeList
    {
        private List<ReachableNode> rnList;
        private List<Node> _nodes;
        private Dictionary<Node, ReachableNode> rnDictionary;

        public ReachableNodeList()
        {
            rnList = new List<ReachableNode>();
            _nodes = new List<Node>();
            rnDictionary = new Dictionary<Node, ReachableNode>();
        }

        public void AddReachableNode(ReachableNode rn)
        {
            if (_nodes.Contains(rn.Node))
            {
                ReachableNode oldRN = GetReachableNodeFromNode(rn.Node);
                if (rn.Edge.Length < oldRN.Edge.Length)
                    oldRN.Edge = rn.Edge;
            }
            else
            {
                rnList.Add(rn);
                _nodes.Add(rn.Node);
                rnDictionary.Add(rn.Node, rn);
            }
        }

        public List<ReachableNode> ReachableNodes
        {
            get { return this.rnList; }
        }

        public void RemoveReachableNode(ReachableNode rn)
        {
            rnList.Remove(rn);
            _nodes.Remove(rn.Node);
        }

        public bool HasNode(Node n)
        {
            return _nodes.Contains(n);
        }

        public ReachableNode GetReachableNodeFromNode(Node n)
        {
            if (rnDictionary.ContainsKey(n))
            {
                return rnDictionary[n];
            }
            else
            {
                return null;
            }
        }

        public void SortReachableNodes()
        {
            rnList.Sort();
        }

        public void Clear()
        {
            this.rnList.Clear();
            this._nodes.Clear();
            this.rnDictionary.Clear();
        }
    }
}
