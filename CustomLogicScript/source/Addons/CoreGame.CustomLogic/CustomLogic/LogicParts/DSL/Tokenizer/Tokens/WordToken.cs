using System.Collections;
using System.Collections.Generic;

namespace CoreGame.DSL
{

    public class WordToken : Token
    {
        static Dictionary<string, TokenType> ms_reserved_words = new Dictionary<string, TokenType>();
        string m_raw_string;

        static WordToken()
        {
            ms_reserved_words["Sin"] = TokenType.SINE;
            ms_reserved_words["sin"] = TokenType.SINE;
            ms_reserved_words["Cos"] = TokenType.COSINE;
            ms_reserved_words["cos"] = TokenType.COSINE;
            ms_reserved_words["Tan"] = TokenType.TANGENT;
            ms_reserved_words["tan"] = TokenType.TANGENT;
            ms_reserved_words["Sqrt"] = TokenType.SQUARE_ROOT;
            ms_reserved_words["sqrt"] = TokenType.SQUARE_ROOT;
            ms_reserved_words["Min"] = TokenType.MINIMUM;
            ms_reserved_words["min"] = TokenType.MINIMUM;
            ms_reserved_words["Max"] = TokenType.MAXIMUM;
            ms_reserved_words["max"] = TokenType.MAXIMUM;
            ms_reserved_words["Clamp"] = TokenType.CLAMP;
            ms_reserved_words["clamp"] = TokenType.CLAMP;
        }

        public override bool FitType(TextBuffer text_buffer)
        {
            char code = Tokenizer.GetCode(text_buffer.Char());
            if (code == Tokenizer.Letter)
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
                char code = Tokenizer.GetCode(ch);
                if (code != Tokenizer.Letter && code != Tokenizer.Digit)
                {
                    break; 
                }
                ++length;
            }
            string str = text_buffer.SubString(start_index, length);
            if (!ms_reserved_words.TryGetValue(str, out m_type))
            {
                m_type = TokenType.IDENTIFIER;
                m_raw_string = str;
            }
            return true;
        }

        public override string GetRawString()
        {
            return m_raw_string;
        }
    }

}