using FileHelpers;

namespace UniversalUnity.Helpers.Parsing.UsingFileHelpers
{
    /// <summary>
    /// All parsed objects by <see cref="CsvParser"/> must inherit from this class.
    /// </summary>
    [DelimitedRecord(",")]
    public abstract class AParsedEntity
    {
        /// <summary>
        /// First column in CSV file must be a unique Id for entity.
        /// </summary>
        [FieldQuoted('"', QuoteMode.OptionalForBoth)]
        public string Id;
    }
}