using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DataGraph.Models
{
    public class DataGraphObject
    {
        [Key]
        public string CustomerId { get; set; }

        [Key]
        public int GraphId { get; set; }

        [ForeignKey(nameof(CustomerId) + ", " + nameof(GraphId))]
        public virtual DataGraphInstance Graph { get; set; }

        public string UserId { get; set; }

        [Key]
        public int ObjectId { get; set; }

        public string ObjectType { get; set; }

        [InverseProperty(nameof(BaseDataGraphPropertyValue.Object))]
        public virtual ICollection<DataGraphLiteralPropertyValue> LiteralPropertyValues { get; set; }

        [InverseProperty(nameof(BaseDataGraphPropertyValue.Object))]
        public virtual ICollection<DataGraphReferencePropertyValue> ReferencePropertyValues { get; set; }

        [InverseProperty(nameof(BaseDataGraphPropertyValue.Object))]
        public virtual ICollection<DataGraphListOfLiteralsPropertyValue> ListOfLiteralsPropertyValues { get; set; }

        [InverseProperty(nameof(BaseDataGraphPropertyValue.Object))]
        public virtual ICollection<DataGraphListOfReferencesPropertyValue> ListOfReferencesPropertyValues { get; set; }
    }

    public class DataGraphUserObject : DataGraphObject
    {
    }
}
