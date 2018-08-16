using System;
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

        public NotifyCollectionHelper<Page> Pages { get; }

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
            this.Pages = new NotifyCollectionHelper<Page>();
            this.Pages.CollectionChanging += Pages_CollectionChanging;
            this.Pages.CollectionChanged += Pages_CollectionChanged;
        }

        public Flow(string name) : this()
        {
            this.Name = name;
            this.Pages.Add(new Page("Main"));

            // test: add pages
            var count = RandomHelper.Next(2, 10);
            for (var i = 1; i < count; i++)
            {
                this.Pages.Add(new Page("Page " + i));
            }
        }

        private void Pages_CollectionChanging(object sender, NotifyCollectionChangingEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    var pageToAdd = e.NewItems[0] as Page;
                    break;

                case NotifyCollectionChangedAction.Remove:
                    var pageToRemove = e.OldItems[0] as Page;
                    if (pageToRemove == this.MainPage)
                    {
                        e.Cancel = true;
                    }
                    break;
            }
        }

        private void Pages_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    var addedPage = e.NewItems[0] as Page;
                    addedPage.ParentFlow = this;
                    break;

                case NotifyCollectionChangedAction.Remove:
                    var removedPage = e.OldItems[0] as Page;
                    removedPage.ParentFlow = null;
                    break;
            }
        }
    }
}
