
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AccountEx.Common
{
    public static class AutoMapperConfiguration
    {
        public static void Configure()
        {
            Initialize();

        }

        private static void Initialize()
        {
            //Mapper.CreateMap<RentChallanExta, RentDetailItem>();
            //Mapper.AssertConfigurationIsValid();
        }

        // ... etc
    }
}
