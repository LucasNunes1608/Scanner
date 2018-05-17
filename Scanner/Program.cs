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
            opRel = 3, // ==|>=|<=|!=
            comment = 4, // // :p
            lChave = 5, //{
            rChave = 6, //}
            id = 7 //Qualquer caractere que inicia com letra
        }


        static void Main(string[] args)
        {


            if (args.Length <= 0)
            {
                Console.WriteLine("Os Argumentos são Scanner -[Arquivo] \n ex: Scanner -file");
            }
            else
            {
                string filePath = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location) + "\\" + args[0] + ".luc";
                string token = string.Empty;
                try
                {
                    StreamReader sr = new StreamReader(filePath);
                    if (new FileInfo(filePath).Length != 0)
                    {
                        while (!sr.EndOfStream)
                        {
                            lexema mlex = lexema.undef;
                            mlex = CheckChar(ref sr, ref token);                            

                            Console.Write($"{token},{mlex.ToString()}\n");
                        }
                    }
                    else
                    {
                        Console.WriteLine("O Arquivo está vazio.");
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
        }

        static lexema CheckChar(ref StreamReader sr, ref string token)
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
            else if (ch == 'i' && (char)sr.Peek() == 'f')
            {
                ch = (char)sr.Read();
                token += ch.ToString();

                if (String.Equals(token.ToLower(), "if"))
                    mlex = lexema.mIf;
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
            else if (char.IsLetter(ch))
            {
                while (char.IsLetterOrDigit((char)sr.Peek()))
                {
                    ch = (char)sr.Read();
                    token += ch.ToString();
                }
                mlex = lexema.id;
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
    }
}
