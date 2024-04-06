using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Robot.Driver
{
    
        public class FixedSizedQueue<T> : INotifyCollectionChanged, IEnumerable<T>
    {
            readonly ConcurrentQueue<T> queue = new ConcurrentQueue<T>();

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public int Size { get; private set; }

            public FixedSizedQueue(int size)
            {
                Size = size;
            }

            public void Enqueue(T obj)
            {
                queue.Enqueue(obj);

                while (queue.Count > Size)
                {
                    T outObj;
                    queue.TryDequeue(out outObj);
                }
            }

        public T Dequeue()
        {
            T outObj = default(T);
            queue.TryDequeue(out outObj);
            return outObj;
           
        }

        public IEnumerator<T> GetEnumerator()
        {
            return queue.GetEnumerator();
        }

      

        IEnumerator IEnumerable.GetEnumerator()
        {
            return queue.GetEnumerator();
        }

        public IList GetArray()
        {
            return queue.ToList();
        }
    }
    
}
