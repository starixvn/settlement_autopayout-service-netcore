using System.Xml.Serialization;

namespace DirectPayout.Early.Models;

public class ResultDetail
{
    public int MatchId { get; set; }

    public int LeagueId { get; set; }

    [XmlAttribute("Id")]
    public string ResourceId { get; set; }

    [XmlAttribute("Home")]
    public string HomeValue { get; set; }

    [XmlAttribute("Away")]
    public string AwayValue { get; set; }

    public DateTime LastModifyDate { get; set; }

    public int WaitingTime { get; set; }
}
