import java.util.ArrayList;

public class CraftableItem implements ItemInterface {
    private ItemDefinition definition;
    ArrayList<ItemInterface> component;
    /**
     * Creates an Item instance with a set definition.
     * The composition list is (created but) left empty. For composite items, the sub-components
     * should be retrieved/removed from some item source, and added with Item::Add(ItemInterface).
     * @param def
     */
    public CraftableItem(ItemDefinition def, ArrayList<ItemInterface> subItem) {
        definition = def;
        component = subItem;
    }

    @Override
    public double getWeight() {
        double weight = 0.0;
        for (ItemInterface item : component) {
            weight += item.getWeight();
        }
        return weight;
    }

    @Override
    public String getName() {
        return definition.getName();
    }

    @Override
    public String getDescription() {
        return definition.getDescription();
    }

    @Override
    public ItemDefinition getDefinition() {
        return definition;
    }

    @Override
    public String getCompositionDescription() {
        return "This item is made by " + definition.componentsString();
    }

    @Override
    public boolean equals(ItemInterface other) {
        return isOf(other.getDefinition());
    }

    @Override
    public boolean isOf(ItemDefinition def) {
        return getName().equals(def.getName());
    }

    @Override
    public String toString() {
        String output = String.format("Item: %s\nDescription: %s\nWeight: %.2f",
            getName(), getDescription(), getWeight());
        output += "\nHashCode: " + Integer.toHexString(this.hashCode());
        return output;
    }

}