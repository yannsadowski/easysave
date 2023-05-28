namespace EasySave.lib.Models
{
    public class CopyModel
    {
        public string SourcePath { get; set; }
        public string DestinationPath { get; set; }
        public int TotalFilesToCopy { get; set; }
        public long TotalFilesSizeToCopy { get; set; }
        public int NbFilesLeft { get; set; }
        public long FilesSizeLeft { get; set; }
        public float Percentage { get; set; }
    }
}