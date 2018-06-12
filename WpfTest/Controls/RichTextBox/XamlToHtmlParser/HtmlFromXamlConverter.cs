//---------------------------------------------------------------------------
// 
// File: HtmlFromXamlConverter.cs
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
// Description: Prototype for Xaml - Html conversion 
//
//---------------------------------------------------------------------------

namespace Xceed.Wpf.Toolkit
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Xml;

    internal static class HtmlFromXamlConverter
    {
        internal static string ConvertXamlToHtml(string xamlString, string defaultFontFamily, string defaultFontSize)
        {
            return ConvertXamlToHtml(xamlString, true, defaultFontFamily, defaultFontSize);
        }

        internal static string ConvertXamlToHtml(string xamlString, bool asFlowDocument, string defaultFontFamily, string defaultFontSize)
        {
            StringBuilder htmlStringBuilder;
            XmlTextWriter htmlWriter;

            if (!asFlowDocument)
            {
                xamlString = "<FlowDocument>" + xamlString + "</FlowDocument>";
            }

            using (XmlTextReader xamlReader = new XmlTextReader(new StringReader(xamlString)))
            {
                htmlStringBuilder = new StringBuilder(100);
                using (StringWriter htmlStringWiter = new StringWriter(htmlStringBuilder, CultureInfo.InvariantCulture))
                {
                    htmlWriter = new XmlTextWriter(htmlStringWiter);

                    if (!WriteFlowDocument(xamlReader, htmlWriter, true, defaultFontFamily, defaultFontSize))
                    {
                        return "";
                    }
                }

                string htmlString = htmlStringBuilder.ToString();
                return htmlString;
            }
        }

        internal static string ConvertXamlToHtmlWithoutHtmlAndBody(string xamlString, bool asFlowDocument, string defaultFontFamily, string defaultFontSize)
        {
            StringBuilder htmlStringBuilder;
            XmlTextWriter htmlWriter;

            if (!asFlowDocument)
            {
                xamlString = "<FlowDocument>" + xamlString + "</FlowDocument>";
            }

            using (XmlTextReader xamlReader = new XmlTextReader(new StringReader(xamlString)))
            {
                htmlStringBuilder = new StringBuilder(100);
                using (StringWriter htmlStringWiter = new StringWriter(htmlStringBuilder, CultureInfo.InvariantCulture))
                {
                    htmlWriter = new XmlTextWriter(htmlStringWiter);

                    if (!WriteFlowDocument(xamlReader, htmlWriter, false, defaultFontFamily, defaultFontSize))
                    {
                        return "";
                    }
                }

                string htmlString = htmlStringBuilder.ToString();
                return htmlString;
            }
        }

        private static bool WriteFlowDocument(XmlTextReader xamlReader, XmlTextWriter htmlWriter, bool withHtmlAndBody, string defaultFontFamily, string defaultFontSize)
        {
            if (!ReadNextToken(xamlReader))
            {
                // Xaml content is empty - nothing to convert
                return false;
            }

            if (xamlReader.NodeType != XmlNodeType.Element || xamlReader.Name != "FlowDocument")
            {
                // Root FlowDocument elemet is missing
                return false;
            }

            // Create a buffer StringBuilder for collecting css properties for inline STYLE attributes
            // on every element level (it will be re-initialized on every level).
            StringBuilder inlineStyle = new StringBuilder();

            if (withHtmlAndBody)
            {
                htmlWriter.WriteStartElement("HTML");
                htmlWriter.WriteStartElement("BODY");
            }

            WriteFormattingProperties(xamlReader, htmlWriter, inlineStyle, defaultFontFamily, defaultFontSize);
            var isContentEmpty = true;
            WriteElementContent(xamlReader, htmlWriter, inlineStyle, ref isContentEmpty, defaultFontFamily, defaultFontSize);

            if (withHtmlAndBody)
            {
                htmlWriter.WriteEndElement();
                htmlWriter.WriteEndElement();
            }

            return true;
        }

        private static void WriteFormattingProperties(XmlTextReader xamlReader, XmlTextWriter htmlWriter, StringBuilder inlineStyle, string defaultFontFamily, string defaultFontSize)
        {
            Debug.Assert(xamlReader.NodeType == XmlNodeType.Element);
            var elementName = xamlReader.Name;

            // Clear string builder for the inline style
            inlineStyle.Remove(0, inlineStyle.Length);

            if (!xamlReader.HasAttributes)
            {
                return;
            }

            bool borderSet = false;
            var fontSizeSet = false;
            var fontFamilySet = false;

            while (xamlReader.MoveToNextAttribute())
            {
                string css = null;

                switch (xamlReader.Name)
                {
                    // Character fomatting properties
                    // ------------------------------
                    case "Background":
                        css = "background-color:" + ParseXamlColor(xamlReader.Value) + ";";
                        break;
                    case "FontFamily":
                        css = "font-family:" + xamlReader.Value + ";";
                        fontFamilySet = true;
                        break;
                    case "FontStyle":
                        css = "font-style:" + xamlReader.Value.ToLower(CultureInfo.InvariantCulture) + ";";
                        break;
                    case "FontWeight":
                        css = "font-weight:" + xamlReader.Value.ToLower(CultureInfo.InvariantCulture) + ";";
                        break;
                    case "FontStretch":
                        break;
                    case "FontSize":
                        css = "font-size:" + ParseXamlLength(xamlReader.Value) + ";";
                        fontSizeSet = true;
                        if (double.TryParse(xamlReader.Value, out double value))
                        {
                            defaultFontSize = value.ToString();
                        }
                        break;
                    case "Foreground":
                        css = "color:" + ParseXamlColor(xamlReader.Value) + ";";
                        break;
                    case "TextDecorations":
                        css = "text-decoration:underline;";
                        break;
                    case "TextEffects":
                        break;
                    case "Emphasis":
                        break;
                    case "StandardLigatures":
                        break;
                    case "Variants":
                        break;
                    case "Capitals":
                        break;
                    case "Fraction":
                        break;

                    // Paragraph formatting properties
                    // -------------------------------
                    case "Padding":
                        css = "padding:" + ParseXamlThickness(xamlReader.Value) + ";";
                        break;
                    case "Margin":
                        css = "margin:" + ParseXamlThickness(xamlReader.Value) + ";";
                        break;
                    case "BorderThickness":
                        css = "border-width:" + ParseXamlThickness(xamlReader.Value) + ";";
                        borderSet = true;
                        break;
                    case "BorderBrush":
                        css = "border-color:" + ParseXamlColor(xamlReader.Value) + ";";
                        borderSet = true;
                        break;
                    case "LineHeight":
                        break;
                    case "TextIndent":
                        css = "text-indent:" + ParseXamlLength(xamlReader.Value) + ";";
                        break;
                    case "TextAlignment":
                        css = "text-align:" + xamlReader.Value + ";";
                        break;
                    case "IsKeptTogether":
                        break;
                    case "IsKeptWithNext":
                        break;
                    case "ColumnBreakBefore":
                        break;
                    case "PageBreakBefore":
                        break;
                    case "FlowDirection":
                        break;
                    case "Width":
                        css = "width:" + ParseXamlLength(xamlReader.Value) + ";";
                        break;
                    case "Height":
                        css = "height:" + ParseXamlLength(xamlReader.Value) + ";";
                        break;

                    // Table attributes
                    // ----------------

                    case "ColumnSpan":
                        htmlWriter.WriteAttributeString("COLSPAN", xamlReader.Value);
                        break;
                    case "RowSpan":
                        htmlWriter.WriteAttributeString("ROWSPAN", xamlReader.Value);
                        break;

                    // Hyperlink Attributes
                    case "NavigateUri":
                        htmlWriter.WriteAttributeString("HREF", xamlReader.Value);
                        break;

                    // Image Attributes
                    case "Source":
                        htmlWriter.WriteAttributeString("SRC", xamlReader.Value);
                        htmlWriter.WriteAttributeString("TITLE", "");
                        htmlWriter.WriteAttributeString("ALT", "");
                        break;

                    // OL Attributes
                    case "MarkerStyle":
                        htmlWriter.WriteAttributeString("type", ParseXamlOlType(xamlReader.Value));
                        break;
                }

                if (css != null)
                {
                    inlineStyle.Append(css);
                }
            }

            if (borderSet)
            {
                inlineStyle.Append("border-style:solid;mso-element:para-border-div;");
            }

            if (elementName == "Paragraph" /*|| elementName == "Run" || elementName == "Span"*/)
            {
                if (!fontSizeSet)
                {
                    inlineStyle.Append("font-size:" + defaultFontSize + "px;");
                }
                if (!fontFamilySet)
                {
                    inlineStyle.Append("font-family:" + defaultFontFamily + ";");
                }
            }

            // Return the xamlReader back to element level
            xamlReader.MoveToElement();
            Debug.Assert(xamlReader.NodeType == XmlNodeType.Element);
        }

        private static string ParseXamlColor(string color)
        {
            if (color.StartsWith("#", StringComparison.OrdinalIgnoreCase))
            {
                // Remove transparancy value
                color = "#" + color.Substring(3);
            }
            return color;
        }

        private static string ParseXamlLength(string length)
        {
            if (string.IsNullOrWhiteSpace(length))
            {
                return "";
            }

            if (length == "0")
            {
                return length;
            }
            else
            {
                return length + "px";
            }
        }

        private static string ParseXamlThickness(string thickness)
        {
            string[] values = thickness.Split(',');

            for (int i = 0; i < values.Length; i++)
            {
                double value;
                if (double.TryParse(values[i], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out value))
                {
                    values[i] = ParseXamlLength(Math.Ceiling(value).ToString(CultureInfo.InvariantCulture));
                }
                else
                {
                    values[i] = "1px";
                }
            }

            string cssThickness;
            switch (values.Length)
            {
                case 1:
                    cssThickness = thickness;
                    break;
                case 2:
                    cssThickness = values[1] + " " + values[0];
                    break;
                case 4:
                    cssThickness = values[1] + " " + values[2] + " " + values[3] + " " + values[0];
                    break;
                default:
                    cssThickness = values[0];
                    break;
            }

            return cssThickness;
        }

        static string ParseXamlOlType(string type)
        {
            switch (type)
            {
                case "Decimal":
                    return "1";
            }

            return "";
        }

        private static void WriteElementContent(XmlTextReader xamlReader, XmlTextWriter htmlWriter, StringBuilder inlineStyle, ref bool isContentEmpty, string defaultFontFamily, string defaultFontSize)
        {
            Debug.Assert(xamlReader.NodeType == XmlNodeType.Element);

            bool elementContentStarted = false;

            if (xamlReader.IsEmptyElement)
            {
                if (htmlWriter != null && !elementContentStarted && inlineStyle.Length > 0)
                {
                    // Output STYLE attribute and clear inlineStyle buffer.
                    htmlWriter.WriteAttributeString("STYLE", inlineStyle.ToString());
                    inlineStyle.Remove(0, inlineStyle.Length);
                }
                elementContentStarted = true;
            }
            else
            {
                while (ReadNextToken(xamlReader) && xamlReader.NodeType != XmlNodeType.EndElement)
                {
                    switch (xamlReader.NodeType)
                    {
                        case XmlNodeType.Element:
                            if (xamlReader.Name.Equals("Image.Source", StringComparison.OrdinalIgnoreCase))
                            {
                                if (ReadNextToken(xamlReader) && xamlReader.NodeType != XmlNodeType.EndElement && xamlReader.Name == "BitmapImage")
                                {
                                    var baseUri = "";
                                    var uriSource = "";
                                    while (xamlReader.MoveToNextAttribute())
                                    {
                                        switch (xamlReader.Name)
                                        {
                                            case "BaseUri":
                                                baseUri = xamlReader.Value;
                                                break;
                                            case "UriSource":
                                                uriSource = xamlReader.Value;
                                                break;
                                            default:
                                                break;
                                        }
                                    }

                                    if (!string.IsNullOrEmpty(baseUri) && !string.IsNullOrEmpty(uriSource))
                                    {
                                        htmlWriter.WriteAttributeString("SRC", "");
                                        isContentEmpty = false;
                                    }
                                }
                                xamlReader.MoveToElement();
                            }
                            else if (xamlReader.Name.Contains("."))
                            {
                                AddComplexProperty(xamlReader, inlineStyle, defaultFontFamily, defaultFontSize);
                            }
                            else
                            {
                                if (htmlWriter != null && !elementContentStarted && inlineStyle.Length > 0)
                                {
                                    // Output STYLE attribute and clear inlineStyle buffer.
                                    htmlWriter.WriteAttributeString("STYLE", inlineStyle.ToString());
                                    inlineStyle.Remove(0, inlineStyle.Length);
                                }
                                elementContentStarted = true;
                                WriteElement(xamlReader, htmlWriter, inlineStyle, ref isContentEmpty, defaultFontFamily, defaultFontSize);
                            }

                            Debug.Assert(xamlReader.NodeType == XmlNodeType.EndElement || xamlReader.NodeType == XmlNodeType.Element && xamlReader.IsEmptyElement);
                            break;

                        case XmlNodeType.Comment:
                            if (htmlWriter != null)
                            {
                                if (!elementContentStarted && inlineStyle.Length > 0)
                                {
                                    htmlWriter.WriteAttributeString("STYLE", inlineStyle.ToString());
                                }
                                htmlWriter.WriteComment(xamlReader.Value);
                            }
                            elementContentStarted = true;
                            isContentEmpty = false;
                            break;

                        case XmlNodeType.CDATA:
                        case XmlNodeType.Text:
                        case XmlNodeType.SignificantWhitespace:
                            if (htmlWriter != null)
                            {
                                if (!string.IsNullOrEmpty(xamlReader.Value))
                                {
                                    if (!elementContentStarted && inlineStyle.Length > 0)
                                    {
                                        htmlWriter.WriteAttributeString("STYLE", inlineStyle.ToString());
                                    }
                                    var buider = new StringBuilder(xamlReader.Value);
                                    buider.Replace(" ", " ").Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;").Replace(" ", "&nbsp;").Replace("\t", "&nbsp;&nbsp;&nbsp;&nbsp;");
                                    htmlWriter.WriteRaw(buider.ToString());
                                    isContentEmpty = false;
                                }
                            }
                            elementContentStarted = true;
                            break;
                    }
                }

                Debug.Assert(xamlReader.NodeType == XmlNodeType.EndElement);
            }
        }

        private static void AddComplexProperty(XmlTextReader xamlReader, StringBuilder inlineStyle, string defaultFontFamily, string defaultFontSize)
        {
            Debug.Assert(xamlReader.NodeType == XmlNodeType.Element);

            if (inlineStyle != null)
            {
                var name = xamlReader.Name;
                if (name.EndsWith(".TextDecorations", StringComparison.OrdinalIgnoreCase))
                {
                    inlineStyle.Append("text-decoration:underline;");
                }
            }

            // Skip the element representing the complex property
            var isContentEmpty = true;
            WriteElementContent(xamlReader, /*htmlWriter:*/null, /*inlineStyle:*/null, ref isContentEmpty, defaultFontFamily, defaultFontSize);
        }

        private static void WriteElement(XmlTextReader xamlReader, XmlTextWriter htmlWriter, StringBuilder inlineStyle, ref bool isContentEmpty, string defaultFontFamily, string defaultFontSize)
        {
            Debug.Assert(xamlReader.NodeType == XmlNodeType.Element);

            var paragraphProperties = "";

            if (htmlWriter == null)
            {
                // Skipping mode; recurse into the xaml element without any output
                WriteElementContent(xamlReader, /*htmlWriter:*/null, null, ref isContentEmpty, defaultFontFamily, defaultFontSize);
            }
            else
            {
                string htmlElementName = null;

                switch (xamlReader.Name)
                {
                    case "Run":
                    case "Span":
                        htmlElementName = "SPAN";
                        break;
                    case "InlineUIContainer":
                        htmlElementName = "SPAN";
                        break;
                    case "Bold":
                        htmlElementName = "B";
                        break;
                    case "Italic":
                        htmlElementName = "I";
                        break;
                    case "Paragraph":
                        htmlElementName = "P";
                        isContentEmpty = true;
                        break;
                    case "BlockUIContainer":
                        htmlElementName = "DIV";
                        break;
                    case "Section":
                        htmlElementName = "DIV";
                        break;
                    case "Table":
                        htmlElementName = "TABLE";
                        break;
                    case "TableColumn":
                        htmlElementName = "COL";
                        break;
                    case "TableRowGroup":
                        htmlElementName = "TBODY";
                        break;
                    case "TableRow":
                        htmlElementName = "TR";
                        break;
                    case "TableCell":
                        htmlElementName = "TD";
                        break;
                    case "List":
                        string marker = xamlReader.GetAttribute("MarkerStyle");
                        if (marker == null || marker == "None" || marker == "Disc" || marker == "Circle" || marker == "Square" || marker == "Box")
                            htmlElementName = "UL";
                        else
                            htmlElementName = "OL";
                        break;
                    case "ListItem":
                        htmlElementName = "LI";
                        break;
                    case "Hyperlink":
                        htmlElementName = "A";
                        break;
                    case "LineBreak":
                        htmlWriter.WriteRaw("<BR />");
                        break;
                    case "Image":
                        htmlElementName = "IMG";
                        isContentEmpty = false;
                        break;
                    default:
                        htmlElementName = null; // Ignore the element
                        break;
                }

                if (htmlWriter != null && !String.IsNullOrEmpty(htmlElementName))
                {
                    var builder = new StringBuilder(100);
                    using (StringWriter htmlStringWiter = new StringWriter(builder, CultureInfo.InvariantCulture))
                    {
                        var htmlWriterTemp = new XmlTextWriter(htmlStringWiter);

                        if (htmlElementName == "DIV")
                        {
                            htmlWriterTemp.WriteStartElement("P");
                            if (inlineStyle != null && !inlineStyle.ToString().Contains("text-align"))
                            {
                                inlineStyle.Append("text-align:center");
                            }
                        }
                        else
                        {
                            htmlWriterTemp.WriteStartElement(htmlElementName);
                            WriteFormattingProperties(xamlReader, htmlWriterTemp, inlineStyle, defaultFontFamily, defaultFontSize);
                        }

                        if (htmlElementName == "P")
                        {
                            paragraphProperties = inlineStyle.ToString();
                        }

                        WriteElementContent(xamlReader, htmlWriterTemp, inlineStyle, ref isContentEmpty, defaultFontFamily, defaultFontSize);
                        htmlWriterTemp.WriteEndElement();
                    }

                    if (isContentEmpty)
                    {
                        if (xamlReader.Name == "Paragraph")
                        {
                            var style = "";
                            if (!string.IsNullOrEmpty(paragraphProperties))
                            {
                                style = $@" STYLE=""{paragraphProperties}""";
                            }
                            htmlWriter.WriteRaw($"<P{style}><BR /></P>");
                        }
                    }
                    else
                    {
                        htmlWriter.WriteRaw(builder.ToString());
                    }
                }
                else
                {
                    // Skip this unrecognized xaml element
                    WriteElementContent(xamlReader, /*htmlWriter:*/null, null, ref isContentEmpty, defaultFontFamily, defaultFontSize);
                }
            }
        }

        private static bool ReadNextToken(XmlReader xamlReader)
        {
            while (xamlReader.Read())
            {
                Debug.Assert(xamlReader.ReadState == ReadState.Interactive, "Reader is expected to be in Interactive state (" + xamlReader.ReadState + ")");
                switch (xamlReader.NodeType)
                {
                    case XmlNodeType.Element:
                    case XmlNodeType.EndElement:
                    case XmlNodeType.None:
                    case XmlNodeType.CDATA:
                    case XmlNodeType.Text:
                    case XmlNodeType.SignificantWhitespace:
                        return true;

                    case XmlNodeType.Whitespace:
                        if (xamlReader.XmlSpace == XmlSpace.Preserve)
                        {
                            return true;
                        }
                        // ignore insignificant whitespace
                        break;

                    case XmlNodeType.EndEntity:
                    case XmlNodeType.EntityReference:
                        //  Implement entity reading
                        //xamlReader.ResolveEntity();
                        //xamlReader.Read();
                        //ReadChildNodes( parent, parentBaseUri, xamlReader, positionInfo);
                        break; // for now we ignore entities as insignificant stuff

                    case XmlNodeType.Comment:
                        return true;
                    case XmlNodeType.ProcessingInstruction:
                    case XmlNodeType.DocumentType:
                    case XmlNodeType.XmlDeclaration:
                    default:
                        // Ignorable stuff
                        break;
                }
            }
            return false;
        }

    }
}
