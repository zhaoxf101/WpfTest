//---------------------------------------------------------------------------
// 
// File: HtmlFromXamlConverter.cs
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
// Description: Prototype for Xaml - Html conversion 
//
//---------------------------------------------------------------------------

namespace WpfRichText
{
    using System;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Text;
    using System.Xml;

    internal static class HtmlFromXamlConverter
    {
        const string DefaultFontFamily = "Î¢ÈíÑÅºÚ";
        const int DefaultFontSize = 14;

        internal static string ConvertXamlToHtml(string xamlString)
        {
            return ConvertXamlToHtml(xamlString, true);
        }

        internal static string ConvertXamlToHtml(string xamlString, bool asFlowDocument)
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

                    if (!WriteFlowDocument(xamlReader, htmlWriter))
                    {
                        return "";
                    }
                }

                string htmlString = htmlStringBuilder.ToString();
                return htmlString;
            }
        }

        internal static string ConvertXamlToHtmlWithoutHtmlAndBody(string xamlString, bool asFlowDocument)
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

                    if (!WriteFlowDocument(xamlReader, htmlWriter, false))
                    {
                        return "";
                    }
                }

                string htmlString = htmlStringBuilder.ToString();
                return htmlString;
            }
        }

        private static bool WriteFlowDocument(XmlTextReader xamlReader, XmlTextWriter htmlWriter, bool withHtmlAndBody = true)
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

            WriteFormattingProperties(xamlReader, htmlWriter, inlineStyle);
            var isContentEmpty = true;
            WriteElementContent(xamlReader, htmlWriter, inlineStyle, ref isContentEmpty);

            if (withHtmlAndBody)
            {
                htmlWriter.WriteEndElement();
                htmlWriter.WriteEndElement();
            }

            return true;
        }

        private static void WriteFormattingProperties(XmlTextReader xamlReader, XmlTextWriter htmlWriter, StringBuilder inlineStyle)
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
                        css = "font-size:" + xamlReader.Value + "px;";
                        fontSizeSet = true;
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
                        css = "text-indent:" + xamlReader.Value + ";";
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

                    // Table attributes
                    // ----------------
                    case "Width":
                        css = "width:" + xamlReader.Value + ";";
                        break;
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

            if (elementName == "Run" || elementName == "Span")
            {
                if (!fontSizeSet)
                {
                    inlineStyle.Append("font-size:" + DefaultFontSize + "px;");
                }
                if (!fontFamilySet)
                {
                    inlineStyle.Append("font-family:" + DefaultFontFamily + ";");
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

        private static string ParseXamlThickness(string thickness)
        {
            string[] values = thickness.Split(',');

            for (int i = 0; i < values.Length; i++)
            {
                double value;
                if (double.TryParse(values[i], NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out value))
                {
                    values[i] = Math.Ceiling(value).ToString(CultureInfo.InvariantCulture);
                }
                else
                {
                    values[i] = "1";
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

        private static void WriteElementContent(XmlTextReader xamlReader, XmlTextWriter htmlWriter, StringBuilder inlineStyle, ref bool isContentEmpty)
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
                                        htmlWriter.WriteAttributeString("SRC", "http://www.baidu.com");
                                        isContentEmpty = false;
                                    }
                                }
                                xamlReader.MoveToElement();
                            }
                            else if (xamlReader.Name.Contains("."))
                            {
                                AddComplexProperty(xamlReader, inlineStyle);
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
                                WriteElement(xamlReader, htmlWriter, inlineStyle, ref isContentEmpty);
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
                                    htmlWriter.WriteRaw(xamlReader.Value.Replace(" ", "&nbsp;"));
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

        private static void AddComplexProperty(XmlTextReader xamlReader, StringBuilder inlineStyle)
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
            WriteElementContent(xamlReader, /*htmlWriter:*/null, /*inlineStyle:*/null, ref isContentEmpty);
        }

        private static void WriteElement(XmlTextReader xamlReader, XmlTextWriter htmlWriter, StringBuilder inlineStyle, ref bool isContentEmpty)
        {
            Debug.Assert(xamlReader.NodeType == XmlNodeType.Element);

            if (htmlWriter == null)
            {
                // Skipping mode; recurse into the xaml element without any output
                WriteElementContent(xamlReader, /*htmlWriter:*/null, null, ref isContentEmpty);
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

                        htmlWriterTemp.WriteStartElement(htmlElementName);
                        WriteFormattingProperties(xamlReader, htmlWriterTemp, inlineStyle);
                        WriteElementContent(xamlReader, htmlWriterTemp, inlineStyle, ref isContentEmpty);
                        htmlWriterTemp.WriteEndElement();
                    }

                    if (isContentEmpty)
                    {
                        if (xamlReader.Name == "Paragraph")
                        {
                            htmlWriter.WriteRaw("<P><BR /></P>");
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
                    WriteElementContent(xamlReader, /*htmlWriter:*/null, null, ref isContentEmpty);
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
