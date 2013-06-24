using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Windows.Forms;

namespace Klippy
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            string tsv;
            // TODO: Support `TextDataFormat.CommaSeparatedValue`
            if (Clipboard.ContainsText(TextDataFormat.UnicodeText))
            {
                tsv = Clipboard.GetText(TextDataFormat.UnicodeText);
            }
            else if (Clipboard.ContainsText(TextDataFormat.Text))
            {
                tsv = Clipboard.GetText(TextDataFormat.Text);
            }
            else
            {
                return;
            }

            // Convert TAB-delimited values into an HTML table:
            Debug.WriteLine(tsv);
            Debug.WriteLine("");

            var lines = tsv.Split('\n');
            if (lines.Length == 0) return;

            // Over-allocate some room to build our HTML output:
            var sb = new StringBuilder((tsv.Length * 7 / 3) + 72 + 48);
            Debug.WriteLine(sb.Capacity);

            // StartFragment and EndFragment comments are required for highest compatibilty with HTML pasting.
            sb.Append("<!DOCTYPE html><html><body><!--StartFragment--><table border='1' cellspacing='0' cellpadding='1'><tbody>");
            foreach (var line in lines)
            {
                sb.Append("<tr>");
                // Remove trailing '\r' and split by '\t':
                var cols = line.TrimEnd('\r').Split('\t');
                // Give each column its own <TD>:
                for (int c = 0; c < cols.Length; ++c)
                    sb.AppendFormat("<td>{0}</td>", System.Web.HttpUtility.HtmlEncode(cols[c]));
                sb.Append("</tr>");
            }
            sb.Append("</tbody></table><!--EndFragment--></body></html>");
            Debug.WriteLine(sb.Capacity);
            Debug.WriteLine(sb.Length);

            var htmlText = sb.ToString();
            Debug.WriteLine(htmlText);

            Clipboard.Clear();
            Clipboard.SetText(htmlText, TextDataFormat.Html);
        }
    }
}
