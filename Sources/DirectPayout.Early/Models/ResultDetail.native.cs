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
    public string RelatedCalResources { get; set; }

    [XmlIgnore]
    public List<string> RelatedCalResourceIds => string.IsNullOrEmpty(RelatedCalResources) ? [] : [.. RelatedCalResources.Split(",")];

    [XmlIgnore]
    public string RelatedSettResources { get; set; }

    [XmlIgnore]
    public List<string> RelatedSettResourcesIds => string.IsNullOrEmpty(RelatedSettResources) ? [] : [.. RelatedSettResources.Split(",")];

    [XmlIgnore]
    public byte ResultStatusId { get; set; }

    [XmlIgnore]
    public short SettlementStatusId { get; set; }

    [XmlIgnore]
    public bool Expired { get; set; }


    [XmlIgnore]
    public bool IsInvalid { get; set; }

    public override string ToString() => $"{ResourceId}({HomeValue}-{AwayValue}) has" +
        $" {(!string.IsNullOrEmpty(RelatedCalResources) ? $"RelatedCalResources({RelatedCalResources})" : "")}" +
        $" {(!string.IsNullOrEmpty(RelatedSettResources) ? $"RelatedSettResources({RelatedSettResources})" : "")}";
}