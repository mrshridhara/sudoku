namespace SudokuSolver
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public static class Puzzle
    {
        public static IReadOnlyCollection<int> SupportedSizes { get; } =
            Enum.GetValues(typeof(GridSize)).Cast<int>().ToArray();

        public static int[,] Solve(int[,] grid)
        {
            var size = (int) Math.Sqrt(grid.Length);
            if ((size * size) != grid.Length)
                throw new ArgumentException("Puzzle length should be a perfect square.", nameof(grid));

            if (!Enum.IsDefined(typeof(GridSize), size))
                throw new ArgumentException("Unsupported puzzle size.", nameof(grid));

            var subGridSize = s_subGridSizeMap[(GridSize) size];
            var cells = CreateCells(size, subGridSize.Rows, subGridSize.Columns, grid);

            if (Solve(cells))
                return cells.Select(c => c.Value).To2DSquareArray();

            throw new ArgumentException("Puzzle can have multiple solutions.", nameof(grid));
        }

        #region Implementation

        static readonly IReadOnlyDictionary<GridSize, (int Rows, int Columns)> s_subGridSizeMap =
            new Dictionary<GridSize, (int, int)>
            {
                { GridSize.NineByNine, (3, 3) }
            };

        private static IReadOnlyCollection<Cell> CreateCells(int size, int subGridRows, int subGridColumns, int[,] grid)
        {
            var cells = new List<Cell>(grid.Length);

            var rows = Enumerable.Range(0, size).Select(_ => new Boundry(size)).ToArray();
            var columns = Enumerable.Range(0, size).Select(_ => new Boundry(size)).ToArray();
            var subGrids = new Boundry[subGridColumns, subGridRows];
            var allowedValues = Enumerable.Range(1, size).ToArray();

            for (var x = 0; x < size; x++)
            {
                var subx = x / subGridRows;

                for (var y = 0; y < size; y++)
                {
                    var suby = y / subGridColumns;

                    if (subGrids[subx, suby] == null)
                        subGrids[subx, suby] = new Boundry(size);

                    cells.Add(new Cell(
                        allowedValues,
                        rows[x],
                        columns[y],
                        subGrids[subx, suby],
                        grid[x, y]));
                }
            }

            return cells;
        }

        static bool Solve(IReadOnlyCollection<Cell> cells)
        {
            var progressed = cells.All(c => c.Value != 0);
            if (progressed) return true;

            foreach (var cell in cells.Where(c => c.Value == 0))
            {
                var updated = cell.TryUpdate();
                progressed = progressed || updated;
            }

            return progressed && Solve(cells);
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
            readonly IReadOnlyCollection<int> _allowedValues;
            readonly IReadOnlyCollection<Boundry> _boundries;

            internal Cell(
                IReadOnlyCollection<int> allowedValues,
                Boundry row,
                Boundry column,
                Boundry subGrid,
                int value)
            {
                _allowedValues = allowedValues;
                if (value != 0 && !_allowedValues.Contains(value))
                    throw new ArgumentException($"Invalid value {value}", nameof(value));

                _boundries = new[] { row, column, subGrid };
                foreach (var boundry in _boundries)
                    boundry.Append(this);

                Value = value;
            }

            internal int Value { get; private set; }

            public override string ToString() => Value == 0 ? "_" : Value.ToString();

            internal bool TryUpdate()
            {
                if (Value != 0) return false;

                var possibleValues = _boundries
                    .Aggregate(
                        (IEnumerable<int>) _allowedValues,
                        (all, cur) => all.Except(cur.GetExistingValues()))
                    .ToArray();

                switch (possibleValues.Length)
                {
                    case 0:
                        throw new InvalidOperationException(
                            $"No possible values for cell. Boundries:\n{string.Join("\n", _boundries)}");

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
