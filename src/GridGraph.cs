namespace AlgoDesign
{
    /// <summary>
    /// 把格子轉成圖（4 鄰居）：每格一個節點，走進一格的成本就是那格的 Cost。
    /// Part 4 拿基礎版 Dijkstra（圖版）和格子版 PathSearch 對照時用。
    /// </summary>
    public static class GridGraph
    {
        /// <summary>格子 → 圖。節點編號 ＝ x ＋ y × Width；擋住的格子有節點、沒有邊。</summary>
        public static Graph FromGrid(Grid grid)
        {
            var graph = new Graph();
            for (int y = 0; y < grid.Height; y++)
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    graph.AddNode(grid.CellCenter(new Cell(x, y)));
                }
            }

            // 單向邊：走進 next 要花 next 的成本，和格子版的算法相同
            for (int y = 0; y < grid.Height; y++)
            {
                for (int x = 0; x < grid.Width; x++)
                {
                    var cell = new Cell(x, y);
                    if (!grid.IsWalkable(cell)) continue;
                    foreach (Cell next in grid.Neighbors4(cell))
                    {
                        graph.AddEdge(NodeOf(grid, cell), NodeOf(grid, next), grid.Cost[next.X, next.Y]);
                    }
                }
            }
            return graph;
        }

        /// <summary>格子 → 節點編號。</summary>
        public static int NodeOf(Grid grid, Cell cell)
        {
            return cell.X + cell.Y * grid.Width;
        }
    }
}
