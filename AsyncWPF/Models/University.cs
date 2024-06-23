using Newtonsoft.Json;

namespace AsyncWPF.Models;

public class University
{
    public string Name { get; set; }

    [JsonProperty("state-province")]
    public string StateProvince { get; set; }

    public string Country { get; set; }

    [JsonProperty("alpha_two_code")]
    public string Code { get; set; }

    public string? WebPage => WebPages.FirstOrDefault();

    [JsonProperty("web_pages")]
    public List<string> WebPages { get; set; }

    public List<string> Domains { get; set; }
}
