using CryptoMinesOptimizer.Models;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Globalization;

namespace CryptoMinesOptimizer
{
    class Program
    {
        private static List<SpaceShipMarket> SpaceShipsMarket = new List<SpaceShipMarket>();
        private static List<WorkerMarket> WorkersMarket = new List<WorkerMarket>();
        private static double EtlPrice = 0;

        static async Task Main(string[] args)
        {
            CultureInfo.CurrentCulture = new CultureInfo("en-US", false);

            int minMp = int.Parse(args[0]);
            int maxMp = int.Parse(args[1]);

            Console.WriteLine(@"ToAchieve : ");
            int mpToAchieve = int.Parse(Console.ReadLine());

            Console.WriteLine(@"1*Ships : ");
            int ships1 = int.Parse(Console.ReadLine());

            Console.WriteLine(@"2*Ships : ");
            int ships2 = int.Parse(Console.ReadLine());

            Console.WriteLine(@"3*Ships : ");
            int ships3 = int.Parse(Console.ReadLine());

            Console.WriteLine(@"4*Ships : ");
            int ships4 = int.Parse(Console.ReadLine());

            Console.WriteLine(@"5*Ships : ");
            int ships5 = int.Parse(Console.ReadLine());

            int maxWk = 1 * ships1 + 2 * ships2 + 3 * ships3 + 4 * ships4 + 5 * ships5;

            // Get Eternal Price
            var hdl1 = new HttpClientHandler();
            hdl1.AutomaticDecompression = DecompressionMethods.None;
            using (var httpClient = new HttpClient(hdl1))
            {
                using (HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("GET"), "https://bsc.api.0x.org/swap/v1/price?sellToken=0xd44fd09d74cd13838f137b590497595d6b3feea4&buyToken=BUSD&sellAmount=1000000000000000000"))
                {
                    HttpResponseMessage response = await httpClient.SendAsync(request);
                    string content = await response.Content.ReadAsStringAsync();
                    EtlPrice = JsonConvert.DeserializeObject<EternalPriceResponse>(content).Price;
                }
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Eternal Price on 0x.org : {EtlPrice.ToString("#.##$")}");

            // Get Ships MarketPlace data
            var handler = new HttpClientHandler();
            handler.AutomaticDecompression = DecompressionMethods.None;
            using (var httpClient = new HttpClient(handler))
            {
                using (HttpRequestMessage request = new HttpRequestMessage(new HttpMethod("GET"), "https://api.cryptomines.app/api/spaceships"))
                {
                    HttpResponseMessage response = await httpClient.SendAsync(request);
                    string content = await response.Content.ReadAsStringAsync();
                    SpaceShipsMarket = JsonConvert.DeserializeObject<List<SpaceShipMarket>>(content);
                }
            }

            // Get Workers MarketPlace data
            var handler2 = new HttpClientHandler();
            handler2.AutomaticDecompression = DecompressionMethods.None;
            using (var httpClient = new HttpClient(handler2))
            {
                using (HttpRequestMessage request2 = new HttpRequestMessage(new HttpMethod("GET"), "https://api.cryptomines.app/api/workers"))
                {

                    HttpResponseMessage response = await httpClient.SendAsync(request2);
                    string content = await response.Content.ReadAsStringAsync();
                    WorkersMarket = JsonConvert.DeserializeObject<List<WorkerMarket>>(content);
                }
            }

            ProcessData(minMp, maxMp, mpToAchieve, maxWk, ships1, ships2, ships3, ships4, ships5);
        }

