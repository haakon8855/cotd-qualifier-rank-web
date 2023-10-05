using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using CotdQualifierRankWeb.Data;
using CotdQualifierRankWeb.Models;

namespace CotdQualifierRankWeb.Pages_Competitions
{
    public class CreateModel : PageModel
    {
        private readonly CotdQualifierRankWeb.Data.CotdContext _context;

        public CreateModel(CotdQualifierRankWeb.Data.CotdContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public Competition Competition { get; set; } = default!;
        

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
          if (!ModelState.IsValid || _context.Competitions == null || Competition == null)
            {
                return Page();
            }

            _context.Competitions.Add(Competition);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
