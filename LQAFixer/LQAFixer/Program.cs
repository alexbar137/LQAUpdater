using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Xml;

namespace LQAFixer
{
    class Program
    {
        static void Main(string[] args)
        {
            Freelancers.FreelancersDataContext site = new Freelancers.FreelancersDataContext(new Uri("http://inside.office.palex/lqa/_vti_bin/ListData.svc"));
            site.Credentials = System.Net.CredentialCache.DefaultCredentials;

            var LQAReports = site.NewLQA;

            XmlUrlResolver resolver = new XmlUrlResolver();
            resolver.Credentials = System.Net.CredentialCache.DefaultCredentials;

            XmlDocument xml = new XmlDocument();
            xml.XmlResolver = resolver;
            int count = 0;
            int total = LQAReports.Count();
            Dictionary<string, string> teams = new Dictionary<string, string>()
            {
                { "Business", "Business Team" },
                { "ML Med", "Med Team" },
                { "Microsoft", "MS Team" },
                { "RU+CIS Tech", "RU-Tech Team" },
                { "Ru-Tech Team", "RU-Tech Team" },
                { "ML Tech", "Tech Team" }
            };

            foreach (var item in LQAReports)
            {
                string path = string.Format("\\\\inside.office.palex\\lqa\\New LQA Test\\{0}", item.Имя);
                try
                {
                    xml.Load(path);
                    XmlNamespaceManager ns = new XmlNamespaceManager(xml.NameTable);
                    ns.AddNamespace("my", "http://schemas.microsoft.com/office/infopath/2003/myXSD/2007-06-11T04:47:13");

                    XmlNode root = xml.DocumentElement;

                    XmlNode team = root.SelectSingleNode("my:Team", ns);

                    if (team != null && teams.ContainsKey(team.InnerText))
                    {
                        team.InnerText = teams[team.InnerText];
                        xml.Save(path);
                        Thread.Sleep(500);
                    }                  
                    
                }
                catch(Exception e)
                {
                    Console.WriteLine("Exception: {0}\r\nSource: {1}", e.Message, e.StackTrace);
                    Console.Read();
                    
                }
                Console.Clear();
                Console.WriteLine("Обработано элементов: {0} из {1}", ++count, total);
            }

            Console.WriteLine("Элементов: {0}", LQAReports.Count());
            Console.WriteLine("Press any key");
            Console.Read();
        }
    }
}
