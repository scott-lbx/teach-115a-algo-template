using System;
using System.Collections.Generic;
using Rhino.Geometry;

namespace AlgoDesign
{
    // 一群鳥：鳥的清單與步數放在這裡，Reset、Step 也是
    public class Flock
    {
        public List<Boid> Boids = new List<Boid>();
        public int StepCount = 0;

        // 初始：隨機位置（盒內，且不在實心障礙裡面）、隨機方向（長度 maxSpeed）
        public void Reset(int boidCount, Point3d min, Point3d max, double maxSpeed, double maxForce, int seed,
            List<Brep> obstacles)
        {
            var random = new Random(seed);
            Boids.Clear();
            StepCount = 0;

            for (int number = 0; number < boidCount; number++)
            {
                // 抽到障礙裡面就重抽，最多 100 次（障礙塞滿整個盒子時才會用完）
                Point3d position = Point3d.Origin;
                for (int attempt = 0; attempt < 100; attempt++)
                {
                    position = new Point3d(
                        min.X + random.NextDouble() * (max.X - min.X),
                        min.Y + random.NextDouble() * (max.Y - min.Y),
                        min.Z + random.NextDouble() * (max.Z - min.Z));
                    if (!Boid.IsInsideAny(position, obstacles)) break;
                }

                // 盒子沒有高度（min.Z = max.Z）時只在平面上飛；z 的亂數照抽，同 seed 結果才和 11/03 版一致
                double directionX = random.NextDouble() - 0.5;
                double directionY = random.NextDouble() - 0.5;
                double directionZ = random.NextDouble() - 0.5;
                bool isFlat = max.Z - min.Z < 1e-9;
                Vector3d direction = new Vector3d(directionX, directionY, isFlat ? 0.0 : directionZ);
                if (!direction.Unitize()) direction = Vector3d.XAxis;
                Boids.Add(new Boid(position, direction * maxSpeed, maxSpeed, maxForce));
            }
        }

        // 單次迭代：先全部算好加速度，再全部一起套用
        // 最後四個參數是 1117 新增的「跟隨路徑」；不給 path 時和 11/03 版完全相同
        public void Step(Point3d min, Point3d max, double maxSpeed, double maxForce,
            double viewRadius, double alignmentWeight,
            double cohesionWeight, double separationRadius, double separationWeight,
            List<Brep> obstacles, double avoidRadius, double avoidWeight,
            Curve path = null, double followWeight = 0.0, double lookAhead = 5.0, double pathRadius = 2.0)
        {
            // 算
            foreach (Boid boid in Boids)
            {
                // 執行中拉 slider 也要生效
                boid.MaxSpeed = maxSpeed;
                boid.MaxForce = maxForce;

                Vector3d acceleration = Vector3d.Zero;
                acceleration += boid.Align(Boids, viewRadius) * alignmentWeight;
                acceleration += boid.Cohesion(Boids, viewRadius) * cohesionWeight;
                acceleration += boid.Separation(Boids, separationRadius) * separationWeight;
                acceleration += boid.Avoid(obstacles, avoidRadius) * avoidWeight;
                if (path != null)
                {
                    acceleration += boid.FollowPath(path, lookAhead, pathRadius) * followWeight;
                }
                boid.Acceleration = acceleration;
            }

            // 套用
            foreach (Boid boid in Boids)
            {
                boid.Update(min, max, obstacles);
            }

            StepCount++;
        }
    }
}
