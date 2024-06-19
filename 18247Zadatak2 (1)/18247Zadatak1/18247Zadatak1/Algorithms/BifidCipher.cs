using System.Globalization;
using System.Security.Cryptography.Xml;

namespace _18247Zadatak1.Algorithms
{
    public class BifidCipher
    {
        private readonly char[][] key;
        private int period;
        private readonly Dictionary<char, (int, int)> letterToPosition;
        private List<int> blankSpaces;
        public BifidCipher(char[][] key, int period)
        {
            this.key = key;
            this.period = period;
            //da sacuvam koordinate svakog slova u matrici
            this.letterToPosition = new Dictionary<char, (int, int)>();
            this.blankSpaces = new List<int>();
            int i = 0, j = 0;

            foreach (var row in key)
            {
                j = 0;
                foreach (var element in row)
                {
                    char letter = element;
                    if (!letterToPosition.ContainsKey(letter))
                    {
                        letterToPosition.Add(letter, (i, j));
                    }
                    j++;
                }
                i++;
            }
        }

        private List<int> FindBlankoSpaces(string text)
        {
            int count = 0;
            foreach(char letter in text)
            {
                if (!char.IsLetter(letter))
                {
                    blankSpaces.Add(count);
                    count = 0;
                }
                else count++;

            }
            return blankSpaces;
        }
        public (List<int>, string) Encrypt(string plainText)
        {
            List<int> blanko = FindBlankoSpaces(plainText);

            (string numbersRow, string numbersCol) = TextToNumbers(plainText);
            string encryptedNumbers = EncryptNumbers(numbersRow, numbersCol, period);
            return (blanko, NumbersToText(encryptedNumbers));
        }

        public string Decrypt(string cipherText, List<int> blanko)
        {
           
            string numbers = TextToNumbersForDecrypt(cipherText);
            (string row, string col) = SplitNumbers(numbers, period);
            string decryptedText = GetLettersFromNumbers(row, col);
            string text = ReturnTextWithBlanko(decryptedText, blanko);

            blankSpaces.Clear();
            blanko.Clear();
            return text;

        }

        public string ReturnTextWithBlanko(string text, List<int> blanko)
        {
            int offset = 0;
            foreach (int position in blanko)
            {
                int insertPosition = position + offset;
                if (insertPosition <= text.Length)
                {
                    text = text.Insert(insertPosition, " ");
                    offset += position + 1;
                }
            }
            return text;
        }

        private string TextToNumbersForDecrypt(string text)
        {
            text = text.ToLower().Replace("j", "i");

            string numbersRow = "";
            string numberCol = "";
            string numbers = "";
            foreach (var character in text)
                if (char.IsLetter(character))
                {
                    var (row, col) = letterToPosition[character];

                    numbersRow = $"{row + 1}";
                    numberCol = $"{col + 1}";
                    numbers += numbersRow + numberCol;

                }
            return numbers;
        }

        private (string, string) TextToNumbers(string text)
        {
            text = text.ToLower().Replace("j", "i");

            string numbersRow = "";
            string numberCol = "";
            foreach (var character in text)
                if (char.IsLetter(character))
                {
                    var (row, col) = letterToPosition[character];

                    numbersRow += $"{row + 1}";
                    numberCol += $"{col + 1}";
                    
                }
            return (numbersRow, numberCol);

        }

        private string NumbersToText(string numbers)
        {
            string text = "";
            for (int i = 0; i < numbers.Length; i += 2)
            {
                int row = int.Parse(numbers[i].ToString()) - 1;
                int col = int.Parse(numbers[i + 1].ToString()) - 1;
                char letter = key[row][col];
                text += letter;
            }
            return text;

        }

        private (string, string) SplitNumbers(string numbers, int segmentLength)
        {
            string row = "", col = "";

            int index = 0;
            while (index + segmentLength <= numbers.Length)
            {
                if (row.Length == col.Length && numbers.Length - index < segmentLength * 2)
                    break;
                string segment = numbers.Substring(index, segmentLength);
                

                if (index / segmentLength % 2 == 0)
                {
                    row += segment;
                }
                else
                {
                    col += segment;
                }

                index += segmentLength;
            }

           string remaining = numbers.Substring(index);
            int remainingLength = remaining.Length;

            string firstHalf = remaining.Substring(0, remainingLength / 2);
            string secondHalf = remaining.Substring(remainingLength / 2);

            row += firstHalf;
            col += secondHalf;

            return (row, col);
        }



        private string EncryptNumbers(string numbersRow, string numbersCol, int period)
        {
            string encryptedNumbers = "";
            string e1, e2;
            for (int i = 0; i < numbersRow.Length; i += period)
            {
                if (numbersRow.Length - i < period) period = numbersRow.Length - i;
                e1 = numbersRow.Substring(i, period);
                e2 = numbersCol.Substring(i, period);
                encryptedNumbers += e1 + e2;
            }
            return encryptedNumbers;
        }

        private string GetLettersFromNumbers(string row, string col)
        {
            string decryptedText = "";
            int rowLength = row.Length;
            int colLength = col.Length;

            int totalChunks = Math.Max(rowLength, colLength);

            for (int i = 0; i < totalChunks; i++)
            {
                int rowIndex = i % rowLength;
                int colIndex = i % colLength;

                int rowNumber = int.Parse(row[rowIndex].ToString()) - 1;
                int colNumber = int.Parse(col[colIndex].ToString()) - 1;

                decryptedText += key[rowNumber][colNumber];
            }

            return decryptedText;
        }


    }


}
