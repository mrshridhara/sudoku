using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SudokuSolver.Test
{
    [TestClass]
    public class PuzzleTests
    {
        [TestMethod]
        public void SolveTests()
        {
            const int _ = 0;
            var grid = new int[9, 9]
            {
                { 7, _, _, 9, 1, 5, _, _, 6 },
                { _, 6, _, _, 8, _, _, 4, _ },
                { _, _, 5, _, _, _, 7, _, _ },
                { _, _, _, 4, _, 1, _, _, _ },
                { 4, 1, 2, 3, _, 6, 8, 7, 5 },
                { _, _, _, 5, _, 8, _, _, _ },
                { _, _, 8, _, _, _, 2, _, _ },
                { _, 4, _, _, 6, _, _, 1, _ },
                { 1, _, _, 7, 5, 3, _, _, 8 }
            };

            var actual = Puzzle.Solve(grid);
            //Assert.
        }
    }
}
