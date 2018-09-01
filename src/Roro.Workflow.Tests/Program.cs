using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Xml.Serialization;

namespace Roro.Workflow.Tests
{
    public class Program
    {
        [XmlRoot("Table")]
        public class DataTableWrapper
        {
            [XmlArrayItem("Column")]
            public List<DataColumnWrapper> Columns { get; } = new List<DataColumnWrapper>();

            [XmlArrayItem("Row")]
            public List<DataRowWrapper> Rows { get; } = new List<DataRowWrapper>();

            private DataTableWrapper()
            {

            }

            public DataTableWrapper(DataTable table)
            {
                foreach (DataColumn column in table.Columns)
                {
                    this.Columns.Add(new DataColumnWrapper(column));
                }
                foreach (DataRow row in table.Rows)
                {
                    this.Rows.Add(new DataRowWrapper(row));
                }

            }
        }

        public class DataColumnWrapper
        {
            [XmlAttribute]
            public string ColumnName { get; set; }

            [XmlAttribute]
            public string DataType { get; set; }

            private DataColumnWrapper()
            {

            }

            public DataColumnWrapper(DataColumn column)
            {
                this.ColumnName = column.ColumnName;
                this.DataType = column.DataType.FullName;
            }
        }

        public class DataRowWrapper
        {
            [XmlArray("Cells")]
            [XmlArrayItem("Cell")]
            public object[] ItemArray { get; set; }

            private DataRowWrapper()
            {
                
            }

            public DataRowWrapper(DataRow row)
            {
                this.ItemArray = row.ItemArray;
            }
        }

        static void Main(string[] args)
        {
            var collection = new CollectionNode();
            Console.WriteLine(XmlSerializerHelper.ToString(collection.Table));
        }
    }

    public abstract class BaseNode
    {
        public Guid Id { get; set; }

        public string Name { get; set; }

        protected BaseNode()
        {
            this.Id = Guid.NewGuid();
            this.Name = this.GetType().Name;
        }
    }

    public class DataNode : BaseNode
    {
        public virtual TypeWrapper VariableType { get; set; }

        public virtual object InitialValue { get; set; }

        public DataNode()
        {
            this.VariableType = new TypeWrapper(typeof(bool));
            this.InitialValue = Argument.GetTypeDefaultValue(this.VariableType);
        }
    }

    public class CollectionNode : DataNode
    {
        public override TypeWrapper VariableType
        {
            get => new TypeWrapper(typeof(DataTable));
            set => base.VariableType = this.VariableType;
        }

        [XmlIgnore]
        public override object InitialValue
        {
            get => null;
            set => base.InitialValue = this.InitialValue;
        }

        [XmlElement("InitialValues")]
        public DataTable Table
        {
            get;
            set;
        }

        public CollectionNode()
        {
            this.Table = new DataTable("Row");
            this.Table.Columns.Add("Text", typeof(string));
            this.Table.Columns.Add("Number", typeof(decimal));
            this.Table.Columns.Add("Flag", typeof(bool));
            this.Table.Columns.Add("DateTime", typeof(DateTime));

            this.Table.Rows.Add("abc", 123, true, DateTime.Now);
            this.Table.Rows.Add("def", 456, false, DateTime.UtcNow);
            this.Table.Rows.Add("ghi", 789, true, DateTime.Today);
        }
    }
}
