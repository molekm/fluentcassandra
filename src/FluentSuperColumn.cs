﻿using System;
using System.Collections.Generic;
using System.Linq;
using FluentCassandra.Types;

namespace FluentCassandra
{
	[Obsolete("Use \"FluentSuperColumn\" class with out generic type")]
	public class FluentSuperColumn<CompareWith, CompareSubcolumnWith> : FluentSuperColumn
		where CompareWith : CassandraType
		where CompareSubcolumnWith : CassandraType
	{
		public FluentSuperColumn()
			: base(new CassandraColumnSchema {
				NameType = typeof(CompareWith),
				ValueType = typeof(CompareSubcolumnWith)
			}) { }
	}

	/// <summary>
	/// 
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class FluentSuperColumn : FluentRecord<FluentColumn>, IFluentBaseColumn, IFluentRecordExpression
	{
		private CassandraType _name;
		private FluentColumnList<FluentColumn> _columns;
		private CassandraColumnSchema _schema;

		/// <summary>
		/// 
		/// </summary>
		public FluentSuperColumn(CassandraColumnSchema schema = null)
		{
			SetSchema(schema);

			_columns = new FluentColumnList<FluentColumn>(GetPath());
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="columns"></param>
		internal FluentSuperColumn(CassandraColumnSchema schema, IEnumerable<FluentColumn> columns)
		{
			SetSchema(schema);

			_columns = new FluentColumnList<FluentColumn>(GetPath(), columns);
		}

		/// <summary>
		/// The column name.
		/// </summary>
		public CassandraType ColumnName
		{
			get { return _name; }
			set
			{
				_name = (CassandraType)value.GetValue(GetSchema().NameType);
			}
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public FluentColumn CreateColumn()
		{
			return new FluentColumn(GetColumnSchema(""));
		}

		/// <summary>
		/// 
		/// </summary>
		/// <returns></returns>
		public FluentColumn CreateColumn(CassandraType name)
		{
			return new FluentColumn(GetColumnSchema(name)) {
				ColumnName = name
			};
		}

		/// <summary>
		/// The columns in the super column.
		/// </summary>
		public override IList<FluentColumn> Columns
		{
			get { return _columns; }
		}

		/// <summary>
		/// 
		/// </summary>
		public FluentSuperColumnFamily Family
		{
			get;
			internal set;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="columnName"></param>
		/// <returns></returns>
		public CassandraType this[CassandraType columnName]
		{
			get
			{
				var value = GetColumnValue(columnName);

				if (value is NullType)
					throw new CassandraException(String.Format("Column, {0}, could not be found.", columnName));

				return value;
			}
		}

		/// <summary>
		/// 
		/// </summary>
		public CassandraColumnSchema GetSchema()
		{
			if (_schema == null)
				_schema = new CassandraColumnSchema { Name = ColumnName, ValueType = typeof(BytesType) };

			return _schema;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="schema"></param>
		public void SetSchema(CassandraColumnSchema schema)
		{
			if (schema == null)
				schema = new CassandraColumnSchema { Name = ColumnName, ValueType = typeof(BytesType) };

			_schema = schema;
		}

		/// <summary>
		/// Gets the path.
		/// </summary>
		/// <returns></returns>
		public FluentColumnPath GetPath()
		{
			return new FluentColumnPath(Family, this, null);
		}

		/// <summary>
		/// Gets the parent.
		/// </summary>
		/// <returns></returns>
		public FluentColumnParent GetParent()
		{
			return new FluentColumnParent(Family, null);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		private CassandraType GetColumnValue(object name)
		{
			var col = Columns.FirstOrDefault(c => c.ColumnName == name);

			if (col == null)
				return NullType.Value;

			var schema = GetColumnSchema(name);
			return (CassandraType)col.ColumnValue.GetValue(schema.ValueType);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		private CassandraColumnSchema GetColumnSchema(object name)
		{
			var schema = GetSchema();

			// mock up a fake schema to send to the fluent column
			return new CassandraColumnSchema { NameType = schema.ValueType, ValueType = typeof(BytesType) };
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="result"></param>
		/// <returns></returns>
		public override bool TryGetColumn(object name, out object result)
		{
			result = GetColumnValue(name);

			return true;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		public override bool TrySetColumn(object name, object value)
		{
			var col = Columns.FirstOrDefault(c => c.ColumnName == name);
			var mutationType = MutationType.Changed;

			// if column doesn't exisit create it and add it to the columns
			if (col == null)
			{
				var schema = GetColumnSchema(name);
				mutationType = MutationType.Added;

				col = new FluentColumn(schema);
				col.ColumnName = CassandraType.GetTypeFromObject(name, schema.NameType);

				_columns.SupressChangeNotification = true;
				_columns.Add(col);
				_columns.SupressChangeNotification = false;
			}

			// set the column value
			col.ColumnValue = CassandraType.GetTypeFromObject(value);

			// notify the tracker that the column has changed
			OnColumnMutated(mutationType, col);

			return true;
		}

		#region IFluentBaseColumn Members

		IFluentBaseColumnFamily IFluentBaseColumn.Family { get { return Family; } }

		void IFluentBaseColumn.SetParent(FluentColumnParent parent)
		{
			UpdateParent(parent);
		}

		private void UpdateParent(FluentColumnParent parent)
		{
			Family = parent.ColumnFamily as FluentSuperColumnFamily;

			var columnParent = GetPath();
			_columns.Parent = columnParent;

			foreach (var col in Columns)
				col.SetParent(columnParent);

			ResetMutationAndAddAllColumns();
		}

		#endregion
	}
}
