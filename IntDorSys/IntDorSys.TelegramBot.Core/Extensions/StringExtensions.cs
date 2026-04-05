using System.Text;

namespace IntDorSys.TelegramBot.Core.Extensions
{
    /// <summary>
    /// Методы расширения для <see cref="string"/>
    /// </summary>
    public static class StringExtensions
    {
        public static string ToTelegramLiteral(this string input)
        {
            var escapingCharacter = new char[]
            {
                '_',
                '*',
                '[',
                ']',
                '(',
                ')',
                '~',
                '`',
                '>',
                '#',
                '+',
                '-',
                '=',
                '|',
                '{',
                '}',
                '.',
                '!',
            };
            var literal = new StringBuilder(input.Length + 2);
            var previousCh = '\0';
            foreach (var c in input)
            {
                if (escapingCharacter.Contains(c) && previousCh != '\\')
                {
                    literal.Append("\\");
                }

                literal.Append(c);
                previousCh = c;
            }

            return literal.ToString();
        }
    }
}