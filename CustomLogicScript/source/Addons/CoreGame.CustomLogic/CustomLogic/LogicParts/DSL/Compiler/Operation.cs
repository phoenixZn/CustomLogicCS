using System.Collections.Generic;

namespace CoreGame.DSL
{

    internal enum OperationCode
    {
        END = 0,
        NEGATE,
        ADD,
        SUBTRACT,
        MULTIPLY,
        DIVIDE,

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

        SIN,
        COS,
        TAN,
        SQRT,
        MIN,
        MAX,
        CLAMP,

        PUSH_NUMBER,
        PUSH_VARIABLE,
        SET_VARIABLE,
    };

    public class OperationSetting
    {
        static Dictionary<int, int> TokenTypeToPriority = new Dictionary<int, int>();
        static Dictionary<int, OperationCode> TokenTypeToOperation = new Dictionary<int, OperationCode>();


        internal static int MaxBinaryPriority = 0;
        internal static int MinBinaryPriority = 0;

        public static bool isMaxBinaryPriority(int priority)
        {
            return priority >= MaxBinaryPriority;
        }

        static OperationSetting()
        {
            MaxBinaryPriority = 3;
            MinBinaryPriority = 0;

            AddBinaryOPToken(TokenType.STAR, 3, OperationCode.MULTIPLY);
            AddBinaryOPToken(TokenType.SLASH, 3, OperationCode.DIVIDE);

            AddBinaryOPToken(TokenType.PLUS, 2, OperationCode.ADD);
            AddBinaryOPToken(TokenType.MINUS, 2, OperationCode.SUBTRACT);

            AddBinaryOPToken(TokenType.GREATER_THAN, 1, OperationCode.GREATER_THAN);
            AddBinaryOPToken(TokenType.LESS_THAN, 1, OperationCode.LESS_THAN);
            AddBinaryOPToken(TokenType.GREATER_EQUAL, 1, OperationCode.GREATER_EQUAL);
            AddBinaryOPToken(TokenType.LESS_EQUAL, 1, OperationCode.LESS_EQUAL);
            AddBinaryOPToken(TokenType.EQUAL, 1, OperationCode.EQUAL);
            AddBinaryOPToken(TokenType.NOT_EQUAL, 1, OperationCode.NOT_EQUAL);
            AddBinaryOPToken(TokenType.AND, 1, OperationCode.AND);
            AddBinaryOPToken(TokenType.OR, 1, OperationCode.OR);

            AddBinaryOPToken(TokenType.SET, 0, OperationCode.SET);
        }

        static void AddBinaryOPToken(TokenType token, int priority, OperationCode op)
        {
            TokenTypeToPriority.Add((int)token, priority);
            TokenTypeToOperation.Add((int)token, op);
        }

        internal static int GetBinaryOPTokenPriority(TokenType token)
        {
            if (TokenTypeToPriority.ContainsKey((int)token))
                return TokenTypeToPriority[(int)token];
            return -1;
        }

        internal static OperationCode GetBinaryOperationCode(TokenType token)
        {
            if (TokenTypeToOperation.ContainsKey((int)token))
                return TokenTypeToOperation[(int)token];
            return OperationCode.END;
        }
    }

}
