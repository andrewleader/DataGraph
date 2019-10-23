using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DataGraph.Data;
using DataGraph.Models;

namespace DataGraph.Blazor.Pages
{
    public class CreateModel : PageModel
    {
        private readonly DataGraph.Data.DataGraphContext _context;

        public CreateModel(DataGraph.Data.DataGraphContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public DataGraphInstance DataGraphInstance { get; set; }

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.DataGraph.Add(DataGraphInstance);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
