namespace AlgoDesign
{
    /// <summary>
    /// 格子上的一格，用整數座標 (X, Y) 表示。
    /// 這是 struct（值型別）：和 Point3d 一樣，指定給別的變數時會整份複製。
    /// </summary>
    public struct Cell
    {
        public int X;
        public int Y;

        public Cell(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>「沒有這一格」的記號，座標 (-1, -1)。用在起點的前一格、走不到的格子。</summary>
        public static Cell None => new Cell(-1, -1);

        /// <summary>是不是「沒有這一格」的記號。</summary>
        public bool IsNone => X < 0 || Y < 0;

        /// <summary>兩格的座標是否相同。</summary>
        public bool IsSame(Cell other) => X == other.X && Y == other.Y;

        public override string ToString() => $"({X}, {Y})";
    }
}
