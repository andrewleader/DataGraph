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
                                return JToken.Parse(graphSchema.Objects.First().LiteralPropertyValues.First(i => i.PropertyName == prop.Name).ProperyValueJson);
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
        public async void Put(string customerId, int graphId, string entry, string path, [FromBody]JToken json)
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
                            switch (prop.Type)
                            {
                                case "string":
                                    {
                                        if (!prop.IsArray)
                                        {
                                            if (bodyToken.Type == JTokenType.String)
                                            {
                                                graphSchema.Objects.First().LiteralPropertyValues.Add(new DataGraphLiteralPropertyValue
                                                {
                                                    PropertyName = prop.Name,
                                                    ProperyValueJson = bodyToken.ToString()
                                                });
                                                await _context.SaveChangesAsync();
                                            }
                                            else
                                            {
                                                throw new InvalidOperationException("Incorrect type");
                                            }
                                        }
                                        else
                                        {
                                            throw new InvalidOperationException("Incorrect type");
                                        }
                                    }
                                    break;

                                default:
                                    throw new InvalidOperationException("Incorrect type");
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
