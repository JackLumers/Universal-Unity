using FileHelpers;

namespace Common.Helpers.Parsing.UsingFileHelpers
{
    /// <summary>
    /// All parsed objects by <see cref="SwearingManager"/> must inherit from this class.
    /// </summary>
    [DelimitedRecord(",")]
    public class SwearingsParsedEntity
    {
        [FieldQuoted('"', QuoteMode.OptionalForBoth)]
        [FieldOptional]
        public string RussainSwearingValue;

        [FieldQuoted('"', QuoteMode.OptionalForBoth)]
        [FieldOptional]
        public string EnglishSwearingValue;

        //[FieldQuoted('"', QuoteMode.OptionalForBoth)]
        //[FieldOptional]
        //public string JapanSwearingValue;
    }
}
