using Roro.Activities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Serialization;

namespace Roro.Workflow
{
    public abstract class Node : NotifyPropertyHelper
    {
        [XmlAttribute]
        public Guid Id { get; set; }

        public string Name
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

        [XmlIgnore]
        public Port[] Ports
        {
            get
            {
                return this.GetType().GetProperties()
                    .Where(x => typeof(Port).IsAssignableFrom(x.PropertyType))
                    .Select(x => x.GetValue(this) as Port)
                    .ToArray();
            }
        }

        public abstract PortAnchor[] Anchors { get; }

        [XmlIgnore]
        public Page ParentPage { get; internal set; }

        public virtual bool CanEndLink => true;

        protected Node()
        {
            this.Id = Guid.NewGuid();
            this.Name = this.GetType().Name;
            //this.X = RandomHelper.Next(0, 1000);
            //this.Y = RandomHelper.Next(0, 600);
            this.Rect = new NodeRect()
            {
                X = RandomHelper.Next(0, 1000),
                Y = RandomHelper.Next(0, 600),
                Width = 4 * Page.GRID_SIZE,
                Height = 2 * Page.GRID_SIZE
            };
        }

        public abstract NodeExecutionResult Execute(NodeExecutionContext context);

        #region ACTIVITY

        private static IEnumerable<Type> _activityTypes;

        public static IEnumerable<Type> GetActivityTypes()
        {
            if (_activityTypes == null)
            {
                Directory.GetFiles(Environment.CurrentDirectory, typeof(Activity).Namespace + "?*.dll").ToList()
                    .ForEach(x => Assembly.LoadFile(x));

                _activityTypes = AppDomain.CurrentDomain.GetAssemblies()
                    .Where(x => x.FullName.StartsWith(typeof(Activity).Namespace))
                    .SelectMany(x => x.GetTypes())
                    .Where(x => !x.IsAbstract)
                    .Where(x => !x.IsInterface)
                    .Where(x => !x.IsGenericType)
                    .Where(x => typeof(Activity).IsAssignableFrom(x));
            }

            return _activityTypes;
        }

        public static Activity CreateActivity(string activityTypeName)
        {
            if (GetActivityTypes().FirstOrDefault(x => x.FullName == activityTypeName) is Type type)
            {
                return (Activity)Activator.CreateInstance(type);
            }

            return null;
        }

        public static Node Create(Type type)
        {
            var activity = CreateActivity(type.FullName);
            if (activity is IAction)
            {
                return new ActionNode()
                {
                    ActionType = type
                };
            }
            else if (activity is IDecision)
            {
                return new DecisionNode()
                {
                    DecisionType = type
                };
            }
            else if (activity is ILoop)
            {
                return new LoopStartNode()
                {
                    LoopType = type
                };
            }
            else if (Activator.CreateInstance(type) is Node node)
            {
                return node;
            }
            else
            {
                return null;
            }
        }

        #endregion
    }
}
