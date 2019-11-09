﻿using System;
using System.Collections.Generic;

namespace Porter2StemmerStandard
{
    public class IsExactlyLookupContainer
    {
        public IsExactlyLookupContainer(params (string word, string value)[] words)
        {
            _root = new LetterNode();

            foreach (var (word, value) in words)
            {
                Insert(word, value);
            }
        }

        public bool TryGetValue(string word, out string value)
        {
            var node = _root;

            value = default;

            for (var i = 0; i < word.Length; i++)
            {
                node = node.Get(word[i]);

                if (node == null) return false;
            }

            value = node.Value;
            return value != null;
        }

        private void Insert(string word, string value)
        {
            var node = _root;

            foreach (var c in word)
            {
                var child = node.Get(c);

                if (child == null)
                {
                    child = new LetterNode();
                    node.Children[c] = child;
                }
                node = child;
            }

            if (node.Fix != null) throw new ArgumentException($"Word '{word}' already in the collection");

            node.Fix = word;
            node.Value = value;
        }

        private readonly LetterNode _root;
    }
}