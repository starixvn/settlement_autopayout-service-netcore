using System.Xml.Serialization;

namespace DirectPayout.Early.Models;

[XmlRoot("Resources")]
public record class Resources<T>([property: XmlElement("Resource")] List<T> ResourceItems);