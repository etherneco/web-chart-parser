using System;
using System.Collections.Generic;
using System.Text;

namespace ConsoleApp4
{
    public static class Program
    {
        public static string GetWordFromText(string input, int wordNumberToFind)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            if (wordNumberToFind < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(wordNumberToFind));
            }

            List<string> words = new List<string>();
            StringBuilder current = new StringBuilder();

            for (int i = 0; i < input.Length; i++)
            {
                char c = input[i];
                if (char.IsLetter(c))
                {
                    current.Append(c);
                }
                else if (current.Length > 0)
                {
                    words.Add(current.ToString());
                    current.Clear();
                }
            }

            if (current.Length > 0)
            {
                words.Add(current.ToString());
            }

            if (words.Count < wordNumberToFind)
            {
                throw new ArgumentOutOfRangeException(nameof(wordNumberToFind));
            }

            return words[wordNumberToFind - 1];
        }

        public static string Reverse(string input)
        {
            if (input == null)
            {
                throw new ArgumentNullException(nameof(input));
            }

            char[] charArray = input.ToCharArray();
            Array.Reverse(charArray);
            return new string(charArray);
        }

        public static void Main(string[] args)
        {
            Console.WriteLine(GetWordFromText("one two three", 2));
            Console.ReadLine();
        }
    }
}
