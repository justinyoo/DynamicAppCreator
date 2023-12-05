using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAC.core
{
    public static class ObjectMapper
    {
        public static IList<TEntity> DataTableToEntity<TEntity>(System.Data.DataTable source) where TEntity : class, new()
        {

            var allProps = typeof(TEntity).GetProperties().Where(t => t.CanWrite);
            List<TEntity> rows = new List<TEntity>();

            foreach (System.Data.DataRow row in source.Rows)
            {
                TEntity NewRow = new TEntity();
                foreach (System.Data.DataColumn col in source.Columns)
                {
                    var oCol = NewRow.GetType().GetProperty(col.ColumnName);
                    if (oCol != null && row[col.ColumnName] != null)
                    {

                        try
                        {
                            if (row[col.ColumnName] is System.DBNull)
                            {
                                continue;
                            }
                            oCol.SetValue(NewRow, row[col.ColumnName]);
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
                rows.Add(NewRow);
            }
            return rows;
        }

        public static void Map<Target>(object source, ref Target target)
        {

            var allProps = source.GetType().GetProperties();
            if (target != null)
            {
                foreach (var item in allProps)
                {
                    var targetrop = target.GetType().GetProperty(item.Name);
                    if (targetrop != null && targetrop.CanWrite && targetrop.PropertyType == item.PropertyType)
                    {
                        try
                        {
                            var val = item.GetValue(source);
                            if (val != null)
                            {
                                targetrop.SetValue(target, item.GetValue(source));
                            }
                        }
                        catch (Exception)
                        {
                            //TODO: Static dev mode açılacak.
                        }
                    }
                }
            }

        }
    }
}
