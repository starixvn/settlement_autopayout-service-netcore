using System.Xml.Serialization;

namespace DirectPayout.Early.Models;
public class CheckOddsResourceItem
{
    [XmlAttribute("Id")]
    public string ResourceId { get; set; }
}