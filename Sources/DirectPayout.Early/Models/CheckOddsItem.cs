using System.Xml.Serialization;

namespace DirectPayout.Early.Models;

public class CheckOddsItem
{
    private readonly List<CheckOddsResourceItem> resourceIdItems = new List<CheckOddsResourceItem>();

    [XmlAttribute("MatchId")]
    public int MatchId { get; set; }

    [XmlElement("ResourceId")]
    public List<CheckOddsResourceItem> ResourceIdItems
    {
        get
        {
            ResourceIds.ForEach(item =>
            {
                resourceIdItems.Add(new CheckOddsResourceItem(item));
            });
            return resourceIdItems;
        }
    }

    [XmlIgnore]
    public List<string> ResourceIds { get; set; }
}
