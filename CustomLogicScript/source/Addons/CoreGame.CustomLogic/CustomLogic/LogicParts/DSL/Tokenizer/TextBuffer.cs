﻿using System.Collections;
using System.Collections.Generic;
namespace CoreGame.DSL
{
    public class TextBuffer : IRecyclable
    {
        string m_buffer;
        int m_index = -1;

        public void Construct(string buffer)
        {
            m_buffer = buffer;
            m_index = 0;
        }

        public void Reset()
        {
            m_buffer = null;
            m_index = -1;
        }

        public int CurrentIndex
        {
            get { return m_index; }
        }

        public bool Eof()
        {
            return m_index >= m_buffer.Length || m_buffer[m_index] == 0;
        }

        public char Char() 
	    { 
		    if (Eof())
			    return '\0';
            return m_buffer[m_index]; 
	    }

        public char NextChar()
        {
            ++m_index;
		    if (Eof())
                return '\0';
            return m_buffer[m_index];
	    }

        public string SubString(int start_index, int length)
        {
            return m_buffer.Substring(start_index, length);
        }

        public bool StartsWith(string str)
        {
            return m_buffer.StartsWith(str);
        }

    }
}