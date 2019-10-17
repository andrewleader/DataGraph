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
                        var globalSchema = graphSchema.Schema.Global;

                        JObject answer = new JObject();

                        foreach (var literalProp in _context.LiteralPropertyValues.Where(i =>
                            i.CustomerId == customerId
                            && i.GraphId == graphId
                            && i.ObjectId == 1))
                        {
                            answer.Add(literalProp.PropertyName, JToken.Parse(literalProp.ProperyValueJson));
                        }

                        foreach (var listItemLiteral in _context.ListOfLiterals.Where(i =>
                            i.CustomerId == customerId
                            && i.GraphId == graphId
                            && i.ObjectId == 1))
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

                        return answer;
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


        [HttpPut("{customerId}/{graphId}/{entry}/{path}")]
        public void Put(string customerId, int graphId, string entry, string path, [FromBody]JToken json)
        {
            // Ex: api/graphs/{customerId}/{graphId}/global/warningMessage
            // Body: "The new warning message"

            var graphSchema = _context.DataGraph.First(i => i.CustomerId == customerId && i.Id == graphId);
            var bodyToken = json;

            switch (entry.ToLower())
            {
                case "global":
                    {
                        var globalSchema = graphSchema.Schema.Global;

                        if (globalSchema.TryGetProperty(path, out DataGraphProperty prop))
                        {
                            // Validate that type matches (note that array vs non array doesn't matter, put only allows adding a single item, not an array)
                            switch (prop.Type)
                            {
                                case "string":
                                    if (bodyToken.Type != JTokenType.String)
                                    {
                                        throw new InvalidOperationException();
                                    }
                                    break;

                                case "int":
                                    if (bodyToken.Type != JTokenType.Integer)
                                    {
                                        throw new InvalidOperationException();
                                    }
                                    break;

                                case "decimal":
                                    if (bodyToken.Type != JTokenType.Float)
                                    {
                                        throw new InvalidOperationException();
                                    }
                                    break;

                                default:
                                    throw new NotImplementedException();
                            }

                            if (!prop.IsArray)
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
                                    throw new NotImplementedException();
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
