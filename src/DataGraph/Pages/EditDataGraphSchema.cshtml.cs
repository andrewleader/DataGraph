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
using Microsoft.AspNetCore.Authorization;
using DataGraph.Helpers;
using Microsoft.AspNetCore.Http;

namespace DataGraph.Pages
{
    [Authorize]
    public class EditDataGraphSchemaModel : PageModel
    {
        private readonly DataGraph.Data.DataGraphContext _context;

        public EditDataGraphSchemaModel(DataGraph.Data.DataGraphContext context)
        {
            _context = context;
        }

        [BindProperty]
        public DataGraphInstance DataGraphInstance { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            DataGraphInstance = await _context.DataGraph.FirstOrDefaultAsync(m => m.CustomerId == User.GetCustomerId() && m.Id == id.Value);

            if (DataGraphInstance == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id, IFormCollection values)
        {
            int graphId = id;
            if (id == 0)
            {
                throw new ArgumentException("Id was 0");
            }

            DataGraphInstance = await _context.DataGraph.FirstAsync(m => m.CustomerId == User.GetCustomerId() && m.Id == graphId);

            string operationType = values["OperationType"];

            // Adding a property to existing class
            switch (operationType)
            {
                case "Save property":
                    {
                        string className = values["ClassName"];

                        var newProp = new DataGraphProperty()
                        {
                            Name = values["Name"],
                            Type = values["Type"],
                            IsArray = values.ContainsKey("IsArray")
                        };

                        switch (className)
                        {
                            case "User":
                                DataGraphInstance.Schema.User.Properties.Add(newProp);
                                break;

                            case "Global":
                                DataGraphInstance.Schema.Global.Properties.Add(newProp);
                                break;

                            default:
                                DataGraphInstance.Schema.CustomTypes.First(i => i.ClassName == className).Properties.Add(newProp);
                                break;
                        }
                    }
                    break;

                case "Save class":
                    {
                        string className = values["ClassName"];

                        DataGraphInstance.Schema.CustomTypes.Add(new DataGraphClass()
                        {
                            ClassName = className
                        });
                    }
                    break;

                default:
                    throw new InvalidOperationException("Unknown operation " + operationType);
            }

            DataGraphInstance.ApplySchemaChanges();

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }

            return Page();
        }

        private bool DataGraphInstanceExists(string id)
        {
            return _context.DataGraph.Any(e => e.CustomerId == id);
        }
    }
}
