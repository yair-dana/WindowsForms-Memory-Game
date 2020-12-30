using System.Windows.Forms;

namespace GameUserInterface
{
    public class Program
    {
        public static void Main()
        {
            Application.EnableVisualStyles();
            GameUI GameUI = new GameUI();
            GameUI.RunGame();
        }
    }
}
