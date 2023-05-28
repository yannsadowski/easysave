namespace EasySave.DistentClient.Models
{
    public class ModelForSocket
    {
        public string NameSaveWork { get; set; }

        public int TypeSaveWork { get; set; }

        public string SourcePathSaveWork { get; set; }

        public string DestinationPathSaveWork { get; set; }

        public string ProgressState { get; set; }

        public float Percentage { get; set; }
    }
}