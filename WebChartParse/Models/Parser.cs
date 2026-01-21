using System;
using System.Globalization;

namespace WebChartParse.Models
{
    public class Parser
    {
        public double Parse(string expression)
        {
            if (string.IsNullOrWhiteSpace(expression))
            {
                return 0;
            }

            string exp = expression.ToLowerInvariant().Replace(',', '.');

            double result1 = 0;
            double result2 = 0;
            double result3 = 0;
            int prior = 0;
            char lastOperation1 = '\0';
            char lastOperation2 = '\0';
            char lastOperation3 = '\0';
            string subexp = string.Empty;
            string subfun = string.Empty;
            string bracketExp = string.Empty;

            int bracketCount = 0;
            bool isBracket = false;

            for (int i = 0; i < exp.Length; i++)
            {
                char current = exp[i];
                if (isBracket)
                {
                    if (current == '(')
                    {
                        bracketCount++;
                    }
                    else if (current == ')')
                    {
                        if (bracketCount > 0)
                        {
                            bracketCount--;
                        }
                        else
                        {
                            subexp = Parse(bracketExp).ToString(CultureInfo.InvariantCulture);
                            bracketExp = string.Empty;
                            isBracket = false;
                            if (!string.IsNullOrEmpty(subfun))
                            {
                                subexp = ResolveFunction(subfun, subexp).ToString(CultureInfo.InvariantCulture);
                            }
                            subfun = string.Empty;
                        }
                    }

                    if (isBracket)
                    {
                        bracketExp += current;
                    }

                    continue;
                }

                switch (current)
                {
                    case '+':
                    case '-':
                        if (!string.IsNullOrEmpty(subfun))
                        {
                            subexp = ResolveFunction(subfun).ToString(CultureInfo.InvariantCulture);
                        }
                        subfun = string.Empty;

                        if (prior == 1)
                        {
                            if (lastOperation1 == '+')
                            {
                                result1 += ParseNumber(subexp);
                            }
                            else if (lastOperation1 == '-')
                            {
                                result1 -= ParseNumber(subexp);
                            }
                        }
                        else if (prior == 2)
                        {
                            if (lastOperation2 == '*')
                            {
                                result1 *= ParseNumber(subexp);
                            }
                            else if (lastOperation2 == '/')
                            {
                                if (ParseNumber(subexp) == 0)
                                {
                                    return 0;
                                }

                                result1 /= ParseNumber(subexp);
                            }

                            if (lastOperation1 == '+')
                            {
                                result1 += result2;
                            }
                            else if (lastOperation1 == '-')
                            {
                                result2 -= result1;
                                result1 = result2;
                            }
                        }
                        else if (prior == 3)
                        {
                            if (lastOperation3 == '^')
                            {
                                result1 = Math.Pow(result1, ParseNumber(subexp));
                            }

                            if (lastOperation2 == '*')
                            {
                                result1 *= result2;
                            }
                            else if (lastOperation2 == '/')
                            {
                                result1 = result2 / result1;
                            }

                            if (lastOperation1 == '+')
                            {
                                result1 += result3;
                            }
                            else if (lastOperation1 == '-')
                            {
                                result1 = result3 - result1;
                            }

                            result3 = 0;
                            result2 = 0;
                        }
                        else
                        {
                            result1 = string.IsNullOrEmpty(subexp) ? 0 : ParseNumber(subexp);
                        }

                        subexp = string.Empty;
                        prior = 1;
                        lastOperation1 = current;
                        break;
                    case '*':
                    case '/':
                        if (!string.IsNullOrEmpty(subfun))
                        {
                            subexp = ResolveFunction(subfun).ToString(CultureInfo.InvariantCulture);
                        }
                        subfun = string.Empty;

                        if (prior == 1)
                        {
                            result2 = result1;
                            result1 = ParseNumber(subexp);
                        }
                        else if (prior == 2)
                        {
                            if (lastOperation2 == '*')
                            {
                                result1 *= ParseNumber(subexp);
                            }
                            else if (lastOperation2 == '/')
                            {
                                if (ParseNumber(subexp) == 0)
                                {
                                    return 0;
                                }

                                result1 /= ParseNumber(subexp);
                            }
                        }
                        else if (prior == 3)
                        {
                            if (lastOperation3 == '^')
                            {
                                result1 = Math.Pow(result1, ParseNumber(subexp));
                            }

                            if (lastOperation2 == '*')
                            {
                                result1 *= result2;
                            }
                            else if (lastOperation2 == '/')
                            {
                                if (result1 == 0)
                                {
                                    return 0;
                                }

                                result1 = result2 / result1;
                            }
                        }
                        else
                        {
                            result1 = ParseNumber(subexp);
                        }

                        prior = 2;
                        subexp = string.Empty;
                        lastOperation2 = current;
                        break;
                    case '^':
                        if (!string.IsNullOrEmpty(subfun))
                        {
                            subexp = ResolveFunction(subfun).ToString(CultureInfo.InvariantCulture);
                        }
                        subfun = string.Empty;

                        if (prior == 1)
                        {
                            result3 = result1;
                            result2 = result1;
                            result1 = ParseNumber(subexp);
                        }
                        else if (prior == 2)
                        {
                            result3 = result2;
                            result2 = result1;
                            result1 = ParseNumber(subexp);
                        }
                        else if (prior == 3)
                        {
                            if (lastOperation3 == '^')
                            {
                                result1 = Math.Pow(result1, ParseNumber(subexp));
                            }
                        }
                        else
                        {
                            result1 = ParseNumber(subexp);
                        }

                        prior = 3;
                        subexp = string.Empty;
                        lastOperation3 = '^';
                        break;
                    default:
                        if (char.IsDigit(current) || current == '.')
                        {
                            subexp += current;
                        }
                        else if (current >= 'a' && current <= 'z')
                        {
                            subfun += current;
                        }
                        else if (current == '(')
                        {
                            isBracket = true;
                        }

                        break;
                }
            }

            if (!string.IsNullOrEmpty(subfun))
            {
                subexp = ResolveFunction(subfun).ToString(CultureInfo.InvariantCulture);
            }
            subfun = string.Empty;

            if (prior == 1)
            {
                if (lastOperation1 == '+')
                {
                    result1 += ParseNumber(subexp);
                }
                else if (lastOperation1 == '-')
                {
                    result1 -= ParseNumber(subexp);
                }
            }
            else if (prior == 2)
            {
                if (lastOperation2 == '*')
                {
                    result1 *= ParseNumber(subexp);
                }
                else if (lastOperation2 == '/')
                {
                    if (ParseNumber(subexp) == 0)
                    {
                        return 0;
                    }

                    result1 /= ParseNumber(subexp);
                }

                if (lastOperation1 == '+')
                {
                    result1 += result2;
                }
                else if (lastOperation1 == '-')
                {
                    result2 -= result1;
                    result1 = result2;
                }
            }
            else if (prior == 3)
            {
                if (lastOperation3 == '^')
                {
                    result1 = Math.Pow(result1, ParseNumber(subexp));
                }

                if (lastOperation2 == '*')
                {
                    result1 *= result2;
                }
                else if (lastOperation2 == '/')
                {
                    if (result1 == 0)
                    {
                        return 0;
                    }

                    result1 = result2 / result1;
                }

                if (lastOperation1 == '+')
                {
                    result1 += result3;
                }
                else if (lastOperation1 == '-')
                {
                    result1 = result3 - result1;
                }
            }
            else
            {
                result1 = ParseNumber(subexp);
            }

            return result1;
        }

        private static double ResolveFunction(string name, string param = "")
        {
            switch (name)
            {
                case "pi":
                    return Math.PI;
                case "e":
                    return Math.E;
                case "abs":
                    return Math.Abs(ParseNumber(param));
                case "sqrt":
                    double sqrtValue = ParseNumber(param);
                    return sqrtValue < 0 ? 0 : Math.Sqrt(sqrtValue);
                case "sin":
                    return Math.Sin(ParseNumber(param));
                case "cos":
                    return Math.Cos(ParseNumber(param));
                case "tg":
                    return Math.Tan(ParseNumber(param));
                case "ctg":
                    double tanValue = Math.Tan(ParseNumber(param));
                    return tanValue == 0 ? 0 : 1 / tanValue;
                default:
                    return 0;
            }
        }

        private static double ParseNumber(string value)
        {
            return double.Parse(value, CultureInfo.InvariantCulture);
        }
    }
}
