using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
// ReSharper disable CheckNamespace
// ReSharper disable IdentifierTypo
// ReSharper disable StringLiteralTypo

namespace MITAB
{
    /// <summary>Represents a single feature beloning to a layer.</summary>
    public sealed class Feature : IDisposable
    {
        /// <summary>Handle used to manipulate the object in the C API.</summary>
        public readonly IntPtr Handle;

        /// <summary>The id of this feature.</summary>
        public readonly int Id;

        /// <summary>The feature type.</summary>
        public readonly FeatureType Type;

        /// <summary>The layer the Feature belongs to.</summary>
        public readonly MiLayer Layer;

        /// <summary>The set of parts comprising this feature.</summary>
        public readonly Parts Parts;

        /// <summary>Конструктор</summary>
        /// <param name="layer">Слой</param>
        /// <param name="featureId">Ключ</param>
        internal Feature(MiLayer layer, int featureId)
        {
            Id = featureId;
            Layer = layer;
            Handle = MiApi.mitab_c_read_feature(layer.Handle, featureId);
            Type = MiApi.mitab_c_get_type(Handle);
            Parts = CreateParts(this);
        }

        /// <summary>Конструктор</summary>
        /// <param name="layer">Слой</param>
        /// <param name="nid">Ключ</param>
        /// <param name="type">Тип</param>
        /// <param name="nParts">Список частей</param>
        /// <param name="nFieldValues">Список значений полей</param>
        /// <param name="nStyle">Стили</param>
        internal Feature(MiLayer layer, int nid, FeatureType type, List<List<Vertex>> nParts, 
            IReadOnlyList<string> nFieldValues, IReadOnlyDictionary<string, string> nStyle)
        {
            Id = nid;
            Handle = MiApi.mitab_c_create_feature(layer.Handle, (int)type);
            Layer = layer;
            Type = type;
            Parts = new Parts(this, nParts);
            if (nStyle != null) SetStyle(nStyle);
            if (nFieldValues != null)
            {
                for (var i = 0; i < nFieldValues.Count; i++)
                {
                    MiApi.mitab_c_set_field(Handle, i, nFieldValues[i]);
                }
            }
            var res = MiApi.mitab_c_write_feature(layer.Handle, Handle);
            if (res.ToInt32() == 1)
            {
                MiApi.mitab_c_destroy_feature(Handle);
            }
            else throw new Exception("Ошибка создания Feature");//
        }

        /// <summary>Override this to support descendants of the Parts class.</summary>
        /// <returns>This layers fields</returns>
        internal Parts CreateParts(Feature feature)
        {
            return new Parts(this);
        }

        /// <summary>Returns text associated with this feature</summary>
        /// <remarks>This will return an empty string, unless this features type is
        /// TABFC_Text.</remarks>
        public string Text => (Type == FeatureType.TABFC_Text) ? MiApi.mitab_c_get_text(Handle) : "";

        private void GetSymbol(Dictionary<string, string> rDict)
        {
            if (rDict == null) throw new ArgumentNullException(nameof(rDict));
            rDict.Add("symbol_no", MiApi.mitab_c_get_symbol_no(Handle).ToString());
            rDict.Add("symbol_size", MiApi.mitab_c_get_symbol_size(Handle).ToString());
            rDict.Add("symbol_color", MiApi.mitab_c_get_symbol_color(Handle).ToString());
        }

        private void GetPen(Dictionary<string, string> rDict)
        {
            if (rDict == null) throw new ArgumentNullException(nameof(rDict));
            rDict.Add("pen_pattern", MiApi.mitab_c_get_pen_pattern(Handle).ToString());
            rDict.Add("pen_width", MiApi.mitab_c_get_pen_width(Handle).ToString());
            rDict.Add("pen_color", MiApi.mitab_c_get_pen_color(Handle).ToString());
        }

