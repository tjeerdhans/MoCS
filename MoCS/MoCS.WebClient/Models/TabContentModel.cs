namespace MoCS.WebClient.Models
{
    public class TabContentModel
    {
        public string Name { get; set; }
        
        /// <summary>
        /// Plain text or C#. Determines the syntax coloring.
        /// </summary>
        public string ContentType { get; set; }

        public string Content { get; set; }
    }
}
