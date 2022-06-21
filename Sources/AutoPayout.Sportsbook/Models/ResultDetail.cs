using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoPayout.Sportsbook.Models
{
    public class ResultDetail
    {
        public string ResourceId { get; set; }

        public string HomeValue { get; set; }

        public string AwayValue { get; set; }

        public int ChildMatchType { get; set; }

        public byte SettlementStatusId { get; set; }
    }
}
