using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataGraph.Models
{
    public class EditableDataGraphClassModel
    {
        public EditableDataGraphClassModel(DataGraphSchema schema, DataGraphClass classItem)
        {
            Schema = schema;
            Class = classItem;
        }

        public DataGraphClass Class { get; set; }

        public DataGraphSchema Schema { get; set; }
    }
}
