using System.Threading;

namespace ParralelLabs.Task3
{
    public class MichaelAndScott<T>
    {
        private class Node<K>
        {
            internal K Item;
            internal Node<K> Next;
            public Node(K item, Node<K> next)
            {
                Item = item;
                Next = next;
            }
        }

        private Node<T> _head;
        private Node<T> _tail;

        public MichaelAndScott()
        {
            _head = new Node<T>(default, null);
            _tail = _head;
        }

        public void Enqueue(T item)
        {
            Node<T> newNode = new(item, null);
            while (true)
            {
                Node<T> curTail = _tail;
                Node<T> nextEl = curTail.Next;

                if (curTail != _tail) continue;
                if (nextEl == null)
                {
                    if (Interlocked.CompareExchange<Node<T>>(
                        ref curTail.Next, newNode, nextEl) != nextEl) continue;
                    Interlocked.CompareExchange(ref _tail, newNode, curTail);
                    return;
                }
                Interlocked.CompareExchange(ref _tail, nextEl, curTail);
            }
        }

        public bool TryDequeue(out T result)
        {
            Node<T> curHead;
            Node<T> curTail;
            Node<T> next;
            do
            {
                curHead = _head;
                curTail = _tail;
                next = curHead.Next;
                if (curHead != _head) continue;
                if (next == null) //Queue is empty
                {
                    result = default;
                    return false;
                }

                if (curHead == curTail)
                {
                    Interlocked.CompareExchange(ref _tail, next, curTail);
                }
                else
                {
                    result = next.Item;
                    if (Interlocked.CompareExchange(ref _head,
                        next, curHead) == curHead)
                        break;
                }
            } while (true);
            return true;
        }
    }
}