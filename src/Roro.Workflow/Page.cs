using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Xml.Serialization;

namespace Roro.Workflow
{
    public sealed partial class Page : NotifyPropertyHelper
    {
        public const string MAIN_PAGE_NAME = "Main";

        public const int GRID_SIZE = 20;

        [XmlAttribute]
        public Guid Id { get; set; }

        public string Name
        {
            get => this._name;
            set
            {
                if (this.Name != Page.MAIN_PAGE_NAME)
                {
                    this.OnPropertyChanged(ref this._name, value);
                }
            }
        }
        private string _name;

        /// <summary>
        /// Use AddNode() and RemoveNode() to modify the collection.
        /// </summary>
        public ObservableCollection<Node> Nodes { get; } = new ObservableCollection<Node>();

        public IEnumerable<Node> SelectedNodes => this.Nodes.Where(x => x.Selected);

        public Node StartNode => this.Nodes.First(x => x is StartNode);

        [XmlIgnore]
        public Flow ParentFlow { get; internal set; }

        private Page()
        {
            this.Id = Guid.NewGuid();
            this.Name = string.Empty;
            this.Nodes.CollectionChanged += Nodes_CollectionChanged;
        }

        private void Nodes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (Node addedNode in e.NewItems)
                    {
                        addedNode.ParentPage = this;
                        addedNode.Ports.ToList().ForEach(x => x.ParentNode = addedNode);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (Node removedNode in e.OldItems)
                    {
                        removedNode.ParentPage = null;
                        removedNode.Ports.ToList().ForEach(x => x.ParentNode = null);
                    }
                    break;
            }
        }

        public Page(string name) : this()
        {
            this.Name = name;

            var startNode = new StartNode();
            var endNode = new EndNode();

            startNode.Next.To = endNode.Id;
            startNode.SetPosition(1 * Page.GRID_SIZE, 2 * Page.GRID_SIZE);
            endNode.SetPosition(1 * Page.GRID_SIZE, 8 * Page.GRID_SIZE);

            this.Nodes.Add(startNode);
            this.Nodes.Add(endNode);
        }
    }
}
