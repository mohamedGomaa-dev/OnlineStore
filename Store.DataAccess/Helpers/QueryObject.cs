using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.DataAccess.Helpers
{
    public class QueryObject
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 20;
        public string? OrderBy { get; set; } // I know it is not optimal but I will use validation before sorting
        public bool IsDescending { get; set; }
    }
}
