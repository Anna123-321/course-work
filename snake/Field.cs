namespace SnakeGame
{
    /// <summary>
    /// Клас ігрового поля. Зберігає розміри та забезпечує
    /// функцію телепортації координат через краї поля.
    /// </summary>
    public class Field
    {
        public int Width { get; }
        public int Height { get; }

        public Field(int width, int height)
        {
            Width = width;
            Height = height;
        }

        /// <summary>
        /// Нормалізує координати точки відповідно до правила
        /// телепортації через стіни (тор-топологія).
        /// </summary>
        public Point Wrap(Point p)
        {
            int x = ((p.X % Width) + Width) % Width;
            int y = ((p.Y % Height) + Height) % Height;
            return new Point(x, y);
        }

        /// <summary>
        /// Перевіряє чи знаходиться точка в межах поля.
        /// </summary>
        public bool Contains(Point p)
        {
            return p.X >= 0 && p.X < Width && p.Y >= 0 && p.Y < Height;
        }
    }
}
