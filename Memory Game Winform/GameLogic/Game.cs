namespace GameLogic
{
    public class Game
    {
        private readonly eGameType r_GameType;
        private User m_UserPlayer1;
        private User m_UserPlayer2;
        private Board m_Board;
        private eGameTurn m_GameTurn;
        private eTurnInterval m_TurnInterval = eTurnInterval.firstInterval;
        private int m_FirstCardRow, m_FirstCardCol, m_SecondCardRow, m_SecondCardCol;

        public enum eGameType
        {
            AgainstAnotherUser,
            AgainstTheComputer
        }

        public enum eGameTurn
        {
            FirstUser,
            SecondUser
        }

        public enum eTurnInterval
        {
            firstInterval,
            secondInterval
        }

        public Game(string i_FirstUserName, string i_SecondUserName, Game.eGameType i_GameType, int i_Lenght, int i_Width)
        {
            bool isSecondUserHuman = i_GameType == eGameType.AgainstTheComputer ? false : true;
            r_GameType = i_GameType;
            m_GameTurn = eGameTurn.FirstUser;
            m_Board = new Board(i_Lenght, i_Width);
            m_UserPlayer1 = new User(i_FirstUserName, true, 0);
            m_UserPlayer2 = new User(i_SecondUserName, isSecondUserHuman, m_Board.MaxPairCards / 2);
            m_Board.AfterExpose += new ExposeCardEventHandler(updateIndexCardByInteval);
            if(m_UserPlayer2.IsHuman == false)
            {
                m_Board.AfterExpose += new ExposeCardEventHandler(updateComputerAI);
            }
        }

        public Board Board
        {
            get
            {
                return m_Board;
            }
        }

        public eGameType Type
        {
            get
            {
                return r_GameType;
            }
        }

        public eGameTurn Turn
        {
            get
            {
                return m_GameTurn;
            }

            set
            {
                m_GameTurn = value;
            }
        }

        public eTurnInterval Interval
        {
            get
            {
                return m_TurnInterval;
            }

            set
            {
                m_TurnInterval = value;
            }
        }

        public User PlayerOne
        {
            get
            {
                return m_UserPlayer1;
            }
        }

        public User PlayerTwo
        {
            get
            {
                return m_UserPlayer2;
            }
        }

        private void updateComputerAI(string i_CardIndex)
        {
            if (m_UserPlayer2.IsHuman == false)
            {
                int row, col;
                Board.ParseIndexCard(i_CardIndex, out row, out col);
                int cardKey = (int)m_Board.gameBoard[row, col].identiykey;
                m_UserPlayer2.AI.AddIndexToMemory(cardKey, i_CardIndex);
            }
        }

        private void updateComputerAI(int i_CardKey, int i_RowIndex, int i_ColIndex)
        {
            if (m_UserPlayer2.IsHuman == false)
            {
                string cardIndex = Board.ParseIndexCard(i_RowIndex, i_ColIndex);
                updateComputerAI(cardIndex);
            }
        }

        public bool IsCardsMatch(int i_FirstCardRow, int i_FirstCardCol, int i_SecondCardRow, int i_SecondCardCol)
        {
            return Board.IsAMatchFound(m_Board, i_FirstCardRow, i_FirstCardCol, i_SecondCardRow, i_SecondCardCol);
        }

        public bool IsFoundPair()
        {
                return m_TurnInterval == eTurnInterval.secondInterval && 
                            Board.IsAMatchFound(m_Board, m_FirstCardRow, m_FirstCardCol, m_SecondCardRow, m_SecondCardCol);   
        }

        public bool IsTheGameOver()
        {
            return m_Board.MaxPairCards == m_Board.NumOfExposedPairCards;
        }

        public void IncreasePoints()
        {
            if (m_GameTurn == eGameTurn.FirstUser)
            {
                m_UserPlayer1.Points++;
            }
            else
            {
                m_UserPlayer2.Points++;
            }

            m_Board.NumOfExposedPairCards++;
        }

        public void SwitchIntervalTurn()
        {
            m_TurnInterval = m_TurnInterval == Game.eTurnInterval.firstInterval ? Game.eTurnInterval.secondInterval : Game.eTurnInterval.firstInterval;
        }

        public User DetermineWinner()
        {
            User winner = null;
            if (m_UserPlayer1.Points > m_UserPlayer2.Points)
            {
                winner = m_UserPlayer1;
            }
            else if (m_UserPlayer1.Points < m_UserPlayer2.Points)
            {
                winner = m_UserPlayer2;
            }

            return winner;
        }

        private void updateIndexCardByInteval(int i_CardKey, int i_Row, int i_Col)
        {
            if(m_TurnInterval == eTurnInterval.firstInterval)
            {
                m_FirstCardRow = i_Row;
                m_FirstCardCol = i_Col;
            }
            else
            {
                m_SecondCardRow = i_Row;
                m_SecondCardCol = i_Col;
            }
        }

        public void HideIntevalsCards()
        {
            m_Board.HideCard(m_FirstCardRow, m_FirstCardCol);
            m_Board.HideCard(m_SecondCardRow, m_SecondCardCol);
        }
    }
}
