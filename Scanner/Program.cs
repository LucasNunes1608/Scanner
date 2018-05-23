using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace Scanner
{
    class Program
    {
        enum lexema
        {
            undef = -1,
            mIf = 0, //if
            lParen = 1, //(
            rParen = 2, //)
            opRel = 3, // ==|>=|<=|=!
            comment = 4, // // :p
            lChave = 5, //{
            rChave = 6, //}
            id = 7, //Qualquer caractere que inicia com letra
            mWhile = 8, // while
            PontoVigula = 9, // ;
            mInt = 10, // int
            mElse = 11, // else
            mSwitch = 12, // Switch
            mCase = 13, // Case
            DoisPontos = 14, // :
            mBreak = 15, // break
            mFor = 16, // for
            valor = 17, // numericos
            mAtribuicao = 18, // =
            mOpAlgebrico = 19, // + - * /
            mString = 20 // "((a-z)*(0-9)*)*"
        }

        static void Main(string[] args)
        {


            if (args.Length <= 0)
            {
                Console.WriteLine("Os Argumentos são Scanner [Arquivo] \n ex: Scanner file");
            }
            else
            {
                string filePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\" + args[0] + ".luc";
                string outputPath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\output.luc";
                string token = string.Empty;
                try
                {
                    using (StreamReader sr = new StreamReader(filePath))
                    {
                        if (new FileInfo(filePath).Length != 0)
                        {
                            using (StreamWriter fw = new StreamWriter(outputPath))
                            {
                                while (!sr.EndOfStream)
                                {
                                    lexema mlex = lexema.undef;
                                    mlex = CheckChar(sr, ref token);

                                    fw.WriteLine($"{token},{mlex.ToString()}\n");
                                }

                                Console.WriteLine("Output criado.");

                            }
                        }
                        else
                        {
                            Console.WriteLine("O Arquivo está vazio.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }

        static lexema CheckChar(StreamReader sr, ref string token)
        {
            lexema mlex;
            //Caracteres indesejados
            List<char> charLixos = new List<char>();
            charLixos.Add('\n');
            charLixos.Add('\r');
            charLixos.Add('\0');
            charLixos.Add('\f');
            charLixos.Add('\t');

            char ch = (char)sr.Read();
            mlex = lexema.undef;

            //Evita characteres indesejados como fim de linha e etc
            while (charLixos.Contains(ch) || char.IsWhiteSpace(ch))
            {
                ch = (char)sr.Read();
            }

            token = ch.ToString();

            //Checks para validar qual lexema os chars se encaixam
            if (ch == '/' && (char)sr.Peek() == '/')
            {
                ch = (char)sr.Read();
                token += ch.ToString();
                mlex = lexema.comment;
                //Pula pra proxima linha
                sr.ReadLine();
            }
            else if (ch == ';')
            {
                mlex = lexema.PontoVigula;
            }
            else if (ch == ':')
            {
                mlex = lexema.DoisPontos;
            }
            else if (ch == '(')
            {
                mlex = lexema.lParen;
            }
            else if (ch == ')')
            {
                mlex = lexema.rParen;
            }
            else if (ch == '{')
            {
                mlex = lexema.lChave;
            }
            else if (ch == '}')
            {
                mlex = lexema.rChave;
            }
            else if ((ch == '=' && ((char)sr.Peek() == '!' || (char)sr.Peek() == '=')) || ((ch == '>' || ch == '<') && (char)sr.Peek() == '=') || (ch == '!' && (char)sr.Peek() == '='))
            {
                ch = (char)sr.Read();
                token += ch.ToString();
                mlex = lexema.opRel;
            }
            else if (ch == '=')
            {
                mlex = lexema.mAtribuicao;
            }
            else if (ch == '+' || ch == '-' || ch == '*' || ch == '/')
            {
                mlex = lexema.mOpAlgebrico;
            }
            else if (ch == '"')
            {
                do
                {
                    ch = (char)sr.Read();
                    token += ch.ToString();
                } while (ch != '"');

                mlex = lexema.mString;
            }
            else if (char.IsLetter(ch))
            {
                while (char.IsLetterOrDigit((char)sr.Peek()))
                {
                    ch = (char)sr.Read();
                    token += ch.ToString();
                }

                mlex = FindLex(token);
            }
            else if (char.IsDigit(ch))
            {
                mlex = lexema.valor;

                while (!char.IsWhiteSpace((char)sr.Peek()))
                {
                    if (char.IsLetter((char)sr.Peek()))
                    {
                        mlex = lexema.undef;
                    }
                    else if (!char.IsNumber((char)sr.Peek()))
                    {
                        break;
                    }

                    ch = (char)sr.Read();
                    token += ch.ToString();
                }
            }
            else
            {
                while (char.IsLetterOrDigit((char)sr.Peek()))
                {
                    ch = (char)sr.Read();
                    token += ch.ToString();
                }
            }

            return mlex;
            //Console.Write($"{token},{mlex.ToString()}\n");

        }

        private static lexema FindLex(string token)
        {
            switch (token)
            {
                case "while":
                    return lexema.mWhile;
                case "if":
                    return lexema.mIf;
                case "int":
                    return lexema.mInt;
                case "else":
                    return lexema.mElse;
                case "switch":
                    return lexema.mSwitch;
                case "case":
                    return lexema.mCase;
                case "break":
                    return lexema.mBreak;
                case "for":
                    return lexema.mFor;
                default:
                    return lexema.id;
            }

        }
    }
}
