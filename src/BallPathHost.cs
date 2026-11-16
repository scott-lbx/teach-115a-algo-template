using Rhino.Geometry;

namespace AlgoDesign
{
    /// <summary>
    /// GH 元件 ball_path 的本體：一顆球沿路徑走。
    /// 球跨次運算保留；reset 或還沒有球時放回路徑起點，run 為 true 時走一步。
    /// </summary>
    public class BallPathHost
    {
        private Ball _ball;

        /// <summary>球（給 GH 預覽）。</summary>
        public Sphere? BallShape { get; private set; }

        /// <summary>走過的軌跡；不到兩點是 null。</summary>
        public Polyline Trail { get; private set; }

        /// <summary>給 GH 的 out 看的狀態文字。</summary>
        public string Report { get; private set; } = "";

        /// <summary>每次 GH 運算呼叫一次。</summary>
        public void Run(Curve path, double speed, double radius, bool reset, bool run)
        {
            if (path == null)
            {
                BallShape = null;
                Trail = null;
                Report = "沒接路徑（找不到路時 path 是空的）";
                return;
            }

            // 重來：球放回路徑起點
            if (reset || _ball == null)
            {
                _ball = new Ball(path.PointAtStart);
            }
            // 前進一步
            else if (run)
            {
                _ball.MoveAlong(path, speed);
            }

            BallShape = new Sphere(_ball.Position, radius > 0 ? radius : 0.5);
            Trail = _ball.Trail.Count >= 2 ? new Polyline(_ball.Trail) : null;
            Report = $"已走 {_ball.Traveled:0.0} ／ 全長 {path.GetLength():0.0}，"
                + (_ball.IsArrived ? "到終點了" : "前進中");
        }
    }
}
