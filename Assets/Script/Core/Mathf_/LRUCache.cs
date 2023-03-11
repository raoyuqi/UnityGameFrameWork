using FrameWork.Core.Manager;
using System.Collections.Generic;
using UnityEngine.Events;
using static FrameWork.Core.Manager.MemoryManger;

namespace FrameWork.Core.Mathf_
{
    public sealed class Node<T>
    {
        public string Key { get; private set; }
        public T Data { get; set; }
        public Node<T> Next { get; set; }
        public Node<T> Prev { get; set; }

        public Node() { }

        public Node(string key, T data)
        {
            this.Key = key;
            this.Data = data;
        }
    }

    public sealed class FreeEvent<T> : UnityEvent<T> { }

    public sealed class LRUCache<T>
    {
        public delegate void FreeOldestCallBack(T assetData);
        public event FreeOldestCallBack FreeOldestNodeCallBack;

        private Node<T> m_Heade;
        private Node<T> m_Tail;
        private Dictionary<string, Node<T>> m_CacheDic;
        public int Size { get { return this.m_CacheDic.Count; } }
        public int Capacity { get; private set; }

        public LRUCache(int capacity)
        {
            this.Capacity = capacity;
            this.m_Heade = new Node<T>();
            this.m_Tail = new Node<T>();
            this.m_Heade.Next = this.m_Tail;
            this.m_Tail.Prev = this.m_Heade;

            this.m_CacheDic = new Dictionary<string, Node<T>>();
        }

        public T Get(string key)
        {
            if (!this.m_CacheDic.ContainsKey(key))
                return default(T);

            var node = this.m_CacheDic[key];
            this.RefreshNodeList(node);
            return node.Data;
        }

        public void Put(string key, T value)
        {
            if (this.Capacity <= 0)
                return;

            if (this.m_CacheDic.TryGetValue(key, out var node))
            {
                node.Data = value;
                this.RefreshNodeList(node);
            }
            else
            {
                if (this.m_CacheDic.Count >= this.Capacity)
                    this.FreeOldestNode();

                node = new Node<T>(key, value);
                this.InsertToHead(node);
                this.m_CacheDic.Add(key, node);
            }
        }

        public void Remove(string key)
        {
            if (this.Capacity <= 0)
                return;

            if (this.m_CacheDic.TryGetValue(key, out var node))
            {
                this.RemoveNode(node);
                this.m_CacheDic.Remove(key);
            }
        }

        public void Clean()
        {
            this.m_CacheDic.Clear();
            this.m_Heade.Next = this.m_Tail;
            this.m_Tail.Prev = this.m_Heade;
        }

        private void RefreshNodeList(Node<T> node)
        {
            this.RemoveNode(node);
            this.InsertToHead(node);
        }

        private void RemoveNode(Node<T> node)
        {
            if (node == this.m_Heade || node == this.m_Tail)
                return;

            node.Next.Prev = node.Prev;
            node.Prev.Next = node.Next;
            node.Prev = null;
            node.Next = null;
        }

        private void InsertToHead(Node<T> node)
        {
            if (node == this.m_Heade || node == this.m_Tail)
                return;

            var temp = this.m_Heade.Next;
            this.m_Heade.Next = node;
            node.Prev = this.m_Heade;
            node.Next = temp;
            temp.Prev = node;
        }

        private void FreeOldestNode()
        {
            if (this.m_Tail.Prev == this.m_Heade)
                return;

            var node = this.m_Tail.Prev;
            this.m_Tail.Prev = node.Prev;
            node.Prev.Next = this.m_Tail;
            node.Prev = null;
            node.Next = null;

            this.m_CacheDic.Remove(node.Key);
            this.FreeOldestNodeCallBack?.Invoke(node.Data);
            node.Data = default(T);
        }
    }
}