using System;
using System.Collections.Generic;
using System.Linq;

namespace SudokuSolver
{
    public static class ArrayExtensions
    {
        public static int[] To1DArray(this int[,] src)
        {
            var arr = new int[src.Length];
            Buffer.BlockCopy(src, 0, arr, 0, arr.Length * 4);
            return arr;
        }

        public static T[,] To2DSquareArray<T>(this IEnumerable<T> src)
        {
            var list = src.ToList();
            var size = (int)Math.Sqrt(list.Count);
            if ((size * size) != list.Count) throw new ArgumentException("Invalid length. Should be a perfect square.", nameof(src));

            var arr = new T[size, size];
            for (int x = 0; x < size; x++)
                for (int y = 0; y < size; y++)
                    arr[x, y] = list[(x * size) + y];

            return arr;
        }
    }
}
