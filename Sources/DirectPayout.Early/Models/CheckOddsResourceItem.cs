using System.Xml.Serialization;

namespace DirectPayout.Early.Models;
public record class CheckOddsResourceItem([property: XmlAttribute("Id")] string ResourceId);