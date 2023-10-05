using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CotdQualifierRankWeb.Data;
using CotdQualifierRankWeb.Models;

namespace CotdQualifierRankWeb.Pages_Competitions
{
    public class DeleteModel : PageModel
    {
        private readonly CotdQualifierRankWeb.Data.CotdContext _context;

        public DeleteModel(CotdQualifierRankWeb.Data.CotdContext context)
        {
            _context = context;
        }

        [BindProperty]
      public Competition Competition { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null || _context.Competitions == null)
            {
                return NotFound();
            }

            var competition = await _context.Competitions.FirstOrDefaultAsync(m => m.Id == id);

            if (competition == null)
            {
                return NotFound();
            }
            else 
            {
                Competition = competition;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null || _context.Competitions == null)
            {
                return NotFound();
            }
            var competition = await _context.Competitions.FindAsync(id);

            if (competition != null)
            {
                Competition = competition;
                _context.Competitions.Remove(Competition);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
