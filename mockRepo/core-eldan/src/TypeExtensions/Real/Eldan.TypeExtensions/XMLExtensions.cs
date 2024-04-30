using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Eldan.TypeExtensions
{
    public static class XMLExtensions
    {
        public static IEnumerable<XElement> ElementsByPath(this XContainer source, params XName[] PathXNames)
        {
            return GetNodesByPath(source, false, PathXNames);
        }

        public static IEnumerable<XElement> DescendantsByPath(this XContainer source, params XName[] PathXNames)
        {
            return GetNodesByPath(source, true, PathXNames);
        }

        private static IEnumerable<XElement> GetNodesByPath(XContainer StatContainer, bool UseDescendans, params XName[] PathXNames)
        {
            List<XElement> CandidateNodes = new List<XElement>();

            if (StatContainer == null || PathXNames == null)
                return CandidateNodes;

            if (PathXNames.Count() > 0)
            {
                IEnumerable<XElement> nodes;
                if (UseDescendans)
                    nodes = StatContainer.Descendants(PathXNames[0]);
                else
                    nodes = StatContainer.Elements(PathXNames[0]);

                foreach (XElement node in nodes)
                {
                    GetCadidateNodes(node, 1, ref CandidateNodes, PathXNames);
                }
            }

            return (IEnumerable<XElement>)CandidateNodes;
        }

        public static string GetAttributeValue(this XElement element, XName AttName)
        {
            XAttribute Att = element.Attribute(AttName);
            if (Att == null)
                return null;
            else
                return Att.Value;
        }

        public static T GetAttributeValue<T>(this XElement element, XName AttName)
        {
            if (element == null)
                return StringExtensions.GetTypeDefualt<T>();
            else
            {
                XAttribute Att = element.Attribute(AttName);
                if (Att == null)
                    return StringExtensions.GetTypeDefualt<T>();
                else
                    return Att.Value.GetTypeValue<T>();
            }
        }

        private static void GetCadidateNodes(XElement Node, int NextIndex, ref List<XElement> Nodes, params XName[] PathXNames)
        {
            if (NextIndex == PathXNames.Count())
                Nodes.Add(Node);
            else
            {
                List<XElement> Children = Node.Elements(PathXNames[NextIndex]).ToList<XElement>();
                foreach (XElement Child in Children)
                {
                    if (Child.Name == PathXNames[NextIndex])
                        GetCadidateNodes(Child, NextIndex + 1, ref Nodes, PathXNames);
                }
            }
        }
        public static XDocument ParseJSON(string text, bool addRoot)
        {
            return ParseJSON(text, addRoot ? "root" : null);
        }

        public static XDocument ParseJSON(string text, string root)
        {
            return new XDocument().ParseJSON(text, root);
        }

        public static XDocument ParseJSON(this XDocument source, string text, string root)
        {
            //if (!string.IsNullOrWhiteSpace(root))
            //    text = "{\""+ root + "\": " + text + "}";

            //return XDocument.Parse(JsonConvert.DeserializeXmlNode(text).InnerXml);
            return XDocument.Parse(text.JSONtoXML(root));

        }

        public static T GetElementValue<T>(this XElement element)
        {
            return GetElementValue(element, StringExtensions.GetTypeDefualt<T>());
        }

        public static T GetElementValue<T>(this XElement element, bool returnDefaulInFailure)
        {
            return GetElementValue(element, returnDefaulInFailure, StringExtensions.GetTypeDefualt<T>());
        }

        public static T GetElementValue<T>(this XElement element, T defaultValue)
        {
            return GetElementValue(element, true, defaultValue);
        }

        public static T GetElementValue<T>(this XElement element, bool returnDefaulInFailure, T defaultValue)
        {
            if (element == null)
                return defaultValue;
            else
                return element.Value.Trim().GetTypeValue(returnDefaulInFailure, defaultValue);
        }
    }
}
