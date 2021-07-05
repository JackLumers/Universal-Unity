namespace UniversalUnity.Helpers.Parsing.UsingFileHelpers
{
    public interface IParsedLightweightEntityFactory<in TParsed, out TLight>
        where TLight: AParsedLightweightEntity
        where TParsed: AParsedEntity
    {
        /// <param name="csvParsedEntity">Entity parsed by <see cref="FileHelpers"/></param>
        /// <returns></returns>
        TLight MakeLightweight(TParsed csvParsedEntity);
    }
}