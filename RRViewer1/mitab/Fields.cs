using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITAB
{
    /// <summary>
    /// Contains the set of fields belonging to a layer.
    /// </summary>
    /// <remarks>
    /// This class descends EnumImpl, meaning the fields in the
    /// set can be iterated using foreach.
    /// It also has an index property allowing any field between 0 and Fields.Count-1
    /// to be accessed directly with Fields[idx]
    /// </remarks>
    public class Fields : EnumImpl
    {
        private List<Field> fields = new List<Field>();

        protected internal Fields(MiLayer layer) : base(MiApi.mitab_c_get_field_count(layer.Handle))
        {
            //fields = new Field[Count];
            for (int i = 0; i < Count; i++)
                fields.Add(this.CreateField(layer, i));
                //fields[i] = CreateField(layer, i);
        }

        /// <summary>
        /// Override this to support descendants of the Field class.
        /// </summary>
        /// <returns>A Field, with the given index, belonging to the given Layer</returns>
        protected internal virtual Field CreateField(MiLayer layer, int index)
        {
            return new Field(layer, index);
        }

        public void AddField(MiLayer layer, 
            string field_name, FieldType field_type, int width, int precision, int indexed, int unique)
        {            //
            fields.Add(new Field(layer, field_name, field_type, width, precision, indexed, unique));
        }

        public virtual Field this[int index]
        {
            get
            {
                return index < Count ? fields[index] : null;
            }
        }

        public override object GetObj(int idx)
        {
            return this[idx];
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            str.Append("Columns:\n");
            foreach (Field field in this)
                str.Append(field.ToString() + "\t");
            return str.ToString();
        }
    }
}
