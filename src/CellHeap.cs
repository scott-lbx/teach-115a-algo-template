using System.Collections.Generic;

namespace AlgoDesign
{
    /// <summary>
    /// V2：優先佇列（最小堆積）。放進去的格子各帶一個優先值，每次拿出優先值最小的。
    /// 基礎版每次掃全部格子找最小（n 格就要看 n 次）；堆積只要看 log n 次。
    /// 自己寫而不用 .NET 的 PriorityQueue：Rhino 8 若以 .NET Framework 模式執行，沒有那個類別。
    /// </summary>
    public class CellHeap
    {
        // 陣列存一棵完全二元樹：索引 i 的兩個孩子是 2i+1、2i+2，父親是 (i-1)/2
        private readonly List<Cell> _cells = new List<Cell>();
        private readonly List<double> _priorities = new List<double>();

        public int Count => _cells.Count;

        /// <summary>放進一格。同一格可以重複放（新的優先值比較小時），拿出時再跳過已定案的。</summary>
        public void Push(Cell cell, double priority)
        {
            _cells.Add(cell);
            _priorities.Add(priority);
            SiftUp(_cells.Count - 1);
        }

        /// <summary>拿出優先值最小的那格；空的回傳 false。</summary>
        public bool TryPop(out Cell cell, out double priority)
        {
            if (_cells.Count == 0)
            {
                cell = Cell.None;
                priority = double.MaxValue;
                return false;
            }

            cell = _cells[0];
            priority = _priorities[0];

            // 最後一個搬到樹根，再往下沉回正確位置
            int last = _cells.Count - 1;
            _cells[0] = _cells[last];
            _priorities[0] = _priorities[last];
            _cells.RemoveAt(last);
            _priorities.RemoveAt(last);
            if (_cells.Count > 0) SiftDown(0);
            return true;
        }

        /// <summary>新放進的往上浮：比父親小就交換。</summary>
        private void SiftUp(int index)
        {
            while (index > 0)
            {
                int parent = (index - 1) / 2;
                if (_priorities[parent] <= _priorities[index]) break;
                Swap(parent, index);
                index = parent;
            }
        }

        /// <summary>樹根往下沉：和比較小的孩子交換，直到兩個孩子都不比它小。</summary>
        private void SiftDown(int index)
        {
            int count = _cells.Count;
            while (true)
            {
                int left = 2 * index + 1;
                int right = left + 1;
                int smallest = index;
                if (left < count && _priorities[left] < _priorities[smallest]) smallest = left;
                if (right < count && _priorities[right] < _priorities[smallest]) smallest = right;
                if (smallest == index) break;
                Swap(smallest, index);
                index = smallest;
            }
        }

        private void Swap(int a, int b)
        {
            Cell cell = _cells[a];
            _cells[a] = _cells[b];
            _cells[b] = cell;

            double priority = _priorities[a];
            _priorities[a] = _priorities[b];
            _priorities[b] = priority;
        }
    }
}
