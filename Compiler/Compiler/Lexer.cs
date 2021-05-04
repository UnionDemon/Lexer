using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compiler
{
    enum TokenCode
    {
        tokenInt = 1,
        tokenBool = 2,
        tokenKeyChar = 3,
        tokenKeyString = 4,
        tokenId = 5, 
        tokenTrue = 6,
        tokenFalse = 7,
        tokenSpace = 8,
        tokenEqual = 9,
        tokenString = 10,
        tokenChar = 11,
        tokenUnsignedNumber = 12,
        tokenEndOp = 13,
        tokenEOF = 14,
        tokenError = 15
    }
    class Terminal
    {
        string str;
        int start_idx;
        int final_idx;
        TokenCode code;

        public Terminal(string str_, int start_idx_, int final_idx_, TokenCode code_)
        {
            str = str_;
            start_idx = start_idx_ + 1;
            final_idx = final_idx_ + 1;
            code = code_;
        }

        public string GetStr()
        {
            if (code == TokenCode.tokenEOF)
            {
                return "<EOF>";
            }
            else if (code == TokenCode.tokenSpace)
            {
                return "<Пробельный символ>";
            }
            return str;
        }

        public int GetStartIdx()
        {
            return start_idx;
        }

        public int GetFinalIdx()
        {
            return final_idx;
        }

        public int GetCode()
        {
            return (int)code;
        }
    }
    
    class Lexer
    {
        int state = 0;
        string input = "\0";
        int index = 0;
        string sub = "";
        int startIndex = 0;
        List<Terminal> terminals = new List<Terminal>();
        
        public List<Terminal> Find(string str)
        {
            str += "\0";

            input = str;
            state = 0;
            index = 0;

            while (index != str.Length)
            {
                switch(state)
                {
                    case 0: state0(); break;
                    case 1: state1(); break;
                    case 2: state2(); break;
                    case 3: state3(); break;
                    case 4: state4(); break;
                    case 5: state5(); break;
                    case 6: state6(); break;
                    case 7: state7(); break;
                    case 8: state8(); break;
                    case 9: state9(); break;
                    case 10: state10(); break;
                    case 11: state11(); break;
                    case 12: state12(); break;
                }
                index++;
            }
            return terminals;
        }

        private void state0()
        {
            char c = input[index];
            startIndex = index;
            if (isLetter(c))
            {
                state = 1;
                sub += c;
            }
            else if (c == ' ' || c == '\r' || c == '\n' || c == '\t')
            {
                state = 2;
                sub += c;
            }
            else if (c == '=')
            {
                state = 3;
                sub += c;
            }
            else if (c == '"')
            {
                state = 4;
                sub += c;
            }
            else if (c == '\'')
            {
                state = 6;
                sub += c;
            }
            else if (isDigit(c))
            {
                state = 11;
                sub += c;
            }
            else if (c == ';')
            {
                state = 12;
                sub += c;
            }
            else if (c == '\0')
            {
                sub += c;

                Terminal terminal = new Terminal(sub, startIndex, index, TokenCode.tokenEOF);
                state = 0;
                sub = "";
                startIndex = index;
                terminals.Add(terminal);
            }
            else
            {
                sub += c;
                Terminal terminal = new Terminal(sub, startIndex, index - 1, TokenCode.tokenError);
                sub = "";
                state = 0;
                startIndex = index;
                terminals.Add(terminal);
            }
        }

        private void state1()
        {
            char c = input[index];
            if (isLetter(c))
            {
                state = 1;
                sub += c;
                return;
            }

            if (sub.Equals("int"))
            {
                Terminal terminal = new Terminal(sub, startIndex, index - 1, TokenCode.tokenInt);
                state = 0;
                sub = "";
                startIndex = index;
                terminals.Add(terminal);
            }
            else if (sub.Equals("bool"))
            {
                Terminal terminal = new Terminal(sub, startIndex, index - 1, TokenCode.tokenBool);
                state = 0;
                sub = "";
                startIndex = index;
                terminals.Add(terminal);
            }
            else if (sub.Equals("char"))
            {
                Terminal terminal = new Terminal(sub, startIndex, index - 1, TokenCode.tokenKeyChar);
                state = 0;
                sub = "";
                startIndex = index;
                terminals.Add(terminal);
            }
            else if (sub.Equals("string"))
            {
                Terminal terminal = new Terminal(sub, startIndex, index - 1, TokenCode.tokenKeyString);
                state = 0;
                sub = "";
                startIndex = index;
                terminals.Add(terminal);
            }
            else if (sub.Equals("true"))
            {
                Terminal terminal = new Terminal(sub, startIndex, index - 1, TokenCode.tokenTrue);
                state = 0;
                sub = "";
                startIndex = index;
                terminals.Add(terminal);
            }
            else if (sub.Equals("false"))
            {
                Terminal terminal = new Terminal(sub, startIndex, index - 1, TokenCode.tokenFalse);
                state = 0;
                sub = "";
                startIndex = index;
                terminals.Add(terminal);
            }
            else
            {
                Terminal terminal = new Terminal(sub, startIndex, index - 1, TokenCode.tokenId);
                state = 0;
                sub = "";
                startIndex = index;
                terminals.Add(terminal);
            }
            index--;
        }
        private void state2()
        {
            Terminal terminal = new Terminal(sub, startIndex, index - 1, TokenCode.tokenSpace);
            state = 0;
            sub = "";
            startIndex = index;
            terminals.Add(terminal);
            index--;
        }

        private void state3()
        {
            Terminal terminal = new Terminal(sub, startIndex, index - 1, TokenCode.tokenEqual);
            state = 0;
            sub = "";
            startIndex = index;
            terminals.Add(terminal);
            index--;
        }

        private void state4()
        {
            char c = input[index];
            if (c == '"')
            {
                state = 5;
                sub += c;
            }
            else
            {
                state = 4;
                sub += c;
            }
        }

        private void state5()
        {
            Terminal terminal = new Terminal(sub, startIndex, index - 1, TokenCode.tokenString);
            sub = "";
            state = 0;
            startIndex = index;
            terminals.Add(terminal);
            index--;
        }

        private void state6()
        {
            char c = input[index];
            if (c == '\\')
            {
                state = 7;
                sub += c;
            }
            else if (c == '\'')
            {
                Terminal terminal = new Terminal(sub, startIndex, index - 1, TokenCode.tokenError);
                sub = "";
                state = 0;
                startIndex = index;
                terminals.Add(terminal);
            }
            else
            {
                state = 10;
                sub += c;
            }
        }

        private void state7()
        {
            char c = input[index];
            if (isLetter(c))
            {
                state = 8;
                sub += c;
            }
            else
            {
                Terminal terminal = new Terminal(sub, startIndex, index - 1, TokenCode.tokenError);
                sub = "";
                state = 0;
                startIndex = index;
                terminals.Add(terminal);
            }
        }

        private void state8()
        {
            char c = input[index];
            if (c == '\'')
            {
                state = 9;
                sub += c;
            }
            else
            {
                Terminal terminal = new Terminal(sub, startIndex, index - 1, TokenCode.tokenError);
                sub = "";
                state = 0;
                startIndex = index;
                terminals.Add(terminal);
            }
        }

        private void state9()
        {
            Terminal terminal = new Terminal(sub, startIndex, index - 1, TokenCode.tokenChar);
            sub = "";
            state = 0;
            startIndex = index;
            terminals.Add(terminal);
            index--;
        }

        private void state10()
        {
            char c = input[index];
            if (c == '\'')
            {
                state = 9;
                sub += c;
            }
            else
            {
                Terminal terminal = new Terminal(sub, startIndex, index - 1, TokenCode.tokenError);
                sub = "";
                state = 0;
                startIndex = index;
                terminals.Add(terminal);
            }
        }

        private void state11()
        {
            char c = input[index];
            if (isDigit(c))
            {
                state = 11;
                sub += c;
                return;
            }
            else
            {
                Terminal terminal = new Terminal(sub, startIndex, index - 1, TokenCode.tokenUnsignedNumber);
                sub = "";
                state = 0;
                startIndex = index;
                terminals.Add(terminal);
                index--;
            }
        }
        private void state12()
        {
            Terminal terminal = new Terminal(sub, startIndex, index - 1, TokenCode.tokenEndOp);
            sub = "";
            state = 0;
            startIndex = index;
            terminals.Add(terminal);
            index--;
        }

        private bool isLetter(char c)
        {
            return (c >= 'a' && c <= 'z') || (c >= 'A' && c <= 'Z');
        }

        private bool isDigit(char c)
        {
            return (c >= '0' && c <= '9');
        }
    }
}
