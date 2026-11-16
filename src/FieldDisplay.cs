using System.Collections.Generic;
using System.Drawing;
using Rhino.Geometry;

namespace AlgoDesign
{
    /// <summary>
    /// 把格子上的數值畫成上色網格，用來看擴散前緣與成本場（配色見 ValueColor）。
    /// </summary>
    public static class FieldDisplay
    {
        /// <summary>每格一個四邊形面，依 values 上色。</summary>
        public static Mesh BuildMesh(Grid grid, double[,] values)
        {
            ValueColor.FindRange(ReachedValues(grid, values), out double minValue, out double maxValue);
            var mesh = new Mesh();

            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    AddCellFace(mesh, grid, x, y);

                    Color color = ValueColor.Pick(grid.Blocked[x, y], values[x, y], minValue, maxValue);
                    for (int corner = 0; corner < 4; corner++)
                    {
                        mesh.VertexColors.Add(color);
                    }
                }
            }

            mesh.Normals.ComputeNormals();
            return mesh;
        }

        /// <summary>沒擋住的格子的值（換算顏色比例用）。</summary>
        private static IEnumerable<double> ReachedValues(Grid grid, double[,] values)
        {
            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    if (!grid.Blocked[x, y]) yield return values[x, y];
                }
            }
        }

        /// <summary>加一格的四個角點與一個四邊形面。</summary>
        private static void AddCellFace(Mesh mesh, Grid grid, int x, int y)
        {
            double size = grid.CellSize;
            double left = grid.Origin.X + x * size;
            double bottom = grid.Origin.Y + y * size;
            double z = grid.Origin.Z;

            int first = mesh.Vertices.Count;
            mesh.Vertices.Add(left, bottom, z);
            mesh.Vertices.Add(left + size, bottom, z);
            mesh.Vertices.Add(left + size, bottom + size, z);
            mesh.Vertices.Add(left, bottom + size, z);
            mesh.Faces.AddFace(first, first + 1, first + 2, first + 3);
        }
    }
}
