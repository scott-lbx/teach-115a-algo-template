namespace AlgoDesign
{
    /// <summary>
    /// 圖上的一條邊：從某個節點走到 To，要花 Weight。
    /// struct（值型別）：複製時整份複製，和 Point3d 一樣。
    /// </summary>
    public struct Edge
    {
        /// <summary>這條邊通到哪個節點（節點編號）。</summary>
        public int To;

        /// <summary>走這條邊要花多少（距離、坡度、擁擠度……）。</summary>
        public double Weight;

        public Edge(int to, double weight)
        {
            To = to;
            Weight = weight;
        }
    }
}
