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
        
        // Група кнопок та властивість для збереження вибраного алгоритму
        private GroupBox algoGroup = null!;
        private RadioButton aStarRadio = null!;
        private RadioButton bfsRadio = null!;
        private RadioButton greedyRadio = null!;

        public PathAlgorithm SelectedAlgorithm { get; private set; } = PathAlgorithm.AStar;

        public RulesForm()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            // Налаштування форми (висоту збільшено, щоб помістилися кнопки)
            Text = "Змійка - Правила гри";
            Size = new Size(620, 720); 
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
                       "  • Червона змійка-бот керується штучним інтелектом.\r\n" +
                       "    Вона також полює за їжею.\r\n\r\n" +
                       "  • При зіткненні зі стіною змійка з'являється з протилежного боку.\r\n\r\n" +
                       "  • НЕ ВРІЗАЙТЕСЬ у власне тіло чи у тіло змійки-суперника!\r\n\r\n" +
                       "  • Перемагає той, хто залишиться живим. У разі загибелі обох —\r\n" +
                       "    хто набрав більше очок.\r\n\r\n" +
                       "  • Пауза/поновлення гри — клавіша SPACE.",
                Font = new Font("Segoe UI", 11, FontStyle.Regular),
                ForeColor = Color.White,
                Location = new UIPoint(40, 80),
                Size = new Size(540, 310),
                TextAlign = ContentAlignment.TopLeft
            };
            Controls.Add(rulesLabel);

            // Панель вибору алгоритму
            algoGroup = new GroupBox
            {
                Text = "Виберіть алгоритм ШІ для бота:",
                ForeColor = Color.LightGray,
                Font = new Font("Segoe UI", 11, FontStyle.Bold),
                Location = new UIPoint(40, 390),
                Size = new Size(520, 180)
            };
            Controls.Add(algoGroup);

            aStarRadio = new RadioButton
            {
                Text = "🔵 A* (А-зірочка) — основний. Завжди знаходить оптимальний шлях і робить це швидко.",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Location = new UIPoint(20, 35),
                Size = new Size(480, 40),
                Checked = true // A* вибрано за замовчуванням
            };
            aStarRadio.CheckedChanged += (s, e) => { if (aStarRadio.Checked) SelectedAlgorithm = PathAlgorithm.AStar; };
            algoGroup.Controls.Add(aStarRadio);

            bfsRadio = new RadioButton
            {
                Text = "🟡 BFS (пошук в ширину). Завжди знаходить найкоротший шлях, але повільніше за A*.",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Location = new UIPoint(20, 80),
                Size = new Size(480, 40)
            };
            bfsRadio.CheckedChanged += (s, e) => { if (bfsRadio.Checked) SelectedAlgorithm = PathAlgorithm.BFS; };
            algoGroup.Controls.Add(bfsRadio);

            greedyRadio = new RadioButton
            {
                Text = "🟢 Greedy (жадібний). Дуже швидкий, але ненадійний — може застрягнути в тупику.",
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 10, FontStyle.Regular),
                Location = new UIPoint(20, 125),
                Size = new Size(480, 40)
            };
            greedyRadio.CheckedChanged += (s, e) => { if (greedyRadio.Checked) SelectedAlgorithm = PathAlgorithm.Greedy; };
            algoGroup.Controls.Add(greedyRadio);

            // Кнопка "Почати гру"
            startButton = new Button
            {
                Text = "ПОЧАТИ ГРУ",
                Font = new Font("Segoe UI", 12, FontStyle.Bold),
                Size = new Size(200, 50),
                Location = new UIPoint(80, 600),
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
                Location = new UIPoint(320, 600),
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
