using ShipRate.Domain.Entities.Base;

namespace ShipRate.Domain.Entities.BaseInfo.Cargos
{
    public class DG : BusinessEntity
    {
        public int DGClassId { get; set; }
        public string DGSubClassNo { get; set; } = string.Empty;
        public string DGUnNumber { get; set; } = string.Empty;
        public string DGName { get; set; } = string.Empty;
        public string DGLocalName { get; set; } = string.Empty;
        public int DGPackingGroupId { get; set; }
        public int DGRegulationSetId { get; set; }


        //Relations
        public DGClass DGClass { get; set; }
        public DGPackingGroup DGPackingGroup { get; set; }
        public DGRegulationSet DGRegulationSet { get; set; }
    }
}


/*فقط برای رفرنس میباشند . بهتره در جداولی که مورد استفاده قرار میگیرند کلاس و یو ان و پکینگ ذکر ش.ود
 * 
 * 3  3.1
 * 
 * 
 */