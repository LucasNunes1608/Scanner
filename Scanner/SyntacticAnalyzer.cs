using System;
using System.IO;

namespace Scanner
{
    public class SyntacticAnalyzer
    {
        static string[] mLexemas;
        static int i;
        static Fluxo mFluxo = Fluxo.undef;

        enum Fluxo
        {
            undef = -1,
            Declaracao = 0,
            Atribuicao = 1,
            ExpBool = 2,
            ExpAlg = 3,
            If = 4,
            Switch = 5,
            Iteracao = 6,
            Case = 7,
            For = 8,
            While = 9,
            Printf = 10
        }

        public static bool AnalyzeOutput()
        {
            try
            {   // Open the text file using a stream reader.
                using (StreamReader sr = new StreamReader("output.luc"))
                {
                    // Read the stream to a string, and write the string to the console.
                    String line = sr.ReadToEnd();

                    mLexemas = line.Split("\n\r\n");
                }

                for (i = 0; i < mLexemas.Length - 1; i++)
                {
                    //Se checkSyntax retornar falso, indicar onde houve o erro;
                    if (!checkSyntax(Fluxo.undef))
                    {
                        Console.WriteLine($"Erro no token {getLexemaValue(i)},{i}\n");
                        return false;
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        private static string getLexemaName(int mIndex)
        {
            return mLexemas[mIndex].Split(',')[1];
        }

        private static string getLexemaValue(int mIndex)
        {
            return mLexemas[mIndex].Split(',')[0];
        }

        private static bool checkSyntax(Fluxo fluxo)
        {
            switch (fluxo)
            {
                case Fluxo.undef:
                    switch (Enum.Parse(typeof(Program.lexema), getLexemaName(i)))
                    {
                        case Program.lexema.mIf:
                            i++;
                            return checkSyntax(Fluxo.If);
                        case Program.lexema.id:
                            i++;
                            switch (Enum.Parse(typeof(Program.lexema), getLexemaName(i)))
                            {
                                case Program.lexema.mOpAlgebrico:
                                    i++;
                                    return checkSyntax(Fluxo.ExpAlg);
                                case Program.lexema.mAtribuicao:
                                    i++;
                                    return checkSyntax(Fluxo.Atribuicao);
                                default:
                                    i++;
                                    return checkSyntax(Fluxo.ExpBool);
                            }
                        case Program.lexema.mInt:
                            i++;
                            return checkSyntax(Fluxo.Declaracao);
                        case Program.lexema.mString:
                            i++;
                            return checkSyntax(Fluxo.Declaracao);
                        case Program.lexema.mSwitch:
                            i++;
                            return checkSyntax(Fluxo.Switch);
                        case Program.lexema.mFor:
                            i++;
                            return checkSyntax(Fluxo.For);
                        case Program.lexema.mWhile:
                            i++;
                            return checkSyntax(Fluxo.While);
                        case Program.lexema.mPrintf:
                            i++;
                            return checkSyntax(Fluxo.Printf);
                        default:
                            return false;
                    }
                case Fluxo.Declaracao:
                    if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.id)
                    {
                        i++;
                        if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.PontoVigula)
                        {
                            return true;
                        }
                        else if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.mAtribuicao)
                        {
                            i++;
                            if (checkSyntax(Fluxo.ExpAlg))
                            {
                                i++;
                                if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.PontoVigula)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                    break;
                case Fluxo.Atribuicao:
                    if (checkSyntax(Fluxo.ExpAlg) || checkSyntax(Fluxo.ExpBool))
                    {
                        i++;
                        if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.PontoVigula)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    else
                    {
                        return false;
                    }
                case Fluxo.ExpBool:
                    if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.id)
                    {
                        i++;
                        if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.opRel)
                        {
                            i++;
                            if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.valor || (Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.id || (Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.mString)
                            {
                                i++;
                                return true;
                            }
                        }
                    }
                    else if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.valor || (Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.id || (Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.mString)
                    {
                        i++;
                        return true;
                    }
                    return false;
                case Fluxo.ExpAlg:
                    if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.id || (Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.valor)
                    {
                        i++;
                        if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.mOpAlgebrico)
                        {
                            i++;
                            return checkSyntax(Fluxo.ExpAlg);
                        }
                        else
                        {
                            i--;
                            return true;
                        }
                    }
                    return false;
                case Fluxo.If:
                    if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.lParen)
                    {
                        i++;
                        checkSyntax(Fluxo.ExpBool);

                        if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.rParen)
                        {
                            i++;
                            if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.lChave)
                            {
                                i++;
                                checkSyntax(Fluxo.undef);
                                if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.rChave)
                                {
                                    i++;
                                    if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.mElse)
                                    {
                                        i++;
                                        if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.lChave)
                                        {
                                            i++;
                                            checkSyntax(Fluxo.undef);
                                            if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.rChave)
                                            {
                                                return true;
                                            }
                                        }
                                        else if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.mIf)
                                        {
                                            i++;
                                            checkSyntax(Fluxo.If);
                                        }
                                        else
                                        {
                                            return false;
                                        }
                                    }
                                    else
                                    {
                                        i--;
                                        return true;
                                    }
                                }
                            }
                        }
                    }
                    break;
                case Fluxo.Switch:
                    if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.lParen)
                    {
                        i++;
                        if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.id)
                        {
                            i++;
                            if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.rParen)
                            {
                                i++;
                                if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.lChave)
                                {
                                    i++;
                                    if (checkSyntax(Fluxo.Case))
                                    {
                                        i++;
                                        if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.rChave)
                                        {
                                            return true;
                                        }
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                }
                            }
                        }
                    }
                    return false;
                case Fluxo.Case:
                    if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.mCase)
                    {
                        i++;
                        if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.id || (Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.valor)
                        {
                            i++;
                            if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.DoisPontos)
                            {
                                i++;
                                if (checkSyntax(Fluxo.undef))
                                {
                                    i++;
                                    if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.mBreak)
                                    {
                                        i++;
                                        if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.PontoVigula)
                                        {

                                            if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i + 1)) == Program.lexema.mCase)
                                            {
                                                i++;
                                                return checkSyntax(Fluxo.Case);
                                            }
                                            else
                                            {
                                                return true;
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    return false;
                                }
                            }
                        }
                    }
                    break;
                case Fluxo.Iteracao:
                    if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.mFor)
                    {
                        return checkSyntax(Fluxo.For);
                    }
                    else if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.mWhile)
                    {
                        return checkSyntax(Fluxo.While);
                    }
                    else
                    {
                        return false;
                    }
                case Fluxo.For:
                    if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.lParen)
                    {
                        i++;
                        if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.mInt || (Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.id)
                        {
                            if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.mInt)
                            {
                                i++;
                                if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) != Program.lexema.id)
                                {
                                    return false;
                                }
                            }

                            i++;
                            if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.mAtribuicao)
                            {
                                i++;
                                if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.id || (Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.valor)
                                {
                                    i++;
                                    if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.PontoVigula)
                                    {
                                        i++;
                                        if (checkSyntax(Fluxo.ExpBool))
                                        {
                                            if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.PontoVigula)
                                            {
                                                i++;
                                                if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.id)
                                                {
                                                    i++;
                                                    if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.mAtribuicao)
                                                    {
                                                        i++;
                                                        if (checkSyntax(Fluxo.ExpAlg))
                                                        {
                                                            i++;
                                                            if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.rParen)
                                                            {
                                                                i++;
                                                                if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.lChave)
                                                                {
                                                                    i++;
                                                                    checkSyntax(Fluxo.undef);

                                                                    i++;
                                                                    if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.rChave)
                                                                    {
                                                                        return true;
                                                                    }
                                                                }
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                    }
                    break;
                case Fluxo.While:
                    if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.lParen)
                    {
                        i++;
                        if (checkSyntax(Fluxo.ExpBool))
                        {
                            if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.rParen)
                            {
                                i++;
                                if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.lChave)
                                {
                                    i++;
                                    if (checkSyntax(Fluxo.undef))
                                    {
                                        i++;
                                        if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.rChave)
                                        {
                                            return true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    break;
                case Fluxo.Printf:
                    if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.lParen)
                    {
                        i++;
                        if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.mString)
                        {
                            i++;
                            if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.rParen)
                            {
                                i++;
                                if ((Program.lexema)Enum.Parse(typeof(Program.lexema), getLexemaName(i)) == Program.lexema.PontoVigula)
                                {
                                    i++;
                                    return true;
                                }
                            }
                        }
                    }
                    break;
                default:
                    return false;
            }
            return false;
        }
    }
}
