using System.Collections.Generic;
using System.Drawing;
using Rhino.Geometry;

namespace AlgoDesign
{
    /// <summary>
    /// 把格子上的數值畫成上色網格，用來看 Dijkstra 的「擴散前緣」。
    /// 低值藍、高值紅；擋住的格子深灰；還走不到（double.MaxValue）的格子白色。
    /// </summary>
    public static class FieldDisplay
    {
        private static readonly Color _blockedColor = Color.FromArgb(60, 60, 60);
        private static readonly Color _unreachedColor = Color.White;

        /// <summary>每格一個四邊形面，依 values 上色。</summary>
        public static Mesh BuildMesh(Grid grid, double[,] values)
        {
            double maxValue = FindMaxReached(grid, values);
            var mesh = new Mesh();

            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    AddCellFace(mesh, grid, x, y);

                    Color color = PickColor(grid.Blocked[x, y], values[x, y], maxValue);
                    for (int corner = 0; corner < 4; corner++)
                    {
                        mesh.VertexColors.Add(color);
                    }
                }
            }

            mesh.Normals.ComputeNormals();
            return mesh;
        }

        /// <summary>走得到的格子裡最大的值，上色時用來換算比例。</summary>
        private static double FindMaxReached(Grid grid, double[,] values)
        {
            double maxValue = 0;
            for (int x = 0; x < grid.Width; x++)
            {
                for (int y = 0; y < grid.Height; y++)
                {
                    double value = values[x, y];
                    if (value != double.MaxValue && value > maxValue) maxValue = value;
                }
            }
            return maxValue;
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

        /// <summary>擋住→深灰、走不到→白、其他依比例由藍到紅。</summary>
        private static Color PickColor(bool isBlocked, double value, double maxValue)
        {
            if (isBlocked) return _blockedColor;
            if (value == double.MaxValue) return _unreachedColor;

            double ratio = maxValue > 0 ? value / maxValue : 0;
            int red = (int)(255 * ratio);
            int blue = 255 - red;
            return Color.FromArgb(red, 60, blue);
        }
    }
}
