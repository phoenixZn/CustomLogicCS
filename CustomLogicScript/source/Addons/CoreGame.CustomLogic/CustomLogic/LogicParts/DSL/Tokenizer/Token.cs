using System.Collections;
using System.Collections.Generic;

namespace CoreGame.DSL
{
    public enum TokenType
    {
	    ERROR = 0,
        EOF,

        //Value
        NUMBER,

        //Symbol
        PLUS,
        MINUS,
        STAR,
        SLASH,
        LEFT_PAREN,
        RIGHT_PAREN,
        LEFT_BRACKET,
        RIGHT_BRACKET,
        PERIOD,
        COMMA,
        COLON,

        //Comparison
        GREATER_THAN,
        LESS_THAN,
        GREATER_EQUAL,
        LESS_EQUAL,
        EQUAL,
        NOT_EQUAL,

        //BooleanLogic
        NOT,
        AND,
        OR,

        //Assignment
        SET,

        // Word-------------------
        //Function
        SINE,
	    COSINE,
	    TANGENT,
	    SQUARE_ROOT,	    
	    MINIMUM,
	    MAXIMUM,
        CLAMP,
        //Variable
        IDENTIFIER,

        STRING_TYPE,
    }

    public abstract class Token
    {
        protected TokenType m_type;

        public TokenType Type
        {
            get { return m_type; }
        }

        public abstract bool FitType(TextBuffer text_buffer);

        public abstract bool Get(TextBuffer text_buffer);

        public virtual FixPoint GetNumber() 
        {
            return FixPoint.Zero;
        }

        public virtual string GetRawString()
        {
            return "";
        }
    }

    public class ErrorToken : Token
    {
        public ErrorToken()
        {
            m_type = TokenType.ERROR;
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