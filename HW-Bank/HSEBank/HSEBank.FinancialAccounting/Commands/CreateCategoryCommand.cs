using HSEBank.FinancialAccounting.Interfaces.Facades;
using HSEBank.FinancialAccounting.Models;
using HSEBank.FinancialAccounting.Models.Enums;

namespace HSEBank.FinancialAccounting.Commands;

public class CreateCategoryCommand : ICommand
{
    private readonly ICategoryFacade _categoryFacade;
    private readonly CategoryType _type;
    private readonly string _name;
    private Category? _createdCategory;
        
    public Category? CreatedCategory => _createdCategory;

    public CreateCategoryCommand(ICategoryFacade categoryFacade, CategoryType type, string name)
    {
        _categoryFacade = categoryFacade ?? throw new ArgumentNullException(nameof(categoryFacade));
        _type = type;
        _name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public void Execute()
    {
        _createdCategory = _categoryFacade.CreateCategory(_type, _name);
    }

    public void Undo()
    {
        if (_createdCategory == null)
        {
            return;
        }
            
        _categoryFacade.DeleteCategory(_createdCategory.Id);
        _createdCategory = null;
    }
}