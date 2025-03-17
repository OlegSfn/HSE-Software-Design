using System.Text.Json;
using HSEBank.FinancialAccounting.Interfaces;
using HSEBank.FinancialAccounting.Interfaces.Facades;

namespace HSEBank.FinancialAccounting.TemplateMethod;

public class JsonDataImporter : DataImporter
{
    public JsonDataImporter(IBankAccountFacade bankAccountFacade, ICategoryFacade categoryFacade, IOperationFacade operationFacade)
        : base(bankAccountFacade, categoryFacade, operationFacade)
    {
    }

    protected override ImportedData ReadDataFromFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException("File not found", filePath);
        }

        var json = File.ReadAllText(filePath);
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };

        var data = JsonSerializer.Deserialize<ImportedData>(json, options);
        if (data == null)
        {
            throw new InvalidOperationException("Failed to deserialize JSON data");
        }

        return data;
    }
}