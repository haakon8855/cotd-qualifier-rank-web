using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using CotdQualifierRankWeb.Data;
using CotdQualifierRankWeb.Models;

namespace CotdQualifierRankWeb.Pages
{
    public class IndexModel : PageModel
    {
        private readonly CotdContext _context;

        public IndexModel(CotdContext context)
        {
            _context = context;
        }

        public IList<Competition> Competition { get; set; } = default!;

        public async Task OnGetAsync()
        {
            if (_context.Competitions != null)
            {
                Competition = await _context.Competitions.ToListAsync();
            }
        }
    }
}
