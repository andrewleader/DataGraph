﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DataGraph.Data;
using DataGraph.Models;

namespace DataGraph.Blazor.Pages
{
    public class EditModel : PageModel
    {
        private readonly DataGraph.Data.DataGraphContext _context;

        public EditModel(DataGraph.Data.DataGraphContext context)
        {
            _context = context;
        }

        [BindProperty]
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

        // To protect from overposting attacks, please enable the specific properties you want to bind to, for
        // more details see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(DataGraphInstance).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DataGraphInstanceExists(DataGraphInstance.CustomerId))
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

        private bool DataGraphInstanceExists(string id)
        {
            return _context.DataGraph.Any(e => e.CustomerId == id);
        }
    }
}
