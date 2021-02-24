using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CloneACar.LogicalHelpers
{
    public class PermutationHelper
    {
        public List<string> FinalMessages;
        public byte[] NextBaseMessage;

        private List<string> FoundPerms;
        private string[] PermString;

        public PermutationHelper(string InputString, string MessageStart)
        {
            // Setup instance vars.
            PermString = InputString.Split(' ');

            // Assign instance vars values.
            FinalMessages = new List<string>();
            FoundPerms = new List<string>();

            // Run next perm setup here and add only string values not seen to list of all perms.
            var PermResults = PermString.GetPermutations().ToList();
            foreach (var PermArray in PermResults)
            {
                string FinalPermString = string.Join(" ", PermArray);
                if (!FoundPerms.Contains(FinalPermString))
                {
                    FoundPerms.Add(FinalPermString);
                    FinalMessages.Add(MessageStart + " " + FinalPermString);
                }
            }

            // Make final list dupe less and set final perm list.
            string NoSpacesPerm = FoundPerms[0].Replace(" ", String.Empty);
            NextBaseMessage = Enumerable.Range(0, NoSpacesPerm.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(NoSpacesPerm.Substring(x, 2), 16))
                .ToArray();
        }
    }

    /// <summary>
    /// Permutation extension class.
    /// </summary>
    internal static class SomeExtensions
    {
        public static IEnumerable<IEnumerable<T>> GetPermutations<T>(this IEnumerable<T> enumerable)
        {
            var array = enumerable as T[] ?? enumerable.ToArray();

            var factorials = Enumerable.Range(0, array.Length + 1)
                .Select(Factorial)
                .ToArray();

            for (var i = 0L; i < factorials[array.Length]; i++)
            {
                var sequence = GenerateSequence(i, array.Length - 1, factorials);

                yield return GeneratePermutation(array, sequence);
            }
        }
        private static IEnumerable<T> GeneratePermutation<T>(T[] array, IReadOnlyList<int> sequence)
        {
            var clone = (T[])array.Clone();

            for (int i = 0; i < clone.Length - 1; i++)
            {
                Swap(ref clone[i], ref clone[i + sequence[i]]);
            }

            return clone;
        }
        private static int[] GenerateSequence(long number, int size, IReadOnlyList<long> factorials)
        {
            var sequence = new int[size];

            for (var j = 0; j < sequence.Length; j++)
            {
                var facto = factorials[sequence.Length - j];

                sequence[j] = (int)(number / facto);
                number = (int)(number % facto);
            }

            return sequence;
        }
        static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }
        private static long Factorial(int n)
        {
            long result = n;

            for (int i = 1; i < n; i++)
            {
                result = result * i;
            }

            return result;
        }
    }

}