        /// <summary>
        /// Calculate data for each MP
        /// </summary>
        /// <param name="minMp">Minimum Mining Power asked</param>
        /// <param name="maxMp">Maximum Mining Power Asked</param>
        /// <param name="mpToAchieve">Total mining power to achieve</param>
        /// <param name="maxWk">Max workers asked to achive total Mining power</param>
        /// <param name="etlPrice">Eternal price in USD</param>
        static void ProcessData(int minMp, int maxMp, int mpToAchieve, double maxWk, int ships1, int ships2, int ships3, int ships4, int ships5)
        {
            //calculate 7D WorkersContract (needed for first mining)
            double workerContrat = 7 / EtlPrice;

            //Calculate data for each MP asked
            List<MpData> result = new List<MpData>();
            for (int tempMp = minMp; tempMp <= maxMp; tempMp++)
            {
                //Get count of workers need for this MP
                int countToGet = mpToAchieve / tempMp;

                //if count is more than asked, skip this MP
                if (countToGet > maxWk)
                {
                    continue;
                }

                //take max workers asked, sorted by price ascending, for same lvl as current MP (+1 worker if needed, to see more options)
                List<WorkerMarket> temp;
                if (WorkersMarket.Where(p => p.Worker.Level == GetWorkerLvl(tempMp) && p.Worker.MinePower >= tempMp).OrderBy(p => p.RealPrice).Take(countToGet).Sum(p => p.Worker.MinePower) < mpToAchieve)
                {
                    temp = WorkersMarket.Where(p => p.Worker.Level == GetWorkerLvl(tempMp) && p.Worker.MinePower >= tempMp).OrderBy(p => p.RealPrice).Take(countToGet + 1).ToList();
                }
                else
                {
                    temp = WorkersMarket.Where(p => p
                    .Worker.Level == GetWorkerLvl(tempMp) && p.Worker.MinePower >= tempMp).OrderBy(p => p.RealPrice).Take(countToGet).ToList();
                }

                //if total MP is below needed, skip this MP
                if ((int)temp.Sum(p => p.Worker.MinePower) < mpToAchieve)
                {
                    continue;
                }

                //Compute data by MP
                result.Add(new MpData()
                {
                    Count = temp.Count, //count of workers for this MP
                    MiningPower = tempMp, //MP calculated
                    TotalMining = (int)temp.Sum(p => p.Worker.MinePower), //Sum of MP
                    WorkersPrice = Math.Round((double)temp.Sum(p => p.RealPrice), 2) + (temp.Count * workerContrat), //Adding 7D workers contract needed to mine
                    ShipsPrice = Math.Round(GetShipsTotalPrice(ships1, ships2, ships3, ships4, ships5), 2)
                });
            }

            //WriteLine X first MpData, sorted by total price ascending
            foreach (MpData temp in result.OrderBy(p => p.TotalPrice).Take(15))
            {
                WriteLine(temp.MiningPower, temp.TotalMining, temp.Count, temp.WorkersPrice, temp.ShipsPrice, temp.TotalPrice, EtlPrice);
            }
        }

        /// <summary>
        /// Write in console in a given format
        /// </summary>
        /// <param name="mp">Mining Power calculated</param>
        /// <param name="totalMp">Total Mining Power</param>
        /// <param name="wkCount">Workers Count</param>
        /// <param name="wkPrice">Workers Price</param>
        /// <param name="shPrice">Ships Price</param>
        /// <param name="totPrice">Total Price</param>
        /// <param name="etlPrice">Eternal Price</param>
        static void WriteLine(int mp, int totalMp, int wkCount, double wkPrice, double shPrice, double totPrice, double etlPrice)
        {
            Console.WriteLine($"MP: {mp.ToString("00#")} - TotalMP: {totalMp} - Wk: {wkCount} - WkPrice: {wkPrice.ToString("00#.00")} - Ships : {shPrice.ToString("00#.00")} Total : {totPrice.ToString("00#.00")} - {(totPrice * etlPrice).ToString("#.##$")}");
        }

        /// <summary>
        /// Get Worker Lvl for a giving MP
        /// </summary>
        /// <param name="mp">Mining Power Asked</param>
        /// <returns>Level</returns>
        static int GetWorkerLvl(int mp)
        {
            switch (mp)
            {
                case <= 50:
                    return 1;
                case <= 100:
                    return 2;
                case <= 150:
                    return 3;
                case <= 200:
                    return 4;
                case <= 300:
                    return 5;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// Calculate price for ships, giving a count of ships needed and giving level
        /// </summary>
        /// <param name="shipsCount">Count of ships asked</param>
        /// <param name="shipsLevel">Level of Ships asked</param>
        /// <returns>Ships price in ETL</returns>
        static double GetShipsSumPrice(int shipsCount, int shipsLevel)
        {
            return SpaceShipsMarket.Where(p => p.SpaceShip.Level == shipsLevel).OrderBy(p => p.RealPrice).Take(shipsCount).Sum(p => p.RealPrice);
        }

        /// <summary>
        /// Calculate Total of ships price for a giving count of workers
        /// </summary>
        /// <param name="workersCount">Count of workers asked</param>
        /// <returns>Total ships price in ETL</returns>
        static double GetShipsTotalPrice(int ships1, int ships2, int ships3, int ships4, int ships5)
        {
            return GetShipsSumPrice(ships1, 1) + GetShipsSumPrice(ships2, 2) + GetShipsSumPrice(ships3, 3) + GetShipsSumPrice(ships4, 4) + GetShipsSumPrice(ships5, 5);
        }
    }
}
