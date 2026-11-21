using System;
using System.Text;

namespace HandlebarsDotNet.Compiler
{
    /// <summary>
    /// Builds the part of a compile error message that shows the template source line
    /// containing the error, with a caret (<c>^</c>) under the column where it was detected.
    /// </summary>
    internal static class SourceHint
    {
        // Longer lines are cut to a window around the error column.
        private const int MaxWidth = 120;
        private const string Ellipsis = "...";

        /// <summary>
        /// Adds a source hint to <paramref name="exception"/> and to any <see cref="HandlebarsException"/>
        /// it wraps, using the template text read by <paramref name="source"/>.
        /// Exceptions that already have a hint (e.g. from a nested template) are left unchanged.
        /// </summary>
        public static void Attach(Exception exception, ExtendedStringReader source)
        {
            for (var current = exception; current != null; current = current.InnerException)
            {
                if (!(current is HandlebarsException handlebarsException)) continue;
                if (handlebarsException.HasSourceHint || handlebarsException.Line == null) continue;

                var line = source.GetLine(handlebarsException.Line.Value);
                if (line == null) continue;

                handlebarsException.SetSourceHint(Format(line, handlebarsException.Column ?? 0));
            }
        }

        internal static string Format(string line, int column)
        {
            column = Math.Max(0, Math.Min(column, line.Length));

            // The reader counts lines by Environment.NewLine; if the template uses a different
            // line ending, the "line" may span several physical lines. Show only the one with the error.
            var segmentStart = column == 0 ? 0 : line.LastIndexOf('\n', column - 1) + 1;
            var segmentEnd = line.IndexOf('\n', column);
            if (segmentEnd < 0) segmentEnd = line.Length;
            line = line.Substring(segmentStart, segmentEnd - segmentStart).TrimEnd('\r');
            column = Math.Min(column - segmentStart, line.Length);

            if (line.Length > MaxWidth)
            {
                var start = Math.Max(0, Math.Min(column - MaxWidth / 2, line.Length - MaxWidth));
                var prefix = start > 0 ? Ellipsis : string.Empty;
                var suffix = start + MaxWidth < line.Length ? Ellipsis : string.Empty;
                line = prefix + line.Substring(start, MaxWidth) + suffix;
                column = column - start + prefix.Length;
            }

            var hint = new StringBuilder(line.Length * 2 + 4);
            hint.Append("\n\n").Append(line).Append('\n');
            for (var index = 0; index < column; index++)
            {
                // keep tabs so the caret lines up with the source as it is displayed
                hint.Append(line[index] == '\t' ? '\t' : ' ');
            }

            return hint.Append('^').ToString();
        }
    }
}
