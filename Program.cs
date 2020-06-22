using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleWebScrapingAtpTour
{
    class Program
    {
        static string sqlIns = @"insert into jugador_atp(RNK, NOMBRE, APATERNO, EDAD) values({0}, '{1}', '{2}', {3});";

        static void Main(string[] args)
        {
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;

            string[] a1;

            // From Web
            var url = "https://www.atptour.com/en/rankings/singles?rankDate=2020-03-16&rankRange=1-5000";
            var web = new HtmlWeb();
            var doc = web.Load(url);

            List<HtmlNode> hn = doc.DocumentNode.SelectNodes("//div[@class='table-rankings-wrapper']/table[@class='mega-table']/tbody/tr").ToList();
            a1 = generarListaJugadores(hn);

            String fileName = Path.Combine(Directory.GetCurrentDirectory(), "jugadores-atp.sql");

            System.IO.File.WriteAllLines(fileName, a1);


        }

        static string[] generarListaJugadores(List<HtmlNode> hn)
        {
            string[] aIns;
            String player = string.Empty;
            int rnk = 0;
            int age = 0;

            int l = hn.Count;
            aIns = new string[l];
            int i = 0;

            foreach (var item in hn)
            {
                var nR = item.SelectSingleNode("td[@class='rank-cell']");
                var nP = item.SelectSingleNode("td[@class='player-cell']/a");
                var nA = item.SelectSingleNode("td[@class='age-cell']");

                try
                {
                    rnk = Convert.ToInt32(nR.InnerText.Trim());
                }
                catch
                {
                    rnk = Convert.ToInt32(nR.InnerText.Trim().Substring(0, nR.InnerText.Trim().IndexOf('T')));
                }

                player = Convert.ToString(nP.InnerText).Trim();
                string[] nombres = player.Split(' ');

                try {age = Convert.ToInt32(nA.InnerText.Trim());} catch { age = 0; };

                aIns[i] = string.Format(sqlIns, rnk, nombres[0], nombres[1], age);

                Console.WriteLine(aIns[i]);
                i++;
            }
            return aIns;
        }
    }
}
