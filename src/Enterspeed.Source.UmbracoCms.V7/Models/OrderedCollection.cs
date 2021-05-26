using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Enterspeed.Source.UmbracoCms.V7.Models
{
    public class OrderedCollection<TItem> : IEnumerable<TItem>
    {
        private readonly List<Type> _types;

        public OrderedCollection()
        {
            _types = new List<Type>();
        }

        public OrderedCollection<TItem> Clear()
        {
            _types.Clear();
            return this;
        }

        public OrderedCollection<TItem> Append<T>()
            where T : TItem
        {
            var type = typeof(T);
            AppendToList(type);
            return this;
        }

        public OrderedCollection<TItem> Append(Type type)
        {
            AppendToList(type);
            return this;
        }

        public OrderedCollection<TItem> Append(IEnumerable<Type> types)
        {
            foreach (var type in types)
            {
                AppendToList(type);
            }

            return this;
        }

        public OrderedCollection<TItem> Insert(int index = 0)
        {
            InsertToList(typeof(TItem), index);
            return this;
        }

        public OrderedCollection<TItem> Insert(Type type, int index = 0)
        {
            InsertToList(type, index);
            return this;
        }

        public OrderedCollection<TItem> InsertBefore<TBefore, T>()
            where TBefore : TItem
            where T : TItem
        {
            var typeBefore = typeof(TBefore);
            var type = typeof(T);

            InsertBeforeInList(typeBefore, type);

            return this;
        }

        public OrderedCollection<TItem> InsertBefore(Type typeBefore, Type type)
        {
            InsertBeforeInList(typeBefore, type);
            return this;
        }

        public OrderedCollection<TItem> InsertAfter<TAfter, T>()
            where TAfter : TItem
            where T : TItem
        {
            var typeAfter = typeof(TAfter);
            var type = typeof(T);

            InsertAfterInList(typeAfter, type);

            return this;
        }

        public OrderedCollection<TItem> InsertAfter(Type typeAfter, Type type)
        {
            InsertAfterInList(typeAfter, type);
            return this;
        }

        public OrderedCollection<TItem> Remove<T>()
            where T : TItem
        {
            var type = typeof(T);
            RemoveFromList(type);
            return this;
        }

        public OrderedCollection<TItem> Remove(Type type)
        {
            RemoveFromList(type);
            return this;
        }

        public OrderedCollection<TItem> Replace<TReplaced, T>()
            where TReplaced : TItem
            where T : TItem
        {
            var typeReplaced = typeof(TReplaced);
            var type = typeof(T);
            ReplaceInList(typeReplaced, type);
            return this;
        }

        public OrderedCollection<TItem> Replace(Type typeReplaced, Type type)
        {
            ReplaceInList(typeReplaced, type);
            return this;
        }

        public IEnumerator<TItem> GetEnumerator()
        {
            return _types.Select(type => (TItem)Activator.CreateInstance(type)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void AppendToList(Type type)
        {
            RemoveFromList(type);
            _types.Add(type);
        }

        private void InsertToList(Type type, int index = 0)
        {
            RemoveFromList(type);
            _types.Insert(index, type);
        }

        private void InsertBeforeInList(Type typeBefore, Type type)
        {
            if (typeBefore == type)
            {
                throw new InvalidOperationException();
            }

            var index = _types.IndexOf(typeBefore);
            if (index < 0)
            {
                throw new InvalidOperationException();
            }

            RemoveFromList(type);

            index = _types.IndexOf(typeBefore);
            _types.Insert(index, type);
        }

        private void InsertAfterInList(Type typeAfter, Type type)
        {
            if (typeAfter == type)
            {
                throw new InvalidOperationException();
            }

            var index = _types.IndexOf(typeAfter);
            if (index < 0)
            {
                throw new InvalidOperationException();
            }

            RemoveFromList(type);

            index = _types.IndexOf(typeAfter);
            index += 1;

            if (index == _types.Count)
            {
                _types.Add(type);
            }
            else
            {
                _types.Insert(index, type);
            }
        }

        private void RemoveFromList(Type type)
        {
            if (_types.Contains(type))
            {
                _types.Remove(type);
            }
        }

        private void ReplaceInList(Type typeReplaced, Type type)
        {
            if (typeReplaced == type)
            {
                return;
            }

            var index = _types.IndexOf(typeReplaced);
            if (index < 0)
            {
                throw new InvalidOperationException();
            }

            RemoveFromList(type);
            index = _types.IndexOf(typeReplaced);
            _types.Insert(index, type);
            _types.Remove(typeReplaced);
        }
    }
}
