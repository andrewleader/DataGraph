using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DataGraph.Models
{
    public class DataGraphSchema
    {
        public DataGraphClass User { get; set; } = new DataGraphClass()
        {
            ClassName = "User"
        };

        public DataGraphClass Global { get; set; } = new DataGraphClass()
        {
            ClassName = "Global"
        };

        public List<DataGraphClass> CustomTypes { get; set; } = new List<DataGraphClass>();

        public void Validate()
        {
            var knownTypes = CustomTypes.Select(i => i.ClassName).Concat(DataGraphHelpers.Literals).Concat(new string[]
            {
                "User",
                "Global"
            }).ToArray();

            // Assert all custom type names are unique
            if (knownTypes.Distinct().Count() != knownTypes.Count())
            {
                throw new InvalidOperationException("Duplicate type names. All custom types must be unique.");
            }

            foreach (var customType in CustomTypes)
            {
                customType.Validate(knownTypes);
            }

            User.Validate(knownTypes);
            Global.Validate(knownTypes);
        }
    }

    public static class DataGraphHelpers
    {
        public static bool IsNameValid(string name)
        {
            Regex regexNames = new Regex("^[a-zA-Z]+$");
            return regexNames.IsMatch(name);
        }

        public static string[] Literals = new string[] { "string", "int", "decimal" };
    }

    public class DataGraphClass : DataGraphType
    {
        public string ClassName { get; set; }

        public List<DataGraphProperty> Properties = new List<DataGraphProperty>();

        public void Validate(string[] knownTypes)
        {
            if (Properties.Select(i => i.Name).Distinct().Count() != Properties.Count())
            {
                throw new InvalidOperationException($"Type {ClassName} had duplicate property names.");
            }

            if (!DataGraphHelpers.IsNameValid(ClassName))
            {
                throw new InvalidOperationException($"Invalid type name {ClassName}");
            }

            foreach (var prop in Properties)
            {
                prop.Validate(knownTypes);
            }
        }
    }

    public abstract class DataGraphType
    {

    }

    public class DataGraphString
    {

    }

    public class DataGraphInt
    {

    }

    public class DataGraphDecimal
    {

    }

    public class DataGraphProperty
    {
        public string Name { get; set; }

        /// <summary>
        /// string, int, decimal, and custom types supported
        /// </summary>
        public string Type { get; set; }

        public bool IsArray { get; set; }

        public void Validate(string[] knownTypes)
        {
            // Assert all names are unique and specified and only contain letters
            if (!DataGraphHelpers.IsNameValid(Name))
            {
                throw new InvalidOperationException($"Invalid property name {Name}");
            }

            if (!knownTypes.Contains(Type))
            {
                throw new InvalidOperationException($"Type {Type} on property {Name} is unknown");
            }
        }

        public bool IsCustomType()
        {
            return !DataGraphHelpers.Literals.Contains(Type);
        }

        /// <summary>
        /// Returns in format of {Name}: string[]
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{Name}: {Type}" + (IsArray ? "[]" : "");
        }
    }
}
