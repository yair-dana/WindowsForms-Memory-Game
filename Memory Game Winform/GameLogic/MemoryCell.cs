namespace GameLogic
{
    internal struct MemoryCell
    {
        private int m_Key;
        private string m_FirstCardIndex;
        private string m_SecondCardIndex;

        internal MemoryCell(int i_Key)
        {
            m_Key = i_Key;
            m_FirstCardIndex = null;
            m_SecondCardIndex = null;
        }

        internal int key
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

        internal string firstCardIndex
        {
            get
            {
                return m_FirstCardIndex;
            }

            set
            {
                m_FirstCardIndex = value;
            }
        }

        internal string secondCardIndex
        {
            get
            {
                return m_SecondCardIndex;
            }

            set
            {
                m_SecondCardIndex = value;
            }
        }
    }
}
