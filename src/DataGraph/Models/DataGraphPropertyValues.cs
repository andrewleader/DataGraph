using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DataGraph.Models
{
    public class BaseDataGraphPropertyValue
    {
        [Key]
        public string CustomerId { get; set; }

        [Key]
        public int GraphId { get; set; }

        [Key]
        public int ObjectId { get; set; }

        [Key]
        public string PropertyName { get; set; }

        [ForeignKey(nameof(CustomerId) + ", " + nameof(GraphId) + ", " + nameof(ObjectId))]
        public virtual DataGraphObject Object { get; set; }
    }

    public class DataGraphLiteralPropertyValue : BaseDataGraphPropertyValue
    {
        public string ProperyValueJson { get; set; }
    }


    public class DataGraphReferencePropertyValue : BaseDataGraphPropertyValue
    {
        public int ReferencedObjectId { get; set; }

        [ForeignKey(nameof(CustomerId) + ", " + nameof(GraphId) + ", " + nameof(ReferencedObjectId))]
        public virtual DataGraphObject ReferencedObject { get; set; }
    }

    public class DataGraphListOfLiteralsPropertyValue : BaseDataGraphPropertyValue
    {
        [Key]
        public int ListItemId { get; set; }

        public string ListItemValueJson { get; set; }
    }

    public class DataGraphListOfReferencesPropertyValue : BaseDataGraphPropertyValue
    {
        /// <summary>
        /// This time it's a key
        /// </summary>
        [Key]
        public int ReferencedObjectId { get; set; }

        [ForeignKey(nameof(CustomerId) + ", " + nameof(GraphId) + ", " + nameof(ReferencedObjectId))]
        public virtual DataGraphObject ReferencedObject { get; set; }
    }
}
