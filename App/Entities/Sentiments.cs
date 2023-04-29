namespace App.Entities
{
    public class Sentiments
    {
        public int Sentiment { get; set; }
        public string? SentimentDescription { get; set; }
        public int StaffID { get; set; }
        public int PostID { get; set; }
    }
}
