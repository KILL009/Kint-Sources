/*
 * This file is part of the OpenNos Emulator Project. See AUTHORS file for Copyright information
 *
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 */

using System.Collections.Generic;

namespace OpenNos.PathFinder
{
    internal class MinHeap
    {
        #region Members

        private readonly List<Node> _array = new List<Node>();

        #endregion

        #region Properties

        public int Count => _array.Count;

        #endregion

        #region Methods

        public Node Pop()
        {
            Node ret = _array[0];
            _array[0] = _array[_array.Count - 1];
            _array.RemoveAt(_array.Count - 1);

            int c = 0;
            while (c < _array.Count)
            {
                int min = c;
                if ((2 * c) + 1 < _array.Count && _array[(2 * c) + 1].CompareTo(_array[min]) == -1)
                {
                    min = (2 * c) + 1;
                }
                if ((2 * c) + 2 < _array.Count && _array[(2 * c) + 2].CompareTo(_array[min]) == -1)
                {
                    min = (2 * c) + 2;
                }

                if (min == c)
                {
                    break;
                }
                else
                {
                    Node tmp = _array[c];
                    _array[c] = _array[min];
                    _array[min] = tmp;
                    c = min;
                }
            }

            return ret;
        }

        public void Push(Node element)
        {
            _array.Add(element);
            int c = _array.Count - 1;
            int parent = (c - 1) >> 1;
            while (c > 0 && _array[c].CompareTo(_array[parent]) < 0)
            {
                Node tmp = _array[c];
                _array[c] = _array[parent];
                _array[parent] = tmp;
                c = parent;
                parent = (c - 1) >> 1;
            }
        }

        #endregion
    }
}