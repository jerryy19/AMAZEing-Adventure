using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Graph<T> where T : IComparable<T>
{

    public Dictionary<Vertex<T>, List<Vertex<T>>> edges = new Dictionary<Vertex<T>, List<Vertex<T>>>();
    public Dictionary<Vertex<T>, List<int>> costs = new Dictionary<Vertex<T>, List<int>>();
    public List<Vertex<T>> vertices = new List<Vertex<T>>();
    public int Count = 0;

    public int getCost(Vertex<T> to, Vertex<T> from) {
        int index = edges[to].IndexOf(from);
        return (index == -1) ? -1 : costs[to][index];
    }

    public void addVertex(T t) {
        if (vertices.IndexOf(new Vertex<T>(t)) == -1) {
            edges.Add(new Vertex<T>(t), new List<Vertex<T>>());
            costs.Add(new Vertex<T>(t), new List<int>());
            vertices.Add(new Vertex<T>(t));
            Count++;
        }
    }

    public void removeVertex(T t) {
        if (vertices.IndexOf(new Vertex<T>(t)) == -1) {
            edges.Remove(new Vertex<T>(t));
            costs.Remove(new Vertex<T>(t));
            vertices.Remove(new Vertex<T>(t));
            Count--;
        }
    }

    public void addEdge(T to, T from, int cost = 1) {
        Vertex<T> t = new Vertex<T>(to);
        Vertex<T> f = new Vertex<T>(from);

        if (edges.ContainsKey(t) && edges[t].IndexOf(f) == -1) {
            edges[t].Add(f);
            costs[t].Add(cost);
        }
    }

    public void removeEdge(T to, T from) {
        Vertex<T> t = new Vertex<T>(to);
        Vertex<T> f = new Vertex<T>(from);

        if (edges.ContainsKey(t) && edges[t].Count != 0) {
            int index = edges[t].IndexOf(f);
            edges[t].Remove(f);
            costs[t].Remove(index);
        }
    }

}

public class Vertex<T> : IComparable<Vertex<T>> where T : IComparable<T> {
    public T data;
    
    public Vertex(T data) {
        this.data = data;
    }

    public override bool Equals(System.Object obj)
    {
        //Check for null and compare run-time types.
        if ((obj == null) || ! this.GetType().Equals(obj.GetType())) {
            return false;
        }
        Vertex<T> o = (Vertex<T>) obj;
        return this.data.Equals(o.data);
    }

    public override int GetHashCode()
    {
        return this.data.GetHashCode();
    }

    public int CompareTo(Vertex<T> vertex) {

        return this.data.CompareTo(vertex.data);
    }

}
