using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;

namespace Banananana
{
    /// <summary>
    /// Workspace containing all data, serialized/deserialized to/from disk (in json format)
    /// </summary>
    public class Workspace
    {
        /// <summary>
        /// A link to an external site
        /// </summary>
        public class ExternalLink
        {
            [JsonProperty]
            public String Target { get; set; }

            public ExternalLink()
            {
                Target = "http://www.google.com";
            }
        }

        /// <summary>
        /// A single task, can contain external links
        /// </summary>
        public class Task
        {
            [JsonProperty]
            public String Text { get; set; }

            [JsonProperty]
            public List<ExternalLink> ExternalLinks { get; set; }

            public Task()
            {
                ExternalLinks = new List<ExternalLink>();
            }
        }

        /// <summary>
        /// A pile of tasks, with a title
        /// </summary>
        public class Pile
        {
            [JsonProperty]
            public String Title { get; set; }

            [JsonProperty]
            public System.Windows.Media.Color Color { get; set; }

            [JsonProperty]
            public List<Task> Tasks { get; set; }

            public Pile()
            {
                Tasks = new List<Task>();
                Color = System.Windows.Media.Color.FromArgb(255,238,173,0);
            }
        }
        
        /// <summary>
        /// All the task piles that the user has defined
        /// </summary>
        public List<Pile> Piles { get; set; }


        public Workspace()
        {
            Piles = new List<Pile>();
        }

        /// <summary>
        /// Serialize our complete workspace to disk
        /// </summary>
        /// <param name="inFilename"></param>
        public void SaveToFile(String inFilename)
        {
            // Ensure path exists
            Directory.CreateDirectory(Path.GetDirectoryName(inFilename));

            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.Formatting = Formatting.Indented;

            using (StreamWriter sw = new StreamWriter(inFilename))
            using (JsonWriter writer = new JsonTextWriter(sw))
            {
                serializer.Serialize(writer, this);
            }
        }

        /// <summary>
        /// Load a workspace from disk
        /// </summary>
        /// <param name="inFilename"></param>
        /// <returns></returns>
        public static Workspace LoadFromFile(String inFilename)
        {
            Workspace data = new Workspace();

            if (!File.Exists(inFilename))
                return data;

            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.Formatting = Formatting.Indented;

            using (StreamReader sr = new StreamReader(inFilename))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                data = serializer.Deserialize<Workspace>(reader);
            }

            return data;
        }

        /// <summary>
        /// Utility function to extract content from a FlowDocument object as a single string of XML data (utf-8 encoded)
        /// </summary>
        /// <param name="inDocument"></param>
        /// <returns></returns>
        public static String GetFlowDocumentContentAsXML(FlowDocument inDocument)
        {
            using (var stream = new MemoryStream())
            {
                TextRange range = new TextRange(inDocument.ContentStart, inDocument.ContentEnd);
                range.Save(stream, DataFormats.Xaml);

                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

        /// <summary>
        /// Utility function to load the content of a FlowDocument object from a single string of XML data (utf-8 encoded)
        /// </summary>
        /// <param name="inDocument"></param>
        /// <param name="inTextXML"></param>
        public static void SetFlowDocumentContentFromXML(FlowDocument inDocument, String inTextXML)
        {            
            byte[] data = Encoding.UTF8.GetBytes(inTextXML);
            using (var stream = new MemoryStream(data))
            {
                TextRange range = new TextRange(inDocument.ContentStart, inDocument.ContentEnd);
                range.Load(stream, DataFormats.Xaml);
            }                       
        }
    }
}
