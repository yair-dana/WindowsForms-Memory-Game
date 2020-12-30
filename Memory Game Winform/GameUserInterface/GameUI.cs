using System.Windows.Forms;
using GameLogic;

namespace GameUserInterface
{
    public class GameUI
    {
        private Game m_GameLogic;
        private FormGame m_GameForm;
        private FormLogin m_WelcomeForm;

        public GameUI()
        {
            m_WelcomeForm = new FormLogin();
            m_WelcomeForm.ShowDialog();
            
            string firstUserName = m_WelcomeForm.FirstPlayerName;
            string secondUserName = m_WelcomeForm.SecondPlayerName;
            int row = m_WelcomeForm.RowSize;
            int col = m_WelcomeForm.ColSize;
            bool isUserTwoHuman = m_WelcomeForm.IsHumanGame;
            Game.eGameType gameType = isUserTwoHuman ? Game.eGameType.AgainstAnotherUser : Game.eGameType.AgainstTheComputer;
            createGameSetting(firstUserName, secondUserName, gameType, row, col);
        }

        private void createGameSetting(string i_FirstUserName, string i_SecondUserName, Game.eGameType i_GameType, int i_Row, int i_Col)
        {
            m_GameLogic = new Game(i_FirstUserName, i_SecondUserName, i_GameType, i_Row, i_Col);
            m_GameForm = new FormGame(i_Row, i_Col, i_FirstUserName, i_SecondUserName);
            m_GameForm.Clicked = new ButtonEventHandler(runTurnes);
            m_GameForm.AfterGameOver = new GameOverEventHandler(gameOverShowWinnerAndMessageBox);
            m_GameLogic.Board.AfterHide += new HideCardEventHandler(m_GameForm.resetButton);
            m_GameLogic.Board.AfterExpose += new ExposeCardEventHandler(m_GameForm.exposedButton);
        }

        private void createNewGameWithSameSetting()
        {
            string firstUserName = m_GameLogic.PlayerOne.Name;
            string secondUserName = m_GameLogic.PlayerTwo.Name;
            Game.eGameType gameType = m_GameLogic.Type;
            int row = m_GameLogic.Board.Row;
            int col = m_GameLogic.Board.Col;
            createGameSetting(firstUserName, secondUserName, gameType, row, col);
        }

        public void RunGame()
        {
            bool PlayGame = true;
            while (PlayGame)
            {
                m_GameForm.ShowDialog();
                if (m_GameForm.IsToStartAnotherGame == DialogResult.Yes)
                {
                    createNewGameWithSameSetting();
                }
                else
                {
                    PlayGame = false;
                }
            }
        }

        private void runTurnes(Button i_Btn)
        {
            if (m_GameLogic.Turn == Game.eGameTurn.FirstUser || (m_GameLogic.Turn == Game.eGameTurn.SecondUser && m_GameLogic.Type == Game.eGameType.AgainstAnotherUser))
            {
                runUserTurn(i_Btn);
            }

            if (!m_GameLogic.IsTheGameOver() && m_GameLogic.Turn == Game.eGameTurn.SecondUser && m_GameLogic.Type == Game.eGameType.AgainstTheComputer)
            {
                bool matchWasFound = false;
                do
                {
                    matchWasFound = runComputerTurn();
                } 
                while (matchWasFound && !m_GameLogic.IsTheGameOver());
            }
        }

        private void runUserTurn(Button i_Btn)
        {
            runUserInterval(i_Btn);
            if (m_GameLogic.Interval == Game.eTurnInterval.secondInterval)
            {
                bool matchFound = m_GameLogic.IsFoundPair();
                matchingCardsHandaling(matchFound);
            }

            if(m_GameLogic.IsTheGameOver())
            {
                m_GameForm.IsTheGameOver = true;
            }

            m_GameLogic.SwitchIntervalTurn();
        }

