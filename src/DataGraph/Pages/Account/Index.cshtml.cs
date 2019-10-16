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
    public class IndexModel : PageModel
    {
        private readonly DataGraph.Data.DataGraphContext _context;

        public IndexModel(DataGraph.Data.DataGraphContext context)
        {
            _context = context;
        }

        public IList<Models.DataGraphInstance> DataGraph { get;set; }

        public async Task OnGetAsync()
        {
            DataGraph = await _context.DataGraph.ToListAsync();
        }
    }
}
