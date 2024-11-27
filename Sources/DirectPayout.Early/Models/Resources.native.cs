using System.Xml.Serialization;

namespace DirectPayout.Early.Models;

[XmlRoot("Resources")]
public class Resources<T>
{
    [XmlElement("Resource")]
    public List<T> ResourceItems { get; set; }
}