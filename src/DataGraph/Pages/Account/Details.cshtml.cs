using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DataGraph.Data;
using DataGraph.Models;

namespace DataGraph.Pages.Account
{
    public class DetailsModel : PageModel
    {
        private readonly DataGraph.Data.DataGraphContext _context;

        public DetailsModel(DataGraph.Data.DataGraphContext context)
        {
            _context = context;
        }

        public Models.DataGraphInstance DataGraph { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            DataGraph = await _context.DataGraph.FirstOrDefaultAsync(m => m.Id == id);

            if (DataGraph == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
