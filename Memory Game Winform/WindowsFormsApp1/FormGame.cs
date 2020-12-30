using System;
using System.Drawing;
using System.Windows.Forms;

namespace GameUserInterface
{
    public delegate void ButtonEventHandler(Button i_Btn);

    public delegate void GameOverEventHandler(out DialogResult i_Result);

    public partial class FormGame : Form
    {
        internal Label m_FirstPlayer;
        internal Label m_SecondPlayer;
        internal Label m_CurrentPlayer;
        internal Button[,] m_Buttons;
        private string[] m_PictuersUrl;
        private bool m_IsTheGameOver = false;
        private DialogResult m_StartAnotherGame = DialogResult.None;
        public ButtonEventHandler Clicked;
        public GameOverEventHandler AfterGameOver;

        public FormGame(int i_Row, int i_Col, string i_FirstPlayerName, string i_SecondPlayerName)
        {
            m_Buttons = new Button[i_Row, i_Col];
            initializeComponent(i_Row, i_Col, i_FirstPlayerName, i_SecondPlayerName);
            createRandomPicturesUrlArray(i_Row, i_Col);
        }

        public bool IsTheGameOver
        {
            set { m_IsTheGameOver = value; }
        }

        public DialogResult IsToStartAnotherGame
        {
            get { return m_StartAnotherGame; }
        }

        private void initializeComponent(int i_Row, int i_Col, string i_FirstPlayerName, string i_SecondPlayerName)
        {
            int currentLoctionTop = 20;
            int currentLoctionLeft = 20;

            createButtons(i_Row, i_Col, ref currentLoctionLeft, ref currentLoctionTop);
            createLables(ref currentLoctionLeft, ref currentLoctionTop, i_FirstPlayerName, i_SecondPlayerName);
            setMainForm(i_Row, i_Col);
        }

        private void createButtons(int i_Row, int i_Col, ref int io_LoctionLeft, ref int io_LoctionTop)
        {
            for (int rowIndex = 0; rowIndex < i_Row; rowIndex++)
            {
                if (rowIndex != 0)
                {
                    io_LoctionTop += 80;
                }

                for (int colIndex = 0; colIndex < i_Col; colIndex++)
                {
                    m_Buttons[rowIndex, colIndex] = new Button();
                    if (colIndex != 0)
                    {
                        io_LoctionLeft += 80;
                    }

                    m_Buttons[rowIndex, colIndex].Location = new Point(io_LoctionLeft, io_LoctionTop);
                    m_Buttons[rowIndex, colIndex].Size = new Size(75, 75);
                    m_Buttons[rowIndex, colIndex].UseVisualStyleBackColor = true;
                    m_Buttons[rowIndex, colIndex].Click += m_Button_Click;
                    m_Buttons[rowIndex, colIndex].Name = string.Format("{0}:{1}", rowIndex, colIndex);
                    m_Buttons[rowIndex, colIndex].BackColor = Color.LightGray;
                    this.Controls.Add(m_Buttons[rowIndex, colIndex]);
                }

                io_LoctionLeft = 20;
            }
        }

        private void createLables(ref int io_LoctionLeft, ref int io_LoctionTop, string i_FirstPlayerName, string i_SecondPlayerName)
        {
            m_FirstPlayer = new Label();
            m_SecondPlayer = new Label();
            m_CurrentPlayer = new Label();

            io_LoctionTop += 100;
            m_CurrentPlayer.Location = new Point(io_LoctionLeft, io_LoctionTop);
            m_CurrentPlayer.Text = "Current Player:" + i_FirstPlayerName;
            m_CurrentPlayer.AutoSize = true;

            io_LoctionTop += 20;
            m_FirstPlayer.Location = new Point(io_LoctionLeft, io_LoctionTop);
            m_FirstPlayer.Text = i_FirstPlayerName + ": 0 Pairs";
            m_FirstPlayer.AutoSize = true;

            io_LoctionTop += 20;
            m_SecondPlayer.Location = new Point(io_LoctionLeft, io_LoctionTop);
            m_SecondPlayer.Text = i_SecondPlayerName + ": 0 Pairs";
            m_SecondPlayer.AutoSize = true;

            m_FirstPlayer.BackColor = Color.FromArgb(192, 255, 192);
            m_SecondPlayer.BackColor = Color.FromArgb(192, 192, 255);
            m_CurrentPlayer.BackColor = m_FirstPlayer.BackColor;

            this.Controls.Add(m_FirstPlayer);
            this.Controls.Add(m_SecondPlayer);
            this.Controls.Add(m_CurrentPlayer);
        }

        private void setMainForm(int i_Row, int i_Col)
        {
            int width = (i_Col * 80) + 50;
            int height = (i_Row * 80) + 160;
            this.Name = "FormGame";
            this.Text = "Memory Game";
            this.Size = new Size(width, height);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
        }

        private void m_Button_Click(object sender, EventArgs e)
        {
            if (sender is Button && (sender as Button).Image == null)
            {
                Button btn = sender as Button;
                int row = int.Parse(btn.Name[0].ToString());
                int col = int.Parse(btn.Name[2].ToString());
                if (Clicked != null)
                {
                    Clicked.Invoke(btn);
                }

                if (m_IsTheGameOver)
                {
                    AfterGameOver.Invoke(out m_StartAnotherGame);
                    endGame();
                }
            }
        }

        internal void resetButton(int i_Row, int i_Col)
        {
            Button buttonToHide = m_Buttons[i_Row, i_Col];
            buttonToHide.Image = null;
            buttonToHide.FlatStyle = FlatStyle.Standard;
            buttonToHide.Refresh();
        }
        
        internal void exposedButton(int i_CardKey, int i_Row, int i_Col)
        {
            PictureBox imageButton = new PictureBox();
            Button buttonToExposed = m_Buttons[i_Row, i_Col];
            imageButton.Load(m_PictuersUrl[i_CardKey - 1]);
            buttonToExposed.Image = imageButton.Image;
            buttonToExposed.FlatStyle = FlatStyle.Flat;
            buttonToExposed.FlatAppearance.BorderColor = m_CurrentPlayer.BackColor;
            buttonToExposed.FlatAppearance.BorderSize = 5;
            buttonToExposed.Refresh();
        }

        private void createRandomPicturesUrlArray(int i_Row, int i_Col)
        {
            int arraySize = (i_Row * i_Col) / 2;
            m_PictuersUrl = new string[arraySize];
            int index = 0;
            do
            {
                System.Net.HttpWebRequest req = (System.Net.HttpWebRequest)System.Net.WebRequest.Create("https://picsum.photos/80");
                System.Net.HttpWebResponse res = (System.Net.HttpWebResponse)req.GetResponse();
                if (isUrlAlreadyExist(res.ResponseUri.ToString(), index) == false)
                {
                    m_PictuersUrl[index] = res.ResponseUri.ToString();
                    index++;
                }

                res.Close();
            } 
            while(index < arraySize);
        }

        private bool isUrlAlreadyExist(string imageUrl, int currIndexArray)
        {
            bool res = false;
            for(int i = 0; i < currIndexArray; i++)
            {
                if(m_PictuersUrl[i] == imageUrl)
                {
                    res = true;
                    break;
                }
            }

            return res;
        }

        private void endGame()
        {
            this.DialogResult = DialogResult.OK;
            this.Close();
        }
    }
}