using System;
using System.Net.Http;
using HtmlAgilityPack;
using System.Collections.Generic;
using System.IO;

namespace _4chanDownload
{
    class Program
    {
        private static HttpClient Client = new HttpClient();
        static void Main(string[] args)
        {

            String url = "";

            if (args == null || args.Length == 0)
            {
                Console.WriteLine("no link provided.");
                Console.ReadKey();
            }
            else
            {
                url = args[0];
                comenzarTarea(url);
            }
        }





        private static void comenzarTarea(String URL4chan)
        {
            String FourchanPage = CallUrl(URL4chan);

            HtmlDocument chanpage = parseHtml(FourchanPage);


            Console.WriteLine("Obteniendo imágenes... ");
            List<String> imagesurls = new List<string>();


            HtmlNodeCollection images = chanpage.DocumentNode.SelectNodes("//a[@class='fileThumb']");
            foreach (HtmlNode image in images)
            {
                imagesurls.Add("https:" + image.Attributes["href"].Value);
            }

            if (imagesurls.Count > 0)
            {
                string[] urlsplit = URL4chan.Split("/");
                string foldername = urlsplit[urlsplit.Length - 1];
                string board = urlsplit[urlsplit.Length - 3];
                Console.WriteLine(imagesurls.Count + " encontradas en el hilo. Descargando... ");
                string DirToDownload = Path.Combine(Directory.GetCurrentDirectory(), board, foldername);
                if (!Directory.Exists(DirToDownload))
                {
                    Directory.CreateDirectory(DirToDownload);
                }
                foreach (var imgurl in imagesurls)
                {
                    DownloadImage(imgurl, DirToDownload);
                }
                Console.WriteLine("Descarga completa.");
            }
            else
            {
                Console.WriteLine("No se encontraron links, probablemente el programador no hizo bien el programa xd");
            }


        }




        private static String CallUrl(string fullUrl)
        {
            Console.WriteLine("Accesando a la url " + fullUrl + " ...");
            var result = Client.GetStringAsync(fullUrl);
            return result.Result;
        }

        private static HtmlDocument parseHtml(string stringHtml)
        {
            //TODO: add try/catch
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(stringHtml);
            return doc;
        }

        private static void DownloadImage(string imgURL, string DirToDownload)
        {
            //TODO: add try/catch

            string imgname = imgURL.Split('/')[4];
            Console.Write(imgname + "...");
            string filePath = Path.Combine(DirToDownload, imgname);
            if (!File.Exists(filePath))
            {
                var imgBytes = Client.GetByteArrayAsync(imgURL).Result;
                File.WriteAllBytes(filePath, imgBytes);
                Console.WriteLine("OK");
            }
            else
            {
                Console.WriteLine("File Exists, Skipping");
            }

        }
    }
}


//ChromeOptions opts = new ChromeOptions();
//opts.AddArguments("headless");
//ChromeDriver browser = new ChromeDriver(".", opts);
//browser.Url = url;

//browser.FindElements(By.XPath("//a[@class='fileThumb']"));
//Console.ReadKey();
