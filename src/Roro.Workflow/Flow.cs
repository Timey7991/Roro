using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Xml.Serialization;

namespace Roro.Workflow
{
    public sealed class Flow : NotifyPropertyHelper
    {
        [XmlAttribute]
        public Guid Id { get; set; }

        public string Name
        {
            get => this._name;
            set => this.OnPropertyChanged(ref this._name, value);
        }
        private string _name;

        public ObservableCollection<Page> Pages { get; } = new ObservableCollection<Page>();

        public Page MainPage => this.Pages.First(x => x.Name == Page.MAIN_PAGE_NAME);

        public Page ExecutingPage
        {
            get => this._executingPage;
            private set => this.OnPropertyChanged(ref this._executingPage, value);
        }
        private Page _executingPage;

        public Node ExecutingNode
        {
            get => this._executingNode;
            private set => this.OnPropertyChanged(ref this._executingNode, value);
        }
        private Node _executingNode;

        private Flow()
        {
            this.Id = Guid.NewGuid();
            this.Name = string.Empty;
            this.Pages.CollectionChanged += Pages_CollectionChanged;
        }

        private void Pages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (Page addedPage in e.NewItems)
                    {
                        addedPage.ParentFlow = this;
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (Page removedPage in e.OldItems)
                    {
                        removedPage.ParentFlow = null;
                    }
                    break;
            }
        }

        public Flow(string name) : this()
        {
            this.Name = name;
            this.Pages.Add(new Page("Main"));

            // test: add pages
            var count = RandomHelper.Next(2, 10);
            for (var i = 1; i < count; i++)
            {
                this.AddPage(new Page("Page " + i));
            }
        }

        public void AddPage(Page page)
        {
            this.Pages.Add(page);
        }

        public void RemovePage(Page page)
        {
            if (page == this.MainPage)
            {
                return;
            }
            this.Pages.Remove(page);
        }
    }
}
