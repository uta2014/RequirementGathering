using RequirementGathering.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RequirementGathering.Helpers
{
    public class EvaluationComparer : IComparer<Evaluation>
    {
        protected string sortName;
        protected bool isAsc;

        public EvaluationComparer(string sortName, bool isAsc)
        {
            this.sortName = sortName;
            this.isAsc = isAsc;
        }

        public int Compare(Evaluation x, Evaluation y)
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

            if (sortName.Equals("ProductName"))
            {

                if (isAsc)
                {
                    if (x.Product.Name == null)
                    {
                        return -1;
                    }
                    return (x.Product.Name.CompareTo(y.Product.Name));
                }
                else
                {
                    if (y.Product.Name == null)
                    {
                        return 1;
                    }
                    return (y.Product.Name.CompareTo(x.Product.Name));
                }
            }

            if (sortName.Equals("Description"))
            {

                if (isAsc)
                {
                    if (x.Description == null)
                    {
                        return -1;
                    }
                    return (x.Description.CompareTo(y.Description));
                }
                else
                {
                    if (y.Description == null)
                    {
                        return 1;
                    }
                    return (y.Description.CompareTo(x.Description));
                }
            }

            if (sortName.Equals("Version"))
            {
                if (isAsc)
                {
                    if (x.Version == null)
                    {
                        return -1;
                    }
                    return (x.Version.CompareTo(y.Version));
                }
                else
                {
                    if (y.Version == null)
                    {
                        return 1;
                    }
                    return (y.Version.CompareTo(x.Version));
                }
            }


            return (x.ToString().CompareTo(y.ToString()));
        }
    }
}