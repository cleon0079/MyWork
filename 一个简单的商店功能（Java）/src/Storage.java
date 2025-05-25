import java.util.ArrayList;

public class Storage {
    private ArrayList<StorageUpdate> players = new ArrayList<>();
    private String storageName;
    private Inventory items;

    public Storage(String name, Inventory startingInventory) {
        storageName = name;
        items = startingInventory;
    }

    public void addPlayer(StorageUpdate player) {
        players.add(player);
    }

    public Inventory getInventory() {
        return items;
    }

    public String getName() {
        return storageName;
    }
    
    public void store(ItemInterface item) {
        items.addOne(item);
        noticePlayer(items);
    }

    public ItemInterface retrieve(ItemInterface item) throws ItemNotAvailableException {
        ItemInterface removed = items.remove(item);
        noticePlayer(items);
        return removed;
    }
    
    private void noticePlayer(Inventory updatedInventory) {
        for (StorageUpdate player : players) {
            player.updateStorage(updatedInventory);
        }
    }
}
