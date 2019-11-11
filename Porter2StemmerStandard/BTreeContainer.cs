using System;
using System.Collections.Generic;

namespace Porter2StemmerStandard
{
    public abstract class BTreeContainer
    {
        protected BTreeContainer(IEnumerable<(string key, string value)> values)
        {
            foreach (var (key, value) in values)
            {
                Insert(key, value);
            }
        }

        protected BTreeContainer(IEnumerable<string> keys)
        {
            foreach (var key in keys)
            {
                Insert(key, null);
            }
        }

        protected void Insert(string key, string value)
        {
            var node = _root;

            foreach (var c in GetChars(key))
            {
                var child = node.Children[c];

                if (child == null)
                {
                    child = new BTreeNode();
                    node.Children[c] = child;
                }
                node = child;
            }

            if (node.Value != null) throw new ArgumentException($"Key '{key}' already in the collection");

            node.Value = value;
            node.Key = key;
        }

        protected virtual IEnumerable<char> GetChars(string word) => word;

        protected readonly BTreeNode _root = new BTreeNode();
    }
}
