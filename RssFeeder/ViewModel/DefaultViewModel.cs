using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft;
using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using RssFeeder.Model;

namespace RssFeeder.ViewModel
{
    public class DefaultViewModel : ViewModelBase
    {
        private Repository repo;
        private ObservableCollection<RssItem> rssItems;
        private ObservableCollection<RssListItem> rssListItems;
        private RssListItem selectedListItem;
        private string name;
        private string uri;

        public ICommand AddLink { get; private set; }
        public ICommand RemoveLink { get; private set; }
        public string Name
        {
            get { return this.name; }
            set { this.name = value; RaisePropertyChanged("Name"); }
        }

        public string Uri
        {
            get { return this.uri; }
            set { this.uri = value; RaisePropertyChanged("Uri"); }
        }

        public RssListItem SelectedListItem
        {
            get { return this.selectedListItem; }
            set { this.selectedListItem = value; RssItems = GetSelectedRssFeed(); }
        }

        public ObservableCollection<RssItem> RssItems
        {
            get { return this.rssItems; }
            set { this.rssItems = value; RaisePropertyChanged("RssItems");}
        }

        public ObservableCollection<RssListItem> RssListItems
        {
            get { return this.rssListItems; }
            set { this.rssListItems = value; RaisePropertyChanged("RssListItems");}
        } 

        public DefaultViewModel()
        {
            repo = new Repository();
            RssListItems = repo.RssList;
            AddLink = new RelayCommand(AddLinkToList);
            RemoveLink = new RelayCommand(RemoveLinkFromList);
        }

        public ObservableCollection<RssItem> GetSelectedRssFeed()
        {
            if (SelectedListItem == null)
            {
                RssItems.Clear();
                return RssItems;
            }

            return repo.GetRssFeeds(SelectedListItem.Uri);
        }

        public void AddLinkToList()
        {
            RssListItem newItem = new RssListItem()
            {
                Name = this.Name,
                Uri = this.Uri
            };

            this.Name = "";
            this.Uri = "";

            repo.AddLinkToList(newItem);
        }

        private void RemoveLinkFromList()
        {
            repo.RemoveLinkFromList(selectedListItem);
        }
    }
}
