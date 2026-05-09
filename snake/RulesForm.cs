using System.Drawing;
using System.Windows.Forms;
// Alias щоб розрізняти System.Drawing.Point та SnakeGame.Point
using UIPoint = System.Drawing.Point;

namespace SnakeGame
{
    /// <summary>
    /// Форма-вітання з правилами гри. Відкривається першою при запуску програми.
    /// </summary>
    public class RulesForm : Form
    {
        private Button startButton = null!;
        private Button exitButton = null!;
        private Label titleLabel = null!;
        private Label rulesLabel = null!;

        public RulesForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Налаштування форми
            Text = "Змійка - Правила гри";
            Size = new Size(620, 580);
            StartPosition = FormStartPosition.CenterScreen;
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            BackColor = Color.FromArgb(30, 30, 40);

            // Заголовок
            titleLabel = new Label
            {
                Text = "ЗМІЙКА",
                Font = new Font("Segoe UI", 28, FontStyle.Bold),
                ForeColor = Color.FromArgb(46, 204, 113),
                Location = new UIPoint(0, 20),
                Size = new Size(620, 50),
                TextAlign = ContentAlignment.MiddleCenter
            };
            Controls.Add(titleLabel);

            // Текст правил
            rulesLabel = new Label
            {
                Text = "ПРАВИЛА ГРИ:\r\n\r\n" +
                       "  • Керуйте зеленою змійкою за допомогою клавіш-стрілок\r\n" +
                       "    (↑ ↓ ← →) або клавіш WASD.\r\n\r\n" +
                       "  • Збирайте червону їжу — за неї дається 1 очко.\r\n\r\n" +
                       "  • Шукайте золоту бонусну їжу — вона дає 5 очок!\r\n\r\n" +
                       "  • Червона змійка-бот керується штучним інтелектом\r\n" +
                       "    (алгоритм A*). Вона також полює за їжею.\r\n\r\n" +
                       "  • При зіткненні зі стіною змійка з'являється з протилежного боку.\r\n\r\n" +
                       "  • НЕ ВРІЗАЙТЕСЬ у власне тіло чи у тіло змійки-суперника!\r\n\r\n" +
                       "  • Перемагає той, хто залишиться живим. У разі загибелі обох —\r\n" +
                       "    хто набрав більше очок.\r\n\r\n" +
                       "  • Пауза/поновлення гри — клавіша SPACE.",
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = Color.White,
                Location = new UIPoint(40, 80),
                Size = new Size(540, 360),
                TextAlign = ContentAlignment.TopLeft
            };
            Controls.Add(rulesLabel);

            // Кнопка "Почати гру"
            startButton = new Button
            {
                Text = "ПОЧАТИ ГРУ",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Size = new Size(200, 50),
                Location = new UIPoint(80, 470),
                BackColor = Color.FromArgb(46, 204, 113),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.OK
            };
            startButton.FlatAppearance.BorderSize = 0;
            Controls.Add(startButton);

            // Кнопка "Вихід"
            exitButton = new Button
            {
                Text = "ВИХІД",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Size = new Size(200, 50),
                Location = new UIPoint(340, 470),
                BackColor = Color.FromArgb(231, 76, 60),
                ForeColor = Color.White,
                FlatStyle = FlatStyle.Flat,
                DialogResult = DialogResult.Cancel
            };
            exitButton.FlatAppearance.BorderSize = 0;
            Controls.Add(exitButton);

            AcceptButton = startButton;
            CancelButton = exitButton;
        }
    }
}
