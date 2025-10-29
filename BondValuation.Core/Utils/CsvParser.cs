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
        /// Reads CSV data from a file and maps it to a list of objects.
        /// </summary>
        /// <typeparam name="T">The type to map CSV records to.</typeparam>
        /// <typeparam name="TMap">The ClassMap type for custom CSV mapping.</typeparam>
        /// <param name="path">Path to the CSV file.</param>
        /// <param name="delimiter">CSV delimiter (default: semicolon for European format).</param>
        /// <returns>List of mapped objects.</returns>
        public static List<T> Read<T, TMap>(string path, string delimiter = ";")
            where TMap : ClassMap<T>
        {
            using StreamReader reader = new(path);

            CsvConfiguration config = new(CultureInfo.InvariantCulture)
            {
                Delimiter = delimiter,
                TrimOptions = TrimOptions.Trim,
                MissingFieldFound = null, // Don't throw on missing fields
                BadDataFound = null // Handle bad data gracefully
            };

            using CsvReader csv = new(reader, config);
            csv.Context.RegisterClassMap<TMap>();

            return [.. csv.GetRecords<T>()];
        }

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
        /// Reads CSV data from a file without custom mapping.
        /// </summary>
        /// <typeparam name="T">The type to map CSV records to.</typeparam>
        /// <param name="path">Path to the CSV file.</param>
        /// <param name="delimiter">CSV delimiter (default: semicolon for European format).</param>
        /// <returns>List of mapped objects.</returns>
        public static List<T> Read<T>(string path, string delimiter = ";")
        {
            using StreamReader reader = new(path);

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

        /// <summary>
        /// Writes a collection of objects to a CSV file.
        /// </summary>
        /// <typeparam name="T">The type of objects to write.</typeparam>
        /// <param name="data">The data to write.</param>
        /// <param name="path">Path to the output CSV file.</param>
        /// <param name="delimiter">CSV delimiter (default: semicolon for European format).</param>
        public static void Write<T>(IEnumerable<T> data, string path, string delimiter = ";")
        {
            using StreamWriter writer = new(path);

            CsvConfiguration config = new(CultureInfo.InvariantCulture)
            {
                Delimiter = delimiter
            };

            using CsvWriter csv = new(writer, config);
            csv.WriteRecords(data);
        }

        /// <summary>
        /// Writes a collection of objects to a CSV file with custom mapping.
        /// </summary>
        /// <typeparam name="T">The type of objects to write.</typeparam>
        /// <typeparam name="TMap">The ClassMap type for custom CSV mapping.</typeparam>
        /// <param name="data">The data to write.</param>
        /// <param name="path">Path to the output CSV file.</param>
        /// <param name="delimiter">CSV delimiter (default: comma).</param>
        public static void Write<T, TMap>(IEnumerable<T> data, string path, string delimiter = ";")
            where TMap : ClassMap<T>
        {
            using StreamWriter writer = new(path);

            CsvConfiguration config = new(CultureInfo.InvariantCulture)
            {
                Delimiter = delimiter
            };

            using CsvWriter csv = new(writer, config);
            csv.Context.RegisterClassMap<TMap>();
            csv.WriteRecords(data);
        }
    }
}
