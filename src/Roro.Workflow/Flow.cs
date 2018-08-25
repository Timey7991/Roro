using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Xml.Serialization;

namespace Roro.Workflow
{
    public sealed class Flow : NotifyPropertyHelper, IEditableFlow
    {
        [XmlAttribute]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name
        {
            get => this._name;
            set => this.OnPropertyChanged(ref this._name, value);
        }
        private string _name;

        [XmlAttribute("Pages")]
        public ObservableCollection<Page> _pages { get; } = new ObservableCollection<Page>();

        public IEnumerable<IEditablePage> Pages => this._pages;

        public IEditablePage MainPage => this._pages.First(x => x.Name == Page.MAIN_PAGE_NAME);

        private Flow()
        {
            this._pages.CollectionChanged += Pages_CollectionChanged;
        }

        public Flow(string name) : this()
        {
            this.Name = name;
            this._pages.Add(new Page(Page.MAIN_PAGE_NAME));
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

        public void AddPage(IEditablePage page)
        {
            this._pages.Add((Page)page);
        }

        public void RemovePage(IEditablePage page)
        {
            if (page == this.MainPage)
            {
                return;
            }
            this._pages.Remove((Page)page);
        }
    }
}
