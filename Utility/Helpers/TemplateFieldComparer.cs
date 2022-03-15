using Documents_backend.Models;
using System.Collections.Generic;

namespace Documents_backend.Utility.Helpers
{
    public class TemplateFieldComparer : IComparer<TemplateField>
    {
        public int Compare(TemplateField x, TemplateField y)
        {
            if (x.TemplateTableId == y.TemplateTableId)
                return x.Order.CompareTo(y.Order);
            else if (x.TemplateTableId == null)
                return x.Order.CompareTo(y.TemplateTableId);
            else if (y.TemplateTableId == null)
                return x.TemplateId.CompareTo(y.Order);
            else 
                return x.Order.CompareTo(y.Order);
        }
    }
}