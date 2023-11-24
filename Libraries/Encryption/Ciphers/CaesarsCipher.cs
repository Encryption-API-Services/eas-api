using System;
using System.Collections.Generic;
using System.Text;

namespace Encryption.Ciphers
{
    public class CaesarsCipher
    {
        private List<string> alaphet = new List<string>() { "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };
        private int alaphetLength { get; set; }
        public CaesarsCipher()
        {
            this.alaphetLength = (alaphet.Count - 1);
        }

        public string Encrypt(string text, int shift)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                int newIndex = (alaphet.IndexOf(text[i].ToString()) + shift);
                // check if the string value is a alapher character in our above listed alaphet.
                if (alaphet.Contains(text[i].ToString()))
                {

                    if (newIndex > this.alaphetLength)
                    {
                        newIndex = (this.alaphetLength - newIndex + shift);
                    }
                    sb.Append(alaphet[newIndex]);
                }
                else
                {
                    sb.Append(text[i]);
                }
            }
            return sb.ToString();
        }

        public string Decrypt(string text, int shift)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < text.Length; i++)
            {
                int newIndex = (alaphet.IndexOf(text[i].ToString()) - shift);
                // check if the string value is a alapher character in our above listed alaphet.
                if (alaphet.Contains(text[i].ToString()))
                {

                    if (newIndex < 0)
                    {
                        newIndex = (this.alaphetLength - Math.Abs(newIndex) + shift);
                    }
                    sb.Append(alaphet[newIndex]);
                }
                else
                {
                    sb.Append(text[i]);
                }
            }
            return sb.ToString();
        }
    }
}