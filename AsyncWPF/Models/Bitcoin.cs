using Newtonsoft.Json;

namespace AsyncWPF.Models
{
    public class Bitcoin
    {
        [JsonProperty("time")]
        public Time Time { get; set; }

        [JsonProperty("disclaimer")]
        public string Disclaimer { get; set; }

        [JsonProperty("chartName")]
        public string ChartName { get; set; }

        [JsonProperty("bpi")]
        public Bpi Bpi { get; set; }

        public override string ToString()
        {
            return $"{Time.Updated}, {Bpi}";
        }
    }

    public class Time
    {
        [JsonProperty("updated")]
        public string Updated { get; set; }

        [JsonProperty("updatedISO")]
        public DateTime UpdatedISO { get; set; }

        [JsonProperty("updateduk")]
        public string Updateduk { get; set; }
    }

    public class Bpi
    {
        [JsonProperty("USD")]
        public Currency USD { get; set; }

        [JsonProperty("GBP")]
        public Currency GBP { get; set; }

        [JsonProperty("EUR")]
        public Currency EUR { get; set; }

        public override string ToString()
        {
            return $"{USD}, {EUR}, {GBP}";
        }
    }

    public class Currency
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }

        [JsonProperty("rate")]
        public string Rate { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("rate_float")]
        public double RateFloat { get; set; }

        public override string ToString()
        {
            return $"{Code}: {RateFloat}";
        }
    }
}