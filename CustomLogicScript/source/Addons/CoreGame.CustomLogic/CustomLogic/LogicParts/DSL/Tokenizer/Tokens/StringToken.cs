using System.Collections;
using System.Collections.Generic;

namespace CoreGame.DSL
{
    public class StringToken : Token
    {
        string m_raw_string;

        public StringToken()
        {
            m_type = TokenType.STRING_TYPE;
        }

        public override bool FitType(TextBuffer text_buffer)
        {
            char code = Tokenizer.GetCode(text_buffer.Char());
            if (code == Tokenizer.Quote)
                return true;
            return false;
        }

        public override bool Get(TextBuffer text_buffer)
        {
            int start_index = text_buffer.CurrentIndex + 1;
            int length = 0;
            while (true)
            {
                char ch = text_buffer.NextChar();
                if (text_buffer.Eof())
                    break;
                if (Tokenizer.GetCode(ch) == Tokenizer.Quote)
                    break;
                ++length;
            }
            m_raw_string = text_buffer.SubString(start_index, length);
            return true;
        }

        public override string GetRawString()
        {
            return m_raw_string;
        }
    }

}