using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DataGraph.Models
{
    public class DataGraphInstance
    {
        private static JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
        {
            //TypeNameHandling = TypeNameHandling.Auto
        };

        /// <summary>
        /// This is their OpenID, like WindowsLive|f93018al93j
        /// </summary>
        public string CustomerId { get; set; }

        public int Id { get; set; }

        public string Name { get; set; }

        /// <summary>
        /// Used only for database serialization purposes
        /// </summary>
        [Column("Schema")]
        public string SchemaJson { get; set; }

        private DataGraphSchema _schema;
        /// <summary>
        /// JSON-serialized blob of the schema info
        /// </summary>
        [NotMapped]
        public DataGraphSchema Schema
        {
            get
            {
                if (_schema == null)
                {
                    try
                    {
                        _schema = JsonConvert.DeserializeObject<DataGraphSchema>(SchemaJson, _jsonSerializerSettings);
                    }
                    catch
                    {
                        _schema = new DataGraphSchema();
                    }
                }

                return _schema;
            }
        }

        public void ApplySchemaChanges()
        {
            Schema.Validate();
            SchemaJson = JsonConvert.SerializeObject(Schema, _jsonSerializerSettings);
        }

        public virtual ICollection<DataGraphObject> Objects { get; set; }
    }
}
