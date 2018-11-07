using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITAB
{
    /// <summary>
    /// Represents a readonly view of field in a layer.
    /// </summary>
    /// <remarks>
    /// A field instance does not relate explicity to a single feature instance. Rather
    /// it represents all the features in the layer. To find the value of a field for a particular
    /// feature one of the GetValueAs methods should be called, passing the feature in.
    /// </remarks>
    public class Field
    {
        /// <summary>
        /// The field name
        /// </summary>
        public readonly string Name;
        /// <summary>
        /// The field type
        /// </summary>
        public FieldType Type;
        /// <summary>
        /// The index of the field within the layers set of fields.
        /// </summary>
        public readonly int Index;
        /// <summary>
        /// The field width
        /// </summary>
        public readonly int Width;
        /// <summary>
        /// The field precision
        /// </summary>
        public readonly short Precision;
        /// <summary>
        /// The layer this field belongs to.
        /// </summary>
        public readonly MiLayer Layer;

        protected internal Field(MiLayer layer, int i)
        {
            this.Layer = layer;
            this.Index = i;
            this.Name = MiApi.mitab_c_get_field_name(layer.Handle, i);
            this.Type = MiApi.mitab_c_get_field_type(layer.Handle, i);
            this.Precision = (short)MiApi.mitab_c_get_field_precision(layer.Handle, i);
            this.Width = MiApi.mitab_c_get_field_width(layer.Handle, i);
        }

        protected internal Field(MiLayer layer, string field_name, FieldType field_type, int width, int precision, int indexed, int unique)
        {
            IntPtr field_id = MiApi.mitab_c_add_field(layer.Handle,
                field_name, (int)field_type, width, precision, indexed, unique); ;
            this.Layer = layer;
            this.Index = field_id.ToInt32();
            this.Name = field_name;
            this.Type = field_type;
            this.Precision = (short)precision;
            this.Width = width;
        }

        /// <summary>
        /// Returns a string representation of this fields value for the given feature.
        /// </summary>
        /// <param name="feature">The feature to find the fields value for.</param>
        /// <returns>A string representation of this fields value for the given feature</returns>
        public string GetValueAsString(Feature feature)
        {
            return MiApi.mitab_c_get_field_as_string(feature.Handle, this.Index);
        }

        /// <summary>
        /// Returns a double representation of this fields value for the given feature.
        /// </summary>
        /// <param name="feature">The feature to find the fields value for.</param>
        /// <returns>A double representation of this fields value for the given feature</returns>
        public double GetValueAsDouble(Feature feature)
        {
            return MiApi.mitab_c_get_field_as_double(feature.Handle, this.Index);
        }

        public override string ToString()
        {
            return this.Name + ", " + this.Type;
        }

    }
}
