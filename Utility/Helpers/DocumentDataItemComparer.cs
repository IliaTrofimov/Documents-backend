using Documents_backend.Models;
using System.Collections.Generic;

namespace Documents_backend.Utility.Helpers
{
    public class DocumentDataItemComparer : IComparer<DocumentDataItem>
    {
        public int Compare(DocumentDataItem x, DocumentDataItem y)
        {
            if (x.Field == y.Field)
            {           
                if (x.Row != y.Row)
                    return (int)(x.Row - y.Row);
                else
                    return (int)(x.Col - y.Col);
            }   
            else 
                return x.Field.CompareTo(y.Field);
        }
    }
}