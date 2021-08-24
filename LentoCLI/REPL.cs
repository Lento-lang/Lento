﻿using System;
using System.Linq;
using LentoCore.Atoms;
using LentoCore.Exception;
using LentoCore.TypeChecker;
using LentoCore.Lexer;
using LentoCore.Parser;
using LentoCore.Util;
using LentoCore.Evaluator;
using Console = EzConsole.EzConsole;

namespace LentoCLI
{
    public static class REPL
    {
        public static void Run(bool verbose)
        {
            Tokenizer lex = new Tokenizer();
            Parser parser = new Parser();
            while (true)
            {
                Console.Write("LI> ");
                string expr = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(expr))
                {
                    try
                    {
                        TokenStream tokens = lex.Tokenize(expr);
                        if (verbose)
                        {
                            Console.Write("Tokens: ");
                            int tokenCount = tokens.Count();
                            foreach ((Token token, int i) in tokens.Select((value, index) => (value, index)))
                            {
                                Console.Write(token.ToString(), ConsoleColor.Yellow);
                                if (i < tokenCount - 1) Console.Write(", ");
                                else Console.WriteLine();
                            }
                        }

                        AST ast = parser.Parse(tokens);
                        if (verbose)
                        {
                            Console.WriteLine("Expressions: ");
                            Console.WriteLine(ast.ToString(), ConsoleColor.Magenta);
                        }

                        TypeChecker tc = new TypeChecker(ast);
                        tc.Run();

                        Atomic result = Evaluator.EvaluateExpression(ast);
                        if (verbose)
                        {
                            Console.Write("Result: ");
                            Console.WriteLine(result.ToString(), ConsoleColor.Cyan);
                        }
                        else
                        {
                            Console.WriteLine(result.ToString());
                        }
                    }
                    catch (SyntaxErrorException e)
                    {
                        Console.WriteLine(e.Message, ConsoleColor.Red);
                    }
                    catch (ParseErrorException e)
                    {
                        Console.WriteLine(e.Message, ConsoleColor.Red);
                    }
                    catch (EvaluateErrorException e)
                    {
                        Console.WriteLine(e.Message, ConsoleColor.Red);
                    }
                }
            }
        }
    }
}
