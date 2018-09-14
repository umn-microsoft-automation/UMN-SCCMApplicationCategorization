using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMN_SCCMApplicationCategorization {
    public class CMApplicationCategory {
        public string CategoryInstance_UniqueID { get; set; }
        public int CategoryInstanceID { get; set; }
        public string CategoryTypeName { get; set; }
        public string LocalizedCategoryInstanceName { get; set; }
        public int ParentCategoryInstanceID { get; set; }
        public string SourceSite { get; set; }
    }
}
