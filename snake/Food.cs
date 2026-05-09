using System;
using System.Collections.Generic;
using System.Drawing;

namespace SnakeGame
{
    /// <summary>
    /// Тип їжі: звичайна або бонусна.
    /// </summary>
    public enum FoodType
    {
        Regular,
        Bonus
    }

    /// <summary>
    /// Клас їжі на ігровому полі.
    /// </summary>
    public class Food
    {
        public Point Position { get; }
        public FoodType Type { get; }

        /// <summary>
        /// Кількість очок, яку дає поглинання їжі.
        /// </summary>
        public int ScoreValue => Type == FoodType.Bonus ? 5 : 1;

        /// <summary>
        /// Колір їжі для відображення.
        /// </summary>
        public Color DisplayColor => Type == FoodType.Bonus ? Color.Gold : Color.Red;

        public Food(Point position, FoodType type)
        {
            Position = position;
            Type = type;
        }

        /// <summary>
        /// Генерує нову їжу у випадковій порожній клітинці поля.
        /// </summary>
        public static Food GenerateRandom(Field field, IEnumerable<Point> occupiedCells, Random random)
        {
            HashSet<Point> occupied = new HashSet<Point>(occupiedCells);
            List<Point> freeCells = new List<Point>();

            for (int x = 0; x < field.Width; x++)
            {
                for (int y = 0; y < field.Height; y++)
                {
                    Point p = new Point(x, y);
                    if (!occupied.Contains(p)) freeCells.Add(p);
                }
            }

            if (freeCells.Count == 0)
            {
                // Поле повністю заповнене - повертаємо першу клітинку як заглушку
                return new Food(new Point(0, 0), FoodType.Regular);
            }

            Point pos = freeCells[random.Next(freeCells.Count)];

            // 20% шанс на бонусну їжу
            FoodType type = random.Next(100) < 20 ? FoodType.Bonus : FoodType.Regular;

            return new Food(pos, type);
        }
    }
}
