using System.Collections.Generic;
using Rhino.Geometry;

namespace AlgoDesign
{
    /// <summary>
    /// GH 元件 path_boids 的本體（課程最後）：一群 Boids 沿路徑飛（規則 5 跟隨路徑）。
    /// 鳥跨次運算保留；reset 時在路徑起點附近重建，run 為 true 時走一步。
    /// </summary>
    public class PathBoidsHost
    {
        private readonly Flock _flock = new Flock();

        /// <summary>每隻鳥的位置。</summary>
        public List<Point3d> Points { get; } = new List<Point3d>();

        /// <summary>每隻鳥的飛行方向（線段）。</summary>
        public List<Line> Headings { get; } = new List<Line>();

        /// <summary>給 GH 的 out 看的狀態文字。</summary>
        public string Report { get; private set; } = "";

        /// <summary>每次 GH 運算呼叫一次。</summary>
        public void Run(Curve path, List<Curve> obstacles, Point3d min, Point3d max,
            int boidCount, int seed, double spawnSize, double maxSpeed, double maxForce,
            double viewRadius, double alignmentWeight, double cohesionWeight,
            double separationRadius, double separationWeight, double avoidRadius, double avoidWeight,
            double followWeight, double lookAhead, double pathRadius, bool reset, bool run)
        {
            // 障礙曲線轉成實心 Brep 給 Boids 閃避
            List<Brep> obstacleBreps = ObstacleBuilder.FromCurves(obstacles);

            // 重來：在路徑起點附近 spawnSize 見方的範圍內重建所有鳥
            if (reset || _flock.Boids.Count == 0)
            {
                Point3d center = path != null ? path.PointAtStart : (min + max) / 2;
                var spawnMin = new Point3d(center.X - spawnSize / 2, center.Y - spawnSize / 2, min.Z);
                var spawnMax = new Point3d(center.X + spawnSize / 2, center.Y + spawnSize / 2, max.Z);
                _flock.Reset(boidCount, spawnMin, spawnMax, maxSpeed, maxForce, seed, obstacleBreps);
            }
            // 前進一步
            else if (run)
            {
                _flock.Step(min, max, maxSpeed, maxForce, viewRadius, alignmentWeight,
                    cohesionWeight, separationRadius, separationWeight,
                    obstacleBreps, avoidRadius, avoidWeight,
                    path, followWeight, lookAhead, pathRadius);
            }

            Points.Clear();
            Headings.Clear();
            foreach (Boid boid in _flock.Boids)
            {
                Points.Add(boid.Position);
                Headings.Add(new Line(boid.Position, boid.Position + boid.Velocity * 3));
            }
            Report = $"鳥 {_flock.Boids.Count} 隻，障礙 {obstacleBreps.Count} 個，第 {_flock.StepCount} 步，"
                + (path != null ? "有路徑" : "沒接路徑");
        }
    }
}
