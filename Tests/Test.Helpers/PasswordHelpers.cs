using System;
using System.Text;

namespace Tests.Helpers
{
    public class PasswordHelpers
    {
        private string[] alaphet { get; set; }
        private int[] numbers { get; set; }
        private string[] specialCharacters { get; set; }
        public PasswordHelpers()
        {
            this.alaphet = new string[] { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
            this.numbers = new int[] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            this.specialCharacters = new string[] { "!", "@", "#", "$", "%", "^", "&", "*", "(", ")" };
        }

        public string GetRandomPassword()
        {
            StringBuilder sb = new StringBuilder();
            Random rnd = new Random();
            for (int i = 0; i < 12; i++)
            {
                // Letters
                if (i < 8)
                {
                    if (i == 0)
                    {
                        string capitalLetter = this.alaphet[rnd.Next(this.alaphet.Length - 1)].ToUpper();
                        sb.Append(capitalLetter);
                    }
                    string letter = this.alaphet[rnd.Next(this.alaphet.Length - 1)];
                    sb.Append(letter);
                }

                if (i >= 8 && i < 10)
                {
                    string number = this.numbers[rnd.Next(this.numbers.Length - 1)].ToString();
                    sb.Append(number);
                }

                if (i >= 10)
                {
                    string specialCharacter = this.specialCharacters[rnd.Next(this.specialCharacters.Length - 1)];
                    sb.Append(specialCharacter);
                }
            }
            return sb.ToString();
        }
    }
}
