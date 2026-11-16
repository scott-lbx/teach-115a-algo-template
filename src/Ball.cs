using System;
using System.Collections.Generic;
using Rhino.Geometry;

namespace AlgoDesign
{
    /// <summary>
    /// 一顆球：把找到的路徑「走出來」。
    /// 先用一顆球看懂路徑規劃，課程最後再換成一群 Boids。
    /// </summary>
    public class Ball
    {
        /// <summary>球現在的位置。</summary>
        public Point3d Position { get; private set; }

        /// <summary>沿路徑已經走了多長。</summary>
        public double Traveled { get; private set; }

        /// <summary>走過的位置，依序記下來畫軌跡。</summary>
        public List<Point3d> Trail { get; } = new List<Point3d>();

        /// <summary>到終點了沒。</summary>
        public bool IsArrived { get; private set; }

        public Ball(Point3d start)
        {
            Position = start;
            Trail.Add(start);
        }

        /// <summary>
        /// 沿路徑往前走 speed 的長度；走到終點就停在終點。
        /// 用「已走長度」找路徑上的點，所以不管路徑怎麼轉彎，每步走的距離都一樣。
        /// </summary>
        public void MoveAlong(Curve path, double speed)
        {
            if (path == null || IsArrived || speed <= 0) return;

            double totalLength = path.GetLength();
            Traveled = Math.Min(Traveled + speed, totalLength);

            double t;
            if (!path.LengthParameter(Traveled, out t)) return;
            Position = path.PointAt(t);
            Trail.Add(Position);

            if (Traveled >= totalLength - 1e-9) IsArrived = true;
        }

        /// <summary>
        /// V5：照給定的方向走 speed 的長度（方向來自距離場所在格子的箭頭）。
        /// 方向是零向量（到了終點格，或這格走不到終點）就停下。
        /// </summary>
        public void MoveInDirection(Vector3d direction, double speed)
        {
            if (IsArrived || speed <= 0) return;
            if (direction.IsZero)
            {
                IsArrived = true;
                return;
            }

            Position = Position + direction * (speed / direction.Length);
            Traveled += speed;
            Trail.Add(Position);
        }
    }
}
