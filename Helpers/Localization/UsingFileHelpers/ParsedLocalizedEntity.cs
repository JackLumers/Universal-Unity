using Common.Helpers.Localization.Enums;
using Common.Helpers.Parsing.UsingFileHelpers;
using FileHelpers;

namespace Common.Helpers.Localization.UsingFileHelpers
{
    /// <summary>
    /// Used for localization CSV tables parsing.
    /// 
    /// For new localization you must change optional field name to localization name
    /// or add new filed named by this localization.
    /// 
    /// Also you need to add new localization type in <see cref="ETextLanguage"/>
    /// and a new logic in <see cref="LightweightLocalizedEntityFactory.LocalizedLightweightEntity"/> 
    /// </summary>
    [DelimitedRecord(",")]
    public class ParsedLocalizedEntity : AParsedEntity
    {
        [FieldQuoted('"', QuoteMode.OptionalForBoth)] [FieldOptional]
        public string RussianLocale;

        [FieldQuoted('"', QuoteMode.OptionalForBoth)] [FieldOptional]
        public string EnglishLocale;

        [FieldQuoted('"', QuoteMode.OptionalForBoth)] [FieldOptional]
        public string JapanLocale;

        [FieldQuoted('"', QuoteMode.OptionalForBoth)] [FieldOptional]
        public string ChineseLocale;

        [FieldQuoted('"', QuoteMode.OptionalForBoth)] [FieldOptional]
        public string ChineseTraditionalLocale;

        [FieldQuoted('"', QuoteMode.OptionalForBoth)] [FieldOptional]
        public string optional1;

        [FieldQuoted('"', QuoteMode.OptionalForBoth)] [FieldOptional]
        public string optional2;

        [FieldQuoted('"', QuoteMode.OptionalForBoth)] [FieldOptional]
        public string optional3;

        [FieldQuoted('"', QuoteMode.OptionalForBoth)] [FieldOptional]
        public string optional4;

        [FieldQuoted('"', QuoteMode.OptionalForBoth)] [FieldOptional]
        public string optional5;

        [FieldQuoted('"', QuoteMode.OptionalForBoth)] [FieldOptional]
        public string optional6;

        [FieldQuoted('"', QuoteMode.OptionalForBoth)] [FieldOptional]
        public string optional7;

        [FieldQuoted('"', QuoteMode.OptionalForBoth)] [FieldOptional]
        public string optional8;
    }
}