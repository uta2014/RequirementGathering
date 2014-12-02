using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Attribute = RequirementGathering.Models.Attribute;

namespace RequirementGathering.Helpers
{
    public class AttributesIgnoreCaseComparer : IEqualityComparer<Attribute>
    {
        private CaseInsensitiveComparer _caseInsesitiveComparer;

        public AttributesIgnoreCaseComparer(CultureInfo myCulture)
        {
            _caseInsesitiveComparer = new CaseInsensitiveComparer(myCulture);
        }

        #region IEqualityComparer<Attribute> Members

        public bool Equals(Attribute x, Attribute y)
        {
            if (_caseInsesitiveComparer.Compare(x.Name.Trim(), y.Name.Trim()) == 0)
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
            return obj.Name.Trim().ToLower().GetHashCode();
        }

        #endregion
    }
}
