using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataGraph.Helpers;
using DataGraph.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace DataGraph.Pages
{
    public class IndexModel : PageModel
    {
        private readonly DataGraph.Data.DataGraphContext _context;

        public IndexModel(DataGraph.Data.DataGraphContext context)
        {
            _context = context;
        }

        public IList<Models.DataGraphInstance> DataGraphs { get; set; }

        public async Task OnGetAsync()
        {
            if (User.Identity.IsAuthenticated)
            {
                DataGraphs = await _context.DataGraph.Where(i => i.CustomerId == User.GetCustomerId()).ToListAsync();
            }
        }
    }
}
