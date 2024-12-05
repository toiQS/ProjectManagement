using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.position
{
    public class DetailPosition
    {
        public string PositionId { get; set; } = string.Empty;
        public string PositionName { get; set; } = string.Empty;
        public string PrositionDescription { get; set; } = string.Empty;
        public int TotalTask {  get; set; } = 0;
        public int DoneTask { get; set; } = 0;
    }
}
