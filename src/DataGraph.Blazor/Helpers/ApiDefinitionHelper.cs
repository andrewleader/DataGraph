using DataGraph.Models;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Http;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataGraph.Helpers
{
    public static class ApiDefinitionHelper
    {
        public static IEnumerable<ApiDefinition> GetApiDefinitions(this DataGraphSchema schema)
        {
            var userSchema = schema.User.GetApiReturnFormat(schema);
            userSchema.Remove("Id");

            yield return new ApiDefinition()
            {
                RelativePath = "/me",
                ReturnFormat = userSchema.ToString()
            };

            foreach (var def in schema.User.GetApiDefinitions(schema, "/me"))
            {
                yield return def;
            }

            var globalSchema = schema.Global.GetApiReturnFormat(schema);
            globalSchema.Remove("Id");

            yield return new ApiDefinition()
            {
                RelativePath = "/global",
                ReturnFormat = globalSchema.ToString()
            };

            foreach (var def in schema.Global.GetApiDefinitions(schema, "/global"))
            {
                yield return def;
            }
        }

        public static IEnumerable<ApiDefinition> GetApiDefinitions(this DataGraphClass classItem, DataGraphSchema schema, string pathPrefix)
        {
            foreach (var prop in classItem.Properties)
            {
                yield return new ApiDefinition()
                {
                    RelativePath = pathPrefix + "/" + prop.Name,
                    ReturnFormat = prop.GetApiReturnFormat(schema).ToString()
                };
            }
        }

        public static JObject GetApiReturnFormat(this DataGraphClass classItem, DataGraphSchema schema)
        {
            JObject obj = new JObject();
            obj.Add("Id", obj.GetHashCode());

            foreach (var prop in classItem.Properties)
            {
                obj.Add(prop.Name, prop.GetApiReturnFormat(schema));
            }

            return obj;
        }

        public static JToken GetApiReturnFormat(this DataGraphProperty property, DataGraphSchema schema)
        {
            if (property.IsCustomType())
            {
                var type = schema.CustomTypes.First(i => i.ClassName == property.Type);

                if (property.IsArray)
                {
                    var array = new JArray();
                    array.Add(type.GetApiReturnFormat(schema));
                    return array;
                }
                else
                {
                    return type.GetApiReturnFormat(schema);
                }
            }
            else
            {
                if (!property.IsArray)
                {
                    switch (property.Type)
                    {
                        case "string":
                            return "Sample string";

                        case "int":
                            return 3;

                        case "decimal":
                            return 4.99;
                    }
                }
                else
                {
                    switch (property.Type)
                    {
                        case "string":
                            return new JArray("Sample string one", "Sample string two");

                        case "int":
                            return new JArray(0, 1);

                        case "decimal":
                            return new JArray(3.52, 4.99);
                    }
                }
            }

            throw new NotImplementedException();
        }
    }

    public class ApiDefinition
    {
        public HttpMethod Method { get; set; } = HttpMethod.Get;

        public string RelativePath { get; set; }

        public string ReturnFormat { get; set; }
    }
}
