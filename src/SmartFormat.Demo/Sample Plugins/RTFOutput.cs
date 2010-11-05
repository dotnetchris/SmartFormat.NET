﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using RTF;
using SmartFormat.Core.Output;
using SmartFormat.Core.Parsing;
using SmartFormat.Core.Plugins;

namespace SmartFormat.Demo.Sample_Plugins
{
    public class RTFOutput : IOutput
    {
        public RTFOutput(Color[] nestedColors, Color errorColor)
        {
            if (nestedColors == null || nestedColors.Length == 0) throw new ArgumentException("Nested colors cannot be null or empty.");
            this.nestedColors = nestedColors;
            this.errorColor = errorColor;
        }

        private readonly Color[] nestedColors;

        private RTFBuilder output = new RTFBuilder();
        private Color errorColor;

        public void Clear()
        {
            //output.Clear();
            output = new RTFBuilder();
        }

        public void Write(string text, FormatDetails formatDetails)
        {
            Write(text, 0, text.Length, formatDetails);
        }

        public void Write(LiteralText item, FormatDetails formatDetails)
        {
            Write(item.baseString, item.startIndex, item.endIndex - item.startIndex, formatDetails);
        }


        public void Write(string text, int startIndex, int length, FormatDetails formatDetails)
        {
            // Depending on the nested level, we will color this item differently:
            if (formatDetails.FormatError != null)
            {
                output.BackColor(errorColor).Append(text, startIndex, length);
            }
            else if (formatDetails.Placeholder == null)
            {
                // There is no "nesting" so just output plain text:
                output.Append(text, startIndex, length);
            }
            else
            {
                var nestedDepth = formatDetails.Placeholder.NestedDepth;
                var backcolor = this.nestedColors[nestedDepth % nestedColors.Length];
                output.BackColor(backcolor).Append(text, startIndex, length);
            }
        }


        public override string ToString()
        {
            return output.ToString();
        }
    }
}