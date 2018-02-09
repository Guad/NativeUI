using System;

using CitizenFX.Core;
using System.Drawing;

namespace NativeUI
{
    public static class MiscExtensions
    {
        public static Random SharedRandom = new Random();

        public static Point AddPoints(this Point left, Point right)
        {
            return new Point(left.X + right.X, left.Y + right.Y);
        }

        public static Point SubtractPoints(this Point left, Point right)
        {
            return new Point(left.X - right.X, left.Y - right.Y);
        }

        public static PointF AddPoints(this PointF left, PointF right)
        {
            return new PointF(left.X + right.X, left.Y + right.Y);
        }

        public static PointF SubtractPoints(this PointF left, PointF right)
        {
            return new PointF(left.X - right.X, left.Y - right.Y);
        }

        public static float Clamp(this float val, float min, float max)
        {
            if (val > max)
                return max;
            if (val < min)
                return min;
            return val;
        }

        public static Vector3 LinearVectorLerp(Vector3 start, Vector3 end, int currentTime, int duration)
        {
            return new Vector3()
            {
                X = LinearFloatLerp(start.X, end.X, currentTime, duration),
                Y = LinearFloatLerp(start.Y, end.Y, currentTime, duration),
                Z = LinearFloatLerp(start.Z, end.Z, currentTime, duration),
            };
        }

        public static Vector3 VectorLerp(Vector3 start, Vector3 end, int currentTime, int duration, Func<float, float, int, int, float> easingFunc)
        {
            return new Vector3()
            {
                X = easingFunc(start.X, end.X, currentTime, duration),
                Y = easingFunc(start.Y, end.Y, currentTime, duration),
                Z = easingFunc(start.Z, end.Z, currentTime, duration),
            };
        }

        public static float LinearFloatLerp(float start, float end, int currentTime, int duration)
        {
            float change = end - start;
            return change * currentTime / duration + start;
        }

        public static float QuadraticEasingLerp(float start, float end, int currentTime, int duration)
        {
            var time = (float)currentTime;
            var dur = (float)duration;

            float change = end - start;

            time /= dur / 2;
            if (time < 1) return change / 2 * time * time + start;
            time--;
            return -change / 2 * (time * (time - 2) - 1) + start;
        }
    }
}