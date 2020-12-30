using System;

namespace GameLogic
{
    internal struct Card
    {
        private int? m_Key;
        private bool m_IsExposed;

        internal Card(int i_Key)
        {
            m_Key = i_Key;
            m_IsExposed = false;
        }

        internal int? identiykey
        {
            get
            {
                return m_Key;
            }

            set
            {
                m_Key = value;
            }
        }

        internal bool isExposed
        {
            get
            {
                return m_IsExposed;
            }

            set
            {
                m_IsExposed = value;
            }
        }

        internal static bool isTheCardEqual(Card i_FirstCard, Card i_SecondCard)
        {
            return i_FirstCard.identiykey == i_SecondCard.identiykey;
        }
    }
}
