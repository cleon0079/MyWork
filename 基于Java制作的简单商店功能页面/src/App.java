import java.util.ArrayList;

import javax.swing.JFrame;

public class App {
    private Player player;
    private Storage storage;
    private JFrame frame;
    private PageManager manager;

    public App(Player p, Storage s) {
        player = p;
        storage = s;
        player.setStorageView(storage.getInventory());
        s.addPlayer(p);

        manager = new PageManager(player, storage);

        // Setup of sorting
        setupSearching((InventoryPage) manager.findPage("Player Inventory"));
        setupSearching((InventoryPage) manager.findPage("Storage"));

        // Setup of craftng
        setupCrafting((ItemCraftPage) manager.findPage("Item Crafting"), player);
        setupUncrafting((ProductPage) manager.findPage("Product Page"), player);

        // Window creation
        manager.refresh();
        frame = new JFrame();
        frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
        frame.setContentPane(manager.getJPanel());
        frame.pack();
        frame.setLocationRelativeTo(null);
        frame.setVisible(true);
    }

    // Task 1: Defining what each button in the UI will do.
    void setupSearching(InventoryPage page) {
        page.addSearchByButton(new SearchByButton("All", () -> {
            player.getInventory().setSearch(new AllSearch());
            player.getStorageView().setSearch(new AllSearch());
        }));

        page.addSearchByButton(new SearchByButton("Name", () -> {
            player.getInventory().setSearch(new NameSearch());
            player.getStorageView().setSearch(new NameSearch());
        }));

        page.addSearchByButton(new SearchByButton("Description", () -> {
            player.getInventory().setSearch(new DescriptionSearch());
            player.getStorageView().setSearch(new DescriptionSearch());
        }));
    }

    void setupCrafting(ItemCraftPage page, Player player) {
        page.setCraftAction((craftableItem) -> {
            ArrayList<ItemInterface> components = craftableItem.getComponents();

            boolean canCraft = true;
            for (ItemInterface component : components) {
                ItemDefinition craftItem = component.getDefinition();
                if (!player.getInventory().indexOfItemByName(craftItem).isPresent()) {
                    canCraft = false;
                    break;
                }
            }

            if (canCraft) {
                for (ItemInterface component : components) {
                    ItemDefinition craftItem = component.getDefinition();
                    player.getInventory().removeOne(craftItem);
                }

                ItemInterface newItem = craftableItem.create();
                player.getInventory().addOne(newItem);
            } else {
                System.out.println("Not enough components to craft.");
            }
        });
    }

    void setupUncrafting(ProductPage page, Player player) {
        page.setUncraftAction((item) -> System.out.println("Uncrafting not implemented"));
    }
}
