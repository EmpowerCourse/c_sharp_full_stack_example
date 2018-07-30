using System;
using System.Collections.Generic;
using System.Text;

namespace Armoire.Common
{
    public abstract class GridColumnSorterMapper<T> where T : GridColumnSorterMapper<T>, new()
    {
        public Dictionary<string, string> ColumnsMap;

        protected abstract void Initialize();

        public string GetEntityPropertyByColumn(string column)
        {
            try
            {
                return ColumnsMap[column];
            }
            catch
            {
            }
            return null;
        }

        public static T CreateMapper()
        {
            var newMapper = new T();
            newMapper.Initialize();
            return newMapper;
        }
    }
}
