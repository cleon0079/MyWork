import java.util.ArrayList;

public class AllSearch implements ItemSearch<ItemInterface>{
    
    @Override
    public ArrayList<ItemInterface> search(Inventory inventory, String searchTerm) {
        String term = searchTerm.toLowerCase();
        ArrayList<ItemInterface> result = new ArrayList<>(inventory.getStock());

        for (int i = 0; i < result.size(); i++) {
            ItemInterface curItem = result.get(i);
            if (!curItem.getName().contains(term) && !curItem.getDescription().contains(term)) {
                result.remove(i);
                i--;
            }
        }

        return result;
    }
}
