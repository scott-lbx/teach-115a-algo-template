using Rhino;
using Rhino.Geometry;

namespace AlgoDesign
{
    /// <summary>
    /// 把 GH 的 Mesh 轉成圖：Mesh 的頂點是節點，Mesh 的邊是邊，權重是邊的長度。
    /// 用「拓樸頂點」：位置相同、焊在一起的頂點算同一個節點。
    /// </summary>
    public static class MeshGraph
    {
        /// <summary>Mesh → 圖。節點編號就是 Mesh 的拓樸頂點編號。</summary>
        public static Graph FromMesh(Mesh mesh)
        {
            var graph = new Graph();
            if (mesh == null) return graph;

            // 節點：每個拓樸頂點一個
            for (int vertex = 0; vertex < mesh.TopologyVertices.Count; vertex++)
            {
                graph.AddNode(new Point3d(mesh.TopologyVertices[vertex]));
            }

            // 邊：每條 Mesh 邊一條雙向邊，權重是兩端的距離
            for (int edge = 0; edge < mesh.TopologyEdges.Count; edge++)
            {
                IndexPair ends = mesh.TopologyEdges.GetTopologyVertices(edge);
                double length = graph.Positions[ends.I].DistanceTo(graph.Positions[ends.J]);
                graph.AddUndirectedEdge(ends.I, ends.J, length);
            }
            return graph;
        }
    }
}
