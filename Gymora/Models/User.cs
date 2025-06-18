namespace Gymora.Models
{
    public class User
    {
        public string BirthdateString { get; set; }
        public string Gym { get; set; }
        public string Goal { get; set; }
        public List<string> SelectedDays { get; set; }
        public string PhotoBase64 { get; set; }
    }
}