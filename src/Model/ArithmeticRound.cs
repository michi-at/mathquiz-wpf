namespace MathQuizWPF
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;

    public enum DifficultyLevel
    {
        Easy,
        MediumTwo,
        MediumMinus,
        Medium,
        MediumPlus,
        Hard,
        Insane
    }

    public class ArithmeticRound : GameRound
    {
        private const int maxNumOfDigits = 2;
        private static readonly int[] power10 = new int[] {
           10, 100, 100, 100, 100, 1000, 1000
        };
        private Random r;
        private bool? afterAttempt;
                
        private List<double> correctAnswers;
        private Dictionary<string, Func<double, double>> unaryFunctions;
        private Dictionary<string, Func<double, double, double>> binaryFunctions;

        public ArithmeticRound()
        {
            unaryFunctions = new Dictionary<string, Func<double, double>>
            {
                {"Square", a => a * a },
                {"Sqrt", a => Math.Sqrt(a) }
            };
            binaryFunctions = new Dictionary<string, Func<double, double, double>>
            {
                {"+", (a, b) => a + b },
                {"-", (a, b) => a - b },
                {"*", (a, b) => a * b },
                {"/", (a, b) => a / b }
            };
            r = new Random();
        }

        public override void Setup(DifficultyLevel mode)
        {
            correctAnswers = new List<double>();
            afterAttempt = null;
            Question = "";

            double evaluationResult = 0;
            int numOfArguments = 2;
            switch (mode)
            {
                case DifficultyLevel.Easy:
                    {
                        numOfArguments = r.Next(1, 4); break;
                    }
                case DifficultyLevel.MediumTwo:
                    {
                        numOfArguments = 2; break;
                    }
                case DifficultyLevel.MediumMinus:
                    {
                        numOfArguments = r.Next(2, 4); break;
                    }
                case DifficultyLevel.Medium:
                    {
                        numOfArguments = r.Next(1, 5); break;
                    }
                case DifficultyLevel.MediumPlus:
                    {
                        numOfArguments = r.Next(1, 5); break;
                    }
                case DifficultyLevel.Hard:
                    {
                        numOfArguments = r.Next(2, 4); break;
                    }
                case DifficultyLevel.Insane:
                    {
                        numOfArguments = r.Next(1, 6); break;
                    }
            }
            if (numOfArguments == 1)
            {
                double argument = RandomArgument(mode);
                string selectedOperator = unaryFunctions.Keys.ElementAt(r.Next(unaryFunctions.Count));
                Question += $"{selectedOperator} of {argument} = ?";
                evaluationResult = unaryFunctions[selectedOperator](argument);
            }
            else
            {
                for (int i = 1; i <= numOfArguments; ++i)
                {
                    double arg = RandomArgument(mode);
                    string selectedOperator = binaryFunctions.Keys.ElementAt(r.Next(binaryFunctions.Count));
                    if (i < numOfArguments)
                        Question += $"{arg} {selectedOperator} ";
                    else
                        Question += $"{arg} = ?";
                }
                evaluationResult = EvaluateQuestion(Question);
            }
            AddCorrAnswer(evaluationResult);
        }

        public override string GetQuestion()
        {
            return Question;
        }

        public override string GetAnswer()
        {
            if (afterAttempt == null) return "Try to answer first!";
            else if (afterAttempt == true) return $"Correct answer: {Answer}";
            else return "Hey! I said that it was correct one!";

        }

        public override bool? CheckAnswer(string input)
        {
            bool isParsed = double.TryParse(input, out double inputVal);
            if (isParsed)
            {
                bool result = correctAnswers.Contains(inputVal);
                afterAttempt = (!result) ? true : false;
                return result;
            }
            else return null;
        }

        private double EvaluateQuestion(string question)
        {
            string expr = question.Substring(0, question.Length - 4);
            List<string> operators = new List<string> { "(", ")" };
            operators.AddRange(binaryFunctions.Keys);
            //string opStr = "\\" + string.Join("\\", operators);
            //expr = Regex.Replace(expr, $"([{opStr}])(\\d)|(\\d)([{opStr}])",
            //    match => (match.Groups[1].Success && match.Groups[2].Success) ? $"{match.Groups[1].Value} {match.Groups[2].Value}" :
            //                                                                    $"{match.Groups[3].Value} {match.Groups[4].Value}"
            //    );
            expr = Regex.Replace(expr, @"\s", "");
            return DijkstraAlgo(expr, operators);
        }

        private double DijkstraAlgo(string expr, IEnumerable<string> operators)
        {
            Dictionary<string, int> precedence = new Dictionary<string, int>();
            int opLen = operators.Count();
            for (int i = 1; i < opLen; i += 2)
            {
                precedence.Add(operators.ToArray()[i], i / 2);
                precedence.Add(operators.ToArray()[i - 1], i / 2);
            }

            Stack<string> ops = new Stack<string>();
            Stack<double> vals = new Stack<double>();
            MatchCollection matches = Regex.Matches(expr, @"[\(\)\+\-\*\/]|\d+[.]\d+|\d+");
            bool isUnaryMinus = false;
            for (int i = 0; i < matches.Count; ++i)
            {
                string s = matches[i].Value;
                if (s.Equals("-") && (i == 0 || precedence.ContainsKey(matches[i - 1].Value))) isUnaryMinus = true;

                // token is a value
                if (!precedence.ContainsKey(s))
                {
                    vals.Push(double.Parse(s));
                    continue;
                }

                // token is an operator
                while (true)
                {
                    // the last condition ensures that the operator with higher precedence is evaluated first
                    if (ops.Count == 0 || s.Equals("(") || precedence[s] > precedence[ops.Peek()])
                    {
                        ops.Push(s);
                        break;
                    }

                    // evaluate expression
                    string op = ops.Pop();

                    // but ignore left parentheses
                    if (op.Equals("("))
                    {
                        break;
                    }

                    // evaluate operator and two operands and push result onto value stack
                    else
                    {
                        if (isUnaryMinus)
                        {
                            double val = vals.Pop();
                            vals.Push(-val);
                            isUnaryMinus = false;
                        }
                        else
                        {
                            double val2 = vals.Pop();
                            double val1 = vals.Pop();
                            vals.Push(binaryFunctions[op](val1, val2));
                        }
                    }
                }
            }

            // finished parsing string - evaluate operator and operands remaining on two stacks
            while (ops.Count > 0)
            {
                string op = ops.Pop();
                double val2 = vals.Pop();
                double val1 = vals.Pop();
                vals.Push(binaryFunctions[op](val1, val2));
            }

            // last value on stack is value of expression
            return vals.Pop();
        }

        private double RandomArgument(DifficultyLevel mode)
        {
            switch (mode)
            {
                case DifficultyLevel.MediumPlus: return Math.Round(r.Next(power10[(int)mode]) + r.NextDouble(), maxNumOfDigits - 1);
                case DifficultyLevel.Insane: return Math.Round(r.Next(power10[(int)mode]) + r.NextDouble(), maxNumOfDigits);
                default: return r.Next(power10[(int)mode]);
            }
        }

        private void AddCorrAnswer(double corAns)
        {
            double fract = corAns - Math.Floor(corAns);
            if (fract != 0)
                for (int i = 0; i <= maxNumOfDigits; ++i)
                    correctAnswers.Add(Math.Round(corAns, i));
            else
                correctAnswers.Add(corAns);
            Answer = String.Join<double>(" or ", correctAnswers);
        }
    }
}
