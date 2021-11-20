using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CryptoMinesOptimizer.Models
{
    public class Spaceship
    {
        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("roll")]
        public int Roll { get; set; }

        [JsonProperty("level")]
        public int Level { get; set; }

        [JsonProperty("workers")]
        public int Workers { get; set; }

        [JsonProperty("firstName")]
        public int FirstName { get; set; }

        [JsonProperty("lastName")]
        public int LastName { get; set; }
    }

    public class SpaceShipMarket
    {
        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("marketId")]
        public string MarketId { get; set; }

        [JsonProperty("nftType")]
        public int NftType { get; set; }

        [JsonProperty("tokenId")]
        public string TokenId { get; set; }

        [JsonProperty("sellerAddress")]
        public string SellerAddress { get; set; }

        [JsonProperty("buyerAddress")]
        public string BuyerAddress { get; set; }

        [JsonProperty("price")]
        public string Price { get; set; }

        [JsonProperty("nftData")]
        public Spaceship SpaceShip { get; set; }

        [JsonProperty("isSold")]
        public bool IsSold { get; set; }

        public double RealPrice
        {
            get
            {
                return double.Parse(Price) / 1000000000000000000;
            }
        }
    }
}
