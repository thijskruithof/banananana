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
        /// A single task, can contain external links and notes
        /// </summary>
        public class Task
        {
            [JsonProperty]
            public String Text { get; set; }

            [JsonProperty]
            public List<ExternalLink> ExternalLinks { get; set; }

            [JsonProperty]
            public String Notes { get; set; }

            public const String cNewNotesText = "<Section xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xml:space=\"preserve\" TextAlignment=\"Left\" LineHeight=\"Auto\" IsHyphenationEnabled=\"False\" xml:lang=\"en-us\" FlowDirection=\"LeftToRight\" NumberSubstitution.CultureSource=\"Text\" NumberSubstitution.Substitution=\"AsCulture\" FontFamily=\"/Banananana;component/Resources/#Nunito\" FontStyle=\"Normal\" FontWeight=\"Normal\" FontStretch=\"Normal\" FontSize=\"12\" Foreground=\"#FF000000\" Typography.StandardLigatures=\"True\" Typography.ContextualLigatures=\"True\" Typography.DiscretionaryLigatures=\"False\" Typography.HistoricalLigatures=\"False\" Typography.AnnotationAlternates=\"0\" Typography.ContextualAlternates=\"True\" Typography.HistoricalForms=\"False\" Typography.Kerning=\"True\" Typography.CapitalSpacing=\"False\" Typography.CaseSensitiveForms=\"False\" Typography.StylisticSet1=\"False\" Typography.StylisticSet2=\"False\" Typography.StylisticSet3=\"False\" Typography.StylisticSet4=\"False\" Typography.StylisticSet5=\"False\" Typography.StylisticSet6=\"False\" Typography.StylisticSet7=\"False\" Typography.StylisticSet8=\"False\" Typography.StylisticSet9=\"False\" Typography.StylisticSet10=\"False\" Typography.StylisticSet11=\"False\" Typography.StylisticSet12=\"False\" Typography.StylisticSet13=\"False\" Typography.StylisticSet14=\"False\" Typography.StylisticSet15=\"False\" Typography.StylisticSet16=\"False\" Typography.StylisticSet17=\"False\" Typography.StylisticSet18=\"False\" Typography.StylisticSet19=\"False\" Typography.StylisticSet20=\"False\" Typography.Fraction=\"Normal\" Typography.SlashedZero=\"False\" Typography.MathematicalGreek=\"False\" Typography.EastAsianExpertForms=\"False\" Typography.Variants=\"Normal\" Typography.Capitals=\"Normal\" Typography.NumeralStyle=\"Normal\" Typography.NumeralAlignment=\"Normal\" Typography.EastAsianWidths=\"Normal\" Typography.EastAsianLanguage=\"Normal\" Typography.StandardSwashes=\"0\" Typography.ContextualSwashes=\"0\" Typography.StylisticAlternates=\"0\"><Paragraph><Run> </Run></Paragraph></Section>";

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
