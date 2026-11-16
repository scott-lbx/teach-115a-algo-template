using System;
using System.Collections.Generic;
using Rhino.Geometry;

namespace AlgoDesign
{
    /// <summary>
    /// 平面格子：每格有成本（走進這一格要花多少）與是否被擋住。
    /// 和 W4 反應擴散一樣用二維陣列存，第一個索引是 X、第二個是 Y。
    /// 格子就是一張「圖」：格子是節點、上下左右相鄰是邊、Cost 是邊的權重。
    /// </summary>
    public class Grid
    {
        // 上、下、左、右四個方向的位移
        private static readonly int[] _stepX = { 1, -1, 0, 0 };
        private static readonly int[] _stepY = { 0, 0, 1, -1 };

        public int Width { get; }
        public int Height { get; }
        public double CellSize { get; }

        /// <summary>格子左下角的位置。</summary>
        public Point3d Origin { get; }

        /// <summary>走進每一格的成本，預設全部是 1。</summary>
        public double[,] Cost { get; }

        /// <summary>被擋住、不能走的格子。</summary>
        public bool[,] Blocked { get; }

        public Grid(Point3d origin, int width, int height, double cellSize)
        {
            Origin = origin;
            Width = Math.Max(1, width);
            Height = Math.Max(1, height);
            CellSize = cellSize > 0 ? cellSize : 1.0;
            Cost = new double[Width, Height];
            Blocked = new bool[Width, Height];

            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    Cost[x, y] = 1.0;
                }
            }
        }

        /// <summary>這一格在格子範圍內嗎。</summary>
        public bool IsInside(Cell cell)
        {
            return cell.X >= 0 && cell.X < Width && cell.Y >= 0 && cell.Y < Height;
        }

        /// <summary>這一格能走嗎：在範圍內，而且沒被擋住。</summary>
        public bool IsWalkable(Cell cell)
        {
            return IsInside(cell) && !Blocked[cell.X, cell.Y];
        }

        /// <summary>上下左右四個鄰居中，能走的那幾格。</summary>
        public List<Cell> Neighbors4(Cell cell)
        {
            var neighbors = new List<Cell>();
            for (int i = 0; i < 4; i++)
            {
                var next = new Cell(cell.X + _stepX[i], cell.Y + _stepY[i]);
                if (IsWalkable(next)) neighbors.Add(next);
            }
            return neighbors;
        }

        /// <summary>一格的中心點（Rhino 座標）。</summary>
        public Point3d CellCenter(Cell cell)
        {
            return new Point3d(
                Origin.X + (cell.X + 0.5) * CellSize,
                Origin.Y + (cell.Y + 0.5) * CellSize,
                Origin.Z);
        }

        /// <summary>Rhino 座標落在哪一格；超出範圍就取最近的邊格。</summary>
        public Cell CellAt(Point3d point)
        {
            int x = (int)Math.Floor((point.X - Origin.X) / CellSize);
            int y = (int)Math.Floor((point.Y - Origin.Y) / CellSize);
            x = Math.Max(0, Math.Min(Width - 1, x));
            y = Math.Max(0, Math.Min(Height - 1, y));
            return new Cell(x, y);
        }

        /// <summary>
        /// 封閉曲線圍住的格子設為擋住（以格子中心判斷，在 XY 平面上比較）。
        /// 回傳新擋住的格數。
        /// </summary>
        public int BlockInside(List<Curve> curves)
        {
            int blockedCount = 0;
            if (curves == null) return blockedCount;

            foreach (Curve curve in curves)
            {
                if (curve == null || !curve.IsClosed) continue;

                for (int x = 0; x < Width; x++)
                {
                    for (int y = 0; y < Height; y++)
                    {
                        if (Blocked[x, y]) continue;
                        Point3d center = CellCenter(new Cell(x, y));
                        if (curve.Contains(center, Plane.WorldXY, 1e-6) == PointContainment.Inside)
                        {
                            Blocked[x, y] = true;
                            blockedCount++;
                        }
                    }
                }
            }
            return blockedCount;
        }
    }
}
