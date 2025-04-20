using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Zoo.Infrastructure.Swagger
{
    public class EnumSchemaFilter : ISchemaFilter
    {
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (!context.Type.IsEnum)
            {
                return;
            }
            schema.Enum.Clear();
                
            var enumValues = Enum.GetNames(context.Type)
                .Select(name => new OpenApiString(name))
                .ToList();
                
            schema.Enum = enumValues.Cast<IOpenApiAny>().ToList();
            schema.Type = "string";
            schema.Format = null;
        }
    }
}