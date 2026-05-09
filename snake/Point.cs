using System;

namespace SnakeGame
{
    /// <summary>
    /// Клас, що представляє координату клітинки на ігровому полі.
    /// </summary>
    public class Point : IEquatable<Point>
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// Створює нову точку зі зміщенням у заданому напрямку.
        /// </summary>
        public Point Move(Direction dir)
        {
            return dir switch
            {
                Direction.Up => new Point(X, Y - 1),
                Direction.Down => new Point(X, Y + 1),
                Direction.Left => new Point(X - 1, Y),
                Direction.Right => new Point(X + 1, Y),
                _ => new Point(X, Y)
            };
        }

        public bool Equals(Point? other)
        {
            if (other is null) return false;
            return X == other.X && Y == other.Y;
        }

        public override bool Equals(object? obj) => Equals(obj as Point);

        public override int GetHashCode() => HashCode.Combine(X, Y);

        public override string ToString() => $"({X}, {Y})";
    }
}
