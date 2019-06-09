using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ShortestPath
{
    public class BDDManager
    {
        public BDDNode Zero { get; private set; }
        
        public BDDNode One { get; private set; }

        public int N
        {
            get { return _n; }
            set
            {
                _n = value;
                Zero.Index = _n;
                One.Index = _n;
            }
        }

        int _n;
        public int nextId = 0;
        IDictionary<Tuple<int, int, int>, WeakReference> _ite_cache;
        List<int> _variable_order;
       
        public int[] VariableOrder
        {
            get
            {
                return _variable_order.ToArray();
            }
        }

        
        public BDDManager(int n)
        {
            this.Zero = Create(n, false);
            this.One = Create(n, true);

            _n = n;
            _ite_cache = new Dictionary<Tuple<int, int, int>, WeakReference>();
            _variable_order = new List<int>(Enumerable.Range(0, n));
            if (_variable_order.Count() != n)
                throw new ArgumentException();

        }

        public BDDNode Create(int index, int high, BDDNode low)
        {
            return Create(index, high == 0 ? Zero : One, low);
        }

        public BDDNode Create(int index, BDDNode high, int low)
        {
            return Create(index, high, low == 0 ? Zero : One);
        }

        public BDDNode Create(int index, int high, int low)
        {
            return Create(index, high == 0 ? Zero : One, low == 0 ? Zero : One);
        }

        public BDDNode Create(int index, BDDNode high, BDDNode low)
        {
            BDDNode unique;

            unique = new BDDNode(index, high, low) { Id = nextId++ };
            high.RefCount++;
            low.RefCount++;

            return unique;
        }

        public BDDNode Create(int index, bool value)
        {
            return new BDDNode(index, value) { Id = nextId++ };
        }
    }
}

