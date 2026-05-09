using System;
using System.Windows.Forms;

namespace SnakeGame
{
    /// <summary>
    /// Точка входу до програми "Змійка".
    /// Запускає головне вікно з правилами гри.
    /// </summary>
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Перед запуском основної гри показуємо вікно з правилами
            using (RulesForm rulesForm = new RulesForm())
            {
                if (rulesForm.ShowDialog() == DialogResult.OK)
                {
                    Application.Run(new GameForm());
                }
            }
        }
    }
}
