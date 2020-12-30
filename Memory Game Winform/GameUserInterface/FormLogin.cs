using System;
using System.Windows.Forms;

namespace GameUserInterface
{
    public partial class FormLogin : Form
    {
        private readonly int r_MaxBoardSizeOptions = 8;
        private int m_BoardSizeClicks = 0;

        private enum eBoardSize
        {
            FourXFour,
            FourXFive,
            FourXSix,
            FiveXFour,
            FiveXSix,
            SixXFour,
            SixXFive,
            SixXSix
        }

        public FormLogin()
        {
            InitializeComponent();
        }

        public string FirstPlayerName
        {
            get
            { 
                return textBoxPlayer1.Text;
            }
        }

        public string SecondPlayerName
        {
            get
            {
                if (textBoxFriend.Enabled == false)
                {
                    return "COMPUTER";
                }
                else
                {
                    return textBoxFriend.Text;
                }
            }
        }

        public bool IsHumanGame
        {
            get { return textBoxFriend.Enabled; }
        }

        public int RowSize
        {
            get
            {
                eBoardSize currentSize = (eBoardSize)(m_BoardSizeClicks % r_MaxBoardSizeOptions);
                if (currentSize == eBoardSize.FiveXFour || currentSize == eBoardSize.FiveXSix)
                {
                    return 5;
                }
                else if (currentSize == eBoardSize.FourXFive || currentSize == eBoardSize.FourXFour || currentSize == eBoardSize.FourXSix)
                {
                    return 4;
                }
                else
                {
                    return 6;
                }
            }
        }

        public int ColSize
        {
            get
            {
                eBoardSize currentSize = (eBoardSize)(m_BoardSizeClicks % r_MaxBoardSizeOptions);
                if (currentSize == eBoardSize.FiveXFour || currentSize == eBoardSize.SixXFour || currentSize == eBoardSize.FourXFour)
                {
                    return 4;
                }
                else if (currentSize == eBoardSize.FiveXSix || currentSize == eBoardSize.FourXSix || currentSize == eBoardSize.SixXSix)
                {
                    return 6;
                }
                else
                {
                    return 5;
                }
            }
        }

        private void buttonPlayer2_Click(object sender, EventArgs e)
        {
            if (textBoxFriend.Enabled == false)
            {
                textBoxFriend.ReadOnly = false;
                textBoxFriend.Enabled = true;
                textBoxFriend.ResetText();
                buttonPlayer2.Text = "Against Computer";
            }
            else
            {
                textBoxFriend.ReadOnly = true;
                textBoxFriend.Enabled = false;
                textBoxFriend.Text = "-computer-";
                buttonPlayer2.Text = "Against a Friend";
            }
        }

        private void buttonBoardSize_Click(object sender, EventArgs e)
        {
            m_BoardSizeClicks++;
            eBoardSize currentSize = (eBoardSize)(m_BoardSizeClicks % r_MaxBoardSizeOptions);
   
            switch (currentSize)
            {
                case eBoardSize.FourXFour:
                    buttonBoardSize.Text = "4 x 4";
                    break;
                case eBoardSize.FourXFive:
                    buttonBoardSize.Text = "4 x 5";
                    break;
                case eBoardSize.FourXSix:
                    buttonBoardSize.Text = "4 x 6";
                    break;
                case eBoardSize.FiveXFour:
                    buttonBoardSize.Text = "5 x 4";
                    break;
                case eBoardSize.FiveXSix:
                    buttonBoardSize.Text = "5 x 6";
                    break;
                case eBoardSize.SixXFour:
                    buttonBoardSize.Text = "6 x 4";
                    break;
                case eBoardSize.SixXFive:
                    buttonBoardSize.Text = "6 x 5";
                    break;
                case eBoardSize.SixXSix:
                    buttonBoardSize.Text = "6 x 6";
                    break;
            }
        }

        private void buttonStart_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}