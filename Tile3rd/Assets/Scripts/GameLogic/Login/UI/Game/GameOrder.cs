using CSFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class GameOrder : BaseUI
{
    public class Order : BaseUI
    {
        public CustomerOrderConfig Data;
        private GameOrder controller;
        public int Status = 0;
        public int ID;
        public int OrderIndex;

        public Order Init(CustomerOrderConfig data, GameOrder controller, int index)
        {
            Data = data;
            this.controller = controller;
            this.OrderIndex = index;
            SetCondition(1);
            ID = Data.ID;
            return this;
        }
        private void GetImage()
        {
            var icon = find_component<Image>("Panel/order/icon");
            icon.sprite = _ui_manager.FindSprite($"{Data.Pack}", $"{Data.Image}", true);
        }
        public void RefreshState()
        {
            int condition = Status;

            if (condition == 1)
            {
                // 显示订单
                Debug.Log($"{Data.ID} 显示订单");
                GetImage();
            }
            else if (condition == 2)
            {
                // 获得钱
                GetReward();
            }
            else if (condition == 3)
            {
                // 从列表移除
                Debug.Log($"{Data.ID} 删除");
                controller.RemoveOrder(this);
                //controller.Customer.isOrderFinish[OrderIndex] = true;
            }
        }
        public void SetCondition(int newCondition)
        {
            Status = newCondition;
            RefreshState();
        }
        
        private void GetReward()
        {
            //获得钱
            Debug.Log($"{Data.ID} 支付订单" + Data.Reward);
            SetCondition(3);
        }
    }

    public GameCustomer.Customer Customer;
    public List<Order> orderList;
    private int currentOrderIndex = 0;

    protected override void on_create()
    {
        orderList = new List<Order>();
    }

    public GameOrder Init(GameCustomer.Customer customer)
    {
        Customer = customer;
        currentOrderIndex = 0; // 避免跨顾客复用 index
        orderList.Clear(); // 清理旧的 UI
        OrderCome();
        return this;
    }
    private void OrderCome()
    {
        Debug.Log("currentOrderIndex" + currentOrderIndex + "/" + Customer.current_orderlist.Count);
        while (orderList.Count < 3 && currentOrderIndex < Customer.current_orderlist.Count)
        {
            var config = Customer.current_orderlist[currentOrderIndex];
            TryAddOrder(config);
            currentOrderIndex++;
        }
    }
    public void TryAddOrder(CustomerOrderConfig config)
    {
        Debug.Log($"尝试添加订单：{config.ID}");

        if (orderList.Count >= 3)
        {
            Debug.Log("订单已满（3个），跳过");
            return;
        }

        if (CanAddOrder(config))
        {
            Transform emptyPos = GetEmptyPosAndSetPosition(Customer.controller.emptyPos.name);

            if (emptyPos != null)
            {
                Order newOrder = null;
                if (emptyPos.name == "Pos_1")
                    newOrder = create_ui<Order>("Template/OrderTemplate_1", emptyPos.name);
                if (emptyPos.name == "Pos_2")
                    newOrder = create_ui<Order>("Template/OrderTemplate_2", emptyPos.name);
                if (emptyPos.name == "Pos_3")
                    newOrder = create_ui<Order>("Template/OrderTemplate_3", emptyPos.name);

                newOrder.Init(config, this, currentOrderIndex);
                orderList.Add(newOrder);
                Debug.Log($"添加订单成功：{config.ID}");
            }
            else
            {
                Debug.Log($"没有空位添加订单 {config.ID}");
            }
        }
        else
        {
            Debug.Log($"订单 {config.ID} 不满足解锁条件");
        }
    }
    private bool CanAddOrder(CustomerOrderConfig config)
    {
        Debug.Log("创建了order");
        //自身的状态为0的
        return true;
    }
    private Transform GetEmptyPos()
    {
        List<Transform> emptySlots = new List<Transform>();
        foreach (Transform pos in gameObject.transform)
        {
            if (pos.childCount == 0)
                emptySlots.Add(pos);
        }

        if (emptySlots.Count > 0)
            return emptySlots[Random.Range(0, emptySlots.Count)];
        else
            return null;
    }
    public void RemoveOrder(Order order)
    {
        destroy_ui(order);
        orderList.Remove(order);
        order.transform.SetParent(null);
        Debug.Log("删除订单成功" + order.ID);
        

        //去判断客人的订单是否全部完成？
        Customer.isOrderFinish[order.OrderIndex] = true;
        Customer.CheckIfOrderFinish();

        RefreshAllOrderStates();
    }

    public void RefreshAllOrderStates()
    {
        Debug.Log("开始刷新订单");
        foreach (var order in orderList)
        {
            order.RefreshState();
        }
        OrderCome();
    }

    private Transform GetEmptyPosAndSetPosition(string index)
    {
        List<Transform> emptySlots = new List<Transform>();
        foreach (Transform pos in gameObject.transform)
        {
            if (pos.childCount == 0)
                emptySlots.Add(pos);
        }

        if (emptySlots.Count == 0)
            return null;

        Transform emptyPos = emptySlots[Random.Range(0, emptySlots.Count)];

        if (index == "Pos_1")
        {
            // 这里根据空位的名字设置位置
            if (emptyPos.name == "Pos_1")
                emptyPos.localPosition = new Vector3(2, 131, 0); // 替换为 a
            else if (emptyPos.name == "Pos_2")
                emptyPos.localPosition = new Vector3(-86, 59, 0); // 替换为 b
            else if (emptyPos.name == "Pos_3")
                emptyPos.localPosition = new Vector3(90, 6, 0); // 替换为 c
        }
        if (index == "Pos_2")
        {
            // 这里根据空位的名字设置位置
            if (emptyPos.name == "Pos_1")
                emptyPos.localPosition = new Vector3(2, 132, 0); // 替换为 a
            else if (emptyPos.name == "Pos_2")
                emptyPos.localPosition = new Vector3(-86, 87, 0); // 替换为 b
            else if (emptyPos.name == "Pos_3")
                emptyPos.localPosition = new Vector3(90, 69, 0); // 替换为 c
        }
        if (index == "Pos_3")
        {
            // 这里根据空位的名字设置位置
            if (emptyPos.name == "Pos_1")
                emptyPos.localPosition = new Vector3(2, 132, 0); // 替换为 a
            else if (emptyPos.name == "Pos_2")
                emptyPos.localPosition = new Vector3(-86, 12, 0); // 替换为 b
            else if (emptyPos.name == "Pos_3")
                emptyPos.localPosition = new Vector3(90, 50, 0); // 替换为 c
        }
        return emptyPos;
    }
}