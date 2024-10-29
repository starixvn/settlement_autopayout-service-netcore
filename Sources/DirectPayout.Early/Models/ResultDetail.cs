using System.Xml.Serialization;

namespace DirectPayout.Early.Models;

public class ResultDetail
{
    [XmlIgnore]
    public int MatchId { get; set; }

    [XmlIgnore]
    public int LeagueId { get; set; }

    [XmlAttribute("Id")]
    public string ResourceId { get; set; }

    [XmlAttribute("Home")]
    public string HomeValue { get; set; }

    [XmlAttribute("Away")]
    public string AwayValue { get; set; }

    [XmlIgnore]
    public DateTime LastModifyDate { get; set; }

    [XmlIgnore]
    public int WaitingTime { get; set; }
}
