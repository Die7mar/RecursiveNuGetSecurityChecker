using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecursiveNuGetSecurityChecker.DataTransferObject
{
    public class TeamsWebHookClass
    {
        public string Type = "MessageCard";
        public string Context = "https://schema.org/extensions";
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        public string Summary { get; set; }

        public string ThemeColor { get; set; } //0072C6
        public string Title { get; set; }
        public List<SectionsObj> Sections { get; set; }

        public class SectionsObj
        {
            public List<Facts> Facts { get; set; }
            public string Text { get; set; }
        }

        public class Facts
        {
            public string Name { get; set; }
            public string Value { get; set; }
            public Facts(string name, string value)
            {
                this.Name = name;
                this.Value = value;
            }
        }
    }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
}
