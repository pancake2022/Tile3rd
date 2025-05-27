using CSFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class GameLevel : BaseUI
{
    //public class Data_Customer : BaseUI
    //{
    //    public int Customer_ID;

    //    public Data_Customer(int id)
    //    {
    //        this.Customer_ID = id;
    //    }
    //}

    //public class Data_Order : BaseUI
    //{
    //    public int Order_ID;
    //    public int Order_Type;
    //    public int Order_Reward;
    //    public int Order_Customer;

    //    public Data_Order(int id, int type, int reward, int customerID)
    //    {
    //        this.Order_ID = id;
    //        this.Order_Type = type;
    //        this.Order_Reward = reward;
    //        this.Order_Customer = customerID;
    //    }
    //}


    //public class Order : BaseUI
    //{
    //    public Data_Order _Order;
    //    public int State { get; private set; }  // 状态：0 ~ 3

    //    protected override void on_create()
    //    {
    //        gameObject.SetActive(false);
    //    }
    //    public Order Init(Data_Order data_Order,int state)
    //    {
    //        _Order = data_Order;
    //        SetOrderType();
    //        SetConditionState(state);
    //        return this;
    //    }
    //    private void SetOrderType()
    //    {
    //        var icon = find_component<Image>("Panel/bubble/order/icon");
    //        icon.sprite = _ui_manager.FindSprite($"M3Tile01", $"e0{_Order.Order_Type}", true);
    //    }
    //    public void SetHind()
    //    {
    //        gameObject.SetActive(false);
    //        Debug.Log("完成订单order");
    //    }
    //    public void SetConditionState(int value)
    //    {
    //        State = value;
    //        RefreshCondition();
    //    }
    //    public void RefreshCondition()
    //    {
    //        Debug.Log(_Order.Order_ID);
    //        if (State == 0)
    //            Condition_0();
    //        if (State == 1)
    //            Condition_1();
    //        if (State == 2)
    //            Condition_2();
    //        if (State == 3)
    //            Condition_3();
    //    }
    //    private void Condition_0()
    //    {
    //        gameObject.SetActive(false);
    //    }
    //    private void Condition_1()
    //    {
    //        gameObject.SetActive(true);
    //    }
    //    private void Condition_2()
    //    {
    //        gameObject.SetActive(true);
    //    }
    //    private void Condition_3()
    //    {
    //        gameObject.SetActive(false);
    //        Debug.Log("隐藏订单"+_Order.Order_ID);
    //    }
    //}
    //public class Customer : BaseUI
    //{
    //    public GameCustom gameCustom;
    //    public Data_Customer _Customer;
    //    public int State { get; private set; }  // 状态：0 ~ 3
    //    public Order order;

    //    protected override void on_create()
    //    {

    //    }
    //    public Customer Init(GameCustom gamecus, Data_Customer _customer, int state)
    //    {
    //        gameCustom = gamecus;
    //        _Customer = _customer;
    //        SetOrder();
    //        SetConditionState(state);
    //        return this;
    //    }
    //    public void SetOrder()
    //    {
    //        foreach (var item in gameCustom.Data_orderList)
    //        {
    //            if (item.Order_Customer == _Customer.Customer_ID)
    //            {
    //                order = create_ui<Order>($"Template/OrderTemplate","Panel");
    //                int state = 1;
    //                gameCustom.orderList.Add(order,state);
    //                order.Init(item,state);

    //                float offsetY = -100f * (item.Order_Type - 1);  // 第一个是 0，第二个是 -100，第三个是 -200...
    //                var rt = order.GetComponent<RectTransform>();
    //                if (rt != null)
    //                    rt.anchoredPosition += new Vector2(0, offsetY);
    //            }
    //        }
    //    }
    //    public void SetConditionState(int value)
    //    {
    //        State = value;
    //        RefreshCondition();
    //    }
    //    public void RefreshCondition()
    //    {
    //        if (State == 0)
    //            Condition_0();
    //        if (State == 1)
    //            Condition_1();
    //        if (State == 2)
    //            Condition_2();
    //        if (State == 3)
    //            Condition_3();
    //    }
    //    private void Condition_0()
    //    {
    //        gameObject.SetActive(false);
    //    }
    //    private void Condition_1()
    //    {
    //        gameObject.SetActive(true);
    //    }
    //    private void Condition_2()
    //    {
    //        gameObject.SetActive(true);
    //    }
    //    private void Condition_3()
    //    {
    //        gameObject.SetActive(false);
    //    }
    //}


    public GameUI gameUI;
    public GameCustomer gameCustomer;
    public List<CustomerConfig> current_customerlist;
    //public Customer customer;
    //public int collectCount;

    //public List<Data_Customer> Data_customerList;
    //public List<Data_Order> Data_orderList { get; private set; }
    //private Dictionary<Customer, int> customerList;
    //private Dictionary<Order, int> orderList;

    protected override void on_create()
    {
        //CreateData_CustomerList();
        //CreateData_OrderList();
        //CreateCustomerList();
        //CustomerShow();
        BGShow();
        current_customerlist = new List<CustomerConfig>();
        CurrentCustomers();
        gameCustomer = create_ui<GameCustomer>("Panel/Customers").Init(this);
    }
    private void BGShow()
    {
        var BG = find_component<Image>("Panel/BG");
        BG.sprite = _ui_manager.FindSprite(
            $"{GameConfigManager.LevelStorage.Current_GameLevel.BgPack}",
            $"{GameConfigManager.LevelStorage.Current_GameLevel.BgImage}", true);
    }
    private void CurrentCustomers()
    {
        List<int> IDList = GameConfigManager.LevelStorage.Current_GameLevel.CustomerList;
        var all_customer = GameConfigManager.GameConfigGroup.CustomerConfigList;
        current_customerlist = all_customer.Where(c => IDList.Contains(c.ID)).ToList();
    }
    //public void CreateData_CustomerList()
    //{
    //    Data_customerList = new List<Data_Customer>();
    //    Data_customerList.Add(new Data_Customer(1));
    //    Data_customerList.Add(new Data_Customer(2));
    //    Data_customerList.Add(new Data_Customer(3));
    //}
    //public void CreateData_OrderList()
    //{
    //    Data_orderList = new List<Data_Order>();
    //    Data_orderList.Add(new Data_Order(101, 1, 100, 1));
    //    Data_orderList.Add(new Data_Order(201, 1, 200, 2));
    //    Data_orderList.Add(new Data_Order(202, 2, 300, 2));
    //    Data_orderList.Add(new Data_Order(203, 3, 400, 2));
    //    Data_orderList.Add(new Data_Order(301, 1, 500, 3));
    //    Data_orderList.Add(new Data_Order(302, 2, 600, 3));
    //}
    //public void CreateCustomerList()
    //{
    //    customerList = new Dictionary<Customer, int>();
    //    orderList = new Dictionary<Order, int>();
    //}
    //public void CustomerShow()
    //{
    //    var c1 = find_component<RectTransform>("Panel/CustomGroup/Cell_1");
    //    var c2 = find_component<RectTransform>("Panel/CustomGroup/Cell_2");
    //    var c3 = find_component<RectTransform>("Panel/CustomGroup/Cell_3");
    //    c1.SetActive(false);
    //    c2.SetActive(false);
    //    c3.SetActive(false);
    //}

    public GameLevel Init(GameUI game)
    {
        gameUI = game;
        return this;
    }
    //private void SetCustomer()
    //{
    //    foreach (var data in Data_customerList)
    //    {
    //        customer = create_ui<Customer>($"Panel/CustomGroup/Cell_{data.Customer_ID}");

    //        int state = data.Customer_ID == 1 ? 1 : 0;
    //        customer.Init(this, data, state);
    //        customerList.Add(customer, state);
    //    }
    //}

    //public void Collect()
    //{
    //    //收集2次出现customer2，收集5次出现customer3
    //    collectCount++;
    //    foreach (var customer in customerList.Keys)
    //    {
    //        if (collectCount == 2 && customer._Customer.Customer_ID == 2)
    //            customer.SetConditionState(1);

    //        if (collectCount == 5 && customer._Customer.Customer_ID == 3)
    //            customer.SetConditionState(1);

    //        customer.RefreshCondition();
    //    }
    //}
    //public void FinishOrder()
    //{
    //    foreach (var customer in gameCustomer.customerList.ToList())
    //    {
    //        foreach (var order in customer.current_orderlist)
    //        {
    //            if (order.ID == gameUI._panel_ui.CollectionUI.currentCellType) 
    //            {
    //                if (customer.gameOrder.current_order.Status == 1)
    //                {
    //                    customer.gameOrder.current_order.SetCondition(2);
    //                }
    //            }
    //            else
    //                Debug.Log("没有匹配的order");
    //        }
    //    }
    //}
    //public void FinishOrder()
    //{
    //    bool found = false;

    //    foreach (var customer in gameCustomer.customerList.ToList())
    //    {
    //        foreach (var order in customer.current_orderlist)
    //        {
    //            if (order.ID == gameUI._panel_ui.CollectionUI.currentCellType)
    //            {
    //                found = true;
    //                if (order.Status == 1)
    //                {
    //                    order.SetCondition(2);
    //                    Debug.Log($"订单完成：客人 {customer.Data.ID} 的订单 {order.ID}");
    //                }
    //                else
    //                {
    //                    Debug.Log($"订单状态不是 1，不能完成：{order.ID}");
    //                }
    //            }
    //        }
    //    }

    //    if (!found)
    //    {
    //        Debug.Log("没有找到匹配的订单");
    //    }
    //}
    public void FinishOrder()
    {
        int targetID = gameUI._panel_ui.CollectionUI.currentCellType;

        foreach (var customer in gameCustomer.customerList)
        {
            foreach (var order in customer.gameOrder.orderList)
            {
                if (order.Data.ID == targetID && order.Status == 1)
                {
                    order.SetCondition(2);
                    Debug.Log($"订单完成：客人 {customer.Data.ID} 的订单 {order.ID}");
                    return; // ✅ 完成后立即退出整个方法
                }
            }
        }

        Debug.Log("没有找到匹配的订单");
    }
}