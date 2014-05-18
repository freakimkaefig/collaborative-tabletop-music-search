using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PieInTheSky
{
    class Selection
    {
        private List<int> _selection = new List<int>();

        public void SetSelection(int level, int selection)
        {
            // make sure internal list is long enough
            while (_selection.Count < level + 1)
            {
                _selection.Add(-1);
            }

            // set selection or deselect if same item is already selected
            if (_selection[level] == selection) _selection[level] = -1;
            else _selection[level] = selection;

            // deselect items at higher level
            for (int i = level + 1; i < _selection.Count; i++)
            {
                _selection[i] = -1;
            }
        }

        public int GetSelection(int level)
        {
            if (level >= _selection.Count) return -1;

            return _selection[level];
        }

        public int GetLevel()
        {
            for (int i = _selection.Count - 1; i >= 0; i--)
            {
                if (_selection[i] != -1) return i;
            }
            return -1;
        }
    }
}
