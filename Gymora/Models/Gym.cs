namespace Gymora.Models
{
    public class Gym
    {
        public string Name { get; set; }
        public string _2GISLink { get; set; }
        public string Rating { get; set; } 
        public List<string> Images { get; set; }
        public string Location { get; set; }
        public List<PriceOption> Prices { get; set; } 
        public List<string> WorkingHours { get; set; }
        public string Number { get; set; }
        public string Website { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class PriceOption
    {
        public string Name { get; set; }
        public Dictionary<string, int> Prices { get; set; }
    }
}
