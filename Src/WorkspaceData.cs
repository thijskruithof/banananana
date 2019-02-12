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
    public class WorkspaceData
    {
        public class ExternalLink
        {
            [JsonProperty]
            public String Target { get; set; }
        }

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
        
        public List<Pile> Piles { get; set; }


        public WorkspaceData()
        {
            Piles = new List<Pile>();
        }

        public void SafeToFile(String inFilename)
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

        public static WorkspaceData LoadFromFile(String inFilename)
        {
            WorkspaceData data = new WorkspaceData();

            if (!File.Exists(inFilename))
                return data;

            JsonSerializer serializer = new JsonSerializer();
            serializer.Converters.Add(new JavaScriptDateTimeConverter());
            serializer.NullValueHandling = NullValueHandling.Ignore;
            serializer.Formatting = Formatting.Indented;

            using (StreamReader sr = new StreamReader(inFilename))
            using (JsonReader reader = new JsonTextReader(sr))
            {
                data = serializer.Deserialize<WorkspaceData>(reader);
            }

            return data;
        }

        public static String GetFlowDocumentContentAsXML(FlowDocument inDocument)
        {
            using (var stream = new MemoryStream())
            {
                TextRange range = new TextRange(inDocument.ContentStart, inDocument.ContentEnd);
                range.Save(stream, DataFormats.Xaml);

                return Encoding.UTF8.GetString(stream.ToArray());
            }
        }

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
