using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataGraph.Data;
using DataGraph.Models;

namespace DataGraph.Pages.Account
{
    public class EditModel : PageModel
    {
        private readonly DataGraph.Data.DataGraphContext _context;

        public EditModel(DataGraph.Data.DataGraphContext context)
        {
            _context = context;
        }

        [BindProperty]
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

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(DataGraph).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DataGraphExists(DataGraph.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool DataGraphExists(int id)
        {
            return _context.DataGraph.Any(e => e.Id == id);
        }
    }
}
