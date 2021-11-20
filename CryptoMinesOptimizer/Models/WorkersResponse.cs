using Newtonsoft.Json;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace CryptoMinesOptimizer.Models
{
    public class MpData
    {
        public int MiningPower { get; set; }

        public int Count { get; set; }

        public int TotalMining { get; set; }

        public double WorkersPrice { get; set; }

        public double ShipsPrice { get; set; }

        public double TotalPrice {
            get {
                return WorkersPrice + ShipsPrice;
            }
        }
    }

    public class Worker
    {
        [JsonProperty("_id")]
        public string Id { get; set; }

        [JsonProperty("roll")]
        public int Roll { get; set; }

        [JsonProperty("level")]
        public int Level { get; set; }

        [JsonProperty("minePower")]
        public int MinePower { get; set; }

        [JsonProperty("firstName")]
        public int FirstName { get; set; }

        [JsonProperty("lastName")]
        public int LastName { get; set; }

        [JsonProperty("contractDueDate")]
        public string ContractDueDate { get; set; }

        [JsonProperty("lastMine")]
        public string LastMine { get; set; }
    }

    public class WorkerMarket
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
        public Worker Worker { get; set; }

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