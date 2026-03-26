using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.DataAccess.Helpers
{
    public class UserQuery : QueryObject
    {
        public string? Name { get; set; } = string.Empty;
    }
}
