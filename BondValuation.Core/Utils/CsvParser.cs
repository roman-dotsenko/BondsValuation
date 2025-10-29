using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;

namespace BondValuation.Core.Utils
{
    /// <summary>
    /// Generic CSV parser utility using CsvHelper with support for custom mappings.
    /// </summary>
    public static class CsvParser
    {
        /// <summary>
        /// Reads CSV data from a TextReader with custom mapping.
        /// </summary>
        /// <typeparam name="T">The type to map CSV records to.</typeparam>
        /// <typeparam name="TMap">The ClassMap type for custom CSV mapping.</typeparam>
        /// <param name="reader">TextReader containing CSV data.</param>
        /// <param name="delimiter">CSV delimiter (default: semicolon for European format).</param>
        /// <returns>List of mapped objects.</returns>
        public static List<T> Read<T, TMap>(TextReader reader, string delimiter = ";")
            where TMap : ClassMap<T>
        {
            CsvConfiguration config = new(CultureInfo.InvariantCulture)
            {
                Delimiter = delimiter,
                TrimOptions = TrimOptions.Trim,
                MissingFieldFound = null,
                BadDataFound = null
            };

            using CsvReader csv = new(reader, config);
            csv.Context.RegisterClassMap<TMap>();

            return [.. csv.GetRecords<T>()];
        }

        /// <summary>
        /// Reads CSV data from a TextReader without custom mapping.
        /// </summary>
        /// <typeparam name="T">The type to map CSV records to.</typeparam>
        /// <param name="reader">TextReader containing CSV data.</param>
        /// <param name="delimiter">CSV delimiter (default: semicolon for European format).</param>
        /// <returns>List of mapped objects.</returns>
        public static List<T> Read<T>(TextReader reader, string delimiter = ";")
        {
            CsvConfiguration config = new(CultureInfo.InvariantCulture)
            {
                Delimiter = delimiter,
                TrimOptions = TrimOptions.Trim,
                MissingFieldFound = null,
                BadDataFound = null
            };

            using CsvReader csv = new(reader, config);
            return [.. csv.GetRecords<T>()];
        }
    }
}
