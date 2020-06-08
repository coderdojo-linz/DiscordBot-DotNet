using System;
using System.Collections.Generic;

namespace LibMCRcon.Remote
{
    public class TransferQueue<T> : Queue<T>
    {
        private readonly object _syncRoot = new object();
        public object SyncRoot { get { return _syncRoot; } }

        public bool IsIdle { get; set; }

        public TransferQueue() : base() { }
        public TransferQueue(int capacity) : base(capacity) { }
        public TransferQueue(IEnumerable<T> ti) : base(ti) { }

        public void MarkIdle()
        {
            if (Count == 0)
                IsIdle = true;
        }

        public bool CheckIdle()
        {
            if (Count > 0)
                IsIdle = false;

            return IsIdle;
        }

        public void Enqueue(List<T> list)
        {
            lock (SyncRoot)
                list.ForEach(x => base.Enqueue(x));

        }

        public new void Enqueue(T fi)
        {
            lock (SyncRoot)
                base.Enqueue(fi);
        }

        public void EnqueueNoDuplicates(T fi)
        {
            lock (SyncRoot)
            {
                if (!base.Contains(fi))
                    base.Enqueue(fi);
            }
        }


        public new T Dequeue()
        {
            lock (SyncRoot)
            {
                if (base.Count > 0)
                    return base.Dequeue();
                
                return default(T);
            }
        }

        public void ForEach(Action<T> ForEachAction)
        {
            lock (SyncRoot)
                foreach (T x in this)
                    ForEachAction(x);
        }
    }
  

    


    
}