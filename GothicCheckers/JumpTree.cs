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
        public bool CheckAgain { get; set; }

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

        public int NodeCount { get; private set; }

        public JumpTree(BoardPosition root, IEnumerable<BoardPosition> targets, GameBoard board)
        {
            _root = new JumpTreeNode
            {
                Depth = 0,
                Position = root,
                Board = board
            };

            NodeCount = 1;

            foreach (BoardPosition pos in targets)
            {
                GameBoard boardCopy = _root.Board.Copy();
                boardCopy.DoMove(new SimpleMove(boardCopy[_root.Position].Occupation, _root.Position, pos), true);
                _root.Children.Add(new JumpTreeNode { Board = boardCopy, Depth = _root.Depth + 1, Parent = _root, Position = pos, CheckAgain = true });
                ++NodeCount;
            }
        }

        public void AddTargets(JumpTreeNode targetNode, IEnumerable<BoardPosition> targets)
        {
            foreach (BoardPosition pos in targets)
            {
                GameBoard board = targetNode.Board.Copy();
                board.DoMove(new SimpleMove(board[targetNode.Position].Occupation, targetNode.Position, pos), true);
                targetNode.Children.Add(new JumpTreeNode { Board = board, Depth = targetNode.Depth + 1, Parent = targetNode, Position = pos, CheckAgain = true });
                ++NodeCount;
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

        public Collection<JumpTreeNode> GetLeaves(bool toCheckOnly = false)
        {
            Collection<JumpTreeNode> leaves = new Collection<JumpTreeNode>();
            Queue<JumpTreeNode> row = new Queue<JumpTreeNode>();
            row.Enqueue(_root);

            while (row.Count > 0)
            {
                JumpTreeNode n = row.Dequeue();

                if (!n.HasChildren)
                {
                    if (toCheckOnly && !n.CheckAgain) continue;

                    leaves.Add(n);
                }
                else foreach (var node in n.Children) row.Enqueue(node);
            }

            return leaves;
        }
    }
}
