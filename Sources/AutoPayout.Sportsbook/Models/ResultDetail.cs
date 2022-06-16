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

        public int? HomeValue { get; set; }

        public int? AwayValue { get; set; }

        public byte ResultStatusId { get; set; }

        public int ChildMatchType { get; set; }
    }
}
