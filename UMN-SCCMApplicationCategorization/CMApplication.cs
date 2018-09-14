using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMN_SCCMApplicationCategorization {
    public class CMApplication {
        public string ApplicationName { get; set; }

        private List<string> applicationCategories;
        public List<string> ApplicationCategories {
            get {
                if(applicationCategories == null) {
                    applicationCategories = new List<string>();
                }
                return applicationCategories;
            }
            set {
                applicationCategories = value;
            }
        }
    }
}
