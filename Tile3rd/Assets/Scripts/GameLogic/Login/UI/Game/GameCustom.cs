using CSFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class GameCustom : BaseUI
{
    public class Data_Customer : BaseUI
    {
        public int Customer_ID;

        public Data_Customer(int id)
        {
            this.Customer_ID = id;
        }
    }

    public class Data_Order : BaseUI
    {
        public int Order_ID;
        public int Order_Type;
        public int Order_Reward;
        public int Order_Customer;

        public Data_Order(int id, int type, int reward, int customerID)
        {
            this.Order_ID = id;
            this.Order_Type = type;
            this.Order_Reward = reward;
            this.Order_Customer = customerID;
        }
    }


    public class Order : BaseUI
    {
        public Data_Order _Order;
        public int State { get; private set; }  // 状态：0 ~ 3

        protected override void on_create()
        {
            gameObject.SetActive(false);
        }
        public Order Init(Data_Order data_Order,int state)
        {
            _Order = data_Order;
            SetOrderType();
            SetConditionState(state);
            return this;
        }
        private void SetOrderType()
        {
            var icon = find_component<Image>("Panel/bubble/order/icon");
            icon.sprite = _ui_manager.FindSprite($"M3Tile01", $"e0{_Order.Order_Type}", true);
        }
        public void SetHind()
        {
            gameObject.SetActive(false);
            Debug.Log("完成订单order");
        }
        public void SetConditionState(int value)
        {
            State = value;
            RefreshCondition();
        }
        public void RefreshCondition()
        {
            Debug.Log(_Order.Order_ID);
            if (State == 0)
                Condition_0();
            if (State == 1)
                Condition_1();
            if (State == 2)
                Condition_2();
            if (State == 3)
                Condition_3();
        }
        private void Condition_0()
        {
            gameObject.SetActive(false);
        }
        private void Condition_1()
        {
            gameObject.SetActive(true);
        }
        private void Condition_2()
        {
            gameObject.SetActive(true);
        }
        private void Condition_3()
        {
            gameObject.SetActive(false);
            Debug.Log("隐藏订单"+_Order.Order_ID);
        }
    }
    public class Customer : BaseUI
    {
        public GameCustom gameCustom;
        public Data_Customer _Customer;
        public int State { get; private set; }  // 状态：0 ~ 3
        public Order order;

        protected override void on_create()
        {

        }
        public Customer Init(GameCustom gamecus, Data_Customer _customer, int state)
        {
            gameCustom = gamecus;
            _Customer = _customer;
            SetOrder();
            SetConditionState(state);
            return this;
        }
        public void SetOrder()
        {
            foreach (var item in gameCustom.Data_orderList)
            {
                if (item.Order_Customer == _Customer.Customer_ID)
                {
                    order = create_ui<Order>($"Template/OrderTemplate","Panel");
                    int state = 1;
                    gameCustom.orderList.Add(order,state);
                    order.Init(item,state);

                    float offsetY = -100f * (item.Order_Type - 1);  // 第一个是 0，第二个是 -100，第三个是 -200...
                    var rt = order.GetComponent<RectTransform>();
                    if (rt != null)
                        rt.anchoredPosition += new Vector2(0, offsetY);
                }
            }
        }
        public void SetConditionState(int value)
        {
            State = value;
            RefreshCondition();
        }
        public void RefreshCondition()
        {
            if (State == 0)
                Condition_0();
            if (State == 1)
                Condition_1();
            if (State == 2)
                Condition_2();
            if (State == 3)
                Condition_3();
        }
        private void Condition_0()
        {
            gameObject.SetActive(false);
        }
        private void Condition_1()
        {
            gameObject.SetActive(true);
        }
        private void Condition_2()
        {
            gameObject.SetActive(true);
        }
        private void Condition_3()
        {
            gameObject.SetActive(false);
        }
    }


    public GameUI gameUI;
    public Customer customer;
    public int collectCount;

    public List<Data_Customer> Data_customerList;
    public List<Data_Order> Data_orderList { get; private set; }
    private Dictionary<Customer, int> customerList;
    private Dictionary<Order, int> orderList;

    protected override void on_create()
    {
        CreateData_CustomerList();
        CreateData_OrderList();
        CreateCustomerList();
        CustomerShow();
    }
    public void CreateData_CustomerList()
    {
        Data_customerList = new List<Data_Customer>();
        Data_customerList.Add(new Data_Customer(1));
        Data_customerList.Add(new Data_Customer(2));
        Data_customerList.Add(new Data_Customer(3));
    }
    public void CreateData_OrderList()
    {
        Data_orderList = new List<Data_Order>();
        Data_orderList.Add(new Data_Order(101, 1, 100, 1));
        Data_orderList.Add(new Data_Order(201, 1, 200, 2));
        Data_orderList.Add(new Data_Order(202, 2, 300, 2));
        Data_orderList.Add(new Data_Order(203, 3, 400, 2));
        Data_orderList.Add(new Data_Order(301, 1, 500, 3));
        Data_orderList.Add(new Data_Order(302, 2, 600, 3));
    }
    public void CreateCustomerList()
    {
        customerList = new Dictionary<Customer, int>();
        orderList = new Dictionary<Order, int>();
    }
    public void CustomerShow()
    {
        var c1 = find_component<RectTransform>("Panel/CustomGroup/Cell_1");
        var c2 = find_component<RectTransform>("Panel/CustomGroup/Cell_2");
        var c3 = find_component<RectTransform>("Panel/CustomGroup/Cell_3");
        c1.SetActive(false);
        c2.SetActive(false);
        c3.SetActive(false);
    }

    public GameCustom Init(GameUI game)
    {
        gameUI = game;
        SetCustomer();
        return this;
    }
    private void SetCustomer()
    {
        foreach (var data in Data_customerList)
        {
            customer = create_ui<Customer>($"Panel/CustomGroup/Cell_{data.Customer_ID}");

            int state = data.Customer_ID == 1 ? 1 : 0;
            customer.Init(this, data, state);
            customerList.Add(customer, state);
        }
    }

    public void Collect()
    {
        //收集2次出现customer2，收集5次出现customer3
        collectCount++;
        foreach (var customer in customerList.Keys)
        {
            if (collectCount == 2 && customer._Customer.Customer_ID == 2)
                customer.SetConditionState(1);
                
            if (collectCount == 5 && customer._Customer.Customer_ID == 3)
                customer.SetConditionState(1);

            customer.RefreshCondition();
        }
    }
    public void FinishOrder()
    {
        foreach (var order in orderList.Keys)
        {
            if (order._Order.Order_Type == gameUI._panel_ui.CollectionUI.currentCellType)
            {
                if (orderList[order] < 3)
                {
                    Debug.Log("完成订单" + order._Order.Order_ID);
                    orderList[order] = 3;
                    order.SetConditionState(3);
                    break;
                }
                
            }
        }
    }
}