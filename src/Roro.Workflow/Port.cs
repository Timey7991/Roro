using System;
using System.Linq;
using System.Xml.Serialization;

namespace Roro.Workflow
{
    public abstract class Port : NotifyPropertyHelper
    {
        [XmlAttribute]
        public Guid To
        {
            get => this._to;
            set
            {
                if (this.ParentNode?.ParentPage?.Nodes.FirstOrDefault(x => x.Id == value) is Node targetNode)
                {
                    if (targetNode.CanEndLink)
                    {
                        OnPropertyChanged(ref this._to, value);
                    }
                }
                else
                {
                    OnPropertyChanged(ref this._to, value);
                }
            }
        }
        private Guid _to;

        [XmlIgnore]
        public Node ParentNode { get; internal set; }

        public virtual string Text => string.Empty;
    }
}
