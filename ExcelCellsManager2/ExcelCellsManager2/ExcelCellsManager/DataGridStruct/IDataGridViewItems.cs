
using System.Collections.Generic;

namespace CellsManagerControl.Utility
{
    public interface IDataGridViewItems
    {
        void SetDataSource(in object values);

        void AddData(object value);
        void ConvertDataSourceToListFromObjectType();

        object GetDataSourceAsArray();
        void GetDataSouceAsObjectList(out List<object>dataList);
        string[] GetFieldNames();
        object ConvertValueFromArray(string[] values);
        void Remove(int index);
        void Add(object value);
        void AddDataList(List<object> values);
        void Insert(int index, object value);
        object GetItem(int index);
        int DataSourceListCount { get; }
        void RemoveAllDataSource();

    }
}
