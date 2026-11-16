using System.Drawing;
using Rhino.Geometry;

namespace AlgoDesign
{
    /// <summary>
    /// 把圖上每個節點的數值畫回 Mesh：每個頂點依它所屬節點的值上色（配色見 ValueColor）。
    /// 用來看 Dijkstra 在 Mesh 上的「擴散前緣」。
    /// </summary>
    public static class GraphDisplay
    {
        /// <summary>
        /// 複製一份 mesh 並上頂點色。nodeValues 的索引是拓樸頂點編號（MeshGraph.FromMesh 的節點編號）。
        /// 先複製，避免改到上游的 Mesh。
        /// </summary>
        public static Mesh ColorMesh(Mesh mesh, double[] nodeValues)
        {
            Mesh colored = mesh.DuplicateMesh();
            ValueColor.FindRange(nodeValues, out double minValue, out double maxValue);

            colored.VertexColors.Clear();
            for (int vertex = 0; vertex < colored.Vertices.Count; vertex++)
            {
                int node = colored.TopologyVertices.TopologyVertexIndex(vertex);
                Color color = ValueColor.Pick(false, nodeValues[node], minValue, maxValue);
                colored.VertexColors.Add(color);
            }
            return colored;
        }
    }
}
