using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace LeastCommonMultiple
{
    class LeastCommonMultiple
    {

        static string CreateCorrectExpression(string expression)
        {
            if (expression.Length == 0)
            {
                throw new FormatException();
            }
            StringBuilder exp = new StringBuilder(expression.Length);
            for (int i = 1; i < expression.Length; i++)
            {
                int index = i;
                char symbol = expression[index - 1];
                char newSymbol = expression[index];
                if (newSymbol == ' ')
                {
                    continue;
                }
                else if (symbol == ' ')
                {
                    index--;
                    symbol = expression[index - 1];
                }

                if (Char.IsDigit(symbol))
                {
                    if (newSymbol == '/' || Char.IsDigit(newSymbol))
                    {
                        exp.Append(symbol);
                    }
                    else if (newSymbol == '+' || newSymbol == '-')
                    {
                        if (index == 1)
                        {
                            exp.Append(symbol);
                            exp.Append('/');
                            exp.Append('1');
                        }
                        int j = 1;
                        while (Char.IsDigit(expression[index - j]) && i > 1)
                        {
                            j++;
                            char oldSymbol = expression[index - j];
                            if (oldSymbol == '/')
                            {
                                exp.Append(symbol);
                                break;
                            }
                            else if (oldSymbol == '+' || oldSymbol == '-'
                                    || index - j < 1)
                            {
                                exp.Append(symbol);
                                exp.Append('/');
                                exp.Append('1');
                                break;
                            }
                        }
                    }
                    else
                    {
                        throw new IndexOutOfRangeException();
                    }
                }
                else if (symbol == '/' || symbol == '+' || symbol == '-')
                {
                    if (Char.IsDigit(newSymbol))
                    {
                        exp.Append(symbol);
                    }
                    else
                    {
                        throw new IndexOutOfRangeException();
                    }
                }
                else
                {
                    throw new IndexOutOfRangeException();
                }
                if (i == expression.Length - 1 && symbol == '/')
                {
                    exp.Append(newSymbol);
                }
                else if (i == expression.Length - 1 && (symbol == '+' || symbol == '-' || Char.IsDigit(symbol)))
                {
                    exp.Append(newSymbol);
                    exp.Append('/');
                    exp.Append('1');
                }
            }

            return exp.ToString();
        }

        static string[] ExtractNumbers(string expression)
        {
            string[] splitResult = expression.Split('+', '-');

            return splitResult;
        }

        static List<string> ExtractOperators(string expression)
        {
            List<string> operatorsOnly = new List<string>();
            string[] operators = Regex.Split(expression, @"[0-9/]+");
            foreach (var item in operators)
            {
                if (item != "")
                {
                    operatorsOnly.Add(item);
                }
            }
            return operatorsOnly;
        }

        static int[] ExtractNominators(string[] numbers)
        {
            int[] nominators = new int[numbers.Length];
            for (int i = 0; i < numbers.Length; i++)
            {
                string[] splitNumbers = numbers[i].Split('/');
                int currentNumber = int.Parse(splitNumbers[0]);
                nominators[i] = currentNumber;
            }
            return nominators;
        }

        static int[] ExtractDenominators(string[] numbers)
        {
            int[] denominators = new int[numbers.Length];
            for (int i = 0; i < numbers.Length; i++)
            {
                string[] splitNumbers = numbers[i].Split('/');
                int currentNumber = int.Parse(splitNumbers[1]);
                denominators[i] = currentNumber;
                if (denominators[i] == 0)
                {
                    throw new DivideByZeroException();
                }
            }
            return denominators;
        }

        public static int GetLCM(int a, int b) // Използваме алгоритъм за намиране на най-малко общо кратно на 2 числа
        {
            int temp;
            while (b != 0)
            {
                temp = a % b;
                a = b;
                b = temp;
            }
            return a;
        }

        static int FindLCM(int[] denominators)
        {
            int leastMultiple = denominators[0];
            for (int i = 1; i < denominators.Length; i++)
            {
                int nextDivider = denominators[i];
                leastMultiple = (leastMultiple * nextDivider)
                        / (GetLCM(leastMultiple, nextDivider));
            }
            return leastMultiple;
        }

        static int[] FindNomitaros(int leastMultiple, int[] nominators,
                int[] denominators)
        {
            int ratio;
            for (int i = 0; i < nominators.Length; i++)
            {

                ratio = leastMultiple / denominators[i];
                nominators[i] *= ratio;
            }
            return nominators;
        }

        static int CalculateNominators(int[] nominators, List<string> operators)
        {
            int result = nominators[0];

            for (int i = 1; i < nominators.Length; i++)
            {
                string nextOperator = operators[i - 1];
                int nextNumber = nominators[i];

                if (nextOperator.Equals("+"))
                {
                    result += nextNumber;
                }
                else if (nextOperator.Equals("-"))
                {
                    result -= nextNumber;
                }
            }
            return result;
        }

        static List<int> GetPrimeNumbers(int number) // Използвам решетка на Ератостен за намиране на всички прости числа до даденото число
        {
            List<int> primeNumbers = new List<int>();
            bool[] isPrime = new bool[Math.Abs(number) + 1];
            for (int i = 2; i < isPrime.Length; i++)
            {
                isPrime[i] = true;
            }
            for (int i = 2; i < isPrime.Length; i++)
            {
                if (isPrime[i] == true)
                {
                    for (int j = i * i; j < isPrime.Length; j += i)
                    {
                        isPrime[j] = false;
                    }
                }
            }
            for (int i = 2; i < isPrime.Length; i++)
            {
                if (isPrime[i] == true) primeNumbers.Add(i);
            }

            return primeNumbers;
        }

        static string CreateReduction(int nominator, int denominator)
        {            
            int absNominator = Math.Abs(nominator);
            int min = Math.Min(absNominator, denominator);
            List<int> primeNumbers = GetPrimeNumbers(min); // намираме всички прости числа съдържащи се в даденото число
            foreach (int prime in primeNumbers)
            {
                while (nominator % prime == 0 && denominator % prime == 0)
                {
                    nominator /= prime;
                    denominator /= prime;
                }
            }
            return nominator + "/" + denominator;
        }


        static void Main(string[] args)
        {
            try
            {
                Console.Write("Input expression of fractions: ");
                string expression = Console.ReadLine();
                if (expression.Length == 1)
                {
                    Console.WriteLine(expression);
                }
                else
                {
                    string newExpression = CreateCorrectExpression(expression); // проверка за коректност за израза
                    string[] numbers = ExtractNumbers(newExpression); // Отделяме дробните числа
                    List<string> operators = ExtractOperators(newExpression); // Отделяме операторите                    
                    string[] numbersReduce = new string[numbers.Length];
                    for (int i = 0; i < numbers.Length; i++)
                    {
                        string[] splitNumber = numbers[i].Split('/');
                        numbersReduce[i] = CreateReduction(int.Parse(splitNumber[0]), int.Parse(splitNumber[1])); // Извършваме съкращение на дробите ако е възможно
                    }
                    int[] nominators = ExtractNominators(numbers); // Отделяме числителите 
                    int[] denominators = ExtractDenominators(numbers); // Отделяме знаменателите
                    int leastMultiple = FindLCM(denominators); // Намираме най-малко общо кратно
                    int[] newNominators = FindNomitaros(leastMultiple, nominators, denominators); // Намираме новата стойност на чеслителите
                    int nominator = CalculateNominators(newNominators, operators); // Изчисляваме сумата на чеслителите
                    string result = CreateReduction(nominator, leastMultiple); // Извършваме съкращеине на резултата ако е възможно
                    string[] splitResult = result.Split('/');
                    if (int.Parse(splitResult[1]) == 1)
                    {
                        result = splitResult[0];
                    }
                    Console.WriteLine(result);
                }
            }
            catch (IndexOutOfRangeException)
            {
                Console.WriteLine("Wrong expresion!");
            }
            catch (FormatException)
            {
                Console.WriteLine("You don't input expression!");
            }
            catch (DivideByZeroException)
            {
                Console.WriteLine("Can't input zero in denominators!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }
}