        private void GetBrush(Dictionary<string, string> rDict)
        {
            rDict.Add("brush_fgcolor", MiApi.mitab_c_get_brush_fgcolor(Handle).ToString());
            rDict.Add("brush_bgcolor", MiApi.mitab_c_get_brush_bgcolor(Handle).ToString());
            rDict.Add("brush_pattern", MiApi.mitab_c_get_brush_pattern(Handle).ToString());
            rDict.Add("brush_transparent", MiApi.mitab_c_get_brush_transparent(Handle).ToString());
        }

        private void GetTextDisplay(Dictionary<string, string> rDict)
        {
            rDict.Add("text_angle", MiApi.mitab_c_get_text_angle(Handle).ToString(CultureInfo.InvariantCulture));
            rDict.Add("text_height", MiApi.mitab_c_get_text_height(Handle).ToString(CultureInfo.InvariantCulture));
            rDict.Add("text_width", MiApi.mitab_c_get_text_width(Handle).ToString(CultureInfo.InvariantCulture));
            rDict.Add("text_fgcolor", MiApi.mitab_c_get_text_fgcolor(Handle).ToString());
            rDict.Add("text_bgcolor", MiApi.mitab_c_get_text_bgcolor(Handle).ToString());
            rDict.Add("text_justification", MiApi.mitab_c_get_text_justification(Handle).ToString());
            rDict.Add("text_spacing", MiApi.mitab_c_get_text_spacing(Handle).ToString());
            rDict.Add("text_linetype", MiApi.mitab_c_get_text_linetype(Handle).ToString());
        }

        /// <summary>Получить стили</summary>
        /// <returns></returns>
        public Dictionary<string, string> GetStyle()
        {
            var rDict = new Dictionary<string, string>();
            switch (Type)
            {
                case FeatureType.TABFC_Point:
                case FeatureType.TABFC_MultiPoint:
                    GetSymbol(rDict);
                    break;
                case FeatureType.TABFC_FontPoint:
                    GetSymbol(rDict);
                    rDict.Add("font", MiApi.mitab_c_get_font(Handle));
                    break;
                case FeatureType.TABFC_Text:
                    rDict.Add("text", MiApi.mitab_c_get_text(Handle));
                    rDict.Add("font", MiApi.mitab_c_get_font(Handle));
                    GetTextDisplay(rDict);
                    break;
                case FeatureType.TABFC_Polyline:
                    GetPen(rDict);
                    break;
                case FeatureType.TABFC_Ellipse:
                    GetBrush(rDict);
                    GetPen(rDict);
                    break;
                case FeatureType.TABFC_Region:
                    GetBrush(rDict);
                    GetPen(rDict);
                    break;
                case FeatureType.TABFC_Rectangle:
                    GetBrush(rDict);
                    GetPen(rDict);
                    break;
            }
            return rDict;
        }

        private string GetStringPr(IReadOnlyDictionary<string, string> rDict, string name, string defval)
        {
            return rDict.ContainsKey(name) ? rDict[name] : defval;
        }

        private int GetIntPr(IReadOnlyDictionary<string, string> rDict, string name, int defval)
        {
            return rDict.ContainsKey(name) ? int.Parse(rDict[name]) : defval;
        }

        private double GetDoublePr(IReadOnlyDictionary<string, string> rDict, string name, double defval)
        {
            return rDict.ContainsKey(name) ? double.Parse(rDict[name]) : defval;
        }

        private void SetSymbol(IReadOnlyDictionary<string, string> rDict)
        {
            int symbolNo = GetIntPr(rDict, "symbol_no", 1), 
                symbolSize = GetIntPr(rDict, "symbol_size", 15), 
                symbolColor = GetIntPr(rDict, "symbol_color", 0);
            MiApi.mitab_c_set_symbol(Handle, symbolNo, symbolSize, symbolColor);
        }

        private void SetPen(IReadOnlyDictionary<string, string> rDict)
        {
            int penPattern = GetIntPr(rDict, "pen_pattern", 1),
                penWidth = GetIntPr(rDict, "pen_width", 1),
                penColor = GetIntPr(rDict, "pen_color", 0);
            MiApi.mitab_c_set_pen(Handle, penWidth, penPattern, penColor);
        }

