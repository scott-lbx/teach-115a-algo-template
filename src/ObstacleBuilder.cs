using System.Collections.Generic;
using Rhino.Geometry;

namespace AlgoDesign
{
    /// <summary>
    /// 把平面上的封閉曲線變成 Boids 用的實心障礙（Brep）。
    /// 同一組曲線可以同時給找路（擋格子）與 Boids（閃避）用，兩邊看到的障礙一致。
    /// </summary>
    public static class ObstacleBuilder
    {
        /// <summary>
        /// 每條封閉曲線往上下各擠出 height 的一半，再封頂底，成為實心 Brep。
        /// 上下對稱是為了讓在曲線平面上飛的鳥，撞到的是側牆而不是底面。
        /// </summary>
        public static List<Brep> FromCurves(List<Curve> curves, double height = 10.0)
        {
            var breps = new List<Brep>();
            if (curves == null) return breps;

            foreach (Curve curve in curves)
            {
                if (curve == null || !curve.IsClosed) continue;

                // 先複製再移動，避免改到上游的曲線
                Curve moved = curve.DuplicateCurve();
                moved.Translate(0, 0, -height / 2);

                Surface side = Surface.CreateExtrusion(moved, new Vector3d(0, 0, height));
                if (side == null) continue;

                Brep solid = side.ToBrep().CapPlanarHoles(1e-6);
                if (solid != null && solid.IsValid) breps.Add(solid);
            }
            return breps;
        }
    }
}
