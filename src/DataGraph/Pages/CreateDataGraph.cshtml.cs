using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using DataGraph.Data;
using DataGraph.Models;
using Microsoft.AspNetCore.Authorization;
using DataGraph.Helpers;

namespace DataGraph.Pages
{
    [Authorize]
    public class CreateDataGraphModel : PageModel
    {
        private readonly DataGraph.Data.DataGraphContext _context;

        public CreateDataGraphModel(DataGraph.Data.DataGraphContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public DataGraphInstance DataGraphInstance { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            DataGraphInstance.CustomerId = User.GetCustomerId();

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.DataGraph.Add(DataGraphInstance);

            // Configure the global object
            _context.Objects.Add(new DataGraphObject()
            {
                UserId = "",
                ObjectType = "Global",
                Graph = DataGraphInstance
            });

            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}