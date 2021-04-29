using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace SudokuSolver
{
    public sealed class Puzzle
    {
        public static IReadOnlyCollection<int> SupportedSizes { get; } = Enum.GetValues(typeof(GridSize)).Cast<int>().ToArray();

        public static int[,] Solve(int[,] grid)
        {
            var size = (int)Math.Sqrt(grid.Length);
            if ((size * size) != grid.Length)
                throw new ArgumentException("Invalid puzzle length. Should be a perfect square.", nameof(grid));

            if (!Enum.IsDefined(typeof(GridSize), size))
                throw new ArgumentException("Unsupported puzzle size.", nameof(grid));

            s_allowedValues = Enumerable.Range(1, size).ToArray();
            var subGridSize = s_subGridSize[(GridSize)size];
            var puzzle = new Puzzle(size, subGridSize.Rows, subGridSize.Columns, grid);

            if (puzzle.Solve())
                return puzzle._cells.Select(c => c.Value).To2DSquareArray();

            throw new ArgumentException("Unsolvable puzzle.", nameof(grid));
        }

        #region Implementation

        static readonly IReadOnlyDictionary<GridSize, (int Rows, int Columns)> s_subGridSize =
            new Dictionary<GridSize, (int, int)>
            {
                { GridSize.NineByNine, (3, 3) }
            };

        static IEnumerable<int> s_allowedValues;

        readonly Cell[] _cells;

        Puzzle(int size, int subGridRows, int subGridColumns, int[,] grid)
        {
            var rows = Enumerable.Range(0, size).Select(_ => new Boundry(size)).ToArray();
            var columns = Enumerable.Range(0, size).Select(_ => new Boundry(size)).ToArray();
            var subGrids = new Boundry[subGridColumns, subGridRows];
            var cells = new List<Cell>(grid.Length);

            for (int x = 0; x < size; x++)
            {
                int subx = x / subGridRows;

                for (int y = 0; y < size; y++)
                {
                    int suby = y / subGridColumns;

                    if (subGrids[subx, suby] == null)
                        subGrids[subx, suby] = new Boundry(size);

                    var subGrid = subGrids[subx, suby];
                    var cell = new Cell(rows[x], columns[y], subGrid, grid[x, y]);
                    cells.Add(cell);
                }
            }

            _cells = cells.ToArray();
        }

        bool Solve()
        {
            var progressed = _cells.All(c => c.Value != 0);
            if (progressed) return true;

            foreach (var cell in _cells.Where(c => c.Value == 0))
            {
                var updated = cell.TryUpdate();
                progressed = progressed || updated;
            }

            return progressed && Solve();
        }

        enum GridSize : int
        {
            NineByNine = 9
        }

        sealed class Boundry
        {
            readonly List<Cell> _cells;

            internal Boundry(int size) => _cells = new List<Cell>(size);

            public override string ToString() => string.Join(", ", _cells);

            internal void Append(Cell cell) => _cells.Add(cell);

            internal IEnumerable<int> GetExistingValues() => _cells.Select(c => c.Value).Where(v => v != 0);
        }

        sealed class Cell
        {
            readonly IReadOnlyCollection<Boundry> _boundries;

            internal Cell(Boundry row, Boundry column, Boundry subGrid, int value)
            {
                if (value != 0 && !s_allowedValues.Contains(value))
                    throw new ArgumentException($"Invalid value {value}", nameof(value));

                _boundries = new[] { row, column, subGrid };
                foreach (var b in _boundries)
                    b.Append(this);

                Value = value;
            }

            internal int Value { get; private set; }

            public override string ToString() => Value == 0 ? "_" : Value.ToString();

            internal bool TryUpdate()
            {
                if (Value != 0) return false;

                var possibleValues = _boundries
                    .Aggregate(s_allowedValues, (all, cur) => all.Except(cur.GetExistingValues()))
                    .ToArray();

                switch (possibleValues.Length)
                {
                    case 0: throw new InvalidOperationException("Unsolvable puzzle.");

                    case 1:
                        Value = possibleValues[0];
                        return true;

                    default: return false;
                }
            }
        }

        #endregion
    }
}
