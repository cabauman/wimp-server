using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace WIMP_Server.Searching
{
    public static class SearchMessageUtilities
    {
        public static bool TryReplaceFirst(this string text, string search, string replace, out string result)
        {
            int pos = text.IndexOf(search);
            if (pos < 0)
            {
                result = text;
                return false;
            }

            result = text[..pos] + replace + text[(pos + search.Length)..];

            return true;
        }

        public static IEnumerable<string> GenerateSearchCandidates(string intel)
        {
            intel = intel.Replace("*", string.Empty)
                .Replace("/", string.Empty)
                .Replace("?", string.Empty)
                .Replace(",", string.Empty);

            var parts = intel
                .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                .Distinct()
                .ToArray();

            var candidates = new List<string>();
            for (var p1 = 0; p1 < parts.Length; ++p1)
            {
                candidates.Add(parts[p1]);

                var sb = new StringBuilder(parts[p1]);
                for (var p2 = p1 + 1; p2 < Math.Min(parts.Length, p1 + 4); ++p2)
                {
                    sb.Append(' ').Append(parts[p2]);
                    candidates.Add(sb.ToString());
                }
            }

            return candidates.Where(c => c.Length > 2);
        }

        public static IEnumerable<string> ExtractNamesFromIntelStringDescendingLength(IEnumerable<string> possibleNames, ref string intel, bool caseSensitive = false, IEnumerable<string> namesFound = null)
        {
            if (intel == null)
            {
                throw new ArgumentNullException(nameof(intel));
            }

            if (possibleNames == null)
            {
                throw new ArgumentNullException(nameof(possibleNames));
            }

            namesFound ??= new List<string>();

            var candidates = GenerateSearchCandidates(intel);

            var bestNameMatch = candidates
                .OrderByDescending(s => s.Length)
                .FirstOrDefault(n => possibleNames
                    .Any(pn => string
                        .Equals(n, pn, caseSensitive ? StringComparison.InvariantCulture : StringComparison.InvariantCultureIgnoreCase)));

            if (bestNameMatch != null)
            {
                if (intel.TryReplaceFirst(bestNameMatch, string.Empty, out intel))
                {
                    return ExtractNamesFromIntelStringDescendingLength(possibleNames, ref intel, caseSensitive, namesFound?.Append(bestNameMatch));
                }
                else
                {
                    // TODO: Can we disregard the disjoint match and continue searching for other matches?
                    return namesFound;
                }
            }
            else
            {
                return namesFound;
            }
        }
    }
}