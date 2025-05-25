using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Inventories : MonoBehaviour
{
    [Header("Inventory")]
    [SerializeField] List<Item> Inventory = new List<Item>();

    [Header("Inventory UI Element")]
    [SerializeField] Image prize;
    [SerializeField] GameObject inventoryGameObject;
    [SerializeField] GameObject inventoryContent;
    [SerializeField] Button exitButton;
    [SerializeField] private  Transform targetPosition;
    private string dragItemLayer = "DragItem";
    
    public float rayDistance = 10f;
    public LayerMask collectableLayer;
    private string showedText = "Press F to collect";
    private DegreePuzzle degreePuzzle;

    GameInput input;
    InputAction inventoryAction;
    InputAction interartAction;
    InputAction escAction;
    Manager uIManager;
    UIManager uIManager2;
    private bool canPick = false;
    private void Awake()
    {
        input = new GameInput();
        inventoryAction = input.Player.Inventory;
        interartAction = input.Player.Interart;
        escAction = input.Player.Esc;

        inventoryAction.started += OnInventoryOpen;

        interartAction.started += PressToPick;
        interartAction.canceled += Release;

        escAction.started += OnCloseInventory;
        exitButton.onClick.AddListener(CloseInventory);
        
    }

    private void Start()
    {
        uIManager = FindObjectOfType<Manager>();
        uIManager2 = FindObjectOfType<UIManager>();
        degreePuzzle = FindObjectOfType<DegreePuzzle>();
    }

    public void BagEnable()
    {
        /*1 ��Ϸ��ʼ��ʱ�� 
          2 �ر���ͣ��ʱ��
          3 ����puzzle ֮��
         */
        inventoryAction.Enable();
    }

    public void BagDisable()
    {
        /* 
        1 ��puzzle ֮�� 
        2 ��Ϸ��ͣ (x)
        3 ��intro ���� level 2 (x)
        4 �ڻ�����ӽ�Ʒ�������ŵ�ʱ��
        5 �ڼ������puzzle��ʱ��  (x)
        6 �ڻ����ܽ�Ʒ�������ŵ�ʱ��
        7 level 2 ����level 3   (x)
        8 ��level 3 ����puzzle��ʱ��  
        9 ��ƴdegree ��Ʒ�������ŵ�ʱ��
        10 �ڴ��ͽ���������ʱ��  (x)


        �������� => �������� = 
 */
        inventoryAction.Disable();
    }

    void OnCloseInventory(InputAction.CallbackContext callbackContext) {
        uIManager2.EnableEscKey(true);
        ActiveMode(false);
        
    }
    
    void Update(){
        checkItem();
    }

    void OnInventoryOpen(InputAction.CallbackContext callbackContext) {
        
        if (inventoryGameObject.activeSelf)
        {
            
            ActiveMode(false);
        }
        else
        {
            ActiveMode(true);
        }
    }
    void PressToPick(InputAction.CallbackContext callbackContext)
    {
        canPick = true;
    }
    void Release(InputAction.CallbackContext callbackContext)
    {
        canPick = false;
    }
    public void checkItem()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;
        Debug.DrawRay(ray.origin, ray.direction * rayDistance, Color.red);
            if (Physics.Raycast(ray, out hit, rayDistance, collectableLayer))
            { 
                 
                
                if (hit.transform.CompareTag("BrokenDegree"))
                {
                    
                    if(degreePuzzle.canInterat){
                        uIManager.UpdateText(showedText);
                        interartAction.Enable();
                        ItemObject itemObject = hit.transform.GetComponent<ItemObject>();
                        if (itemObject != null)
                        {   
                            if(canPick)
                            {
                                AddItem(itemObject.item);
                                itemObject.PickUp(); 
                                return;
                            } 
                        }
                    }
                    else
                    {
                        interartAction.Disable();
                    }
                    
                }else if (hit.transform.CompareTag("PickableItem"))
                {
                    
                    if(hit.collider != null){
                        uIManager.UpdateText(showedText);
                        interartAction.Enable();
                        ItemObject itemObject = hit.transform.GetComponent<ItemObject>();
                        if (itemObject != null)
                        {   
                            if(canPick)
                            {
                                AddItem(itemObject.item);
                                itemObject.PickUp(); 
                                uIManager.UpdateText("");
                                
                                return;
                            } 
                        }
                    }
                    else
                    {
                        interartAction.Disable();
                    }
                    
                }
            }else{
                if(degreePuzzle.isPlayerExit)
                {
                    uIManager.UpdateText("");
                }
            }
    }
    public void AddItem(Item _item)
    {
        if (Inventory.Count >= 10)
        {
            return;
        }
        else
        {
            Inventory.Add(_item);
            //DisplayItemsCanvas();
        }
    }

    public void RemoveItem(Item _item)
    {
        if (Inventory.Contains(_item))
        {
            Inventory.Remove(_item);
        }
    }

    void DestroyAllChildren(Transform _parent)
    {
        foreach (Transform child in _parent)
        {
            Destroy(child.gameObject);
        }
    }
    private IEnumerator EnableColliderAndGravityWithDelay(Collider collider, Rigidbody rb, float delay)
    {
        yield return new WaitForSeconds(delay);
        
        if (collider != null)
        {
            collider.enabled = true;  
        }

        if (rb != null)
        {
            rb.isKinematic = false; 
        }
    }
    void RespawnPuzzles(ItemObject itemObject)
{
    if (itemObject != null)
    {
        // 将物体位置重置到固定位置
        itemObject.transform.position = targetPosition.position;

        // 启用渲染器并禁用碰撞器
        itemObject.GetComponent<Renderer>().enabled = true;
        Collider collider = itemObject.GetComponent<Collider>();
        Rigidbody rb = itemObject.GetComponent<Rigidbody>();
        
        if (collider != null)
        {
            collider.enabled = false;  // 先禁用碰撞器
        }

        if (rb != null)
        {
            rb.isKinematic = true;  // 设置为 kinematic，这样物体不会受重力影响
        }
        
        // 启动协程，在延迟后启用碰撞器和重力
        StartCoroutine(EnableColliderAndGravityWithDelay(collider, rb, 0.5f));

        itemObject.gameObject.AddComponent<Outline>();
        itemObject.gameObject.AddComponent<DegreeItem>();
        itemObject.GetComponentInParent<DegreePuzzle>().enabled = false;
        itemObject.gameObject.layer = LayerMask.NameToLayer(dragItemLayer);

        RemoveItem(itemObject.item);
        DisplayItemsCanvas();
    }
}
    ItemObject FindItemObject(Item item)
    {
        ItemObject[] allItems = FindObjectsOfType<ItemObject>();
        foreach (var obj in allItems)
        {
            if (obj.item == item)
            {
                return obj;
            }
        }
        return null;
    }
    void DisplayItemsCanvas()
    {
        DestroyAllChildren(inventoryContent.transform);
        for (int i = 0; i < Inventory.Count; i++)
        {
            Image imageGO = Instantiate<Image>(prize, inventoryContent.transform);
            Item item = Inventory[i];
            imageGO.sprite = item.Image;

            if(item.Type == Item.Index.BookPuzzlePrize)
            {
                uIManager.bookShelfFinish = true;
            }
            
            if(item.Type == Item.Index.TablePuzzlePrize){
                uIManager.tablesFinish = true;
            }
            Button itemButton = imageGO.gameObject.AddComponent<Button>();
            ColorBlock colors = itemButton.colors;
            colors.normalColor = Color.white; 
            colors.highlightedColor = Color.gray;
            colors.pressedColor = Color.gray;
            itemButton.colors = colors;

            //itemButton.onClick.AddListener(() => RespawnPuzzles());

            ItemObject itemObject = FindItemObject(item);
            if (itemObject != null)
            {
                AddItemButton(itemButton, itemObject);
            }
            
            
        }
    }
    void AddItemButton(Button itemButton, ItemObject itemObject)
    {

        itemButton.onClick.AddListener(() => RespawnPuzzles(itemObject));
    }
    public void CanInventory(bool canOpen) {
        if (canOpen)
        {
            inventoryAction.Enable();
        }
        else
        {
            inventoryAction.Disable();
        }
    }

    public void CloseInventory() {
        uIManager2.EnableEscKey(true);
        ActiveMode(false);
    }

    public void ActiveMode(bool active) {
        inventoryGameObject.SetActive(active);
        if (active)
        {
            escAction.Enable();
            DisplayItemsCanvas();
        }
        else
        {
            escAction.Disable();
        }
        uIManager.Inventory(!active);
        uIManager2.EnableEscKey(!active);
        uIManager.CursorMode(active);
    }
}