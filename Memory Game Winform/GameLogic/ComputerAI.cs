using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GameLogic
{
    public class ComputerAI
    {
        private readonly int r_MaxKeyToRemember;
        private readonly Dictionary<int, MemoryCell> r_Memory;

        public ComputerAI(int i_NumKeysToRemember)
        {
            r_MaxKeyToRemember = i_NumKeysToRemember;
            r_Memory = new Dictionary<int, MemoryCell>();
        }

        public void ChooseCardsComputer(Board i_Board, out string o_IndexOfFirstCard, out string o_IndexOfSecondCard)
        {
            string firstIndex = null, secondIndex = null;
            bool isAPairFound = searchForAnUnexposedMatchInMemory(ref firstIndex, ref secondIndex, i_Board);
            int RowIndex, ColIndex;
            if (!isAPairFound)
            {
                firstIndex = chooseARandomIndex(i_Board, out RowIndex, out ColIndex);
                int cardKey = i_Board.gameBoard[RowIndex, ColIndex].identiykey.Value;
                AddIndexToMemory(cardKey, firstIndex);
                isAPairFound = searchForIdentityCard(cardKey, firstIndex, ref secondIndex);
            }

            if (!isAPairFound)
            {
                do
                {
                    secondIndex = chooseARandomIndex(i_Board, out RowIndex, out ColIndex);
                }
                while (secondIndex == firstIndex);

                AddIndexToMemory(i_Board.gameBoard[RowIndex, ColIndex].identiykey.Value, secondIndex);
            }

            o_IndexOfFirstCard = firstIndex;
            o_IndexOfSecondCard = secondIndex;
        }

        public void AddIndexToMemory(int i_Key, string i_IndexOfCardInBoard)
        {
            if (!r_Memory.ContainsKey(i_Key))
            {
                if (r_Memory.Count == r_MaxKeyToRemember)
                {
                    r_Memory.Remove(r_Memory.Keys.First());
                }

                MemoryCell newCell = new MemoryCell(i_Key);
                newCell.firstCardIndex = i_IndexOfCardInBoard;
                r_Memory.Add(i_Key, newCell);
            }
            else if (r_Memory[i_Key].firstCardIndex != i_IndexOfCardInBoard)
            {
                MemoryCell updateCell = r_Memory[i_Key];
                updateCell.secondCardIndex = i_IndexOfCardInBoard;
                r_Memory[i_Key] = updateCell;
            }
        }

        private string chooseARandomIndex(Board i_Board, out int o_RowIndex, out int o_ColIndex)
        {
            Random rnd = new Random();
            do
            {
                o_RowIndex = rnd.Next(i_Board.Row);
                o_ColIndex = rnd.Next(i_Board.Col);
            }
            while (i_Board.IsCardExposed(o_RowIndex, o_ColIndex));

            return Board.ParseIndexCard(o_RowIndex, o_ColIndex);
        }

        private bool searchForIdentityCard(int i_Key, string i_CardIndex, ref string io_IdentitiyCardIndex)
        {
            bool res = false;
            if (r_Memory.ContainsKey(i_Key))
            {
                MemoryCell cellToCheck = r_Memory[i_Key];
                if (cellToCheck.firstCardIndex == i_CardIndex && cellToCheck.secondCardIndex != null)
                {
                    io_IdentitiyCardIndex = cellToCheck.secondCardIndex;
                    res = true;
                }
                else if (cellToCheck.secondCardIndex == i_CardIndex && cellToCheck.firstCardIndex != null)
                {
                    io_IdentitiyCardIndex = cellToCheck.firstCardIndex;
                    res = true;
                }
                else
                {
                    io_IdentitiyCardIndex = null;
                }
            }

            return res;
        }

        private bool searchForAnUnexposedMatchInMemory(ref string io_FirstCardIndex, ref string io_SecondCardIndex, Board i_Board)
        {
            bool res = false;
            Dictionary<int, MemoryCell>.ValueCollection memoryCells = r_Memory.Values;
            foreach (MemoryCell memoryToSeach in memoryCells)
            {
                if (memoryToSeach.firstCardIndex != null & memoryToSeach.secondCardIndex != null)
                {
                    if (!i_Board.IsCardExposed(memoryToSeach.firstCardIndex) && !i_Board.IsCardExposed(memoryToSeach.secondCardIndex))
                    {
                        io_FirstCardIndex = memoryToSeach.firstCardIndex;
                        io_SecondCardIndex = memoryToSeach.secondCardIndex;
                        res = true;
                        break;
                    }
                }
            }

            return res;
        }
    }
}
