using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DataGraph.Data;
using DataGraph.Models;
using Microsoft.AspNetCore.Authorization;
using DataGraph.Helpers;

namespace DataGraph.Pages
{
    [Authorize]
    public class ViewDataGraphModel : PageModel
    {
        private readonly DataGraph.Data.DataGraphContext _context;

        public ViewDataGraphModel(DataGraph.Data.DataGraphContext context)
        {
            _context = context;
        }

        public DataGraphInstance DataGraphInstance { get; set; }

        public string EndpointUrl { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            EndpointUrl = $"{Request.Scheme}://{Request.Host}/api/graphs/{User.GetCustomerId()}/{id.Value}";

            DataGraphInstance = await _context.DataGraph.FirstOrDefaultAsync(m => m.CustomerId == User.GetCustomerId() && m.Id == id.Value);

            if (DataGraphInstance == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
