namespace EasySave.lib.Models
{
    public class LogModel
    {
        public string Name { get; set; }
        public string FileSource { get; set; }
        public string FileTarget { get; set; }
        public string DestPath { get; set; }
        public int TimeToCrypt { get; set; }
        public long FileSize { get; set; }
        public double FileTransferTime { get; set; }
        public DateTime Time { get; set; }
    }
}