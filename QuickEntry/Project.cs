using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;

namespace QuickEntry
{

    public class ProjectTask
    {
        public Guid id { get; set; }
        public string name { get; set; }
        public override string ToString() { return name; }

    }

    public class Project
    {
        public Guid id { get; set; }    
        public Guid bookableResource { get; set; }
        public string name { get; set; }
        public List<ProjectTask> tasks { get; set; } = new List<ProjectTask>();

        public override string ToString() { return name; }
    }
}
