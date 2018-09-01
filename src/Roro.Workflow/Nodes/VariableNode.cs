using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Xml.Serialization;

namespace Roro.Workflow
{
    public sealed class VariableNode : Node
    {
        public TypeWrapper VariableType
        {
            get => this._variableType;
            set
            {
                if (Argument.Types.Contains(value))
                {
                    this.OnPropertyChanged(ref this._variableType, value);
                    this.InitialValue = this.InitialValue;
                }
                else
                {
                    throw new InvalidCastException();
                }
            }
        }
        private TypeWrapper _variableType;

        public DateTime DateTimeValue
        {
            get => this._dateTimeValue;
            set => this.OnPropertyChanged(ref this._dateTimeValue, value);

        }
        private DateTime _dateTimeValue;

        public object InitialValue
        {
            get => this._initialValue;
            set
            {
                try
                {
                    value = Convert.ChangeType(value, this.VariableType.WrappedType);
                }
                catch
                {
                    value = Argument.GetTypeDefaultValue(this.VariableType);
                }
                this.OnPropertyChanged(ref this._initialValue, value);
            }
        }
        private object _initialValue;

        [XmlIgnore]
        public object RuntimeValue
        {
            get => this._runtimeValue;
            private set => this.OnPropertyChanged(ref this._runtimeValue, value);
        }
        private object _runtimeValue;

        public override ObservableCollection<Argument> Arguments => null;

        public override IEnumerable<PortAnchor> Anchors => new PortAnchor[] { };

        public VariableNode()
        {
            this.VariableType = Argument.Types.First();
        }

        public override NodeExecutionResult Execute(NodeExecutionContext context)
        {
            throw new NotImplementedException();
        }
    }
}