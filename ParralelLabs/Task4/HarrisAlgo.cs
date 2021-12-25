using System;
using System.Threading;

namespace ParralelLabs.Task4
{
    public class HarrisOrderedList<T> where T: IComparable<T>
    {

        private Node<T> head = new(default, null);
        class Node<T>
        {
            public T Data;
            public Node<T> Next;
            public Node(T data, Node<T> next) {
                Data = data;
                Next = next;
            }
        }

        public bool Remove(T data) {
            if (data is null) {
                throw new Exception("Argument should not be null");
            }

            Node<T> prevEl = head;
            while (prevEl.Next is not null) {
                Node<T> currEl = prevEl.Next;
                Node<T> nextEl = currEl.Next;

                if (currEl.Data.CompareTo(data) == 0) {
                    if (Interlocked.CompareExchange(ref currEl.Next, null, nextEl) == nextEl &&
                        Interlocked.CompareExchange(ref prevEl.Next, nextEl, currEl) == currEl
                    ){
                        return true;
                    }
                } else {
                    prevEl = currEl;
                }
            }

            return false;
        }

        public void Add(T data) {
            if (data is null) {
                throw new Exception("Argument should not be null");
            }

            Node<T> newEl = new (data, null);
            Node<T> currentEl = head;

            while (true) {
                Node<T> nextEl = currentEl.Next;
                if (nextEl is not null) {
                    if (nextEl.Data.CompareTo(data) >= 0) {
                        newEl.Next = nextEl;
                        if (Interlocked.CompareExchange(ref currentEl.Next, newEl, nextEl) == nextEl) {
                            return;
                        }
                    } else {
                        currentEl = nextEl;
                    }
                } else if (Interlocked.CompareExchange(ref currentEl.Next, newEl, null) == null ) {
                    return;
                }
            }
        }
  }
}