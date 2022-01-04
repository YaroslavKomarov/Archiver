using System;

namespace Archiver.Domain.Models.Haffman
{
    public class Tree
    {
        public int Value { get; set; }
        public Tree Left { get; set; }
        public Tree Right { get; set; }

        public Tree(int left, int right)
        {
            Value = left + right;
            Left = new Tree(left);
            Right = new Tree(right);
        }

        public Tree(int value)
        {
            Value = value;
        }
    }
}
