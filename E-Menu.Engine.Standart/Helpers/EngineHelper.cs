using Microsoft.Crm.Sdk.Messages;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Messages;
using System;
using System.Collections.Generic;
using System.Linq;

namespace E_Menu.Engine.Helpers
{
    public static class EngineHelper
    {

        public static Entity GetAliasedEntity(Entity extractRecord, string aliasName, string logicalName, string primaryFieldName = "")
        {
            if (extractRecord == null)
                throw new ArgumentNullException(nameof(extractRecord), "Entity cannot be null");

            if (string.IsNullOrWhiteSpace(aliasName))
                throw new ArgumentException("Alias name cannot be null or empty", nameof(aliasName));

            if (string.IsNullOrWhiteSpace(logicalName))
                throw new ArgumentException("Logical name cannot be null or empty", nameof(logicalName));

            var entity = new Entity(logicalName);
            var formattedAliasName = aliasName.EndsWith(".", StringComparison.Ordinal) ? aliasName : $"{aliasName}.";
            var attributes = extractRecord.Attributes;

            foreach (var attribute in attributes)
            {
                if (attribute.Key.StartsWith(formattedAliasName, StringComparison.OrdinalIgnoreCase))
                {
                    var attributeKey = attribute.Key.Substring(formattedAliasName.Length);

                    if (attribute.Value is AliasedValue aliasedValue)
                    {
                        entity[attributeKey] = aliasedValue.Value;

                        if (!string.IsNullOrWhiteSpace(primaryFieldName) &&
                            attributeKey.Equals(primaryFieldName, StringComparison.OrdinalIgnoreCase) &&
                            aliasedValue.Value is Guid idValue)
                        {

                            entity.Id = idValue;
                        }
                    }

                    if (extractRecord.FormattedValues.Contains(attribute.Key))
                    {
                        entity.FormattedValues[attributeKey] = extractRecord.FormattedValues[attribute.Key];
                    }

                }
            }

            return entity;
        }
    }
}
