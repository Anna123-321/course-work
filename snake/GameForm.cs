using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
// Alias щоб розрізняти System.Drawing.Point (для UI-координат)
// та SnakeGame.Point (для координат на ігровому полі)
using UIPoint = System.Drawing.Point;

namespace SnakeGame
{
    /// <summary>
    /// Головна форма гри. Відповідає за рендеринг ігрового стану,
    /// обробку клавіатурного вводу та таймер ігрових тактів.
    /// </summary>
    public class GameForm : Form
    {
        private const int FieldWidth = 20;
        private const int FieldHeight = 20;
        private const int CellSize = 28;
        private const int InfoPanelWidth = 240;
        private const int GameTickMs = 130;

        private GameState gameState;
        private System.Windows.Forms.Timer gameTimer = null!;
        private Panel gamePanel = null!;
        private Panel infoPanel = null!;
        private Label playerScoreLabel = null!;
        private Label botScoreLabel = null!;
        private Label statusLabel = null!;
        private Label aiOpsLabel = null!;
        private Button restartButton = null!;
        private bool isPaused;

        public GameForm()
        {
            gameState = new GameState(FieldWidth, FieldHeight);
            InitializeComponent();
            StartGame();
        }

        private void InitializeComponent()
        {
            Text = "Змійка - комп'ютерна гра з елементами ШІ";
            Size = new Size(FieldWidth * CellSize + InfoPanelWidth + 40,
                           FieldHeight * CellSize + 60);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            BackColor = Color.FromArgb(30, 30, 40);
            KeyPreview = true;
            DoubleBuffered = true;

            // Панель ігрового поля
            gamePanel = new Panel
            {
                Location = new UIPoint(10, 10),
                Size = new Size(FieldWidth * CellSize, FieldHeight * CellSize),
                BackColor = Color.FromArgb(20, 20, 30)
            };
            // Ввімкнути подвійну буферизацію через рефлексію
            typeof(Control).GetProperty("DoubleBuffered",
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.NonPublic)?
                .SetValue(gamePanel, true, null);
            gamePanel.Paint += GamePanel_Paint;
            Controls.Add(gamePanel);

            // Інформаційна панель
            infoPanel = new Panel
            {
                Location = new UIPoint(FieldWidth * CellSize + 20, 10),
                Size = new Size(InfoPanelWidth, FieldHeight * CellSize),
                BackColor = Color.FromArgb(40, 40, 55)
            };
            Controls.Add(infoPanel);

            BuildInfoPanel();

            // Таймер гри
            gameTimer = new System.Windows.Forms.Timer
            {
                Interval = GameTickMs
            };
            gameTimer.Tick += GameTimer_Tick;

        }

