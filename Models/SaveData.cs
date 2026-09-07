using System.Collections.Generic;

namespace ChumFrenzy.Models
{
    public class ModSaveData
    {
        public int Version { get; set; } = 1;
        public List<ChumHotspot> Hotspots { get; set; } = new List<ChumHotspot>();
    }
}
