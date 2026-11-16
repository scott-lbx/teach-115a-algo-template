using System.Collections.Generic;
using Rhino.Geometry;

namespace AlgoDesign
{
    /// <summary>
    /// GH 元件 ball_field 的本體（Part 4 V5）：距離場 ＋ 一顆球照箭頭走。
    /// reset 時建格子與距離場、球回到 start；之後每次 run 走一步。
    /// </summary>
    public class BallFieldHost
    {
        private FlowField _field;
        private Ball _ball;
        private bool _isStartReachable;

        /// <summary>球（給 GH 預覽）。</summary>
        public Sphere? BallShape { get; private set; }

        /// <summary>走過的軌跡；不到兩點是 null。</summary>
        public Polyline Trail { get; private set; }

        /// <summary>每格往終點的箭頭。</summary>
        public List<Line> Arrows { get; private set; } = new List<Line>();

        /// <summary>距離場上色網格。</summary>
        public Mesh Field { get; private set; }

        /// <summary>給 GH 的 out 看的狀態文字。</summary>
        public string Report { get; private set; } = "";

        /// <summary>每次 GH 運算呼叫一次。改了格子、障礙、終點、成本或 start，要按 reset 才會重算。</summary>
        public void Run(Point3d origin, int width, int height, double cellSize, List<Curve> obstacles,
            Point3d start, Point3d goal, bool allowDiagonal,
            List<Point3d> attractors, double attractorRadius, double attractorStrength,
            double speed, double radius, bool reset, bool run)
        {
            // 重來：建距離場，球放回 start
            if (reset || _field == null)
            {
                var grid = new Grid(origin, width, height, cellSize);
                grid.BlockInside(obstacles);
                CostField.AddAttractors(grid, attractors, attractorRadius, attractorStrength);

                _field = new FlowField(grid, grid.CellAt(goal), allowDiagonal);
                Field = FieldDisplay.BuildMesh(grid, _field.Distance);
                Arrows = _field.Arrows(grid.CellSize * 0.8);

                Cell startCell = grid.CellAt(start);
                _isStartReachable = _field.Distance[startCell.X, startCell.Y] != double.MaxValue;
                _ball = new Ball(start);
            }
            // 前進一步：查所在格子的箭頭
            else if (run)
            {
                _ball.MoveInDirection(_field.DirectionAt(_ball.Position), speed);
            }

            BallShape = new Sphere(_ball.Position, radius > 0 ? radius : 0.5);
            Trail = _ball.Trail.Count >= 2 ? new Polyline(_ball.Trail) : null;
            string state = !_isStartReachable ? "起點走不到終點"
                : _ball.IsArrived ? "到終點了"
                : "前進中";
            Report = $"終點格 {_field.Goal}，已走 {_ball.Traveled:0.0}，{state}";
        }
    }
}
