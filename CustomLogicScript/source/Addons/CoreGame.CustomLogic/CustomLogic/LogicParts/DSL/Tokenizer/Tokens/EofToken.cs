using System.Collections;
using System.Collections.Generic;

namespace CoreGame.DSL
{


    public class EofToken : Token
    {
        public EofToken()
        {
            m_type = TokenType.EOF;
        }
        public override bool FitType(TextBuffer text_buffer)
        {
            return true;
        }
        public override bool Get(TextBuffer text_buffer)
        {
            return true;
        }
    }

  
}