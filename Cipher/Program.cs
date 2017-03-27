using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using NetSpell.SpellChecker.Dictionary;

namespace Cipher
{
    class Program
    {
        const int CHAR_OFFSET = 97;
        public static void Main(string[] args)
        {
            Console.WriteLine("Welcome to Web Security Basics - Lab 10");
            bool exit = false;
            do
            {
                Console.WriteLine("MENU:");
                Console.WriteLine("1 - Encrypt and then Decrypt a message using a Ceasar Shift");
                Console.WriteLine("2 - Decrypt a message (without providing a key!)");
                Console.WriteLine("3 - Exit");
                int menuChoice;
                while (int.TryParse(Console.ReadLine(), out menuChoice) && (menuChoice < 1 || menuChoice > 3))
                {
                    Console.WriteLine("Please select either '1' or '2'");
                }
                if (menuChoice == 1)
                {
                    //Get message from user
                    Console.WriteLine("Enter a shift key for Caesar Cipher");
                    Console.WriteLine("(it should be integer and 0 - 25)");
                    int shiftKey;
                    while (int.TryParse(Console.ReadLine(), out shiftKey) && (shiftKey < 0 || shiftKey > 25))
                    {
                        Console.WriteLine("Please enter an integer from 0 to 25.");
                    }
                    Console.WriteLine("Please enter a message (letters only)");
                    string message = Console.ReadLine();
                    while (!Regex.IsMatch(message, @"^\s?([a-zA-Z]\s?){1,}$"))
                    {
                        Console.WriteLine("Message can be letters and single spaces only");
                        message = Console.ReadLine();
                    }
                    //break message into words
                    message = message.Trim();
                    var words = message.ToLower().Split(' ');
                    string cipherText = string.Empty;
                    //apply shift to message
                    foreach (var word in words)
                        cipherText += CreateCipherText(word, shiftKey) + " ";

                    cipherText = cipherText.Trim();
                    string recoveredText = string.Empty;

                    var cipherTextWords = cipherText.Split(' ');
                    foreach (var word in cipherTextWords)
                        recoveredText += DecryptCiperText(word, shiftKey) + " ";

                    // display cipher text then perform decryption
                    Console.WriteLine("The cipher text for message:'" + message + "' is: " + cipherText);

                    Console.WriteLine("The recovered plain text is: " + recoveredText.Trim());
                    Console.WriteLine("Press any key to continue");

                    Console.ReadKey();
                }
                else if (menuChoice == 2)
                {
                    Console.WriteLine("Enter a message(letters and space only)");
                    Console.WriteLine("Will list all possible matches (in order of likelihood)");

                    // fun time!
                    List<Tuple<string, int>> allMatchingPossibilities = new List<Tuple<string, int>>();

                    string cipherText = Console.ReadLine();
                    while (!Regex.IsMatch(cipherText, @"^\s?([a-zA-Z]\s?){1,}$"))
                    {
                        Console.WriteLine("Message can be letters and single spaces only");
                        cipherText = Console.ReadLine();
                    }
                    var words = cipherText.ToLower().Split(' ');
                    WordDictionary dict = new WordDictionary();
                    dict.DictionaryFile = "en-US.dic";
                    dict.Initialize();
                    NetSpell.SpellChecker.Spelling oSpell = new NetSpell.SpellChecker.Spelling();

                    oSpell.Dictionary = dict;

                    for (int key = 0; key < 26; key++)
                    {
                        int matchCount = 0;
                        string result = string.Empty;
                        foreach (var word in words)
                        {
                            var currentWord = DecryptCiperText(word, key);
                            result += currentWord + " ";
                            if (oSpell.TestWord(currentWord))
                            {
                                matchCount++;
                            }
                        }
                        if (matchCount > 0)
                        {
                            allMatchingPossibilities.Add(Tuple.Create(result.Trim(), key));
                        }
                    }
                    // sorting in descending order in terms of matches
                    allMatchingPossibilities.OrderByDescending(match => match.Item2);
                    bool first = true;
                    if (allMatchingPossibilities.Count > 0)
                        allMatchingPossibilities.ForEach(match =>
                           {
                               if (first)
                               {
                                   first = false;
                                   Console.WriteLine("There was/were " + allMatchingPossibilities.Count + " match(es).");
                                   Console.WriteLine("The most likely recovered text is: " + match.Item1);
                                   Console.WriteLine("The most likely key is: " + match.Item2);
                               }
                               else
                               {
                                   Console.WriteLine("Result: " + match.Item1 + " --- Key: " + match.Item2);

                               }
                           });
                    else
                        Console.WriteLine("No matches :(");

                    Console.WriteLine("Press any key to continue");
                    Console.ReadKey();
                    //Determine how many keys that have at least 

                }
                else
                    exit = true;
            } while (!exit);
        }
        public static string CreateCipherText(string input, int key)
        {
            string result = string.Empty;
            foreach (var letter in input)
            {
                var x = Convert.ToInt32(letter) - CHAR_OFFSET;
                var y = (x + key) % 26;
                result += Convert.ToChar(y + CHAR_OFFSET);
            }
            return result;
        }
        public static string DecryptCiperText(string input, int key)
        {
            string result = string.Empty;
            foreach (var letter in input)
            {
                var x = Convert.ToInt32(letter) - CHAR_OFFSET - key;
                if (x < 0)
                    x = 26 + x;
                var y = x % 26;
                result += Convert.ToChar(y + CHAR_OFFSET);
            }
            return result;
        }
    }
}