        private void SetBrush(IReadOnlyDictionary<string, string> rDict)
        {
            int brushFgcolor = GetIntPr(rDict, "brush_fgcolor", 0),
                brushBgcolor = GetIntPr(rDict, "brush_bgcolor", 0),
                brushPattern = GetIntPr(rDict, "brush_pattern", 1),
                brushTransparent = GetIntPr(rDict, "brush_transparent", 0);
            MiApi.mitab_c_set_brush(Handle, brushFgcolor, brushBgcolor, brushPattern, brushTransparent);
        }

        private void SetTextDisplay(IReadOnlyDictionary<string, string> rDict)
        {
            double textAngle = GetDoublePr(rDict, "text_angle", 0),
                textHeight = GetDoublePr(rDict, "text_height", 1),
                textWidth = GetDoublePr(rDict, "text_width", 1);
            int textFgcolor = GetIntPr(rDict, "text_fgcolor", 0),
                textBgcolor = GetIntPr(rDict, "text_bgcolor", 0),
                textJustification = GetIntPr(rDict, "text_justification", 0),
                textSpacing = GetIntPr(rDict, "text_spacing", 0),
                textLinetype = GetIntPr(rDict, "text_linetype", 0);
            MiApi.mitab_c_set_text_display(Handle, textAngle, textHeight, textWidth,
                textFgcolor, textBgcolor, textJustification, textSpacing, textLinetype);
        }

        /// <summary>Установка значений стилей</summary>
        /// <param name="rDict"></param>
        public void SetStyle(IReadOnlyDictionary<string, string> rDict)
        {
            switch (Type)
            {
                case FeatureType.TABFC_Point:
                case FeatureType.TABFC_MultiPoint:
                    SetSymbol(rDict);
                    break;
                case FeatureType.TABFC_FontPoint:
                    SetSymbol(rDict);
                    MiApi.mitab_c_set_font(Handle, GetStringPr(rDict, "font", "Arial"));
                    break;
                case FeatureType.TABFC_Text:
                    MiApi.mitab_c_set_text(Handle, GetStringPr(rDict, "text", "text"));
                    MiApi.mitab_c_set_font(Handle, GetStringPr(rDict, "font", "Arial"));
                    SetTextDisplay(rDict);
                    break;
                case FeatureType.TABFC_Polyline:
                    SetPen(rDict);
                    break;
                case FeatureType.TABFC_Ellipse:
                    SetBrush(rDict);
                    SetPen(rDict);
                    break;
                case FeatureType.TABFC_Region:
                    SetBrush(rDict);
                    SetPen(rDict);
                    break;
                case FeatureType.TABFC_Rectangle:
                    SetBrush(rDict);
                    SetPen(rDict);
                    break;
                case FeatureType.TABFC_NoGeom:
                    break;
                case FeatureType.TABFC_CustomPoint:
                    break;
                case FeatureType.TABFC_Arc:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <inheritdoc />
        public override string ToString()
        {
            var str = new StringBuilder();
            str.Append($"Feature: {Id}\nFields:\n");
            foreach (Field field in Layer.Fields) str.Append($"{field.GetValueAsString(this).Trim()}\t");
            str.Append($"\n{Parts}\n");
            var prs = GetStyle();
            foreach (var pr in prs) str.AppendLine($"{pr.Key}: {pr.Value}");
            return str.ToString();
        }

        /// <summary>Convenience method to return the next Feature in the file.</summary>
        /// <returns>A following feature in the file.</returns>
        public Feature GetNext()
        {
            return new Feature(Layer, MiApi.mitab_c_next_feature_id(Layer.Handle, Id));
        }

        private bool _disposed;

        /// <summary>Освобождаем ресурсы</summary>
        /// <param name="disposing"></param>
        public void Dispose(bool disposing)
        {
            if (!disposing || _disposed) return;
            MiApi.mitab_c_destroy_feature(Handle);
            _disposed = true;
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
        }

        /// <inheritdoc />
        ~Feature()
        {
            Dispose(false);
        }
    }
}