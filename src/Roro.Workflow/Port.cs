using System;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace Roro.Workflow
{
    public abstract class Port : NotifyPropertyHelper, IEditablePort
    {
        [XmlAttribute]
        public Guid To
        {
            get => this._to;
            set => this.OnPropertyChanged(ref this._to, value);
        }
        private Guid _to;

        [XmlIgnore]
        public IEditableNode ParentNode { get; internal set; }

        public virtual string Text => string.Empty;

        [XmlIgnore]
        public PortAnchor CurrentAnchor
        {
            get => this._currentAnchor;
            set => this.OnPropertyChanged(ref this._currentAnchor, value);
        }
        private PortAnchor _currentAnchor;

        public abstract PortAnchor DefaultAnchor { get; }

        public abstract IEnumerable<PortAnchor> Anchors { get; }

        protected Port()
        {
            this.CurrentAnchor = this.DefaultAnchor;
        }

        public void Connect(IEditableNode node)
        {
            if (node is null)
            {
                this.To = Guid.Empty;
            }
            else if (node is StartNode)
            {
                ;
            }
            else if (node is VariableNode)
            {
                ;
            }
            else
            {
                this.To = node.Id;
            }
        }
    }
}