using RequirementGathering.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RequirementGathering.Helpers
{
    public class ProductComparer : IComparer<Product>
    {
        protected string sortName;
        protected bool isAsc;

        public ProductComparer(string sortName, bool isAsc)
        {
            this.sortName = sortName;
            this.isAsc = isAsc;
        }

        public int Compare(Product x, Product y)
        {
            if (sortName.Equals("IsActive"))
            {

                if (isAsc)
                {
                    return (x.IsActive.CompareTo(y.IsActive));
                }
                else
                {
                    return (y.IsActive.CompareTo(x.IsActive));
                }
            }

            if (sortName.Equals("Name"))
            {
                if (isAsc)
                {
                    if (x.Name == null)
                    {
                        return -1;
                    }
                    return (x.Name.CompareTo(y.Name));
                }
                else
                {
                    if (y.Name == null)
                    {
                        return 1;
                    }

                    return (y.Name.CompareTo(x.Name));
                }
            }


            return (x.ToString().CompareTo(y.ToString()));
        }
    }
}