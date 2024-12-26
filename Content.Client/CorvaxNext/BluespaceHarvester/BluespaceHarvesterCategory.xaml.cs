using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface;
using Robust.Client.UserInterface.XAML;
using Content.Shared.CorvaxNext.BluespaceHarvester;

namespace Content.Client.CorvaxNext.BluespaceHarvester;

[GenerateTypedNameReferences]
public sealed partial class BluespaceHarvesterCategory : Control
{
    public BluespaceHarvesterCategory(BluespaceHarvesterCategoryInfo category, bool canBuy)
    {
        RobustXamlLoader.Load(this);

        CategoryLabel.Text = Loc.GetString($"bluespace-harvester-category-{Enum.GetName(typeof(Shared.CorvaxNext.BluespaceHarvester.BluespaceHarvesterCategory), category.Type)}");

        CategoryButton.Text = $"{category.Cost}";
        CategoryButton.Disabled = !canBuy;
    }
}
