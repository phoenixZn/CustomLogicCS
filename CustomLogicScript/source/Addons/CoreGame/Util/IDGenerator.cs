namespace CoreGame
{
    public class IDGenerator : IDestruct
    {
        private int m_next_id = 0;
        private int m_first_id = 0;
        private int m_range = -1;

        public IDGenerator(int first_id = 0, int range = -1)
        {
            m_first_id = first_id;
            m_range = range;
            m_next_id = first_id;
        }

        public void Destruct()
        {
        }

        public void Reset()
        {
            m_next_id = 0;
            m_first_id = 0;
            m_range = -1;
        }

        public int GenID()
        {
            if (m_range > 0 && m_next_id >= m_first_id + m_range)
            {
                LogWrapper.LogError(string.Format("IDGenerator Out Range! FirstID = {0}, range = {1}", m_first_id, m_range));
            }

            return m_next_id++;
        }
    }
}