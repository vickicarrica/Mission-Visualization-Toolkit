using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LoudDictionary<TKey, TValue> : IDictionary<TKey, TValue>
{
    private IDictionary<TKey, TValue> d;
    public event EventHandler OnChange;
    public LoudDictionary() {d = new Dictionary<TKey, TValue>();}
    public LoudDictionary(int capacity) {d = new Dictionary<TKey, TValue>(capacity);}
    public LoudDictionary(IEqualityComparer<TKey> comparer) {d = new Dictionary<TKey, TValue>(comparer);}
    public LoudDictionary(int capacity, IEqualityComparer<TKey> comparer) {d = new Dictionary<TKey, TValue>(capacity, comparer);}
    public LoudDictionary(IDictionary<TKey, TValue> _d) {d = new Dictionary<TKey, TValue>(_d);}
    private void callEvent(EventArgs e)
    {
        EventHandler handler = OnChange;
        handler?.Invoke(this, e);
    }
    public void Add(TKey key, TValue value)
    {
        d.Add(key, value);
        callEvent(EventArgs.Empty);
    }
    public void Add(KeyValuePair<TKey, TValue> i) {Add(i.Key, i.Value);}
    public void Clear()
    {
        d.Clear();
        callEvent(EventArgs.Empty);
    }
    public bool Remove(TKey key)
    {
        bool success = d.Remove(key);
        callEvent(EventArgs.Empty);
        return success;
    }

    public bool TryGetValue(TKey key, out TValue value) => d.TryGetValue(key, out value);
    public bool ContainsKey(TKey key) => d.ContainsKey(key);
    public ICollection<TKey> Keys {get{return d.Keys;}}
    public ICollection<TValue> Values {get{return d.Values;}}
    public TValue this[TKey key]{get{return d[key];} set{d[key] = value;}}
    public bool Contains(KeyValuePair<TKey, TValue> i) => d.Contains(i);
    public void CopyTo(KeyValuePair<TKey, TValue>[] ar, int arI) {d.CopyTo(ar, arI);}
    public int Count {get{return d.Count;}}
    public bool IsReadOnly {get{return d.IsReadOnly;}}
    public bool Remove(KeyValuePair<TKey, TValue> i) => d.Remove(i);
    public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => d.GetEnumerator();
    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

}