        private void BuildInfoPanel()
        {
            var titleLabel = new Label
            {
                Text = "СТАТУС ГРИ",
                Font = new Font("Segoe UI", 13, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new UIPoint(10, 15),
                Size = new Size(220, 30),
                TextAlign = ContentAlignment.MiddleCenter
            };
            infoPanel.Controls.Add(titleLabel);

            playerScoreLabel = new Label
            {
                Text = "Гравець: 0",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(46, 204, 113),
                Location = new UIPoint(15, 70),
                Size = new Size(210, 30)
            };
            infoPanel.Controls.Add(playerScoreLabel);

            botScoreLabel = new Label
            {
                Text = "Бот (ШІ): 0",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                ForeColor = Color.FromArgb(231, 76, 60),
                Location = new UIPoint(15, 105),
                Size = new Size(210, 30)
            };
            infoPanel.Controls.Add(botScoreLabel);

            statusLabel = new Label
            {
                Text = "Гра триває",
                Font = new Font("Segoe UI", 11, FontStyle.Italic),
                ForeColor = Color.LightGray,
                Location = new UIPoint(15, 160),
                Size = new Size(210, 60),
                TextAlign = ContentAlignment.MiddleLeft
            };
            infoPanel.Controls.Add(statusLabel);

            aiOpsLabel = new Label
            {
                Text = "Операцій ШІ: 0\r\nВикликів пошуку: 0",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.LightGray,
                Location = new UIPoint(15, 240),
                Size = new Size(210, 60)
            };
            infoPanel.Controls.Add(aiOpsLabel);

            var hintLabel = new Label
            {
                Text = "↑↓←→ або WASD\r\nSPACE - пауза",
                Font = new Font("Segoe UI", 9, FontStyle.Regular),
                ForeColor = Color.Gray,
                Location = new UIPoint(15, 320),
                Size = new Size(210, 50)
            };
            infoPanel.Controls.Add(hintLabel);

            restartButton = new Button
            {
                Text = "РЕСТАРТ",
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Size = new Size(200, 45),
                Location = new UIPoint(20, 410),
                BackColor = Color.FromArgb(52, 152, 219),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            restartButton.FlatAppearance.BorderSize = 0;
            restartButton.Click += RestartButton_Click;
            infoPanel.Controls.Add(restartButton);

            var saveButton = new Button
            {
                Text = "ЗБЕРЕГТИ РЕЗУЛЬТАТ",
                Font = new Font("Segoe UI", 9, FontStyle.Bold),
                Size = new Size(200, 35),
                Location = new UIPoint(20, 465),
                BackColor = Color.FromArgb(155, 89, 182),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat
            };
            saveButton.FlatAppearance.BorderSize = 0;
            saveButton.Click += SaveButton_Click;
            infoPanel.Controls.Add(saveButton);
        }

        private void StartGame()
        {
            gameTimer.Start();
            isPaused = false;
            Focus();
        }

        private void GameTimer_Tick(object? sender, EventArgs e)
        {
            if (gameState.Status != GameStatus.Running)
            {
                gameTimer.Stop();
                ShowGameOverDialog();
                return;
            }

            gameState.Update();
            UpdateInfoPanel();
            gamePanel.Invalidate();
        }

        private void UpdateInfoPanel()
        {
            playerScoreLabel.Text = $"Гравець: {gameState.Player.Score}  (довж. {gameState.Player.Length})";
            botScoreLabel.Text = $"Бот (ШІ): {gameState.Bot.Score}  (довж. {gameState.Bot.Length})";

            string st = gameState.Status switch
            {
                GameStatus.Running => isPaused ? "ПАУЗА" : "Гра триває",
                GameStatus.PlayerWon => "ВИ ПЕРЕМОГЛИ!",
                GameStatus.PlayerLost => "Ви програли...",
                GameStatus.Draw => "Нічия",
                _ => ""
            };
            statusLabel.Text = st;

            aiOpsLabel.Text = $"Операцій ШІ: {gameState.AI.TotalOperations}\r\n" +
                              $"Викликів пошуку: {gameState.AI.PathFindCallsCount}";
        }

        private void GamePanel_Paint(object? sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            // Малюємо сітку поля
            DrawGrid(g);

            // Малюємо їжу
            foreach (var food in gameState.Foods)
            {
                DrawFood(g, food);
            }

            // Малюємо змійки
            DrawSnake(g, gameState.Bot);
            DrawSnake(g, gameState.Player);
        }

        private void DrawGrid(Graphics g)
        {
            using var pen = new Pen(Color.FromArgb(40, 40, 60), 1);
            for (int x = 0; x <= FieldWidth; x++)
            {
                g.DrawLine(pen, x * CellSize, 0, x * CellSize, FieldHeight * CellSize);
            }
            for (int y = 0; y <= FieldHeight; y++)
            {
                g.DrawLine(pen, 0, y * CellSize, FieldWidth * CellSize, y * CellSize);
            }
        }

        private void DrawFood(Graphics g, Food food)
        {
            int padding = food.Type == FoodType.Bonus ? 2 : 4;
            var rect = new Rectangle(
                food.Position.X * CellSize + padding,
                food.Position.Y * CellSize + padding,
                CellSize - 2 * padding,
                CellSize - 2 * padding);

            using var brush = new SolidBrush(food.DisplayColor);
            g.FillEllipse(brush, rect);

            if (food.Type == FoodType.Bonus)
            {
                using var pen = new Pen(Color.Yellow, 2);
                g.DrawEllipse(pen, rect);
            }
        }

        private void DrawSnake(Graphics g, Snake snake)
        {
            if (!snake.IsAlive)
            {
                // Мертва змійка - сіра
                DrawSnakeBody(g, snake, Color.DimGray, Color.Gray);
                return;
            }
            DrawSnakeBody(g, snake, snake.HeadColor, snake.BodyColor);
        }

        private void DrawSnakeBody(Graphics g, Snake snake, Color headColor, Color bodyColor)
        {
            int idx = 0;
            foreach (var part in snake.Body)
            {
                int padding = 2;
                var rect = new Rectangle(
                    part.X * CellSize + padding,
                    part.Y * CellSize + padding,
                    CellSize - 2 * padding,
                    CellSize - 2 * padding);

                Color c = idx == 0 ? headColor : bodyColor;
                using var brush = new SolidBrush(c);
                g.FillRectangle(brush, rect);

                // Голова - з очима
                if (idx == 0)
                {
                    DrawSnakeEyes(g, snake, rect);
                }
                idx++;
            }
        }

        private void DrawSnakeEyes(Graphics g, Snake snake, Rectangle headRect)
        {
            using var eyeBrush = new SolidBrush(Color.White);
            using var pupilBrush = new SolidBrush(Color.Black);
            int eyeSize = 5;
            int pupilSize = 3;

            // Розташування очей залежно від напрямку
            int leftX, leftY, rightX, rightY;
            switch (snake.CurrentDirection)
            {
                case Direction.Up:
                    leftX = headRect.X + 4; leftY = headRect.Y + 4;
                    rightX = headRect.Right - 4 - eyeSize; rightY = headRect.Y + 4;
                    break;
                case Direction.Down:
                    leftX = headRect.X + 4; leftY = headRect.Bottom - 4 - eyeSize;
                    rightX = headRect.Right - 4 - eyeSize; rightY = headRect.Bottom - 4 - eyeSize;
                    break;
                case Direction.Left:
                    leftX = headRect.X + 4; leftY = headRect.Y + 4;
                    rightX = headRect.X + 4; rightY = headRect.Bottom - 4 - eyeSize;
                    break;
                default: // Right
                    leftX = headRect.Right - 4 - eyeSize; leftY = headRect.Y + 4;
                    rightX = headRect.Right - 4 - eyeSize; rightY = headRect.Bottom - 4 - eyeSize;
                    break;
            }

            g.FillEllipse(eyeBrush, leftX, leftY, eyeSize, eyeSize);
            g.FillEllipse(eyeBrush, rightX, rightY, eyeSize, eyeSize);
            g.FillEllipse(pupilBrush, leftX + 1, leftY + 1, pupilSize, pupilSize);
            g.FillEllipse(pupilBrush, rightX + 1, rightY + 1, pupilSize, pupilSize);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Up:
                case Keys.W:
                    // Додано перевірку: дія виконується ТІЛЬКИ якщо гра НЕ на паузі
                    if (!isPaused) gameState.Player.ChangeDirection(Direction.Up);
                    return true;

                case Keys.Down:
                case Keys.S:
                    if (!isPaused) gameState.Player.ChangeDirection(Direction.Down);
                    return true;

                case Keys.Left:
                case Keys.A:
                    if (!isPaused) gameState.Player.ChangeDirection(Direction.Left);
                    return true;

                case Keys.Right:
                case Keys.D:
                    if (!isPaused) gameState.Player.ChangeDirection(Direction.Right);
                    return true;

                case Keys.Space:
                    // Пробіл залишаємо без змін, щоб можна було зняти з паузи
                    TogglePause();
                    return true;
            }

            // Якщо натиснута інша клавіша, викликаємо базовий метод
            return base.ProcessCmdKey(ref msg, keyData);
        }
        private void TogglePause()
        {
            if (gameState.Status != GameStatus.Running) return;

            if (isPaused)
            {
                gameTimer.Start();
                isPaused = false;
            }
            else
            {
                gameTimer.Stop();
                isPaused = true;
            }
            UpdateInfoPanel();
        }

        private void RestartButton_Click(object? sender, EventArgs e)
        {
            gameTimer.Stop();
            gameState = new GameState(FieldWidth, FieldHeight);
            UpdateInfoPanel();
            gamePanel.Invalidate();
            StartGame();
        }

        private void ShowGameOverDialog()
        {
            string msg = gameState.Status switch
            {
                GameStatus.PlayerWon => $"Вітаємо! Ви перемогли!\r\nВаш рахунок: {gameState.Player.Score}",
                GameStatus.PlayerLost => $"На жаль, ви програли.\r\nВаш рахунок: {gameState.Player.Score}\r\nРахунок бота: {gameState.Bot.Score}",
                GameStatus.Draw => $"Нічия! Обидві змійки загинули з однаковим рахунком ({gameState.Player.Score}).",
                _ => "Гра завершена."
            };
            MessageBox.Show(msg, "Гра завершена", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void SaveButton_Click(object? sender, EventArgs e)
        {
            try
            {
                using var dialog = new SaveFileDialog
                {
                    Filter = "Текстовий файл (*.txt)|*.txt",
                    FileName = $"snake_result_{DateTime.Now:yyyyMMdd_HHmmss}.txt"
                };
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    string content = $"Результати гри \"Змійка\"\r\n" +
                                     $"Дата: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\r\n" +
                                     $"-----------------------------------------\r\n" +
                                     $"Гравець: {gameState.Player.Score} очок (довжина {gameState.Player.Length})\r\n" +
                                     $"Бот (ШІ): {gameState.Bot.Score} очок (довжина {gameState.Bot.Length})\r\n" +
                                     $"Алгоритм ШІ: {gameState.AI.Algorithm}\r\n" +
                                     $"Загалом операцій ШІ: {gameState.AI.TotalOperations}\r\n" +
                                     $"Кількість викликів пошуку: {gameState.AI.PathFindCallsCount}\r\n" +
                                     $"Статус: {gameState.Status}\r\n";
                    System.IO.File.WriteAllText(dialog.FileName, content);
                    MessageBox.Show("Результати збережено!", "Успіх",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Помилка збереження: {ex.Message}", "Помилка",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
