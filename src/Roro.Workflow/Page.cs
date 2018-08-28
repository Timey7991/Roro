using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Xml.Serialization;

namespace Roro.Workflow
{
    public sealed partial class Page : NotifyPropertyHelper, IEditablePage
    {
        [XmlAttribute]
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name
        {
            get => this._name;
            set => this.OnPropertyChanged(ref this._name, value);
        }
        private string _name;

        [XmlArray("Nodes")]
        public ObservableCollection<Node> _nodes { get; } = new ObservableCollection<Node>();

        public IEnumerable<IEditableNode> Nodes => this._nodes;

        public IEnumerable<IEditableNode> SelectedNodes => this._nodes.Where(x => x.Selected);

        public IEditableNode StartNode => this._nodes.First(x => x is StartNode);

        [XmlIgnore]
        public IEditableFlow ParentFlow { get; internal set; }

        private Page()
        {
            this._nodes.CollectionChanged += Nodes_CollectionChanged;
        }

        public Page(string name) : this()
        {
            this.Name = name;

            var startNode = new StartNode();
            var endNode = new EndNode();

            startNode.Next.To = endNode.Id;
            startNode.SetLocation(1 * Page.GRID_SIZE, 2 * Page.GRID_SIZE);
            endNode.SetLocation(1 * Page.GRID_SIZE, 8 * Page.GRID_SIZE);

            this._nodes.Add(startNode);
            this._nodes.Add(endNode);
        }

        private void Nodes_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    foreach (Node addedNode in e.NewItems)
                    {
                        addedNode.ParentPage = this;
                        addedNode.Ports.Cast<Port>().ToList().ForEach(x => x.ParentNode = addedNode);
                    }
                    break;

                case NotifyCollectionChangedAction.Remove:
                    foreach (Node removedNode in e.OldItems)
                    {
                        removedNode.ParentPage = null;
                        removedNode.Ports.Cast<Port>().ToList().ForEach(x => x.ParentNode = null);
                    }
                    break;
            }
        }
    }
}