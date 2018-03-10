namespace SudokuSolver

module Puzzle =
    open System

    let private solveGrid (grid: int[,]) =
        // TODO: Solve the puzzle.
        Some grid

    let solve (grid: int[,]) :int[,] =
        let size = grid.Length |> float |> sqrt |> int
        if size * size <> grid.Length then
            raise (ArgumentException("Invalid puzzle size, should be a perfect square."))
        else
            let solved = solveGrid grid
            match solved with
            | None -> raise (ArgumentException("Unsolvable puzzle."))
            | Some slnGrid -> slnGrid
