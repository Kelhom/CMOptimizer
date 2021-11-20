using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CryptoMinesOptimizer.Models
{
    public class EternalPriceResponse
    {
        [JsonProperty("price")]
        public double Price { get; set; }
    }
}
