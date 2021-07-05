namespace Common.Helpers.Parsing.UsingFileHelpers
{
    /// <summary>
    /// For less memory allocation you can use <see cref="IParsedLightweightEntityFactory{TParsed,TLight}"/>
    /// to make a lightweight version for <see cref="AParsedEntity"/>
    /// </summary>
    public abstract class AParsedLightweightEntity
    {
        public string Id { get; }

        protected AParsedLightweightEntity(AParsedEntity parsedEntity)
        {
            Id = parsedEntity.Id;
        }

        public override string ToString()
        {
            return $"Id: {Id}";
        }
    }
}