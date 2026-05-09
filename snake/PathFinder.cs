using System;
using System.Collections.Generic;

namespace SnakeGame
{
    /// <summary>
    /// Тип алгоритму пошуку шляху для ШІ.
    /// </summary>
    public enum PathAlgorithm
    {
        AStar,
        BFS,
        Greedy
    }

    /// <summary>
    /// Клас з реалізаціями алгоритмів пошуку найкоротшого шляху
    /// для ШІ-змійки на полі з перешкодами.
    /// </summary>
    public class PathFinder
    {
        /// <summary>
        /// Лічильник елементарних операцій (для оцінки практичної складності
        /// в розділі "Аналіз результатів").
        /// </summary>
        public int OperationsCount { get; private set; }

        /// <summary>
        /// Знаходить найкоротший шлях від start до target обраним методом.
        /// Повертає список точок (без стартової) або null якщо шлях не існує.
        /// </summary>
        public List<Point>? FindPath(Point start, Point target, Field field,
                                     HashSet<Point> obstacles, PathAlgorithm algorithm)
        {
            OperationsCount = 0;

            return algorithm switch
            {
                PathAlgorithm.AStar => AStarSearch(start, target, field, obstacles),
                PathAlgorithm.BFS => BreadthFirstSearch(start, target, field, obstacles),
                PathAlgorithm.Greedy => GreedySearch(start, target, field, obstacles),
                _ => null
            };
        }

        /// <summary>
        /// Манхеттенська відстань з урахуванням телепортації через стіни.
        /// </summary>
        private static int ManhattanDistance(Point a, Point b, Field field)
        {
            int dx = Math.Abs(a.X - b.X);
            int dy = Math.Abs(a.Y - b.Y);
            // На торі мінімальна відстань - менша з двох
            dx = Math.Min(dx, field.Width - dx);
            dy = Math.Min(dy, field.Height - dy);
            return dx + dy;
        }

        /// <summary>
        /// Повертає сусідні клітинки з урахуванням телепортації.
        /// </summary>
        private static IEnumerable<Point> GetNeighbors(Point p, Field field)
        {
            yield return field.Wrap(p.Move(Direction.Up));
            yield return field.Wrap(p.Move(Direction.Down));
            yield return field.Wrap(p.Move(Direction.Left));
            yield return field.Wrap(p.Move(Direction.Right));
        }

        /// <summary>
        /// Алгоритм A* (А-зірочка) - оптимальний за швидкістю та якістю.
        /// f(n) = g(n) + h(n), де g - вартість досягнення, h - евристика.
        /// </summary>
        private List<Point>? AStarSearch(Point start, Point target, Field field, HashSet<Point> obstacles)
        {
            // Пріоритетна черга через SortedSet
            var openSet = new PriorityQueue<Point, int>();
            var cameFrom = new Dictionary<Point, Point>();
            var gScore = new Dictionary<Point, int>();
            var fScore = new Dictionary<Point, int>();

            gScore[start] = 0;
            fScore[start] = ManhattanDistance(start, target, field);
            openSet.Enqueue(start, fScore[start]);

            var inOpenSet = new HashSet<Point> { start };

            while (openSet.Count > 0)
            {
                OperationsCount++;
                Point current = openSet.Dequeue();
                inOpenSet.Remove(current);

                if (current.Equals(target))
                {
                    return ReconstructPath(cameFrom, current);
                }

                foreach (var neighbor in GetNeighbors(current, field))
                {
                    OperationsCount++;
                    // Не входити в перешкоду, але дозволити цільову клітинку
                    if (obstacles.Contains(neighbor) && !neighbor.Equals(target)) continue;

                    int tentativeG = gScore[current] + 1;

                    if (!gScore.ContainsKey(neighbor) || tentativeG < gScore[neighbor])
                    {
                        cameFrom[neighbor] = current;
                        gScore[neighbor] = tentativeG;
                        fScore[neighbor] = tentativeG + ManhattanDistance(neighbor, target, field);

                        if (!inOpenSet.Contains(neighbor))
                        {
                            openSet.Enqueue(neighbor, fScore[neighbor]);
                            inOpenSet.Add(neighbor);
                        }
                    }
                }
            }

            return null; // шлях не знайдено
        }

