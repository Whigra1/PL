using System;
using System.Collections.Generic;
using System.Threading;

public class SkipList<T> where T : IComparable{

    private int height;
    private double p;
    private Node<T> head;

    class Node<T> where T : IComparable {

        public T Data { get; set; }
        public Node<T> Right;
        public Node<T> Down { get; set; }

        public Node(T data, Node<T> right, Node<T> down) {
            Data = data;
            Right = right;
            Down = down;
        }
    }

    public SkipList(int height, double p) {
        this.height = height;
        this.p = p;

        Node<T> element = new(default, null, null);
        head = element;

        for (int i = 0; i < height - 1; i++) {
            Node<T> newElementHead = new(default, null, null);
            element.Down = newElementHead;
            element = newElementHead;
        }
    }

    public bool Remove(T data) {
        if (data is null) {
            throw new Exception("Argument should not be null");
        }

        Node<T> currEl = head;
        var currentLevel = height;
        var towerUnmarked = true;

        while (currentLevel > 0) {
            Node<T> rightEl = currEl.Right;
            if (rightEl is not null && rightEl.Data.CompareTo(data) == 0) {
                Node<T> afterRightEl = rightEl.Right;
                if (towerUnmarked) {
                    Node<T> towerEl = rightEl;
                    while (towerEl is not null)
                    {
                        Interlocked.CompareExchange(ref towerEl.Right, null, towerEl.Right);
                        towerEl = towerEl.Down;
                    }
                    towerUnmarked = false;
                }
                Interlocked.CompareExchange(ref currEl.Right, afterRightEl, rightEl);
            }

            if (rightEl is not null && rightEl.Data.CompareTo(data) < 0) {
                currEl = rightEl;
            } else {
                currEl = currEl.Down;
                currentLevel--;
            }
        }

        return !towerUnmarked;
    }

    public bool Add(T data) {
        if (data is null) {
            throw new Exception("Argument should not be null");
        }

        List<Node<T>> prev = new ();
        List<Node<T>> prevRight = new ();
        Node<T> currEl = head;
        var levelOfTower = GetRandomLevel();
        var currentLevel = height;

        while (currentLevel > 0) {
            Node<T> rightEl = currEl.Right;

            if (currentLevel <= levelOfTower) {
                if (rightEl is null|| rightEl.Data.CompareTo(data) >= 0) {
                    prev.Add(currEl);
                    prevRight.Add(rightEl);
                }
            }

            if (rightEl is not null && rightEl.Data.CompareTo(data) < 0) {
                currEl = rightEl;
            } else {
                currEl = currEl.Down;
                currentLevel--;
            }
        }

        Node<T> downEl = null;
        for (int i = prev.Count - 1; i >= 0; i--) {
            Node<T> newEl = new(data, prevRight[i], null);

            if (downEl is not null) {
                newEl.Down = downEl;
            }

            if (Interlocked.CompareExchange(ref prev[i].Right, newEl, prevRight[i]) == prevRight[i] && i == prev.Count - 1) {
                return false;
            }

            downEl = newEl;
        }

        return true;
    }

    private int GetRandomLevel() {
        var lvl = 1;
        var random = new Random();
        while (lvl < height && random.Next() < p) {
            lvl++;
        }

        return lvl;
    }
}