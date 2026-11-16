using System;
using System.Collections.Generic;
using System.Drawing;

namespace AlgoDesign
{
    /// <summary>
    /// 數值 → 顏色，距離場、成本場共用同一套配色：
    /// 低值藍、高值紅；擋住的深灰；還走不到（double.MaxValue）的白色。
    /// </summary>
    public static class ValueColor
    {
        public static readonly Color Blocked = Color.FromArgb(60, 60, 60);
        public static readonly Color Unreached = Color.White;

        /// <summary>依 value 在 minValue～maxValue 之間的位置，由藍到紅。</summary>
        public static Color Pick(bool isBlocked, double value, double minValue, double maxValue)
        {
            if (isBlocked) return Blocked;
            if (value == double.MaxValue) return Unreached;

            double range = maxValue - minValue;
            double ratio = range > 0 ? (value - minValue) / range : 0;
            ratio = Math.Max(0.0, Math.Min(1.0, ratio));
            int red = (int)(255 * ratio);
            return Color.FromArgb(red, 60, 255 - red);
        }

        /// <summary>
        /// 走得到的值裡最小與最大的，上色時用來換算比例；全部走不到時兩個都是 0。
        /// 距離場最小是 0；成本場最小是 1，用最小到最大換算，顏色才拉得開。
        /// </summary>
        public static void FindRange(IEnumerable<double> values, out double minValue, out double maxValue)
        {
            minValue = double.MaxValue;
            maxValue = double.MinValue;
            foreach (double value in values)
            {
                if (value == double.MaxValue) continue;
                if (value < minValue) minValue = value;
                if (value > maxValue) maxValue = value;
            }
            if (minValue > maxValue)
            {
                minValue = 0;
                maxValue = 0;
            }
        }
    }
}
