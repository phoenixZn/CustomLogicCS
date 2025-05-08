using System.Collections;
using System.Collections.Generic;

namespace CoreGame.DSL
{


    public class NumberToken : Token
    {
        FixPoint m_value;

        public NumberToken()
        {
            m_type = TokenType.NUMBER;
        }

        public override bool FitType(TextBuffer text_buffer)
        {
            char code = Tokenizer.GetCode(text_buffer.Char());
            if (code == Tokenizer.Digit)
                return true;
            return false;
        }

        public override bool Get(TextBuffer text_buffer)
        {
            int start_index = text_buffer.CurrentIndex;
            int length = 1;
            while (true)
            {
                char ch = text_buffer.NextChar();
                if (ch == '.' || Tokenizer.GetCode(ch) == Tokenizer.Digit)
                    ++length;
                else
                    break;
            }
            string str = text_buffer.SubString(start_index, length);
            m_value = FixPoint.Parse(str);
            return true;
        }

        public override FixPoint GetNumber()
        {
            return m_value;
        }
    }

}