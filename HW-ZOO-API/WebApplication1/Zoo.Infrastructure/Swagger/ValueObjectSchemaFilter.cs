using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Zoo.Infrastructure.Swagger
{
    public class ValueObjectSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context.Type == typeof(string) && 
                context.MemberInfo?.DeclaringType?.Namespace?.Contains("WebApplication1.Presentation.DTOs") == true)
            {
                string propertyName = context.MemberInfo.Name.ToLower();
                
                if (propertyName.Contains("gender"))
                {
                    AddEnumExample(schema, new[] { "Male", "Female" });
                }
                else if (propertyName.Contains("healthstatus") || propertyName.Contains("health"))
                {
                    AddEnumExample(schema, new[] { "Healthy", "Sick" });
                }
                else if (propertyName.Contains("enclosuretype") || propertyName.EndsWith("type") && propertyName.Contains("enclosure"))
                {
                    AddEnumExample(schema, new[] { "Predator", "Herbivore", "Bird", "Reptile", "Aviary", "Aquarium", "Terrarium" });
                }
                else if (propertyName.Contains("foodtype") || propertyName.Contains("food"))
                {
                    AddEnumExample(schema, new[] { "Meat", "Fish", "Vegetables", "Fruits", "Insects", "Seeds", "Hay" });
                }
                else if (propertyName.Contains("animaltype") || (propertyName.EndsWith("type") && !propertyName.Contains("enclosure") && !propertyName.Contains("food")))
                {
                    AddEnumExample(schema, new[] { "Predator", "Herbivore", "Bird", "Aquatic", "Amphibian", "Reptile" });
                }
            }
            
            HandleSpecificValueObject(schema, context);
        }

        private void HandleSpecificValueObject(OpenApiSchema schema, SchemaFilterContext context)
        {
            Type? propertyType = context.Type;
            
            if (propertyType == typeof(string))
            {
                if (context.MemberInfo != null)
                {
                    string propertyName = context.MemberInfo.Name.ToLower();
                    
                    if (propertyName == "gender")
                    {
                        schema.Example = new OpenApiString("Male");
                        schema.Description = "Gender of the animal. Possible values: Male, Female";
                    }
                    else if (propertyName == "healthstatus")
                    {
                        schema.Example = new OpenApiString("Healthy");
                        schema.Description = "Health status of the animal. Possible values: Healthy, Sick";
                    }
                    else if (propertyName == "type" && context.MemberInfo.DeclaringType?.Name.Contains("Enclosure") == true)
                    {
                        schema.Example = new OpenApiString("Predator");
                        schema.Description = "Type of enclosure. Possible values: Predator, Herbivore, Bird, Reptile, Aviary, Aquarium, Terrarium";
                    }
                    else if (propertyName == "foodtype" || propertyName == "favoritefood")
                    {
                        schema.Example = new OpenApiString("Meat");
                        schema.Description = "Type of food. Possible values: Meat, Fish, Vegetables, Fruits, Insects, Seeds, Hay";
                    }
                    else if (propertyName == "animaltype" || propertyName.Contains("animaltype"))
                    {
                        schema.Example = new OpenApiString("Predator");
                        schema.Description = "Type of animal. Possible values: Predator, Herbivore, Bird, Aquatic, Amphibian, Reptile";
                    }
                }
            }
        }

        private void AddEnumExample(OpenApiSchema schema, string[] possibleValues)
        {
            // Set the example to the first value
            schema.Example = new OpenApiString(possibleValues[0]);
            
            // Add enum values for dropdown in Swagger UI
            schema.Enum = possibleValues.Select(v => (IOpenApiAny)new OpenApiString(v)).ToList();
            
            // Ensure type is string 
            schema.Type = "string";
            
            // Add description with all possible values
            if (string.IsNullOrEmpty(schema.Description))
            {
                schema.Description = $"Possible values: {string.Join(", ", possibleValues)}";
            }
            else if (!schema.Description.Contains("Possible values"))
            {
                schema.Description += $". Possible values: {string.Join(", ", possibleValues)}";
            }
        }
    }
}