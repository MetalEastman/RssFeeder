using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel.Syndication;
using System.Text.RegularExpressions;
using System.Xml;
using System.IO;
using System.Xml.Linq;

namespace RssFeeder.Model
{
    public class Repository
    {
        const string Path = "rsslist.txt";
        public ObservableCollection<RssItem> RssItems;
        public ObservableCollection<RssListItem> RssList;
        public Repository()
        {
            CheckCreateFile();
            RssList = new ObservableCollection<RssListItem>();
            RssItems = new ObservableCollection<RssItem>();
            RssList = GetRssList();
            //RssItems = GetRssFeeds();
        }

        private void CheckCreateFile()
        {
            if (!File.Exists(Path))
            {
                using (StreamWriter sw = File.CreateText(Path))
                {
                    sw.WriteLine("Feber" + ";" + "http://feber.se/rss");
                }
            }
        }

        private ObservableCollection<RssListItem> GetRssList()
        {
            var listitems = new ObservableCollection<RssListItem>();

            List<string> lines = File.ReadAllLines(Path).ToList();

            foreach (var l in lines)
            {
                string[] split = l.Split(';');

                RssListItem listitem = new RssListItem()
                {
                    Name = split[0],
                    Uri = split[1]
                };
                listitems.Add(listitem);
            }


            return listitems;
        }

        public ObservableCollection<RssItem> GetRssFeeds(string url)
        {
            var items = new ObservableCollection<RssItem>();
            string imgPath = "";

            using (var reader = XmlReader.Create(url))
            {
                SyndicationFeed feed = SyndicationFeed.Load(reader);

                foreach (SyndicationItem item in feed.Items)
                {

                    Regex regx = new Regex("(http|https)://([\\w+?\\.\\w+])+([a-zA-Z0-9\\~\\!\\@\\#\\$\\%\\^\\&amp;\\*\\(\\)_\\-\\=\\+\\\\\\/\\?\\.\\:\\;\\'\\,]*)?.(?:jpg|bmp|gif|png|img)", RegexOptions.IgnoreCase);
                    MatchCollection matches = regx.Matches(item.Summary.Text);

                    if (matches.Count > 0)
                        imgPath = matches[0].ToString();

                    matches = null;

                    string text = Regex.Replace(item.Summary.Text, @"(?></?\w+)(?>(?:[^>'""]+|'[^']*'|""[^""]*"")*)>", "");
                    string newtext = Regex.Replace(text, @"\t|\n|\r", "");

                    RssItem rssitem = new RssItem()
                    {
                        UniqueId = item.Id,
                        Title = item.Title.Text,
                        Description = newtext,
                        ImagePath = imgPath,
                        Link = item.Links.FirstOrDefault().Uri.ToString()
                    };

                    imgPath = null;
                    items.Add(rssitem);
                }
            }
            return items;
        }

        internal void RemoveLinkFromList(RssListItem selectedListItem)
        {
            File.WriteAllText(Path, String.Empty);

            RssList.Remove(selectedListItem);
            if (File.Exists(Path))
            {
                using (StreamWriter sw = File.CreateText(Path))
                {
                    foreach (var i in RssList)
                    {
                        sw.WriteLine(i.Name + ";" + i.Uri);
                    }
                }
            }
        }

        internal void AddLinkToList(RssListItem newItem)
        {
            File.WriteAllText(Path, String.Empty);

            RssList.Add(newItem);
            if (File.Exists(Path))
            {
                using (StreamWriter sw = File.CreateText(Path))
                {
                    foreach (var i in RssList)
                    {
                        sw.WriteLine(i.Name + ";" + i.Uri);
                    }
                }
            }

        }
    }
}
