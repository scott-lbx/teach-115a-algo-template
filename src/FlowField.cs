using System.Collections.Generic;
using Rhino.Geometry;

namespace AlgoDesign
{
    /// <summary>
    /// V5：距離場（Flow Field）。從終點反向擴散一次，整張圖每一格都知道「往哪走最快到終點」。
    /// 一條路只服務一個起點；距離場一次算好，上千隻 Boids 從哪裡出發都能共用（人群疏散）。
    /// </summary>
    public class FlowField
    {
        private readonly Grid _grid;

        /// <summary>從每一格走到終點的最小成本；走不到的是 double.MaxValue。</summary>
        public double[,] Distance { get; }

        /// <summary>每一格往終點的方向（單位向量）；終點、擋住、走不到的格子是零向量。</summary>
        public Vector3d[,] Direction { get; }

        public Cell Goal { get; }

        /// <summary>建好就算完：從 goal 反向擴散整張圖，再由每格的 Previous 得到方向。</summary>
        public FlowField(Grid grid, Cell goal, bool allowDiagonal = true)
        {
            _grid = grid;
            Goal = goal;
            Direction = new Vector3d[grid.Width, grid.Height];

            var search = new PathSearch(grid) { AllowDiagonal = allowDiagonal };
            search.SolveAll(goal, true);
            Distance = search.Distance;

            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    Cell next = search.Previous[x, y];
                    if (next.IsNone) continue;

                    // 相鄰兩格的格心距離是 1 或 √2 格寬，除以長度變單位向量
                    Vector3d direction = grid.CellCenter(next) - grid.CellCenter(new Cell(x, y));
                    Direction[x, y] = direction / direction.Length;
                }
            }
        }

        /// <summary>某個位置所在格子的方向；超出格子範圍就用最近的邊格。</summary>
        public Vector3d DirectionAt(Point3d point)
        {
            Cell cell = _grid.CellAt(point);
            return Direction[cell.X, cell.Y];
        }

        /// <summary>每格一支箭頭（線段），長度 length，用來在 Rhino 裡看場的流向。</summary>
        public List<Line> Arrows(double length)
        {
            var arrows = new List<Line>();
            for (int x = 0; x < _grid.Width; x++)
            {
                for (int y = 0; y < _grid.Height; y++)
                {
                    Vector3d direction = Direction[x, y];
                    if (direction.IsZero) continue;

                    Point3d center = _grid.CellCenter(new Cell(x, y));
                    arrows.Add(new Line(center - direction * length / 2, center + direction * length / 2));
                }
            }
            return arrows;
        }
    }
}
