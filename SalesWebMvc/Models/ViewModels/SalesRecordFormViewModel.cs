using System.Collections.Generic;

namespace SalesWebMvc.Models.ViewModels {
    public class SalesRecordFormViewModel {
        public SalesRecord SalesRecord { get; set; }
        public ICollection<Seller> Sellers { get; set; }
    }
}
