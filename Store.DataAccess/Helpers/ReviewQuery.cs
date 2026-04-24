using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.DataAccess.Helpers
{
    public class ReviewQuery : QueryObject
    {
        public int ProductId { get; set; } // filter reviews by product id (must)
        [Range(1, 5)]
        public decimal? Rating { get; set; } // Filter reviews by rating (1 to 5)
    }
}
