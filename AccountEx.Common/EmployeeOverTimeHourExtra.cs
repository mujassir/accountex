﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Common
{
    public class EmployeeOverTimeHourExtra
    {
        public int AccountId { get; set; }
        public string Name { get; set; }
        public int Month { get; set; }
        public int Year { get; set; }
        public int OverTimeTypeId { get; set; }
        public Nullable<System.DateTime> FromDate { get; set; }
        public Nullable<System.DateTime> ToDate { get; set; }
        public int Hours { get; set; }
        public decimal Rate { get; set; }
        public Nullable<decimal> Amount { get; set; }
    }
}