        /// <summary>
        /// Алгоритм BFS (пошук в ширину) - гарантує найкоротший шлях
        /// у незваженому графі, але повільніший за A*.
        /// </summary>
        private List<Point>? BreadthFirstSearch(Point start, Point target, Field field, HashSet<Point> obstacles)
        {
            var queue = new Queue<Point>();
            var cameFrom = new Dictionary<Point, Point>();
            var visited = new HashSet<Point> { start };

            queue.Enqueue(start);

            while (queue.Count > 0)
            {
                OperationsCount++;
                Point current = queue.Dequeue();

                if (current.Equals(target))
                {
                    return ReconstructPath(cameFrom, current);
                }

                foreach (var neighbor in GetNeighbors(current, field))
                {
                    OperationsCount++;
                    if (visited.Contains(neighbor)) continue;
                    if (obstacles.Contains(neighbor) && !neighbor.Equals(target)) continue;

                    visited.Add(neighbor);
                    cameFrom[neighbor] = current;
                    queue.Enqueue(neighbor);
                }
            }

            return null;
        }

        /// <summary>
        /// Жадібний алгоритм - на кожному кроці обирає сусіда найближчого до цілі.
        /// Швидкий, але не гарантує знаходження шляху чи його оптимальності.
        /// </summary>
        private List<Point>? GreedySearch(Point start, Point target, Field field, HashSet<Point> obstacles)
        {
            var path = new List<Point>();
            var visited = new HashSet<Point> { start };
            Point current = start;

            int maxSteps = field.Width * field.Height;
            int steps = 0;

            while (!current.Equals(target) && steps < maxSteps)
            {
                OperationsCount++;
                steps++;

                Point? bestNeighbor = null;
                int bestDistance = int.MaxValue;

                foreach (var neighbor in GetNeighbors(current, field))
                {
                    OperationsCount++;
                    if (visited.Contains(neighbor)) continue;
                    if (obstacles.Contains(neighbor) && !neighbor.Equals(target)) continue;

                    int dist = ManhattanDistance(neighbor, target, field);
                    if (dist < bestDistance)
                    {
                        bestDistance = dist;
                        bestNeighbor = neighbor;
                    }
                }

                if (bestNeighbor == null) return null; // тупик

                visited.Add(bestNeighbor);
                path.Add(bestNeighbor);
                current = bestNeighbor;
            }

            return current.Equals(target) ? path : null;
        }

        /// <summary>
        /// Відновлює шлях від cameFrom-словника.
        /// </summary>
        private static List<Point> ReconstructPath(Dictionary<Point, Point> cameFrom, Point target)
        {
            var path = new List<Point>();
            Point current = target;

            while (cameFrom.ContainsKey(current))
            {
                path.Add(current);
                current = cameFrom[current];
            }

            path.Reverse();
            return path;
        }

        /// <summary>
        /// На основі першої точки шляху визначає напрямок руху.
        /// </summary>
        public static Direction? GetDirectionFromPath(Point head, Point next, Field field)
        {
            // Враховуємо телепортацію - перевіряємо обидва варіанти
            int dx = next.X - head.X;
            int dy = next.Y - head.Y;

            // Нормалізація для тор-поля
            if (dx > field.Width / 2) dx -= field.Width;
            if (dx < -field.Width / 2) dx += field.Width;
            if (dy > field.Height / 2) dy -= field.Height;
            if (dy < -field.Height / 2) dy += field.Height;

            if (dx == 1) return Direction.Right;
            if (dx == -1) return Direction.Left;
            if (dy == 1) return Direction.Down;
            if (dy == -1) return Direction.Up;

            return null;
        }
    }
}
