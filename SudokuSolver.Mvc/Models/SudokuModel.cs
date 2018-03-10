using System.Collections.Generic;

namespace SudokuSolver.Models
{
    public class SudokuModel
    {
        public IReadOnlyList<int> AvailableSizes { get; } = SudokuSolver.Puzzle.SupportedSizes;

        public bool IsValid { get; set; } = true;
        public int? Size { get; set; }
        public int?[] Puzzle { get; set; }
    }
}
