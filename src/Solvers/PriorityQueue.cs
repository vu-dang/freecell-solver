﻿using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using System.Collections.Generic;

namespace FreeCellSolver.Solvers
{
    public sealed class PriorityQueue<T> where T : IComparable<T>, IEquatable<T>
    {
        private const int Arity = 4;
        private const int Log2Arity = 2;
        private const int MinCapacity = 4;

        private readonly HashSet<T> _hash;

        private T[] _nodes;
        private int _size;

        public int Count => _size;

        public PriorityQueue()
        {
            _nodes = new T[MinCapacity];
            _hash = new(MinCapacity);
        }

        public PriorityQueue(int initialCapacity)
        {
            Debug.Assert(initialCapacity > 0);

            _nodes = new T[Math.Max(initialCapacity, MinCapacity)];
            _hash = new(Math.Max(initialCapacity, MinCapacity));
        }

        public void Enqueue(T element)
        {
            Debug.Assert(!_hash.Contains(element));

            var currentSize = _size++;

            if (_nodes.Length == currentSize)
            {
                Array.Resize(ref _nodes, _nodes.Length * 2);
            }

            _hash.Add(element);
            MoveUp(element, currentSize);
        }

        public T Dequeue()
        {
            Debug.Assert(_size > 0);

            var element = _nodes[0];
            _hash.Remove(element);
            RemoveRootNode();
            return element;
        }

        public void Replace(T existing, T replacement)
        {
            Debug.Assert(_hash.Contains(existing));
            Debug.Assert(!_hash.Contains(replacement) || !ReferenceEquals(existing, replacement));

            _hash.Remove(existing);
            _hash.Add(replacement);
            var index = _nodes.AsSpan().IndexOf(existing);

            if (index != 0 && replacement.CompareTo(_nodes[GetParentIndex(index)]) < 0)
            {
                MoveUp(replacement, index);
            }
            else
            {
                MoveDown(replacement, index);
            }
        }

        public bool TryGetValue(T equalValue, out T actualValue)
            => _hash.TryGetValue(equalValue, out actualValue);

        public void Clear()
        {
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                Array.Clear(_nodes, 0, _size);
            }

            _size = 0;
        }

        private void RemoveRootNode()
        {
            var lastNodeIndex = _size - 1;
            var lastNode = _nodes[lastNodeIndex];
            if (RuntimeHelpers.IsReferenceOrContainsReferences<T>())
            {
                _nodes[lastNodeIndex] = default;
            }

            if (--_size > 0)
            {
                MoveDown(lastNode, 0);
            }
        }

        private static int GetParentIndex(int index) => (index - 1) >> Log2Arity;

        private static int GetFirstChildIndex(int index) => (index << Log2Arity) + 1;

        private void MoveUp(T node, int nodeIndex)
        {
            var nodes = _nodes;

            while (nodeIndex > 0)
            {
                var parentIndex = GetParentIndex(nodeIndex);
                var parent = nodes[parentIndex];

                if (node.CompareTo(parent) < 0)
                {
                    nodes[nodeIndex] = parent;
                    nodeIndex = parentIndex;
                }
                else
                {
                    break;
                }
            }

            nodes[nodeIndex] = node;
        }

        private void MoveDown(T node, int nodeIndex)
        {
            var nodes = _nodes;
            var size = _size;

            int i;
            while ((i = GetFirstChildIndex(nodeIndex)) < size)
            {
                var minChild = nodes[i];
                var minChildIndex = i;

                var childIndexUpperBound = Math.Min(i + Arity, size);
                while (++i < childIndexUpperBound)
                {
                    var nextChild = nodes[i];
                    if (nextChild.CompareTo(minChild) < 0)
                    {
                        minChild = nextChild;
                        minChildIndex = i;
                    }
                }

                if (node.CompareTo(minChild) <= 0)
                {
                    break;
                }

                nodes[nodeIndex] = minChild;
                nodeIndex = minChildIndex;
            }

            nodes[nodeIndex] = node;
        }
    }
}