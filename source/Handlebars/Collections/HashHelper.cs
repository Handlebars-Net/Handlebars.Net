namespace HandlebarsDotNet.Collections
{
    internal static class HashHelper
    {
        // must never be written to
        internal static readonly int[] SizeOneIntArray = new int[1];
        
        public static readonly int[] Primes = {
            2, 3, 5, 7, 11, 13, 17, 19, 23, 29, 31, 37, 41, 43, 47, 53, 59, 61, 67, 71, 73, 79, 83, 89, 97, 101, 103,
            107, 109, 113, 127, 131, 137, 139, 149, 151, 157, 163, 167, 173, 179, 181, 191, 193, 197, 199
        };
        
        public static int FindClosestPrime(int bucketSize)
        {
            for (int i = 0; i < Primes.Length; i++)
            {
                int prime = Primes[i];
                if (prime >= bucketSize) return prime;
            }

            return Primes[Primes.Length - 1];
        }
        
        public static int AlignBy2(int size)
        {
            size--;
            size |= size >> 1;
            size |= size >> 2;
            size |= size >> 4;
            size |= size >> 8;
            size |= size >> 16;
            size++;

            return size;
        }
        
        public static int PowerOf2(int v)
        {
            if ((v & (v - 1)) == 0) return v;
            int i = 2;
            while (i < v) i <<= 1;
            return i;
        }
    }
}