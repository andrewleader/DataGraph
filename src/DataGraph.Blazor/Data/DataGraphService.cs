using DataGraph.Data;
using DataGraph.Helpers;
using DataGraph.Models;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DataGraph.Blazor.Data
{
    public class DataGraphService
    {
        private DataGraphContext _context;

        public DataGraphContext Context => _context;

        public DataGraphService()
        {
            _context = new DataGraphContext(Startup.DbConnectionString);
        }

        public DataGraphInstance[] GetGraphsForCustomer(AuthenticationState authState)
        {
            var customerId = authState.User.GetCustomerId();

            return _context.DataGraph.Where(i => i.CustomerId == customerId).ToArray();
        }

        public DataGraphInstance GetGraphForCustomer(AuthenticationState authState, int graphId)
        {
            var customerId = authState.User.GetCustomerId();

            return _context.DataGraph.FirstOrDefault(i => i.CustomerId == customerId && i.Id == graphId);
        }

        public int CreateGraphForCustomer(AuthenticationState authState, string graphName)
        {
            var dataGraph = new DataGraphInstance()
            {
                CustomerId = authState.User.GetCustomerId(),
                Name = graphName
            };

            _context.DataGraph.Add(dataGraph);

            // Configure the global object
            var globalObj = new DataGraphObject()
            {
                UserId = "",
                ObjectType = "Global",
                Graph = dataGraph
            };
            _context.Objects.Add(globalObj);

            _context.SaveChanges();

            dataGraph.GlobalObjectId = globalObj.ObjectId;

            _context.SaveChanges();

            // Respond with the graph ID
            return dataGraph.Id;
        }

        public void UpdateSchema(AuthenticationState authState, int graphId, DataGraphSchema schema)
        {
            var graph = GetGraphForCustomer(authState, graphId);
            graph.Schema = schema;
            graph.ApplySchemaChanges();
            _context.SaveChanges();
        }
    }
}
