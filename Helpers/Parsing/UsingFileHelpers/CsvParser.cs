using System;
using System.Collections.Generic;
using System.Text;
using FileHelpers;
using JetBrains.Annotations;
using UnityEngine;
using UniversalUnity.Helpers.Logs;

namespace UniversalUnity.Helpers.Parsing.UsingFileHelpers
{
    public static class CsvParser
    {
        public static Dictionary<string, TDataEntity> Parse<TDataEntity>
            (TextAsset csvTextAsset, bool skipHeader = true)
            where TDataEntity : AParsedEntity
        {
            Dictionary<string, TDataEntity> result;

            try
            {
                result = PrivateParse<TDataEntity>(csvTextAsset.text, skipHeader);
            }
            catch (Exception e)
            {
                Debug.LogError("Parse failed!\n" + $"File: {csvTextAsset.name}\n" + $"Error: {e}");
                throw;
            }

            return result;
        }

        private static Dictionary<string, TDataEntity> PrivateParse<TDataEntity>
            ([NotNull] string stringToRead, bool skipHeader)
            where TDataEntity : AParsedEntity
        {
            if (stringToRead == null) throw new ArgumentNullException(nameof(stringToRead));

            var engine = new FileHelperEngine<TDataEntity>(Encoding.ASCII);
            var parsedData = new Dictionary<string, TDataEntity>();

            if (skipHeader)
            {
                engine.Options.IgnoreFirstLines = 1;
            }

            try
            {
                foreach (var entity in engine.ReadString(stringToRead))
                {
                    LogHelper.LogInfo($"Adding parsed entity: {entity.Id}", nameof(PrivateParse));
                    parsedData.Add(entity.Id, entity);
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Parse failed!\n Error: {e}");
                throw;
            }

            return parsedData;
        }

        public static Dictionary<string, TLightweightEntity> ParseLightweight<TDataEntity, TLightweightEntity>
        (TextAsset csvTextAsset, IParsedLightweightEntityFactory<TDataEntity, TLightweightEntity> entityFactory,
            bool skipHeader = true)
            where TDataEntity : AParsedEntity
            where TLightweightEntity : AParsedLightweightEntity
        {
            Dictionary<string, TLightweightEntity> result;

            try
            {
                result = PrivateParseLightweight(csvTextAsset.text, entityFactory, skipHeader);
            }
            catch (Exception)
            {
                Debug.LogError("Parse failed!\n" + $"File: {csvTextAsset.name}");
                throw;
            }

            return result;
        }

        private static Dictionary<string, TLightweightEntity> PrivateParseLightweight<TDataEntity, TLightweightEntity>
        ([NotNull] string stringToRead,
            [NotNull] IParsedLightweightEntityFactory<TDataEntity, TLightweightEntity> entityFactory,
            bool skipHeader)
            where TDataEntity : AParsedEntity
            where TLightweightEntity : AParsedLightweightEntity
        {
            if (stringToRead == null) throw new ArgumentNullException(nameof(stringToRead));
            if (entityFactory == null) throw new ArgumentNullException(nameof(entityFactory));

            var engine = new FileHelperEngine<TDataEntity>(Encoding.ASCII);
            var parsedData = new Dictionary<string, TLightweightEntity>();

            if (skipHeader)
            {
                engine.Options.IgnoreFirstLines = 1;
            }

            try
            {
                foreach (var entity in engine.ReadString(stringToRead))
                {
                    LogHelper.LogInfo(
                        $"Adding parsed lightweight entity: {entityFactory.MakeLightweight(entity)}", 
                        nameof(PrivateParseLightweight));
                    parsedData.Add(entity.Id, entityFactory.MakeLightweight(entity));
                }
            }
            catch (Exception)
            {
                Debug.LogError("Parse failed!\n");
                throw;
            }

            return parsedData;
        }
    }
}