using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Attribute = RequirementGathering.Models.Attribute;

namespace RequirementGathering.Helpers
{
    public class AttributesIgnoreCaseComparer : IEqualityComparer<Attribute>
    {
        public CaseInsensitiveComparer myComparer;

        public AttributesIgnoreCaseComparer(CultureInfo myCulture)
        {
            myComparer = new CaseInsensitiveComparer(myCulture);
        }

        #region IEqualityComparer<Attribute> Members

        public bool Equals(Attribute x, Attribute y)
        {
            if (myComparer.Compare(x.Name, y.Name) == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetHashCode(Attribute obj)
        {
            return obj.Name.ToLower().GetHashCode();
        }

        #endregion
    }
}
