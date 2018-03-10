using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using SudokuSolver.Models;

namespace SudokuSolver.Controllers
{
    public class HomeController : Controller
    {
        [HttpGet]
        public IActionResult Index() => View(new SudokuModel());

        [HttpPost]
        public IActionResult Index(SudokuModel model)
        {
            if (model.Size == null)
                return RedirectToAction(nameof(Index));

            if (model.Puzzle == null)
            {
                var size = (int)model.Size;
                model.Puzzle = new int?[size * size];
                return View(model);
            }

            try
            {
                var data = model.Puzzle.Select(i => i.GetValueOrDefault()).To2DSquareArray();
                var solved = Puzzle.solve(data).To1DArray();
                model.Puzzle = solved.Select(i => i == 0 ? default(int?) : i).ToArray();
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError(nameof(model.Puzzle), ex.Message);
            }

            model.IsValid = ModelState.IsValid;
            return View(model);
        }

        [HttpGet]
        public IActionResult About() => View();

        [HttpGet]
        public IActionResult Contact() => View();

        [HttpGet]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }
    }
}
