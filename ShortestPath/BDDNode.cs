using System;
using System.Collections.Generic;
using System.Linq;

namespace ShortestPath
{
    public class BDDNode
    {
        public int Id { get; set; }

        public bool? Value { get; set; }

        public int Index { get; set; }

        public BDDNode Low { get; set; }

        public BDDNode High { get; set; }

        public int RefCount { get; set; }

        public bool IsOne
        {
            get
            {
                return Value != null && ((bool)Value) == true;
            }
        }

        public bool IsZero
        {
            get { return Value != null && ((bool)Value) == false; }
        }

        public BDDNode()
        {
        }
        // Лучше использовать create
        public BDDNode(int index, BDDNode high, BDDNode low) : this()
        {
            this.Index = index;
            this.High = high;
            this.Low = low;
        }
        // Лучше использовать create
        public BDDNode(int index, bool value) : this()
        {
            this.Value = value;
            this.Index = index;
        }
    }
}

