using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MITAB
{
    /// <summary>
    /// Represents a single feature beloning to a layer.
    /// </summary>
    public class Feature : IDisposable
    {
        /// <summary>
        /// Handle used to manipulate the object in the C API.
        /// </summary>
        public readonly IntPtr Handle;
        /// <summary>
        /// The id of this feature.
        /// </summary>
        public readonly int Id;
        /// <summary>
        /// The feature type.
        /// </summary>
        public readonly FeatureType Type;
        /// <summary>
        /// The layer the Feature belongs to.
        /// </summary>
        public readonly MiLayer Layer;
        /// <summary>
        /// The set of parts comprising this feature.
        /// </summary>
        public readonly Parts Parts;

        protected internal Feature(MiLayer layer, int featureId)
        {
            this.Id = featureId;
            this.Layer = layer;
            this.Handle = MiApi.mitab_c_read_feature(layer.Handle, featureId);
            this.Type = MiApi.mitab_c_get_type(Handle);
            this.Parts = CreateParts(this);
        }

        protected internal Feature(MiLayer layer, int nid, FeatureType type, List<List<Vertex>> nParts, 
            List<String> nFieldValues, Dictionary<string, string> nStyle)
        {
            this.Id = nid;
            this.Handle = MiApi.mitab_c_create_feature(layer.Handle, (int)type);
            this.Layer = layer;
            this.Type = type;
            this.Parts = new Parts(this, nParts);
            if (nStyle != null) this.SetStyle(nStyle);
            if (nFieldValues != null)
            {
                for (int i=0; i < nFieldValues.Count; i++)
                {
                    MiApi.mitab_c_set_field(this.Handle, i, nFieldValues[i]);
                }
            }
            IntPtr res = MiApi.mitab_c_write_feature(layer.Handle, this.Handle);
            if (res.ToInt32() == 1)
            {
             //?
                MiApi.mitab_c_destroy_feature(this.Handle);
                //this.Id = MiApi.mitab_c_get_feature_id(this.Handle);
            }
            else throw new Exception("Ошибка создания Feature");//
        }

        /// <summary>
        /// Override this to support descendants of the Parts class.
        /// </summary>
        /// <returns>This layers fields</returns>
        protected internal virtual Parts CreateParts(Feature feature)
        {
            return new Parts(this);
        }

        /// <summary>
        /// Returns text associated with this feature
        /// </summary>
        /// <remarks>This will return an empty string, unless this features type is
        /// TABFC_Text.</remarks>
        public string Text
        {
            get
            {
                return (this.Type == FeatureType.TABFC_Text) ?
                    MiApi.mitab_c_get_text(this.Handle) :
                    "";
            }
        }

        private void GetSymbol(Dictionary<string, string> rDict)
        {
            rDict.Add("symbol_no", MiApi.mitab_c_get_symbol_no(this.Handle).ToString());
            rDict.Add("symbol_size", MiApi.mitab_c_get_symbol_size(this.Handle).ToString());
            rDict.Add("symbol_color", MiApi.mitab_c_get_symbol_color(this.Handle).ToString());
        }

        private void GetPen(Dictionary<string, string> rDict)
        {
            rDict.Add("pen_pattern", MiApi.mitab_c_get_pen_pattern(this.Handle).ToString());
            rDict.Add("pen_width", MiApi.mitab_c_get_pen_width(this.Handle).ToString());
            rDict.Add("pen_color", MiApi.mitab_c_get_pen_color(this.Handle).ToString());
        }

        private void GetBrush(Dictionary<string, string> rDict)
        {
            rDict.Add("brush_fgcolor", MiApi.mitab_c_get_brush_fgcolor(this.Handle).ToString());
            rDict.Add("brush_bgcolor", MiApi.mitab_c_get_brush_bgcolor(this.Handle).ToString());
            rDict.Add("brush_pattern", MiApi.mitab_c_get_brush_pattern(this.Handle).ToString());
            rDict.Add("brush_transparent", MiApi.mitab_c_get_brush_transparent(this.Handle).ToString());
        }

        private void GetTextDisplay(Dictionary<string, string> rDict)
        {
            rDict.Add("text_angle", MiApi.mitab_c_get_text_angle(this.Handle).ToString());
            rDict.Add("text_height", MiApi.mitab_c_get_text_height(this.Handle).ToString());
            rDict.Add("text_width", MiApi.mitab_c_get_text_width(this.Handle).ToString());
            rDict.Add("text_fgcolor", MiApi.mitab_c_get_text_fgcolor(this.Handle).ToString());
            rDict.Add("text_bgcolor", MiApi.mitab_c_get_text_bgcolor(this.Handle).ToString());
            rDict.Add("text_justification", MiApi.mitab_c_get_text_justification(this.Handle).ToString());
            rDict.Add("text_spacing", MiApi.mitab_c_get_text_spacing(this.Handle).ToString());
            rDict.Add("text_linetype", MiApi.mitab_c_get_text_linetype(this.Handle).ToString());
        }

        public Dictionary<string, string> GetStyle()
        {
            Dictionary<string, string> rDict = new Dictionary<string, string>();
            switch (this.Type)
            {
                case FeatureType.TABFC_Point:
                case FeatureType.TABFC_MultiPoint:
                    this.GetSymbol(rDict);
                    break;
                case FeatureType.TABFC_FontPoint:
                    this.GetSymbol(rDict);
                    rDict.Add("font", MiApi.mitab_c_get_font(this.Handle));
                    break;
                case FeatureType.TABFC_Text:
                    rDict.Add("text", MiApi.mitab_c_get_text(this.Handle));
                    rDict.Add("font", MiApi.mitab_c_get_font(this.Handle));
                    this.GetTextDisplay(rDict);
                    break;
                case FeatureType.TABFC_Polyline:
                    this.GetPen(rDict);
                    break;
                case FeatureType.TABFC_Ellipse:
                    this.GetBrush(rDict);
                    this.GetPen(rDict);
                    break;
                case FeatureType.TABFC_Region:
                    this.GetBrush(rDict);
                    this.GetPen(rDict);
                    break;
                case FeatureType.TABFC_Rectangle:
                    this.GetBrush(rDict);
                    this.GetPen(rDict);
                    break;
            }
            return rDict;
        }

        private string GetStringPr(Dictionary<string, string> rDict, string name, string defval)
        {
            if (rDict.ContainsKey(name)) return rDict[name];
            else return defval;
        }

        private int GetIntPr(Dictionary<string, string> rDict, string name, int defval)
        {
            if (rDict.ContainsKey(name)) return int.Parse(rDict[name]);
            else return defval;
        }

        private double GetDoublePr(Dictionary<string, string> rDict, string name, double defval)
        {
            if (rDict.ContainsKey(name)) return double.Parse(rDict[name]);
            else return defval;
        }

        private void SetSymbol(Dictionary<string, string> rDict)
        {
            int symbol_no = this.GetIntPr(rDict, "symbol_no", 1), 
                symbol_size = this.GetIntPr(rDict, "symbol_size", 15), 
                symbol_color = this.GetIntPr(rDict, "symbol_color", 0);
            MiApi.mitab_c_set_symbol(this.Handle, symbol_no, symbol_size, symbol_color);
        }

        private void SetPen(Dictionary<string, string> rDict)
        {
            int pen_pattern = this.GetIntPr(rDict, "pen_pattern", 1),
                pen_width = this.GetIntPr(rDict, "pen_width", 1),
                pen_color = this.GetIntPr(rDict, "pen_color", 0);
            MiApi.mitab_c_set_pen(this.Handle, pen_width, pen_pattern, pen_color);
        }

        private void SetBrush(Dictionary<string, string> rDict)
        {
            int brush_fgcolor = this.GetIntPr(rDict, "brush_fgcolor", 0),
                brush_bgcolor = this.GetIntPr(rDict, "brush_bgcolor", 0),
                brush_pattern = this.GetIntPr(rDict, "brush_pattern", 1),
                brush_transparent = this.GetIntPr(rDict, "brush_transparent", 0);
            MiApi.mitab_c_set_brush(this.Handle, brush_fgcolor, brush_bgcolor, brush_pattern, brush_transparent);
        }

        private void SetTextDisplay(Dictionary<string, string> rDict)
        {
            double text_angle = this.GetDoublePr(rDict, "text_angle", 0),
                text_height = this.GetDoublePr(rDict, "text_height", 1),
                text_width = this.GetDoublePr(rDict, "text_width", 1);
            int text_fgcolor = this.GetIntPr(rDict, "text_fgcolor", 0),
                text_bgcolor = this.GetIntPr(rDict, "text_bgcolor", 0),
                text_justification = this.GetIntPr(rDict, "text_justification", 0),
                text_spacing = this.GetIntPr(rDict, "text_spacing", 0),
                text_linetype = this.GetIntPr(rDict, "text_linetype", 0);
            MiApi.mitab_c_set_text_display(this.Handle, text_angle, text_height, text_width,
                text_fgcolor, text_bgcolor, text_justification, text_spacing, text_linetype);
        }

        public void SetStyle(Dictionary<string, string> rDict)
        {
            switch (this.Type)
            {
                case FeatureType.TABFC_Point:
                case FeatureType.TABFC_MultiPoint:
                    this.SetSymbol(rDict);
                    break;
                case FeatureType.TABFC_FontPoint:
                    this.SetSymbol(rDict);
                    MiApi.mitab_c_set_font(this.Handle, this.GetStringPr(rDict, "font", "Arial"));
                    break;
                case FeatureType.TABFC_Text:
                    MiApi.mitab_c_set_text(this.Handle, this.GetStringPr(rDict, "text", "text"));
                    MiApi.mitab_c_set_font(this.Handle, this.GetStringPr(rDict, "font", "Arial"));
                    this.SetTextDisplay(rDict);
                    break;
                case FeatureType.TABFC_Polyline:
                    this.SetPen(rDict);
                    break;
                case FeatureType.TABFC_Ellipse:
                    this.SetBrush(rDict);
                    this.SetPen(rDict);
                    break;
                case FeatureType.TABFC_Region:
                    this.SetBrush(rDict);
                    this.SetPen(rDict);
                    break;
                case FeatureType.TABFC_Rectangle:
                    this.SetBrush(rDict);
                    this.SetPen(rDict);
                    break;
            }
        }

        public override string ToString()
        {
            StringBuilder str = new StringBuilder();
            str.Append($"Feature: {Id}\nFields:\n");

            foreach (Field field in this.Layer.Fields)
                str.Append($"{field.GetValueAsString(this).Trim()}\t");

            str.Append($"\n{this.Parts.ToString()}\n");

            Dictionary<string, string> prs = this.GetStyle();

            foreach (KeyValuePair<string, string> pr in prs)
            {
                str.AppendLine($"{pr.Key}: {pr.Value}");
            }

            return str.ToString();
        }

        /// <summary>
        /// Convenience method to return the next Feature in the file.
        /// </summary>
        /// <returns>A following feature in the file.</returns>
        public Feature GetNext()
        {
            return new Feature(this.Layer, MiApi.mitab_c_next_feature_id(Layer.Handle, this.Id));
        }

        private bool disposed = false;

        public void Dispose(bool disposing)
        {
            if (disposing && !disposed)
            {
                MiApi.mitab_c_destroy_feature(this.Handle);
                disposed = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        ~Feature()
        {
            Dispose(false);
        }
    }
}
