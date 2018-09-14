using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UMN_SCCMApplicationCategorization {
    public class CheckedListBoxItem {
        public string Name { get; set; }
        public bool IsChecked { get; set; } = true;

        public CheckedListBoxItem(string name, bool isChecked) {
            Name = name;
            IsChecked = isChecked;
        }

        public static implicit operator CheckedListBoxItem(string str) {
            return new CheckedListBoxItem( str, false );
        }
    }
}
