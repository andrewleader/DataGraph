using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DataGraph.Data;
using DataGraph.Models;

namespace DataGraph.Blazor.Pages
{
    public class DetailsModel : PageModel
    {
        private readonly DataGraph.Data.DataGraphContext _context;

        public DetailsModel(DataGraph.Data.DataGraphContext context)
        {
            _context = context;
        }

        public DataGraphInstance DataGraphInstance { get; set; }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            DataGraphInstance = await _context.DataGraph.FirstOrDefaultAsync(m => m.CustomerId == id);

            if (DataGraphInstance == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
