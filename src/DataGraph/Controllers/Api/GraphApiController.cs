using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DataGraph.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace DataGraph.Controllers.Api
{
    [Route("api/graphs")]
    [ApiController]
    public class GraphApiController : ControllerBase
    {
        private readonly Data.DataGraphContext _context;

        public GraphApiController(Data.DataGraphContext context)
        {
            _context = context;
        }

        // GET: api/graphs/WindowsLive|f381j31/1/global
        [HttpGet("{customerId}/{graphId}/{entry}")]
        public JObject Get(string customerId, int graphId, string entry)
        {
            var graphSchema = _context.DataGraph.First(i => i.CustomerId == customerId && i.Id == graphId);

            switch (entry.ToLower())
            {
                case "global":
                    {
                        return GetObjectJson(customerId, graphId, 1);
                    }

                case "me":
                    {
                        throw new NotImplementedException();
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        private JObject GetObjectJson(string customerId, int graphId, int objectId)
        {
            JObject answer = new JObject();

            foreach (var literalProp in _context.LiteralPropertyValues.Where(i =>
                i.CustomerId == customerId
                && i.GraphId == graphId
                && i.ObjectId == objectId))
            {
                answer.Add(literalProp.PropertyName, JToken.Parse(literalProp.ProperyValueJson));
            }

            foreach (var listItemLiteral in _context.ListOfLiterals.Where(i =>
                i.CustomerId == customerId
                && i.GraphId == graphId
                && i.ObjectId == objectId))
            {
                JArray array;
                if (answer.TryGetValue(listItemLiteral.PropertyName, out JToken token) && token is JArray existingArray)
                {
                    array = existingArray;
                }
                else
                {
                    array = new JArray();
                    answer.Add(listItemLiteral.PropertyName, array);
                }

                array.Add(JToken.Parse(listItemLiteral.ListItemValueJson));
            }

            foreach (var reference in _context.ReferencePropertyValues.Where(i =>
                            i.CustomerId == customerId
                            && i.GraphId == graphId
                            && i.ObjectId == objectId))
            {
                answer.Add(reference.PropertyName, GetObjectJson(customerId, graphId, reference.ReferencedObjectId));
            }

            foreach (var listItemRef in _context.ListOfReferences.Where(i =>
                i.CustomerId == customerId
                && i.GraphId == graphId
                && i.ObjectId == objectId))
            {
                JArray array;
                if (answer.TryGetValue(listItemRef.PropertyName, out JToken token) && token is JArray existingArray)
                {
                    array = existingArray;
                }
                else
                {
                    array = new JArray();
                    answer.Add(listItemRef.PropertyName, array);
                }

                array.Add(GetObjectJson(customerId, graphId, listItemRef.ReferencedObjectId));
            }

            return answer;
        }

        // GET: api/graphs/WindowsLive|f381j31/1/me/cart
        [HttpGet("{customerId}/{graphId}/{entry}/{path}")]
        public JToken Get(string customerId, int graphId, string entry, string path)
        {
            var graphSchema = _context.DataGraph.First(i => i.CustomerId == customerId && i.Id == graphId);

            switch (entry.ToLower())
            {
                case "global":
                    {
                        var globalSchema = graphSchema.Schema.Global;

                        if (globalSchema.TryGetProperty(path, out DataGraphProperty prop))
                        {
                            if (!prop.IsCustomType())
                            {
                                var json = _context.LiteralPropertyValues.First(i =>
                                    i.CustomerId == customerId
                                    && i.GraphId == graphId
                                    && i.ObjectId == 1
                                    && i.PropertyName == prop.Name).ProperyValueJson;

                                var token = JToken.Parse(json);
                                return token;
                            }
                            else
                            {
                                throw new NotImplementedException();
                            }
                        }
                        else
                        {
                            throw new KeyNotFoundException();
                        }
                    }
                    break;

                case "me":
                    {
                        throw new NotImplementedException();
                    }
                    break;

                default:
                    throw new NotImplementedException();
            }
        }

        // GET: api/GraphApi/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/GraphApi
        [HttpPost("{customerId}/{graphId}/{entry}/{path}")]
        public void Post(string customerId, int graphId, string entry, string path, [FromBody]string json)
        {
        }

        private static void AssertTypeMatches(JToken jtoken, DataGraphProperty property)
        {
            // Note that we ignore arrays since the PUT operations don't accept arrays, they accept single values
            if (property.IsCustomType())
            {
                if (jtoken.Type != JTokenType.Object)
                {
                    throw new InvalidOperationException();
                }
            }
            else
            {
                switch (property.Type)
                {
                    case "string":
                        if (jtoken.Type != JTokenType.String)
                        {
                            throw new InvalidOperationException();
                        }
                        break;

                    case "int":
                        if (jtoken.Type != JTokenType.Integer)
                        {
                            throw new InvalidOperationException();
                        }
                        break;

                    case "decimal":
                        if (jtoken.Type != JTokenType.Float)
                        {
                            throw new InvalidOperationException();
                        }
                        break;

                    default:
                        throw new NotImplementedException();
                }
            }
        }

        private DataGraphObject AddObject(string customerId, int graphId, string userId, DataGraphClass customType, JObject obj)
        {
            var newObject = new DataGraphObject
            {
                CustomerId = customerId,
                GraphId = graphId,
                ObjectType = customType.ClassName,
                UserId = userId
            };

            foreach (var p in obj.Properties())
            {
                if (customType.TryGetProperty(p.Name, out DataGraphProperty typeProp))
                {
                    // Scoping it to simple props right now
                    if (!typeProp.IsCustomType() && !typeProp.IsArray)
                    {
                        AssertTypeMatches(p.Value, typeProp);

                        _context.LiteralPropertyValues.Add(new DataGraphLiteralPropertyValue()
                        {
                            CustomerId = customerId,
                            GraphId = graphId,
                            Object = newObject,
                            PropertyName = typeProp.Name,
                            ProperyValueJson = p.Value.ToString(Newtonsoft.Json.Formatting.None)
                        });
                    }
                }
            }

            return newObject;
        }


        [HttpPut("{customerId}/{graphId}/{entry}/{path}")]
        public void Put(string customerId, int graphId, string entry, string path, [FromBody]JToken json)
        {
            // Ex: api/graphs/{customerId}/{graphId}/global/warningMessage
            // Body: "The new warning message"

            var graph = _context.DataGraph.First(i => i.CustomerId == customerId && i.Id == graphId);
            var bodyToken = json;

            switch (entry.ToLower())
            {
                case "global":
                    {
                        var globalSchema = graph.Schema.Global;

                        if (globalSchema.TryGetProperty(path, out DataGraphProperty prop))
                        {
                            // Validate that type matches (note that array vs non array doesn't matter, put only allows adding a single item, not an array)
                            AssertTypeMatches(bodyToken, prop);

                            if (!prop.IsArray)
                            {
                                if (prop.IsCustomType())
                                {
                                    DataGraphReferencePropertyValue literalReference = _context.ReferencePropertyValues.FirstOrDefault(i =>
                                        i.CustomerId == customerId
                                        && i.GraphId == graphId
                                        && i.ObjectId == 1
                                        && i.PropertyName == prop.Name);

                                    // Nuke the old value
                                    if (literalReference != null)
                                    {
                                        var toDelete = _context.Objects.FirstOrDefault(i =>
                                            i.CustomerId == customerId
                                            && i.GraphId == graphId
                                            && i.ObjectId == literalReference.ObjectId);

                                        if (toDelete != null)
                                        {
                                            _context.Objects.Remove(toDelete);
                                        }
                                    }
                                    else
                                    {
                                        literalReference = new DataGraphReferencePropertyValue
                                        {
                                            CustomerId = customerId,
                                            GraphId = graphId,
                                            ObjectId = 1,
                                            PropertyName = prop.Name
                                        };

                                        _context.ReferencePropertyValues.Add(literalReference);
                                    }

                                    var customType = graph.Schema.CustomTypes.First(i => i.ClassName == prop.Type);

                                    literalReference.ReferencedObject = AddObject(customerId, graphId, "", customType, json as JObject);
                                }

                                else
                                {
                                    var existing = _context.LiteralPropertyValues.FirstOrDefault(i =>
                                                        i.CustomerId == customerId
                                                        && i.GraphId == graphId
                                                        && i.ObjectId == 1
                                                        && i.PropertyName == prop.Name);
                                    if (existing != null)
                                    {
                                        // Strings require Formatting.None to output using the "" quotes around the string
                                        existing.ProperyValueJson = bodyToken.ToString(Newtonsoft.Json.Formatting.None);
                                    }
                                    else
                                    {
                                        _context.LiteralPropertyValues.Add(new DataGraphLiteralPropertyValue()
                                        {
                                            CustomerId = customerId,
                                            GraphId = graphId,
                                            ObjectId = 1,
                                            PropertyName = prop.Name,
                                            ProperyValueJson = bodyToken.ToString(Newtonsoft.Json.Formatting.None)
                                        });
                                    }
                                }

                                _context.SaveChanges();
                            }
                            else
                            {
                                // Arrays

                                if (!prop.IsCustomType())
                                {
                                    _context.ListOfLiterals.Add(new DataGraphListOfLiteralsPropertyValue
                                    {
                                        CustomerId = customerId,
                                        GraphId = graphId,
                                        ObjectId = 1,
                                        PropertyName = prop.Name,
                                        ListItemValueJson = bodyToken.ToString(Newtonsoft.Json.Formatting.None)
                                    });
                                }
                                else
                                {
                                    var customType = graph.Schema.CustomTypes.First(i => i.ClassName == prop.Type);

                                    _context.ListOfReferences.Add(new DataGraphListOfReferencesPropertyValue
                                    {
                                        CustomerId = customerId,
                                        GraphId = graphId,
                                        ObjectId = 1,
                                        PropertyName = prop.Name,
                                        ReferencedObject = AddObject(customerId, graphId, "", customType, json as JObject)
                                    });
                                }

                                _context.SaveChanges();
                            }
                        }
                        else
                        {
                            throw new KeyNotFoundException("The property wasn't found");
                        }
                    }
                    break;

                case "me":
                    {

                    }
                    break;

                default:
                    throw new InvalidOperationException("Unknown entry, must either be /global or /me");
            }
        }

        // PUT: api/GraphApi/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
