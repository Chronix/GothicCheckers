using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace GothicCheckers
{
    public class JumpTreeNode
    {
        public int Depth { get; set; }
        public JumpTreeNode Parent { get; set; }
        public Collection<JumpTreeNode> Children { get; private set; }
        public BoardPosition Position { get; set; }
        public GameBoard Board { get; set; }

        public bool HasChildren
        {
            get { return Children.Count > 0; }
        }

        public JumpTreeNode()
        {
            Children = new Collection<JumpTreeNode>();
        }
    }

    public class JumpTree
    {
        private JumpTreeNode _root;

        public JumpTree(BoardPosition root, GameBoard board)
        {
            _root = new JumpTreeNode
            {
                Depth = 0,
                Position = root,
                Board = board
            };
        }

        public void AddTargets(BoardPosition to, IEnumerable<BoardPosition> targets)
        {

            List<JumpTreeNode> possibleNodes = new List<JumpTreeNode>();
            Queue<JumpTreeNode> row = new Queue<JumpTreeNode>();
            row.Enqueue(_root);

            while (row.Count > 0)
            {
                JumpTreeNode n = row.Dequeue();

                if (n.Position.Equals(to))
                {
                    possibleNodes.Add(n);
                }

                foreach (var node in n.Children) row.Enqueue(node);
            }

            JumpTreeNode targetNode = possibleNodes[0];

            for (int i = 1; i < possibleNodes.Count; ++i)
            {
                if (possibleNodes[i].Depth > targetNode.Depth) targetNode = possibleNodes[i];
            }

            foreach (BoardPosition pos in targets)
            {
                GameBoard board = targetNode.Board.Copy();
                board.DoRawMove(targetNode.Position, pos, true, false);
                targetNode.Children.Add(new JumpTreeNode { Board = board, Depth = targetNode.Depth + 1, Parent = targetNode, Position = pos });
            }
        }

        public List<BoardPosition>[] Linearize()
        {
            var result = new List<List<BoardPosition>>();

            Collection<JumpTreeNode> leaves = GetLeaves();

            foreach (JumpTreeNode leaf in leaves)
            {
                List<BoardPosition> path = new List<BoardPosition>();
                JumpTreeNode work = leaf;

                while (work != null)
                {
                    path.Add(work.Position);
                    work = work.Parent;
                }

                path.Reverse(); //cesta z listu do korene, tah ve hre je ovsem v opacnem smeru
                result.Add(path);
            }

            return result.ToArray();
        }

        public Collection<JumpTreeNode> GetLeaves()
        {
            Collection<JumpTreeNode> leaves = new Collection<JumpTreeNode>();
            Queue<JumpTreeNode> row = new Queue<JumpTreeNode>();
            row.Enqueue(_root);

            while (row.Count > 0)
            {
                JumpTreeNode n = row.Dequeue();

                if (!n.HasChildren) leaves.Add(n);
                else foreach (var node in n.Children) row.Enqueue(node);
            }

            return leaves;
        }
    }
}
