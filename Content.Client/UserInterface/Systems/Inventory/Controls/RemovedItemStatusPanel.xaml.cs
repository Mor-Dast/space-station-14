using Content.Client.Items;
using Content.Client.Resources;
using Content.Shared.Hands.Components;
using Content.Shared.IdentityManagement;
using Content.Shared.Inventory.VirtualItem;
using Robust.Client.AutoGenerated;
using Robust.Client.Graphics;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;
using Robust.Shared.Timing;
using Robust.Shared.Utility;
using static Content.Client.IoC.StaticIoC;

namespace Content.Client.UserInterface.Systems.Inventory.Controls;

//RADIUM: REVERT HAND STATE SYSTEM

[GenerateTypedNameReferences]
public sealed partial class ItemStatusPanel : BoxContainer
{
    [Dependency] private readonly IEntityManager _entityManager = default!;

    [ViewVariables] private EntityUid? _entity;

    public ItemStatusPanel()
    {
        RobustXamlLoader.Load(this);
        IoCManager.InjectDependencies(this);

        SetSide(HandLocation.Middle);
    }

    public void SetSide(HandLocation location)
    {
        string texture;
        StyleBox.Margin cutOut;
        StyleBox.Margin flat;
        Label.AlignMode textAlign;

        switch (location)
        {
            case HandLocation.Left:
                texture = "/Textures/Radium/Interface/Nano/item_status_right.svg.96dpi.png";
                cutOut = StyleBox.Margin.Left | StyleBox.Margin.Top;
                flat = StyleBox.Margin.Right | StyleBox.Margin.Bottom;
                textAlign = Label.AlignMode.Right;
                break;
            case HandLocation.Middle:
                texture = "/Textures/Radium/Interface/Nano/item_status_middle.svg.96dpi.png";
                cutOut = StyleBox.Margin.Right | StyleBox.Margin.Top;
                flat = StyleBox.Margin.Left | StyleBox.Margin.Bottom;
                textAlign = Label.AlignMode.Left;
                break;
            case HandLocation.Right:
                texture = "/Textures/Radium/Interface/Nano/item_status_left.svg.96dpi.png";
                cutOut = StyleBox.Margin.Right | StyleBox.Margin.Top;
                flat = StyleBox.Margin.Left | StyleBox.Margin.Bottom;
                textAlign = Label.AlignMode.Left;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(location), location, null);
        }

        var panel = (StyleBoxTexture) Panel.PanelOverride!;
        panel.Texture = ResC.GetTexture(texture);
        panel.SetPatchMargin(flat, 2);
        panel.SetPatchMargin(cutOut, 13);

        ItemNameLabel.Align = textAlign;
    }

    protected override void FrameUpdate(FrameEventArgs args)
    {
        base.FrameUpdate(args);
        UpdateItemName();
    }

    public void Update(EntityUid? entity)
    {
        if (entity == null)
        {
            ClearOldStatus();
            _entity = null;
            Panel.Visible = false;
            return;
        }

        if (entity != _entity)
        {
            _entity = entity.Value;
            BuildNewEntityStatus();

            UpdateItemName();
        }

        Panel.Visible = true;
    }

    private void UpdateItemName()
    {
        if (_entity == null)
            return;

        if (!_entityManager.TryGetComponent<MetaDataComponent>(_entity, out var meta) || meta.Deleted)
        {
            Update(null);
            return;
        }

        if (_entityManager.TryGetComponent(_entity, out VirtualItemComponent? virtualItem)
            && _entityManager.EntityExists(virtualItem.BlockingEntity))
        {
            // Uses identity because we can be blocked by pulling someone
            ItemNameLabel.Text = Identity.Name(virtualItem.BlockingEntity, _entityManager);
        }
        else
        {
            ItemNameLabel.Text = Identity.Name(_entity.Value, _entityManager);
        }
    }

    private void ClearOldStatus()
    {
        StatusContents.RemoveAllChildren();
    }

    private void BuildNewEntityStatus()
    {
        DebugTools.AssertNotNull(_entity);

        ClearOldStatus();

        var collectMsg = new ItemStatusCollectMessage();
        _entityManager.EventBus.RaiseLocalEvent(_entity!.Value, collectMsg, true);

        foreach (var control in collectMsg.Controls)
        {
            StatusContents.AddChild(control);
        }
    }
}
