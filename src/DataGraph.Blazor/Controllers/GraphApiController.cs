using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using DataGraph.Blazor.Data;
using DataGraph.Data;
using DataGraph.Models;
using JWT;
using JWT.Serializers;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Samples.EFLogging;
using Newtonsoft.Json.Linq;

namespace DataGraph.Blazor.Controllers
{
    [Route("api/graphs")]
    [ApiController]
    public class GraphApiController : ControllerBase
    {
        private readonly DataGraphContext _context;

        public GraphApiController(DataGraphService service)
        {
            _context = service.Context;

#if DEBUG
            //_context.ConfigureLogging(s => Debug.WriteLine(s));
#endif
        }

        // GET: api/graphs/WindowsLive|f381j31/1/global
        [HttpGet("{customerId}/{graphId}/{entry}")]
        public JObject Get(string customerId, int graphId, string entry)
        {
            var graph = _context.DataGraph.First(i => i.CustomerId == customerId && i.Id == graphId);

            switch (entry.ToLower())
            {
                case "global":
                    {
                        var globalObj = GetObjectJson(customerId, graphId, graph.GlobalObjectId);
                        globalObj.Remove("Id");
                        return globalObj;
                    }

                case "me":
                    {
                        var userId = AuthUser();

                        int userObjId = GetUserObjectId(customerId, graphId, userId);

                        var userObj = GetObjectJson(customerId, graphId, userObjId);
                        userObj.Remove("Id");
                        return userObj;
                    }

                default:
                    throw new NotImplementedException();
            }
        }

        private int GetUserObjectId(string customerId, int graphId, string userId)
        {
            return _context.Objects.Where(i =>
                i.CustomerId == customerId
                && i.GraphId == graphId
                && i.UserId == userId
                && i.ObjectType == "User").Select(i => i.ObjectId).First();
        }


