using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json.Serialization;
using System.Xml.Serialization;
using VDS.RDF;

namespace Gw2_WikiParser.Utility
{
    public class RdfGraphContainer
    {
        private Graph _graph;
        
        public string Xml { get; set; }

        public Graph GetGraph()
        {
            if (_graph == null)
            {
                _graph = new Graph();
                _graph.LoadFromString(Xml);
            }
            return _graph;
        }

        public RdfGraphContainer(string xml)
        {
            Xml = xml;
        }

        public RdfGraphContainer()
        {

        }
    }
}
