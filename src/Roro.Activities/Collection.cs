using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Dynamic;
using System.Xml.Serialization;

namespace Roro.Activities
{
    public sealed class Collection : DynamicObject
    {
        private readonly DataTable _dataTable;

        [XmlArray("Columns")]
        [XmlArrayItem("Column")]
        public ObservableCollection<CollectionColumn> _c0lumns { get; } = new ObservableCollection<CollectionColumn>();

        [XmlArray("Rows")]
        [XmlArrayItem("Row")]
        public ObservableCollection<CollectionRow> _r0ws { get; } = new ObservableCollection<CollectionRow>();

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            return base.TryGetMember(binder, out result);
        }

        public Collection()
        {
            this._dataTable = new DataTable();
        }

        public Collection(DataTable table) : this()
        {
            this._dataTable = table;
            foreach (DataColumn column in this._dataTable.Columns)
            {
                this._c0lumns.Add(new CollectionColumn(column));
            }
            foreach (DataRow row in this._dataTable.Rows)
            {
                this._r0ws.Add(new CollectionRow(row));
            }
        }
    }

    public sealed class CollectionColumn
    {
        public string Name { get; set; }

        public string Type { get; set; }

        private CollectionColumn()
        {

        }

        public CollectionColumn(DataColumn column)
        {
            this.Name = column.ColumnName;
            this.Type = column.DataType.FullName;
        }
    }

    public sealed class CollectionRow
    {
        [XmlArray("Cells")]
        [XmlArrayItem("Cell")]
        public ObservableCollection<object> Cells { get; } = new ObservableCollection<object>();

        private CollectionRow()
        {

        }

        public CollectionRow(DataRow row)
        {
            foreach (object cell in row.ItemArray)
            {
                this.Cells.Add(cell);
            }
        }
    }
}
