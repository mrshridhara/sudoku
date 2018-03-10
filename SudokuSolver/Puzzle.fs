namespace SudokuSolver

module Puzzle =
    open System

    //let private convertToGrid (puzzle: int array) =
    //    let size = puzzle.Length |> float |> sqrt |> int
    //    if size * size <> puzzle.Length then None
    //    else Some (Array2D.init size size (fun x y -> puzzle.[(x * size) + y]))

    //let private convertToArray (grid: int[,]) =
    //    let arr = Array.zeroCreate grid.Length
    //    Buffer.BlockCopy(grid, 0, arr, 0, arr.Length * 4)
    //    assert (Array.forall (fun i -> i <> 0) arr)
    //    arr

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
