using System.Collections;
using System.Collections.Generic;

namespace CoreGame.DSL
{

    public class SymbolToken : Token
    {
        static Dictionary<string, TokenType> ms_reserved_symbol = new Dictionary<string, TokenType>();
        static SymbolToken()
        {
            //定义一些符号的别名 （例如xml中不能出现<&, 此时可以用别名）
            ms_reserved_symbol["#LT"] = TokenType.LESS_THAN;
            ms_reserved_symbol["#GT"] = TokenType.GREATER_THAN;
            ms_reserved_symbol["#LE"] = TokenType.LESS_EQUAL;
            ms_reserved_symbol["#GE"] = TokenType.GREATER_EQUAL;
            ms_reserved_symbol["#AND"] = TokenType.AND;
            ms_reserved_symbol["#And"] = TokenType.AND;
            ms_reserved_symbol["#and"] = TokenType.AND;
            ms_reserved_symbol["#OR"] = TokenType.OR;
            ms_reserved_symbol["#Or"] = TokenType.OR;
            ms_reserved_symbol["#or"] = TokenType.OR;
        }

        public override bool FitType(TextBuffer text_buffer)
        {
            char code = Tokenizer.GetCode(text_buffer.Char());
            if (code == Tokenizer.Symbol)
                return true;
            return false;
        }

        public override bool Get(TextBuffer text_buffer)
        {
            char ch = text_buffer.Char();
            switch (ch)
            {
                case '+':
                    m_type = TokenType.PLUS;
                    text_buffer.NextChar();
                    break;
                case '-':
                    m_type = TokenType.MINUS;
                    text_buffer.NextChar();
                    break;
                case '*':
                    m_type = TokenType.STAR;
                    text_buffer.NextChar();
                    break;
                case '/':
                    m_type = TokenType.SLASH;
                    text_buffer.NextChar();
                    break;
                case '(':
                    m_type = TokenType.LEFT_PAREN;
                    text_buffer.NextChar();
                    break;
                case ')':
                    m_type = TokenType.RIGHT_PAREN;
                    text_buffer.NextChar();
                    break;
                case '[':
                    m_type = TokenType.LEFT_BRACKET;
                    text_buffer.NextChar();
                    break;
                case ']':
                    m_type = TokenType.RIGHT_BRACKET;
                    text_buffer.NextChar();
                    break;
                case '.':
                    m_type = TokenType.PERIOD;
                    text_buffer.NextChar();
                    break;
                case ',':
                    m_type = TokenType.COMMA;
                    text_buffer.NextChar();
                    break;
                case ':':
                    m_type = TokenType.COLON;
                    text_buffer.NextChar();
                    break;
                case '=':
                    {
                        m_type = TokenType.SET;
                        char nextch = text_buffer.NextChar();
                        if (nextch == '=')
                        {
                            m_type = TokenType.EQUAL;
                            text_buffer.NextChar();
                        }
                        break;
                    }
                case '!':
                    {
                        m_type = TokenType.NOT;
                        char nextch = text_buffer.NextChar();
                        if (nextch == '=')
                        {
                            m_type = TokenType.NOT_EQUAL;
                            text_buffer.NextChar();
                        }
                        break;
                    }
                case '<':
                    {
                        m_type = TokenType.LESS_THAN;
                        char nextch = text_buffer.NextChar();
                        if (nextch == '=')
                        {
                            m_type = TokenType.LESS_EQUAL;
                            text_buffer.NextChar();
                        }
                        break;
                    }
                case '>':
                    {
                        m_type = TokenType.GREATER_THAN;
                        char nextch = text_buffer.NextChar();
                        if (nextch == '=')
                        {
                            m_type = TokenType.GREATER_EQUAL;
                            text_buffer.NextChar();
                        }
                        break;
                    }
                case '&':
                    {
                        m_type = TokenType.AND;
                        text_buffer.NextChar();
                        break;
                    }
                case '|':
                    {
                        m_type = TokenType.OR;
                        text_buffer.NextChar();
                        break;
                    }
                case '#':
                    {
                        int start_index = text_buffer.CurrentIndex;
                        int length = 1;
                        char nch = text_buffer.NextChar();
                        char code = Tokenizer.GetCode(nch);
                        while (code != Tokenizer.WhiteSpace)
                        {
                            nch = text_buffer.NextChar();
                            code = Tokenizer.GetCode(nch);
                            ++length;
                        }
                        string jstr = text_buffer.SubString(start_index, length);
                        if (!ms_reserved_symbol.TryGetValue(jstr, out m_type))
                        {
                            DSLHelper.LogError("Expression: SymbolToken.Get() # Error!");
                            m_type = TokenType.ERROR;
                        }
                    }
                    break;
                default:
                    DSLHelper.LogError("Expression: SymbolToken.Get(), illegal symbol, index = ", text_buffer.CurrentIndex, ", char = ", text_buffer.Char());
                    m_type = TokenType.ERROR;
                    return false;
            }
            return true;
        }
    }

}