namespace Gymora.Models
{
    public class Gym
    {
        public string Name { get; set; }
        public string _2GISLink { get; set; }
        public double Rating { get; set; } 
        public List<string> Images { get; set; }
        public string Location { get; set; }
        public string District { get; set; }
        public List<PriceOption> Prices { get; set; }
        public List<string> WorkingHours { get; set; }
        public string Number { get; set; }
        public string Website { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public bool MatchesFilters(List<string> selectedDistricts,
                                 Dictionary<string, (double? min, double? max)> priceFilters,
                                 bool highRatingOnly)
        {
            if (selectedDistricts != null && selectedDistricts.Any() && !selectedDistricts.Contains(District))
                return false;

            if (highRatingOnly && Rating < 4.6)
                return false;

            if (priceFilters != null && priceFilters.Any())
            {
                bool hasMatchingPrice = false;

                foreach (var filter in priceFilters)
                {
                    string periodKey = GetPeriodKey(filter.Key);

                    foreach (var priceOption in Prices)
                    {
                        foreach (var price in priceOption.Prices)
                        {
                            if (PeriodMatches(price.Key, periodKey))
                            {
                                if ((!filter.Value.min.HasValue || price.Value >= filter.Value.min) &&
                                    (!filter.Value.max.HasValue || price.Value <= filter.Value.max))
                                {
                                    hasMatchingPrice = true;
                                    break;
                                }
                            }
                        }
                        if (hasMatchingPrice) break;
                    }
                    if (!hasMatchingPrice) return false;
                }
            }

            return true;
        }

        private string GetPeriodKey(string filterPeriod)
        {
            switch (filterPeriod)
            {
                case "разовое": return "1 месяц"; 
                case "1 месяц": return "1 месяц";
                case "полгода": return "6 месяцев";
                case "год": return "12 месяцев";
                default: return filterPeriod;
            }
        }

        private bool PeriodMatches(string pricePeriod, string filterPeriod)
        {
            return pricePeriod.Contains(filterPeriod) || filterPeriod.Contains(pricePeriod);
        }
    }

    public class PriceOption
    {
        public string Name { get; set; }
        public Dictionary<string, int> Prices { get; set; }
    }
}
