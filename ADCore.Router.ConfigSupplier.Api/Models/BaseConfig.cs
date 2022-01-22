using System;
using System.Collections.Generic;

namespace ADCore.Router.ConfigSupplier.Api.Models
{
    public class BaseConfig
    {
        public BaseConfig()
        {
            ConfigDetails = new List<ConfigDetail>();
        }
        public List<ConfigDetail> ConfigDetails { get; set; }
    }
     
}
