import java.util.Optional;
import java.util.ArrayList;

public class ItemDefinition {
    private String name;
    private String description;
    private String[] componentNames;
    private boolean isBaseItem;
    private Optional<Double> weight;
    private ArrayList<ItemInterface> componentItems = new ArrayList<ItemInterface>();
    ItemDictionary dict;

    public ItemDefinition(String n, String desc, Optional<Double> weightIfBase, String[] components) {
        name = n;
        description = desc;
        componentNames = components;
        isBaseItem = weightIfBase.isPresent();
        weight = weightIfBase;

        // This may be helpful for the compsite pattern to find the appropriate item definitions
        dict = ItemDictionary.get();
        for (String componentName : componentNames) {
            Optional<ItemInterface> subItem = createSubItem(componentName);
            if (subItem.isPresent()) {
                componentItems.add(subItem.get());
            }
        }
    }



    /**
     * Create an instance of the item described by this ItemDefinition.
     * If the Item is made up of other items, then each sub-item should also be created.
     * @return An Item instance described by the ItemDefinition
     */
    public ItemInterface create() {
        if (componentNames.length > 0) {
            return new CraftableItem(this, componentItems);
        } else {
            return new Item(this);
        }
    }

    private Optional<ItemInterface> createSubItem(String componentName) {
        Optional<ItemDefinition> componentDefinition = dict.defByName(componentName);
        if (componentDefinition.isPresent()) {
            return Optional.of(componentDefinition.get().create());
        } else {
            System.err.println("Component '" + componentName + "' not found in the item dictionary.");
            return Optional.empty();
        }
    }

    // ItemDefinition might "craft" and return an item, using items from some source inventory.
    // You might use the Milestone 1 Basket transaction code as a guide

    public String getName() {
        return name;
    }

    public String getDescription() {
        return description;
    }

    public ArrayList<ItemInterface> getComponents() {
        return componentItems;
    }

    /**
     * Format: {ITEM 1}, {ITEM 2}, ...
     * @return a String of sub-item/component names in the above format
     */
    public String componentsString() {
        String output = "";
        for (String componentName : componentNames) {
            output += componentName + ", ";
        }
        return output;
    }

    public boolean isBaseItemDef() {
        return isBaseItem;
    }

    public Optional<Double> getWeight() {
        return weight;
    }

    public boolean equals(Item other) {
        return isOf(other.getDefinition());
    }

	public boolean isOf(ItemDefinition def) {
		return getName().equals(def.getName());
	}

}