        private bool runComputerTurn()
        {
            string firstCardIndex, secondCardIndex;
            m_GameLogic.PlayerTwo.AI.ChooseCardsComputer(m_GameLogic.Board, out firstCardIndex, out secondCardIndex);
            m_GameLogic.Board.ExposedCard(firstCardIndex);
            m_GameLogic.SwitchIntervalTurn();
            System.Threading.Thread.Sleep(1000);
            m_GameLogic.Board.ExposedCard(secondCardIndex);
            bool foundMatch = m_GameLogic.IsFoundPair();
            matchingCardsHandaling(foundMatch);
            m_GameLogic.SwitchIntervalTurn();
            if (m_GameLogic.IsTheGameOver())
            {
                m_GameForm.IsTheGameOver = true;
            }

            return foundMatch;
        }

        private void runUserInterval(Button i_Btn)
        {
            int row = int.Parse(i_Btn.Name[0].ToString());
            int col = int.Parse(i_Btn.Name[2].ToString());
            m_GameLogic.Board.ExposedCard(row, col);
        }

        private void matchingCardsHandaling(bool i_FoundMatch)
        {
            if (i_FoundMatch)
            {
                m_GameLogic.IncreasePoints();
                updateScoreOnGameForm();
                System.Threading.Thread.Sleep(1000);
            }
            else
            {
                m_GameLogic.Turn = m_GameLogic.Turn == Game.eGameTurn.FirstUser ? Game.eGameTurn.SecondUser : Game.eGameTurn.FirstUser;
                System.Threading.Thread.Sleep(1000);
                m_GameLogic.HideIntevalsCards();
                changeCurrentPlayerOnForm();
            }
        }

        private void changeCurrentPlayerOnForm()
        {
            if (m_GameLogic.Turn == Game.eGameTurn.FirstUser)
            {
                m_GameForm.m_CurrentPlayer.Text = string.Format("Current Player: {0}", m_GameLogic.PlayerOne.Name);
                m_GameForm.m_CurrentPlayer.BackColor = m_GameForm.m_FirstPlayer.BackColor;
            }
            else
            {
                m_GameForm.m_CurrentPlayer.Text = string.Format("Current Player: {0}", m_GameLogic.PlayerTwo.Name);
                m_GameForm.m_CurrentPlayer.BackColor = m_GameForm.m_SecondPlayer.BackColor;
            }

            m_GameForm.m_CurrentPlayer.Refresh();
        }

        private void updateScoreOnGameForm()
        {
            if (m_GameLogic.Turn == Game.eGameTurn.FirstUser)
            {
                m_GameForm.m_FirstPlayer.Text = string.Format("{0}: {1} Pairs", m_GameLogic.PlayerOne.Name, m_GameLogic.PlayerOne.Points);
            }
            else
            {
                m_GameForm.m_SecondPlayer.Text = string.Format("{0}: {1} Pairs", m_GameLogic.PlayerTwo.Name, m_GameLogic.PlayerTwo.Points);
            }
        }

        private void gameOverShowWinnerAndMessageBox(out DialogResult i_Result)
        {
            User winner = m_GameLogic.DetermineWinner();
            i_Result = showMessageBoxAndAskForAnotherGame(winner);
        }

        private DialogResult showMessageBoxAndAskForAnotherGame(User i_Winner)
        {
            string anotherGameMsg = "Do you want to start another Game?";
            string winnerMsg;
            if (i_Winner != null)
            {
                winnerMsg = string.Format("The Winner is {0}, with {1} Points!", i_Winner.Name, i_Winner.Points);
            }
            else
            {
                winnerMsg = string.Format("The Game Over with equality of {0} Points", m_GameLogic.Board.MaxPairCards / 2, m_GameLogic.Board);
            }

            string message = winnerMsg + System.Environment.NewLine + anotherGameMsg;
            string title = "Game Over";
            MessageBoxButtons buttons = MessageBoxButtons.YesNo;
            return MessageBox.Show(message, title, buttons);
        }
    }
}
