using UnityEngine;
using System.Collections.Generic;
using Playroom;
public class OrderManager : MonoBehaviour
{
    public static OrderManager Instance { get; private set; }
    private PlayroomKit _playroom;
    
    [Header("Order Management")]
    public static Dictionary<string, HeldItemType> OrderHistory = new Dictionary<string, HeldItemType>();
    public static Dictionary<string, HeldItemType> ActiveOrders = new Dictionary<string, HeldItemType>();
    
    [Header("Order Bubble")]
    [SerializeField] private GameObject orderBubblePrefab;
    
    [Header("Coffee Types")]
    [SerializeField] private HeldItemType[] availableCoffeeTypes = new HeldItemType[] 
    { 
        HeldItemType.CoffeeBlendBrown, 
        HeldItemType.CoffeeBlendWhite, 
        HeldItemType.MixedCoffeeBlend 
    };
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        _playroom = PlayroomManager.Instance.GetPlayroomKit();
        if (_playroom == null)
        {
            Debug.LogError("PlayroomKit instance not found!");
            return;
        }
        
        // Initialize order management
        ActiveOrders.Clear();
        OrderHistory.Clear();
    }
    
    /// <param name="customerID">Unique identifier for the customer</param>
    /// <returns>The generated order type</returns>
    public void CreateOrder(string customerID)
    {
        if (string.IsNullOrEmpty(customerID))
        {
            Debug.LogError("Customer ID cannot be null or empty");
            return;
        }

        if (_playroom.IsHost())
        {
            // Generate random coffee order
            HeldItemType orderType = availableCoffeeTypes[Random.Range(0, availableCoffeeTypes.Length)];
            
            // Notify all clients about the new order via RPC
            string payload = $"{customerID}|?|{orderType}";
            _playroom.RpcCall("HandleNewOrder", payload, PlayroomKit.RpcMode.ALL);
            
            Debug.Log($"Host created order for customer {customerID}: {orderType}");
        }
        else
        {
            // Non-host clients should not create orders directly
            Debug.LogWarning("Non-host clients cannot create orders directly");
        }
    }
    
    public bool ReceiveOrder(string customerID, HeldItemType receivedOrder)
    {
        if (string.IsNullOrEmpty(customerID))
        {
            Debug.LogWarning("Customer ID cannot be null or empty");
            return false;
        }
        
        if (receivedOrder == HeldItemType.None)
        {
            Debug.LogWarning($"Customer {customerID}: Received an empty order");
            return false;
        }
        
        if (!ActiveOrders.ContainsKey(customerID))
        {
            Debug.LogWarning($"Customer {customerID}: No active order found");
            return false;
        }
        
        HeldItemType expectedOrder = ActiveOrders[customerID];
        
        if (receivedOrder == expectedOrder)
        {
            Debug.Log($"Customer {customerID}: Order served correctly! {receivedOrder}");
            
            // Move to order history
            OrderHistory[customerID] = receivedOrder;
            ActiveOrders.Remove(customerID);
            
            return true;
        }
        else
        {
            Debug.LogWarning($"Customer {customerID}: Wrong order received. Expected: {expectedOrder}, Got: {receivedOrder}");
            return false;
        }
    }
    
    /// <summary>
    /// Gets the current active order for a customer
    /// </summary>
    /// <param name="customerID">Customer identifier</param>
    /// <returns>The active order type, or None if no active order</returns>
    public HeldItemType GetActiveOrder(string customerID)
    {
        if (ActiveOrders.ContainsKey(customerID))
        {
            return ActiveOrders[customerID];
        }
        return HeldItemType.None;
    }
    
    /// <summary>
    /// Checks if a customer has an active order
    /// </summary>
    /// <param name="customerID">Customer identifier</param>
    /// <returns>True if customer has an active order</returns>
    public bool HasActiveOrder(string customerID)
    {
        return ActiveOrders.ContainsKey(customerID);
    }
    
    /// <summary>
    /// Removes an order for a customer (e.g., when they leave without ordering)
    /// </summary>
    /// <param name="customerID">Customer identifier</param>
    public void CancelOrder(string customerID)
    {
        if (ActiveOrders.ContainsKey(customerID))
        {
            Debug.Log($"Cancelled order for customer {customerID}: {ActiveOrders[customerID]}");
            ActiveOrders.Remove(customerID);
        }
    }
    
    /// <summary>
    /// Creates an order bubble for a customer
    /// </summary>
    /// <param name="customerTransform">Customer's transform</param>
    /// <param name="orderType">Type of order to display</param>
    /// <returns>The created order bubble GameObject</returns>
    public GameObject CreateOrderBubble(Transform customerTransform, HeldItemType orderType)
    {
        if (orderBubblePrefab == null)
        {
            Debug.LogError("Order bubble prefab not assigned!");
            return null;
        }
        
        Vector3 bubblePosition = customerTransform.position + new Vector3(1.5f, 3f, 0f);
        GameObject orderBubble = Instantiate(orderBubblePrefab, bubblePosition, Quaternion.identity, customerTransform);
        
        OrderBubble bubbleComponent = orderBubble.GetComponent<OrderBubble>();
        if (bubbleComponent != null)
        {
            bubbleComponent.SetIcon(orderType);
        }
        
        return orderBubble;
    }
    
    public string[] GetActiveCustomerIDs()
    {
        string[] customerIDs = new string[ActiveOrders.Count];
        ActiveOrders.Keys.CopyTo(customerIDs, 0);
        return customerIDs;
    }

    public int GetActiveOrderCount()
    {
        return ActiveOrders.Count;
    }
    
    public void ClearAllOrders()
    {
        ActiveOrders.Clear();
        Debug.Log("All active orders cleared");
    }

    public void SyncOrderFromRPC(string customerID, HeldItemType orderType)
    {
        if (string.IsNullOrEmpty(customerID))
        {
            Debug.LogWarning("Customer ID cannot be null or empty when syncing order");
            return;
        }
        
        // Add or update the order in active orders
        ActiveOrders[customerID] = orderType;
        Debug.Log($"Synced order from RPC for customer {customerID}: {orderType}");
    }
    
}
