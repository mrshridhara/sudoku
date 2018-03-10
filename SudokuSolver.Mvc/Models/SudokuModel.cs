using System.Collections.Generic;

namespace SudokuSolver.Models
{
    public class SudokuModel
    {
        public SudokuModel() => AvailableSizes = new List<int> { 9, 6 }.AsReadOnly();

        public IReadOnlyList<int> AvailableSizes { get; }

        public bool IsValid { get; set; } = true;
        public int? Size { get; set; }
        public int?[] Puzzle { get; set; }
    }
}
