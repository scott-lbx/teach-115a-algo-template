using System.Collections.Generic;
using Rhino.Geometry;

namespace AlgoDesign
{
    /// <summary>
    /// GH 元件 field_boids 的本體（課程最後，方法 B）：Boids 照距離場導航（規則 6）。
    /// reset 時建格子與距離場，鳥散在整個格子範圍；之後每次 run 走一步。
    /// </summary>
    public class FieldBoidsHost
    {
        private readonly Flock _flock = new Flock();
        private FlowField _field;
        private List<Brep> _obstacleBreps = new List<Brep>();
        private Point3d _boxMin;
        private Point3d _boxMax;

        /// <summary>每隻鳥的位置。</summary>
        public List<Point3d> Points { get; } = new List<Point3d>();

        /// <summary>每隻鳥的飛行方向（線段）。</summary>
        public List<Line> Headings { get; } = new List<Line>();

        /// <summary>每格往終點的箭頭。</summary>
        public List<Line> Arrows { get; private set; } = new List<Line>();

        /// <summary>距離場上色網格。</summary>
        public Mesh Field { get; private set; }

        /// <summary>給 GH 的 out 看的狀態文字。</summary>
        public string Report { get; private set; } = "";

        /// <summary>每次 GH 運算呼叫一次。改了格子、障礙、終點、成本，要按 reset 才會重算距離場。</summary>
        public void Run(Point3d origin, int width, int height, double cellSize, List<Curve> obstacles,
            Point3d goal, bool allowDiagonal,
            List<Point3d> attractors, double attractorRadius, double attractorStrength,
            int boidCount, int seed, double maxSpeed, double maxForce, double viewRadius,
            double alignmentWeight, double cohesionWeight, double separationRadius, double separationWeight,
            double avoidRadius, double avoidWeight, double fieldWeight, bool reset, bool run)
        {
            // 重來：建距離場，鳥散在整個格子範圍
            if (reset || _field == null)
            {
                var grid = new Grid(origin, width, height, cellSize);
                grid.BlockInside(obstacles);
                CostField.AddAttractors(grid, attractors, attractorRadius, attractorStrength);

                _field = new FlowField(grid, grid.CellAt(goal), allowDiagonal);
                _obstacleBreps = ObstacleBuilder.FromCurves(obstacles);
                Field = FieldDisplay.BuildMesh(grid, _field.Distance);
                Arrows = _field.Arrows(grid.CellSize * 0.8);

                _boxMin = grid.Origin;
                _boxMax = new Point3d(
                    grid.Origin.X + grid.Width * grid.CellSize,
                    grid.Origin.Y + grid.Height * grid.CellSize,
                    grid.Origin.Z);
                _flock.Reset(boidCount, _boxMin, _boxMax, maxSpeed, maxForce, seed, _obstacleBreps);
            }
            // 前進一步
            else if (run)
            {
                _flock.Step(_boxMin, _boxMax, maxSpeed, maxForce, viewRadius, alignmentWeight,
                    cohesionWeight, separationRadius, separationWeight,
                    _obstacleBreps, avoidRadius, avoidWeight,
                    field: _field, fieldWeight: fieldWeight);
            }

            Points.Clear();
            Headings.Clear();
            foreach (Boid boid in _flock.Boids)
            {
                Points.Add(boid.Position);
                Headings.Add(new Line(boid.Position, boid.Position + boid.Velocity * 3));
            }
            Report = $"鳥 {_flock.Boids.Count} 隻，障礙 {_obstacleBreps.Count} 個，"
                + $"第 {_flock.StepCount} 步，終點格 {_field.Goal}";
        }
    }
}
