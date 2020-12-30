using System;
using System.Text;

namespace GameLogic
{
    public delegate void ExposeCardEventHandler(int i_CardKey, int i_Row, int i_Col);

    public delegate void HideCardEventHandler(int i_Row, int i_col);

    public class Board
    { 
        public event ExposeCardEventHandler AfterExpose;

        public event HideCardEventHandler AfterHide;

        private Card[,] m_GameBoard;
        private int m_Row;
        private int m_Col;
        private int m_NumOfExposedPairCards;
        private int m_MaxPairCards;
        private Random m_rnd = new Random();

        public Board(int i_Row, int i_Col)
        {
            m_Row = i_Row;
            m_Col = i_Col;
            m_MaxPairCards = (i_Row * i_Col) / 2;
            m_NumOfExposedPairCards = 0;
            m_GameBoard = new Card[i_Row, i_Col];
            createBoardValues();
        }

        public int Row
        {
            get
            {
                return m_Row;
            }
        }

        public int Col
        {
            get
            {
                return m_Col;
            }
        }

        internal Card[,] gameBoard
        {
            get
            {
                return m_GameBoard;
            }
        }

        public int NumOfExposedPairCards
        {
            get
            {
                return m_NumOfExposedPairCards;
            }

            set
            {
                m_NumOfExposedPairCards = value;
            }
        }

        public int MaxPairCards
        {
            get
            {
                return m_MaxPairCards;
            }
        }

        public static void ParseIndexCard(string i_Card, out int o_IdxRow, out int o_IdxCol)
        {
            o_IdxRow = i_Card[1] - '1';
            o_IdxCol = i_Card[0] - 'A';
        }

        public static string ParseIndexCard(int i_IdxRow, int i_IdxCol)
        {
            StringBuilder indexOfCard = new StringBuilder();
            indexOfCard.Append((char)((int)'A' + i_IdxCol));
            indexOfCard.Append((char)((int)'1' + i_IdxRow));
            return indexOfCard.ToString();
        }

        private void createBoardValues()
        {
            for (int key = 1; key <= m_MaxPairCards; key++)
            {
                chooseRandomPlaceAndPutIdentiyKey(key);
                chooseRandomPlaceAndPutIdentiyKey(key);
            }
        }

        private void chooseRandomPlaceAndPutIdentiyKey(int i_Key)
        {
            int idxRow = 0;
            int idxCol = 0;
            do
            {
                idxRow = m_rnd.Next(m_Row);
                idxCol = m_rnd.Next(m_Col);
            }
            while (m_GameBoard[idxRow, idxCol].identiykey.HasValue);
            m_GameBoard[idxRow, idxCol].identiykey = i_Key;
        }

        public void ExposedCard(string i_IndexCard)
        {
            int RowIndex, ColIndex;
            ParseIndexCard(i_IndexCard, out RowIndex, out ColIndex);
            ExposedCard(RowIndex, ColIndex);
        }

        public void ExposedCard(int i_RowIndex, int i_ColIndex)
        {
            m_GameBoard[i_RowIndex, i_ColIndex].isExposed = true;
            if (AfterExpose != null)
            {
                AfterExpose.Invoke((int)m_GameBoard[i_RowIndex, i_ColIndex].identiykey, i_RowIndex, i_ColIndex);
            }
        }

        public void HideCard(string i_IndexCard)
        {
            int RowIndex, ColIndex;
            ParseIndexCard(i_IndexCard, out RowIndex, out ColIndex);
            HideCard(RowIndex, ColIndex);
        }

        public void HideCard(int i_RowIndex, int i_ColIndex)
        {
            m_GameBoard[i_RowIndex, i_ColIndex].isExposed = false;
            if (AfterHide != null)
            {
                AfterHide.Invoke(i_RowIndex, i_ColIndex);
            }
        }

        public bool IsCardExposed(string i_IndexCard)
        {
            int idxRow, idxCol;
            Board.ParseIndexCard(i_IndexCard, out idxRow, out idxCol);
            return IsCardExposed(idxRow, idxCol);
        }

        public bool IsCardExposed(int i_RowIndex, int i_ColIndex)
        {
            return m_GameBoard[i_RowIndex, i_ColIndex].isExposed;
        }

        public static bool IsAMatchFound(Board i_Board, int i_IdxRowFirstCard, int i_IdxColFirstCard, int i_IdxRowSecondCard, int i_IdxColSecondCard)
        {
            Card firstCard = i_Board.gameBoard[i_IdxRowFirstCard, i_IdxColFirstCard];
            Card SecondCard = i_Board.gameBoard[i_IdxRowSecondCard, i_IdxColSecondCard];
            return Card.isTheCardEqual(firstCard, SecondCard);
        }
    }
}
