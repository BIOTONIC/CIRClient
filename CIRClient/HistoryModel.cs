namespace CIRClient
{
    public class HistoryModel
    {
        public string time { get; set; }
        public string person { get; set; }
        public string devType { get; set; }
        public string version { get; set; }
        public string desc { get; set; }
        public string file1 { get; set; }
        public string file2 { get; set; }
        public int isSuccess { get; set; }

        public HistoryModel()
        {
        }

        public HistoryModel(string time, string person, string devType, string version, string desc, string file1, string file2, int isSuccess)
        {
            this.time = time;
            this.person = person;
            this.devType = devType;
            this.version = version;
            this.desc = desc;
            this.file1 = file1;
            this.file2 = file2;
            this.isSuccess = isSuccess;
        }
    }
}
