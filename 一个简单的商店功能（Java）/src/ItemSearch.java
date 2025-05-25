import java.util.ArrayList;

public interface ItemSearch<T extends ItemInterface> {
    ArrayList<T> search(Inventory inventory, String searchTrem);
}