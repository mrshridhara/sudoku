using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SudokuSolver
{
    public sealed class Puzzle
    {
        public static int[] SupportedSizes { get; } = Enum.GetValues(typeof(Sizes)).Cast<int>().ToArray();

        public static int[,] Solve(int[,] grid)
        {
            var size = (int)Math.Sqrt(grid.Length);
            if ((size * size) != grid.Length)
                throw new ArgumentException("Invalid puzzle length. Should be a perfect square.", nameof(grid));

            if (!Enum.IsDefined(typeof(Sizes), size))
                throw new ArgumentException("Unsupported puzzle size.", nameof(grid));

            var subGridSize = s_subGridSize[(Sizes)size];
            var puzzle = new Puzzle(size, subGridSize.Item1, subGridSize.Item2, grid);

            if (puzzle.Solve())
                return puzzle._cells.Select(c => c.Value).To2DSquareArray();

            throw new ArgumentException("Unsolvable puzzle.", nameof(grid));
        }

        #region Implementation

        static readonly IReadOnlyDictionary<Sizes, ValueTuple<int, int>> s_subGridSize =
            new Dictionary<Sizes, ValueTuple<int, int>>
            {
                { Sizes.NineByNine, (3, 3) }
            };

        readonly Cell[] _cells;
        readonly Boundry[] _rows;
        readonly Boundry[] _columns;
        readonly Boundry[,] _subGrids;

        Puzzle(int size, int subGridRows, int subGridColumns, int[,] grid)
        {
            Func<Boundry> ctor = () => new Boundry(size);

            _rows = Enumerable.Repeat(ctor, size).Select(f => f()).ToArray();
            _columns = Enumerable.Repeat(ctor, size).Select(f => f()).ToArray();
            _subGrids = new Boundry[subGridColumns, subGridRows];

            var cells = new List<Cell>(grid.Length);
            for (int x = 0; x < size; x++)
            {
                int subx = x / subGridRows;

                for (int y = 0; y < size; y++)
                {
                    int suby = y / subGridColumns;
                    var subGrid = _subGrids[subx, suby] ?? (_subGrids[subx, suby] = new Boundry(size));

                    var cell = new Cell(_rows[x], _columns[y], subGrid, size, grid[x, y]);
                    cells.Add(cell);
                }
            }

            _cells = cells.ToArray();
        }

        bool Solve()
        {
            if (_cells.All(c => c.Value != 0)) return true;

            var status = false;
            foreach (var cell in _cells)
            {
                var cur = cell.TryUpdate();
                status = status || cur;
            }

            return status && Solve();
        }

        enum Sizes : int
        {
            NineByNine = 9
        }

        class Boundry
        {
            readonly int _size;
            readonly List<Cell> _cells;

            public Boundry(int size)
            {
                _size = size;
                _cells = new List<Cell>(size);
            }

            public bool Has(int value) => _cells.Any(c => c.Value == value);

            public Boundry AddCell(Cell cell)
            {
                _cells.Add(cell);
                return this;
            }
        }

        [DebuggerDisplay("Value = {Value}")]
        class Cell
        {
            readonly Boundry[] _boundries;
            readonly int _size;
            readonly int[] _allowedValues;

            public Cell(Boundry row, Boundry column, Boundry subGrid, int size, int value)
            {
                _allowedValues = Enumerable.Range(1, size).ToArray();
                if (value != 0 && !_allowedValues.Contains(value))
                    throw new ArgumentException($"Invalid value {value}", nameof(value));

                Value = value;

                _boundries = new[] { row.AddCell(this), column.AddCell(this), subGrid.AddCell(this) };
                _size = size;
            }

            public int Value { get; private set; }

            public bool TryUpdate()
            {
                if (Value != 0) return false;

                var possibleValues = _allowedValues.Where(i => _boundries.All(b => !b.Has(i))).ToArray();
                if (possibleValues.Length != 1) return false;

                Value = possibleValues[0];
                return true;
            }
        }

        #endregion
    }
}
