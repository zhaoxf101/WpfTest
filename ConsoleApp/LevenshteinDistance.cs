using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ConsoleApp1
{
    class LevenshteinDistance
    {
        public static int GetLevenshteinDistance(string string1, string string2)
        {
            int length1 = string1?.Length ?? 0;
            int length2 = string2?.Length ?? 0;
            if (length1 == 0)
            {
                return length2;
            }
            else if (length2 == 0)
            {
                return length1;
            }

            if (length1 > length2)
            {
                // swap the input strings to consume less memory
                var tmp = string1;
                string1 = string2;
                string2 = tmp;
                length1 = length2;
                length2 = string2.Length;
            }
            var previousArray = new int[length1 + 1]; //'previous' cost array, horizontally
            var currentArray = new int[length1 + 1]; // cost array, horizontally
            int[] temp; //placeholder to assist in swapping p and d
                        // indexes into strings s and t

            int i; // iterates through s
            int j; // iterates through t
            char ch; // jth character of t
            int cost; // cost
            for (i = 0; i <= length1; i++)
            {
                previousArray[i] = i;
            }

            for (j = 1; j <= length2; j++)
            {
                ch = string2[j - 1];
                currentArray[0] = j;

                for (i = 1; i <= length1; i++)
                {
                    cost = string1[i - 1] == ch ? 0 : 1;
                    // minimum of cell to the left+1, to the top+1, diagonally left and up +cost
                    currentArray[i] = Math.Min(Math.Min(currentArray[i - 1] + 1, previousArray[i] + 1), previousArray[i - 1] + cost);
                }
                // copy current distance counts to 'previous row' distance counts
                temp = previousArray;
                previousArray = currentArray;
                currentArray = temp;
            }
            // our last action in the above loop was to switch d and p, so p now
            // actually has the most recent cost counts
            return previousArray[length1];
        }

    }
}
