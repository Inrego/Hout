using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Hout.Models.Device
{
    public class PropertyCollection : IDictionary<string, object>, INotifyCollectionChanged, INotifyPropertyChanged
    {
        private readonly IDictionary<string, object> _internalDic;
        private const string CountString = "Count";
        private const string IndexerName = "Item[]";
        private const string KeysName = "Keys";
        private const string ValuesName = "Values";
        public PropertyCollection() : this(new Dictionary<string, object>()) { }

        public PropertyCollection(Dictionary<string, object> values)
        {
            _internalDic = values;
        }
        private void Insert(string key, object value, bool add)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            object item;
            if (_internalDic.TryGetValue(key, out item))
            {
                if (add) throw new ArgumentException("An item with the same key has already been added.");
                if (Equals(item, value)) return;
                _internalDic[key] = value;

                OnCollectionChanged(NotifyCollectionChangedAction.Replace, new KeyValuePair<string, object>(key, value), new KeyValuePair<string, object>(key, item));
                OnPropertyChanged(key);
            }
            else
            {
                _internalDic[key] = value;

                OnCollectionChanged(NotifyCollectionChangedAction.Add, new KeyValuePair<string, object>(key, value));
                OnPropertyChanged(key);
            }
        }
        IEnumerator<KeyValuePair<string, object>> IEnumerable<KeyValuePair<string, object>>.GetEnumerator()
        {
            return _internalDic.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _internalDic.GetEnumerator();
        }

        void ICollection<KeyValuePair<string, object>>.Add(KeyValuePair<string, object> item)
        {
            Insert(item.Key, item.Value, true);
        }

        void ICollection<KeyValuePair<string, object>>.Clear()
        {
            if (_internalDic.Count <= 0) return;
            _internalDic.Clear();
            OnCollectionChanged(NotifyCollectionChangedAction.Reset);
        }

        bool ICollection<KeyValuePair<string, object>>.Contains(KeyValuePair<string, object> item)
        {
            return _internalDic.Contains(item);
        }

        void ICollection<KeyValuePair<string, object>>.CopyTo(KeyValuePair<string, object>[] array, int arrayIndex)
        {
            _internalDic.CopyTo(array, arrayIndex);
        }

        bool ICollection<KeyValuePair<string, object>>.Remove(KeyValuePair<string, object> item)
        {
            return Remove(item.Key);
        }

        int ICollection<KeyValuePair<string, object>>.Count => _internalDic.Count;

        bool ICollection<KeyValuePair<string, object>>.IsReadOnly => _internalDic.IsReadOnly;

        bool IDictionary<string, object>.ContainsKey(string key)
        {
            return _internalDic.ContainsKey(key);
        }

        public void Add(string key, object value)
        {
            Insert(key, value, true);
        }

        bool IDictionary<string, object>.Remove(string key)
        {
            return Remove(key);
        }

        private bool Remove(string key)
        {
            if (key == null) throw new ArgumentNullException(nameof(key));

            object value;
            _internalDic.TryGetValue(key, out value);
            var removed = _internalDic.Remove(key);
            if (removed)
                OnCollectionChanged(NotifyCollectionChangedAction.Remove, new KeyValuePair<string, object>(key, value));
            return removed;
        }

        bool IDictionary<string, object>.TryGetValue(string key, out object value)
        {
            return _internalDic.TryGetValue(key, out value);
        }

        public bool ContainsKey(string key)
        {
            return _internalDic.ContainsKey(key);
        }

        public object this[string key]
        {
            get { return _internalDic[key]; }
            set { Insert(key, value, false); }
        }
        public T GetValue<T>(string key)
        {
            var val = _internalDic[key];
            return val is T ? (T)val : (T)Convert.ChangeType(val, typeof(T));
        }

        ICollection<string> IDictionary<string, object>.Keys => _internalDic.Keys;

        ICollection<object> IDictionary<string, object>.Values => _internalDic.Values;
        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged()
        {
            OnPropertyChanged(CountString);
            OnPropertyChanged(IndexerName);
            OnPropertyChanged(KeysName);
            OnPropertyChanged(ValuesName);
        }

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private void OnCollectionChanged(NotifyCollectionChangedAction action)
        {
            OnPropertyChanged();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action));
        }
        private void OnCollectionChanged(NotifyCollectionChangedAction action, KeyValuePair<string, object> changedItem)
        {
            OnPropertyChanged();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action, changedItem, 0));
        }

        private void OnCollectionChanged(NotifyCollectionChangedAction action, KeyValuePair<string, object> newItem, KeyValuePair<string, object> oldItem)
        {
            OnPropertyChanged();
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action, newItem, oldItem, 0));
        }
    }
}
