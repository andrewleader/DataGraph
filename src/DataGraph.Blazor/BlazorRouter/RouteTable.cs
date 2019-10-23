using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BlazorRouter
{
    internal class RouteTable
    {
        private readonly List<RouteEntry> routes = new List<RouteEntry>();

        public void Add(string templateText, bool matchChildren, RenderFragment fragment)
        {
            var template = TemplateParser.ParseTemplate(templateText);
            var entry = new RouteEntry(template, matchChildren, fragment);
            routes.Add(entry);
        }

        internal void Route(RouteContext routeContext)
        {
            foreach (var route in routes)
            {
                route.Match(routeContext);
                if (routeContext.Fragment != null)
                {
                    return;
                }
            }
        }
    }
}
