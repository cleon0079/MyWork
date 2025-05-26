# Task 1
Behavioural pattern - Options: *Strategy* or *Observer* pattern.
You chose: Strategy

## Itemise changes made (which class files were modified)
### 1. ItemSearch.java  
- Created the ItemSearch interface.
### 2. AllSearch.java
### 3. NameSearch.java
### 4. DescriptionSearch.java  
- Implemented three search strategies
### 5. Inventory.java  
- Updated the Inventory class setSearch method to enable dynamic switching of search strategies. Also, updated the searchItems method to utilize the selected search strategy.
### 6. App.java  
- Modified the App class to work with the search options setup for the GUI.

# Task 2
Structural pattern - *Composite* pattern.

## Itemise changes made (which class files were modified)
### 1. ItemDefinition.java  
- To check if it is a craftable item and store the item trat made the craftable item to fit the CraftableItem.java
### 2. CraftableItem.java  
- Give the weight of a craftable item and the CompositionDescription and save the component of the craftable item
### 3. App.java  
- Implemented the crafting functionalities within the App class.


# Task 3

## Implement the Observer Pattern for Player Updates 
### What was changed
- Modified the application to implement the Observer Pattern for Player Updates.  
1. Storage.java
2. StorageUpdate.java
3. Player.java
### Why it was changed
- In the future the inventory system might be put to work in a multi-player game. In this scenario, multiple players may have access to the same Storage box where the Observer (aka Pub-Sub) might prove useful. For this to work smoothly, each player should have their own copy of Storage to work with. Whenever an update happens to the storage (such as storing or retreiving a new item), all players be updated about the new stock of the Inventory. 
- In the provided starting code, the player is given a reference to the instance of storage in the constructor of App.java. Apply the observer pattern such that the player is a subscriber/observer of the storage.

### The benefits of the change
- Facilitates multiplayer functionality.
- Players now receive real-time updates about changes in the inventory.
- Code becomes more scalable for future multiplayer features.

## Use Generics for ItemSearch
### What was changed
- Updated the ItemSearch interface and related classes to use Generics.
1. ItemSearch.java
2. AllSearch.java
3. NameSearch.java
4. DescriptionSearch.java
5. Inventory.java
### Why Was It Changed?
- To enhance the flexibility and generality of the ItemSearch system. Using Generics allows for searching different types of items.
### Benefits of the Change:
- The code now supports searching for various types of items
- Promotes code reusability and adaptability.
- Makes the item search system more extensible for future item types.