        [HttpGet("{customerId}/{graphId}/global/{propArray}/{itemId}")]
        public JObject GetGlobalArrayItem(string customerId, int graphId, string propArray, int itemId)
        {
            var graph = _context.DataGraph.First(i => i.CustomerId == customerId && i.Id == graphId);

            if (graph.Schema.Global.TryGetProperty(propArray, out DataGraphProperty prop))
            {
                if (prop.IsArray)
                {
                    // Check it's actually in that specified collection
                    if (_context.ListOfReferences.Any(i =>
                        i.CustomerId == customerId
                        && i.GraphId == graphId
                        && i.ObjectId == graph.GlobalObjectId
                        && i.PropertyName == prop.Name
                        && i.ReferencedObjectId == itemId))
                    {
                        return GetObjectJson(customerId, graphId, itemId);
                    }
                    else
                    {
                        throw new KeyNotFoundException();
                    }
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        [HttpDelete("{customerId}/{graphId}/global/{propArray}/{itemId}")]
        public void DeleteGlobalArrayItem(string customerId, int graphId, string propArray, int itemId)
        {
            var graph = _context.DataGraph.First(i => i.CustomerId == customerId && i.Id == graphId);

            if (graph.Schema.Global.TryGetProperty(propArray, out DataGraphProperty prop))
            {
                if (prop.IsArray)
                {
                    // Check it's actually in that specified collection
                    var reference = _context.ListOfReferences.FirstOrDefault(i =>
                        i.CustomerId == customerId
                        && i.GraphId == graphId
                        && i.ObjectId == graph.GlobalObjectId
                        && i.PropertyName == prop.Name
                        && i.ReferencedObjectId == itemId);

                    if (reference != null)
                    {
                        _context.ListOfReferences.Remove(reference);
                        _context.SaveChanges();
                    }
                    else
                    {
                        throw new KeyNotFoundException();
                    }
                }
                else
                {
                    throw new InvalidOperationException();
                }
            }
            else
            {
                throw new InvalidOperationException();
            }
        }

        private JObject GetObjectJson(string customerId, int graphId, int objectId)
        {
            JObject answer = new JObject();

            answer.Add("Id", objectId);

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
                            && i.ObjectId == objectId).ToArray())
            {
                answer.Add(reference.PropertyName, GetObjectJson(customerId, graphId, reference.ReferencedObjectId));
            }

            foreach (var listItemRef in _context.ListOfReferences.Where(i =>
                i.CustomerId == customerId
                && i.GraphId == graphId
                && i.ObjectId == objectId).ToArray())
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

        public JToken GetPropertyValueJson(string customerId, int graphId, int objectId, DataGraphProperty prop)
        {
            if (!prop.IsCustomType())
            {
                if (!prop.IsArray)
                {
                    var json = _context.LiteralPropertyValues.First(i =>
                        i.CustomerId == customerId
                        && i.GraphId == graphId
                        && i.ObjectId == objectId
                        && i.PropertyName == prop.Name).ProperyValueJson;

                    var token = JToken.Parse(json);
                    return token;
                }
                else
                {
                    JArray array = new JArray();

                    foreach (var itemValueJson in _context.ListOfLiterals.Where(i =>
                        i.CustomerId == customerId
                        && i.GraphId == graphId
                        && i.ObjectId == objectId
                        && i.PropertyName == prop.Name).Select(i => i.ListItemValueJson))
                    {
                        array.Add(JToken.Parse(itemValueJson));
                    }

                    return array;
                }
            }
            else
            {
                if (!prop.IsArray)
                {
                    int refObjId = _context.ReferencePropertyValues.Where(i =>
                        i.CustomerId == customerId
                        && i.GraphId == graphId
                        && i.ObjectId == objectId
                        && i.PropertyName == prop.Name).Select(i => i.ReferencedObjectId).First();

                    return GetObjectJson(customerId, graphId, refObjId);
                }
                else
                {
                    JArray array = new JArray();

                    foreach (var refObjId in _context.ListOfReferences.Where(i =>
                        i.CustomerId == customerId
                        && i.GraphId == graphId
                        && i.ObjectId == objectId
                        && i.PropertyName == prop.Name).Select(i => i.ReferencedObjectId).ToArray())
                    {
                        array.Add(GetObjectJson(customerId, graphId, refObjId));
                    }

                    return array;
                }
            }
        }

        // GET: api/graphs/WindowsLive|f381j31/1/me/cart
        [HttpGet("{customerId}/{graphId}/{entry}/{path}")]
        public JToken Get(string customerId, int graphId, string entry, string path)
        {
            var graph = _context.DataGraph.First(i => i.CustomerId == customerId && i.Id == graphId);

            switch (entry.ToLower())
            {
                case "global":
                    {
                        var globalSchema = graph.Schema.Global;

                        if (globalSchema.TryGetProperty(path, out DataGraphProperty prop))
                        {
                            return GetPropertyValueJson(customerId, graphId, graph.GlobalObjectId, prop);
                        }
                        else
                        {
                            throw new KeyNotFoundException();
                        }
                    }
                    break;

                case "me":
                    {
                        var userId = AuthUser();

                        var userSchema = graph.Schema.User;

                        if (userSchema.TryGetProperty(path, out DataGraphProperty prop))
                        {
                            int userObjId = GetUserObjectId(customerId, graphId, userId);

                            return GetPropertyValueJson(customerId, graphId, userObjId, prop);
                        }
                        else
                        {
                            throw new KeyNotFoundException();
                        }
                    }
                    break;

                default:
                    throw new NotImplementedException();
            }
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
                switch (jtoken.Type)
                {
                    // Both objects and integers (reference to the object ID) are allowed
                    case JTokenType.Object:
                    case JTokenType.Integer:
                        return;

                    default:
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

        private string AuthUser()
        {
            if (Request.Headers.TryGetValue("Authorization", out StringValues authValues))
            {
                var authVal = authValues.First();

                if (authVal.StartsWith("Bearer "))
                {
                    var token = authVal.Substring("Bearer ".Length);

                    var serializer = new JsonNetSerializer();
                    var provider = new UtcDateTimeProvider();
                    var validator = new JwtValidator(serializer, provider);
                    var urlEncoder = new JwtBase64UrlEncoder();
                    var decoder = new JwtDecoder(serializer, validator, urlEncoder);

                    var tokenJson = decoder.Decode(token);

                    JObject tokenObj = JObject.Parse(tokenJson);

                    // TODO: Need to validate issuer matches the allowed issuer of the DataGraph
                    // TODO: Need to validate issuer signature

                    return tokenObj.Value<string>("sub");
                }
                else
                {
                    throw new InvalidOperationException("Invalid Authorization format");
                }
            }
            else
            {
                throw new InvalidOperationException("Authorization header wasn't provided");
            }
        }

        [HttpPut("{customerId}/{graphId}/me/{path}")]
        [HttpPost("{customerId}/{graphId}/me/{path}")]
        public void PutForUser(string customerId, int graphId, string path, [FromBody]JToken json)
        {
            string userId = AuthUser();

            var graph = _context.DataGraph.First(i => i.CustomerId == customerId && i.Id == graphId);

            var userSchema = graph.Schema.User;

            if (userSchema.TryGetProperty(path, out DataGraphProperty prop))
            {
                var userObj = _context.Objects.FirstOrDefault(i =>
                    i.CustomerId == customerId
                    && i.GraphId == graphId
                    && i.UserId == userId
                    && i.ObjectType == "User");

                if (userObj == null)
                {
                    userObj = new DataGraphObject
                    {
                        CustomerId = customerId,
                        GraphId = graphId,
                        UserId = userId,
                        ObjectType = "User"
                    };

                    _context.Objects.Add(userObj);

                    // Save changes so we get an ObjectId assigned
                    _context.SaveChanges();
                }

                PutIntoObject(customerId, graphId, userId, userObj.ObjectId, graph.Schema.CustomTypes, prop, json);
            }
            else
            {
                throw new InvalidOperationException("Unknown property");
            }
        }


        [HttpPut("{customerId}/{graphId}/global/{path}")]
        [HttpPost("{customerId}/{graphId}/global/{path}")]
        public void PutForGlobal(string customerId, int graphId, string path, [FromBody]JToken json)
        {
            // Ex: api/graphs/{customerId}/{graphId}/global/warningMessage
            // Body: "The new warning message"

            var graph = _context.DataGraph.First(i => i.CustomerId == customerId && i.Id == graphId);
            var bodyToken = json;

            var globalSchema = graph.Schema.Global;

            if (globalSchema.TryGetProperty(path, out DataGraphProperty prop))
            {
                PutIntoObject(customerId, graphId, "", graph.GlobalObjectId, graph.Schema.CustomTypes, prop, json);
            }
            else
            {
                throw new KeyNotFoundException("The property wasn't found");
            }
        }

        private void PutIntoObject(string customerId, int graphId, string userId, int objectId, IEnumerable<DataGraphClass> customTypes, DataGraphProperty prop, JToken bodyToken)
        {
            // Validate that type matches (note that array vs non array doesn't matter, put only allows adding a single item, not an array)
            AssertTypeMatches(bodyToken, prop);

            if (!prop.IsArray)
            {
                if (prop.IsCustomType())
                {
                    var customType = customTypes.First(i => i.ClassName == prop.Type);

                    DataGraphReferencePropertyValue literalReference = _context.ReferencePropertyValues.FirstOrDefault(i =>
                        i.CustomerId == customerId
                        && i.GraphId == graphId
                        && i.ObjectId == objectId
                        && i.PropertyName == prop.Name);

                    if (bodyToken.Type == JTokenType.Integer)
                    {
                        int objIdToReference = bodyToken.Value<int>();

                        AssertReferencedObject(customerId, graphId, userId, objIdToReference, customType.ClassName);

                        if (literalReference == null)
                        {
                            literalReference = new DataGraphReferencePropertyValue
                            {
                                CustomerId = customerId,
                                GraphId = graphId,
                                ObjectId = objectId,
                                PropertyName = prop.Name,
                                ReferencedObjectId = objIdToReference
                            };

                            _context.ReferencePropertyValues.Add(literalReference);
                        }
                        else
                        {
                            literalReference.ReferencedObjectId = objIdToReference;
                        }
                    }

                    else
                    {
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
                                ObjectId = objectId,
                                PropertyName = prop.Name
                            };

                            _context.ReferencePropertyValues.Add(literalReference);
                        }

                        literalReference.ReferencedObject = AddObject(customerId, graphId, "", customType, bodyToken as JObject);
                    }
                }

                else
                {
                    var existing = _context.LiteralPropertyValues.FirstOrDefault(i =>
                                        i.CustomerId == customerId
                                        && i.GraphId == graphId
                                        && i.ObjectId == objectId
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
                            ObjectId = objectId,
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
                        ObjectId = objectId,
                        PropertyName = prop.Name,
                        ListItemValueJson = bodyToken.ToString(Newtonsoft.Json.Formatting.None)
                    });
                }
                else
                {
                    var customType = customTypes.First(i => i.ClassName == prop.Type);

                    if (bodyToken.Type == JTokenType.Integer)
                    {
                        int objIdToAdd = bodyToken.Value<int>();

                        // Check object exists and is correct type and user has permission to reference it
                        AssertReferencedObject(customerId, graphId, userId, objIdToAdd, customType.ClassName);

                        _context.ListOfReferences.Add(new DataGraphListOfReferencesPropertyValue
                        {
                            CustomerId = customerId,
                            GraphId = graphId,
                            ObjectId = objectId,
                            PropertyName = prop.Name,
                            ReferencedObjectId = objIdToAdd
                        });
                    }

                    else
                    {
                        var dbObj = AddObject(customerId, graphId, "", customType, bodyToken as JObject);
                        _context.SaveChanges();

                        _context.ListOfReferences.Add(new DataGraphListOfReferencesPropertyValue
                        {
                            CustomerId = customerId,
                            GraphId = graphId,
                            ObjectId = objectId,
                            PropertyName = prop.Name,
                            ReferencedObject = dbObj
                        });
                    }
                }

                _context.SaveChanges();
            }
        }

        /// <summary>
        ///  Asserts that the object exists, is the correct type, and user has permission to reference it
        /// </summary>
        /// <param name="customerId"></param>
        /// <param name="graphId"></param>
        /// <param name="userId"></param>
        /// <param name="objectId"></param>
        /// <param name="objectType"></param>
        private void AssertReferencedObject(string customerId, int graphId, string userId, int objectId, string objectType)
        {
            if (!_context.Objects.Any(i =>
                i.CustomerId == customerId
                && i.GraphId == graphId
                && i.ObjectId == objectId
                && (i.UserId == "" || i.UserId == userId)
                && i.ObjectType == objectType))
            {
                throw new InvalidOperationException("Referenced object doesn't exist, it's the wrong type, or user doesn't have permission to access it");
            }
        }
    }
}
