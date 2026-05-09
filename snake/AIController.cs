using System.Collections.Generic;

namespace SnakeGame
{
    /// <summary>
    /// Клас, що керує змійкою-ботом. На кожному кроці виконує
    /// пошук шляху до найближчої їжі та обирає напрямок руху.
    /// </summary>
    public class AIController
    {
        private readonly Snake snake;
        private readonly Field field;
        private readonly PathFinder pathFinder;

        public PathAlgorithm Algorithm { get; set; }

        /// <summary>
        /// Загальний лічильник елементарних операцій ШІ
        /// (накопичується між викликами для аналізу ефективності).
        /// </summary>
        public long TotalOperations { get; private set; }

        /// <summary>
        /// Кількість викликів пошуку шляху.
        /// </summary>
        public int PathFindCallsCount { get; private set; }

        public AIController(Snake snake, Field field, PathAlgorithm algorithm = PathAlgorithm.AStar)
        {
            this.snake = snake;
            this.field = field;
            this.pathFinder = new PathFinder();
            this.Algorithm = algorithm;
        }

        /// <summary>
        /// Виконує крок прийняття рішення. Знаходить найближчу їжу,
        /// будує шлях до неї та обирає наступний напрямок.
        /// </summary>
        public void MakeDecision(IEnumerable<Food> foods, IEnumerable<Snake> allSnakes)
        {
            if (!snake.IsAlive) return;

            // Збираємо перешкоди: тіла всіх змійок крім хвоста (хвіст рухається)
            HashSet<Point> obstacles = BuildObstacleSet(allSnakes);

            // Шукаємо найближчу їжу за манхеттенською відстанню
            Food? targetFood = FindClosestFood(foods);
            if (targetFood == null) return;

            // Шукаємо шлях обраним алгоритмом
            List<Point>? path = pathFinder.FindPath(
                snake.Head, targetFood.Position, field, obstacles, Algorithm);

            TotalOperations += pathFinder.OperationsCount;
            PathFindCallsCount++;

            if (path == null || path.Count == 0)
            {
                // Шлях не знайдено - намагаємось хоча б рухатись у безпечну клітинку
                TryMoveSafely(obstacles);
                return;
            }

            // Визначаємо напрямок до першої точки шляху
            Direction? dir = PathFinder.GetDirectionFromPath(snake.Head, path[0], field);
            if (dir.HasSelfCollisionRisk(snake) == false && dir.HasValue)
            {
                snake.ChangeDirection(dir.Value);
            }
        }

        /// <summary>
        /// Будує множину перешкод для пошуку шляху.
        /// </summary>
        private HashSet<Point> BuildObstacleSet(IEnumerable<Snake> allSnakes)
        {
            var obstacles = new HashSet<Point>();
            foreach (var s in allSnakes)
            {
                if (!s.IsAlive) continue;
                int idx = 0;
                foreach (var part in s.Body)
                {
                    // Хвіст пропускаємо - він зрушиться, тож наступним кроком клітинка буде вільна
                    bool isOwnTail = (s == snake) && (idx == s.Length - 1);
                    if (!isOwnTail)
                    {
                        obstacles.Add(part);
                    }
                    idx++;
                }
            }
            return obstacles;
        }

        /// <summary>
        /// Знаходить їжу, найближчу до голови змійки.
        /// </summary>
        private Food? FindClosestFood(IEnumerable<Food> foods)
        {
            Food? best = null;
            int bestDist = int.MaxValue;

            foreach (var f in foods)
            {
                int dx = System.Math.Abs(snake.Head.X - f.Position.X);
                int dy = System.Math.Abs(snake.Head.Y - f.Position.Y);
                dx = System.Math.Min(dx, field.Width - dx);
                dy = System.Math.Min(dy, field.Height - dy);
                int dist = dx + dy;

                if (dist < bestDist)
                {
                    bestDist = dist;
                    best = f;
                }
            }

            return best;
        }

        /// <summary>
        /// Резервна стратегія: коли шлях не знайдено, рухаємось у будь-яку
        /// безпечну сусідню клітинку, щоб не врізатись.
        /// </summary>
        private void TryMoveSafely(HashSet<Point> obstacles)
        {
            Direction[] dirs = { Direction.Up, Direction.Down, Direction.Left, Direction.Right };
            foreach (var d in dirs)
            {
                Point next = field.Wrap(snake.Head.Move(d));
                if (!obstacles.Contains(next))
                {
                    snake.ChangeDirection(d);
                    return;
                }
            }
        }
    }

    /// <summary>
    /// Розширення для Direction (допоміжна перевірка).
    /// </summary>
    internal static class DirectionExtensions
    {
        public static bool HasSelfCollisionRisk(this Direction? dir, Snake snake)
        {
            // Заглушка для семантики - ChangeDirection сам захищає від розвороту
            return false;
        }
    }
}
