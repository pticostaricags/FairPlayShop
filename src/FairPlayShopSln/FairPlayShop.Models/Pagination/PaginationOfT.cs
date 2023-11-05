﻿using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FairPlayShop.Models.Pagination
{
    public class PaginationOfT<T>
    {
        public int TotalItems { get; set; }
        public T[]? Items { get; set; }
        public int TotalPages { get; set; }
        public int PageSize { get; set; }
    }
}
