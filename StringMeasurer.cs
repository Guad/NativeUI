using System.Collections.Generic;
using GTA;

namespace NativeUI
{
    public static class StringMeasurer
    {
        private static readonly Dictionary<char, int> CharMap = new Dictionary<char, int>
        {
            {' ', 6},
            {'!', 6},
            {'"', 6},
            {'#', 11},
            {'$', 10},
            {'%', 17},
            {'&', 13},
            {'\'', 4},
            {'(', 6}, // ?
            {')', 6}, // ?
            {'*', 7}, // ?
            {'+', 10},
            {',', 4},
            {'-', 6},
            {'.', 4},
            {'/', 7},
            {'0', 12},
            {'1', 7},
            {'2', 11},
            {'3', 11},
            {'4', 11},
            {'5', 11},
            {'6', 12},
            {'7', 10}, // ?
            {'8', 11},
            {'9', 11},
            {':', 5},
            {';', 4},
            {'<', 9},
            {'=', 9},
            {'>', 9},
            {'?', 10},
            {'@', 15},
            {'A', 12},
            {'B', 13}, // 12?
            {'C', 14},
            {'D', 14},
            {'E', 12},
            {'F', 12},
            {'G', 15},
            {'H', 14},
            {'I', 5},
            {'J', 11},
            {'K', 13},
            {'L', 11},
            {'M', 16},
            {'N', 14},
            {'O', 16},
            {'P', 12},
            {'Q', 15},
            {'R', 13},
            {'S', 12},
            {'T', 11},
            {'U', 13},
            {'V', 12},
            {'W', 18},
            {'X', 11},
            {'Y', 11},
            {'Z', 12},
            {'[', 6},
            {'\\', 7},
            {']', 6}, // 5?
            {'^', 9},
            {'_', 18},
            {'`', 8},
            {'a', 11},
            {'b', 12},
            {'c', 11},
            {'d', 12},
            {'e', 12},
            {'f', 5}, // 6? 7?
            {'g',13},
            {'h', 11},
            {'i', 4},
            {'j', 4}, // 5?
            {'k', 10},
            {'l', 4},
            {'m', 18},
            {'n', 11},
            {'o', 12},
            {'p', 12},
            {'q', 12},
            {'r', 7},
            {'s', 9}, // ?
            {'t', 5},
            {'u', 11},
            {'v', 10},
            {'w', 14},
            {'x', 9}, // ?
            {'y', 10},
            {'z', 9},
            {'{', 6},
            {'|', 3},
            {'}', 6},
        };

        /// <summary>
        /// Measures width of a 0.35 scale string.
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static int MeasureString(string input)
        {
            int output = 0;
            for (int i = 0; i < input.Length; i++)
            {
                if (!CharMap.ContainsKey(input[i])) continue;
                output += CharMap[input[i]] + 1;
            }
            return output;
        }

        public static float MeasureString(string input, Font font, float scale)
        {
            return UIResText.MeasureStringWidth(input, font, scale);
        }
    }
}