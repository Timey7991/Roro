using Roro.Activities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Serialization;

namespace Roro.Workflow
{
    public abstract class Node : NotifyPropertyHelper, IEditableNode
    {
        [XmlAttribute]
        public Guid Id { get; set; }

        public virtual string Name
        {
            get => this._name;
            set => this.OnPropertyChanged(ref this._name, value);
        }
        private string _name;

        [XmlIgnore]
        public bool Selected
        {
            get => this._selected;
            set => this.OnPropertyChanged(ref this._selected, value);
        }
        private bool _selected;

        public NodeRect Rect
        {
            get => this._bounds;
            set => this.OnPropertyChanged(ref this._bounds, value);
        }
        private NodeRect _bounds;

        public void SetLocation(int x, int y) => this.Rect = new NodeRect() { X = x, Y = y, Width = this.Rect.Width, Height = this.Rect.Height };

        [XmlIgnore]
        public IEnumerable<IEditablePort> Ports
        {
            get
            {
                return this.GetType().GetProperties()
                    .Where(x => typeof(Port).IsAssignableFrom(x.PropertyType))
                    .Select(x => x.GetValue(this))
                    .Cast<Port>();
            }
        }

        public abstract IEnumerable<PortAnchor> Anchors { get; }

        [XmlIgnore]
        public IEditablePage ParentPage { get; internal set; }

        protected Node()
        {
            this.Id = Guid.NewGuid();
            this.Name = this.GetType().Name;
            this.Rect = new NodeRect()
            {
                Width = 4 * Page.GRID_SIZE,
                Height = 2 * Page.GRID_SIZE
            };
        }

        public abstract NodeExecutionResult Execute(NodeExecutionContext context);

        public static Node Create(TypeWrapper type)
        {
            if (type is null)
            {
                return null;
            }
            else if (typeof(IAction).IsAssignableFrom(type.WrappedType))
            {
                return new ActionNode(type);
            }
            else if (typeof(IDecision).IsAssignableFrom(type.WrappedType))
            {
                return new DecisionNode() { DecisionType = type };
            }
            else if (typeof(ILoop).IsAssignableFrom(type.WrappedType))
            {
                return new LoopStartNode() { LoopType = type };
            }
            else if (Activator.CreateInstance(type.WrappedType) is Node node)
            {
                return node;
            }
            else
            {
                return null;
            }
        }
    }
}
