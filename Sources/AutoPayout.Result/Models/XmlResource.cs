namespace AutoPayout.Result.Models;

public class XmlResource
{
    [XmlAttribute("Id")]
    public string ResourceId { get; set; }

    [XmlAttribute]
    public string HomeValue { get; set; }

    [XmlAttribute]
    public int SettlementStatusId { get; set; }

    [XmlAttribute]
    public int ResultStatusId { get; set; }
}