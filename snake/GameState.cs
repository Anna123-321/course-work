using System.Collections.Generic;
using System.Drawing;
using System;

namespace SnakeGame
{
    /// <summary>
    /// Стан гри.
    /// </summary>
    public enum GameStatus
    {
        Running,
        PlayerWon,
        PlayerLost,
        Draw
    }

    /// <summary>
    /// Центральний клас, що інкапсулює стан гри: поле, змійки, їжу,
    /// логіку оновлення та обробку зіткнень.
    /// </summary>
    public class GameState
    {
        public Field Field { get; }
        public Snake Player { get; }
        public Snake Bot { get; }
        public AIController AI { get; }
        public List<Food> Foods { get; private set; }
        public GameStatus Status { get; private set; }

        private readonly Random random;
        private const int FoodCount = 3;

        public GameState(int width, int height, PathAlgorithm aiAlgorithm = PathAlgorithm.AStar)
        {
            Field = new Field(width, height);
            random = new Random();

            // Гравець стартує зліва, бот - справа
            Player = new Snake(
                new Point(width / 4, height / 2),
                Direction.Right,
                Color.FromArgb(46, 204, 113),     // яскраво-зелений
                Color.FromArgb(39, 174, 96),       // темно-зелений
                "Гравець"
            );

            Bot = new Snake(
                new Point(3 * width / 4, height / 2),
                Direction.Left,
                Color.FromArgb(231, 76, 60),       // яскраво-червоний
                Color.FromArgb(192, 57, 43),       // темно-червоний
                "Бот (ШІ)"
            );

            AI = new AIController(Bot, Field, aiAlgorithm);
            Foods = new List<Food>();
            Status = GameStatus.Running;

            // Початкова генерація їжі
            for (int i = 0; i < FoodCount; i++)
            {
                AddFood();
            }
        }

        /// <summary>
        /// Виконує один такт гри: ШІ приймає рішення, обидві змійки рухаються,
        /// перевіряються зіткнення та поглинання їжі.
        /// </summary>
        public void Update()
        {
            if (Status != GameStatus.Running) return;

            // Спочатку ШІ приймає рішення на основі поточного стану
            AI.MakeDecision(Foods, new[] { Player, Bot });

            // Рухаємо обидві змійки одночасно
            Player.Move(Field);
            Bot.Move(Field);

            // Обробляємо поглинання їжі
            HandleFoodConsumption(Player);
            HandleFoodConsumption(Bot);

            // Перевіряємо зіткнення та визначаємо стан гри
            CheckCollisions();
            UpdateGameStatus();
        }

        /// <summary>
        /// Перевіряє чи з'їла змійка їжу. Якщо так - їжа зникає, з'являється нова.
        /// </summary>
        private void HandleFoodConsumption(Snake snake)
        {
            if (!snake.IsAlive) return;

            for (int i = Foods.Count - 1; i >= 0; i--)
            {
                if (Foods[i].Position.Equals(snake.Head))
                {
                    snake.Grow(Foods[i].ScoreValue);
                    Foods.RemoveAt(i);
                    AddFood();
                }
            }
        }

        /// <summary>
        /// Додає нову їжу у випадкову порожню клітинку.
        /// </summary>
        private void AddFood()
        {
            var occupied = new List<Point>();
            occupied.AddRange(Player.Body);
            occupied.AddRange(Bot.Body);
            foreach (var f in Foods) occupied.Add(f.Position);

            Foods.Add(Food.GenerateRandom(Field, occupied, random));
        }

        /// <summary>
        /// Перевіряє всі типи зіткнень: самозіткнення та зіткнення з іншою змійкою.
        /// </summary>
        private void CheckCollisions()
        {
            // Самозіткнення
            if (Player.HasSelfCollision()) Player.Die();
            if (Bot.HasSelfCollision()) Bot.Die();

            // Зіткнення головою у тіло іншої змійки
            if (Player.IsAlive && Bot.IsAlive)
            {
                // Голова в голову - обоє програли
                if (Player.Head.Equals(Bot.Head))
                {
                    Player.Die();
                    Bot.Die();
                    return;
                }

                if (Bot.Contains(Player.Head)) Player.Die();
                if (Player.Contains(Bot.Head)) Bot.Die();
            }
        }

        /// <summary>
        /// Оновлює загальний статус гри на основі стану змійок.
        /// </summary>
        private void UpdateGameStatus()
        {
            if (Player.IsAlive && Bot.IsAlive)
            {
                Status = GameStatus.Running;
            }
            else if (!Player.IsAlive && !Bot.IsAlive)
            {
                // Обидві мертві - визначаємо за рахунком
                if (Player.Score > Bot.Score) Status = GameStatus.PlayerWon;
                else if (Bot.Score > Player.Score) Status = GameStatus.PlayerLost;
                else Status = GameStatus.Draw;
            }
            else if (!Player.IsAlive)
            {
                Status = GameStatus.PlayerLost;
            }
            else // !Bot.IsAlive
            {
                Status = GameStatus.PlayerWon;
            }
        }
    }
}
