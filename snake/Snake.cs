using System.Collections.Generic;
using System.Drawing;

namespace SnakeGame
{
    /// <summary>
    /// Клас, що представляє змійку (керовану гравцем або ботом).
    /// Зберігає тіло, напрямок руху, життєвий стан, рахунок та колір.
    /// </summary>
    public class Snake
    {
        private List<Point> body;
        private bool growPending;

        public Direction CurrentDirection { get; private set; }
        public bool IsAlive { get; private set; } = true;
        public int Score { get; private set; } = 0;
        public Color HeadColor { get; }
        public Color BodyColor { get; }
        public string Name { get; }

        /// <summary>
        /// Голова змійки (перший елемент тіла).
        /// </summary>
        public Point Head => body[0];

        /// <summary>
        /// Все тіло змійки (тільки для читання).
        /// </summary>
        public IReadOnlyList<Point> Body => body.AsReadOnly();

        public int Length => body.Count;

        public Snake(Point startPosition, Direction startDirection, Color headColor, Color bodyColor, string name)
        {
            body = new List<Point> { startPosition };
            CurrentDirection = startDirection;
            HeadColor = headColor;
            BodyColor = bodyColor;
            Name = name;
        }

        /// <summary>
        /// Змінює напрямок руху, але забороняє розворот на 180°.
        /// </summary>
        public void ChangeDirection(Direction newDir)
        {
            if (IsOpposite(CurrentDirection, newDir)) return;
            CurrentDirection = newDir;
        }

        /// <summary>
        /// Перевіряє чи два напрямки протилежні.
        /// </summary>
        private static bool IsOpposite(Direction a, Direction b)
        {
            return (a == Direction.Up && b == Direction.Down) ||
                   (a == Direction.Down && b == Direction.Up) ||
                   (a == Direction.Left && b == Direction.Right) ||
                   (a == Direction.Right && b == Direction.Left);
        }

        /// <summary>
        /// Виконує крок руху змійки. Поле потрібне для телепортації через стіни.
        /// </summary>
        public void Move(Field field)
        {
            if (!IsAlive) return;

            Point newHead = field.Wrap(Head.Move(CurrentDirection));
            body.Insert(0, newHead);

            if (growPending)
            {
                growPending = false;
            }
            else
            {
                body.RemoveAt(body.Count - 1);
            }
        }

        /// <summary>
        /// Позначає що змійка має вирости на наступному кроці.
        /// Викликається після поглинання їжі.
        /// </summary>
        public void Grow(int scoreIncrement)
        {
            growPending = true;
            Score += scoreIncrement;
        }

        /// <summary>
        /// Переводить змійку в стан "мертва".
        /// </summary>
        public void Die()
        {
            IsAlive = false;
        }

        /// <summary>
        /// Перевіряє чи містить тіло змійки задану точку.
        /// </summary>
        public bool Contains(Point p)
        {
            foreach (var part in body)
            {
                if (part.Equals(p)) return true;
            }
            return false;
        }

        /// <summary>
        /// Перевіряє самозіткнення (голова збігається з частиною тіла).
        /// </summary>
        public bool HasSelfCollision()
        {
            for (int i = 1; i < body.Count; i++)
            {
                if (body[i].Equals(Head)) return true;
            }
            return false;
        }
    }
}
