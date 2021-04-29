using Xunit;

namespace SudokuSolver.Test
{
    public sealed class PuzzleTests
    {
        [Fact]
        public void Solve_ForEasy_Succeeds()
        {
            // Arrange.
            var grid = new int[9, 9]
            {
                { 4, 1, 0, 0, 6, 0, 0, 7, 8 },
                { 7, 3, 0, 5, 0, 1, 4, 2, 0 },
                { 0, 0, 8, 4, 7, 3, 0, 6, 0 },
                { 0, 5, 0, 0, 9, 4, 8, 3, 0 },
                { 3, 9, 0, 0, 1, 0, 7, 0, 0 },
                { 2, 8, 4, 3, 0, 0, 0, 0, 0 },
                { 6, 0, 0, 0, 0, 0, 0, 8, 0 },
                { 0, 0, 1, 9, 4, 0, 0, 0, 0 },
                { 0, 4, 9, 0, 2, 8, 0, 0, 0 },
            };

            // Act.
            var actual = Puzzle.Solve(grid);

            // Assert.
            Assert.NotNull(actual);
            var array = actual.To1DArray();
            Assert.All(array, each => Assert.NotEqual(0, each));
        }
    }
}
