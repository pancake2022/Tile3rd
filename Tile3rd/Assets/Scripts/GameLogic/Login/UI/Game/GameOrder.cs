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

        protected override void on_create()
        {
        }
        public Order Init(CustomerOrderConfig data, GameOrder controller)
        {
            Data = data;
            this.controller = controller;
            //GetImage();
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
                //SetCondition(2);
            }
            else if (condition == 2)
            {
                // 获得钱
                Debug.Log($"{Data.ID} 支付订单");
                GetReward();
            }
            else if (condition == 3)
            {
                // 从列表移除
                Debug.Log($"{Data.ID} 删除");
                controller.RemoveOrder(this);
            }
        }
        public void SetCondition(int newCondition)
        {
            Status = newCondition;
            RefreshState();
        }
        
        private void GetReward()
        {
            //获得点赞
            SetCondition(3);
        }
    }

    public GameCustomer.Customer Customer;
    public List<Order> orderList;
    public Order current_order;
    private int currentOrderIndex = 0;

    protected override void on_create()
    {
        orderList = new List<Order>();
    }

    public GameOrder Init(GameCustomer.Customer customer)
    {
        Customer = customer;
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
            Transform emptyPos = GetEmptyPos();
            if (emptyPos != null)
            {
                current_order = create_ui<Order>("Template/OrderTemplate_1", emptyPos.name);
                current_order.Init(config, this);
                orderList.Add(current_order);
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
        RefreshAllOrderStates();
    }

    public void RefreshAllOrderStates()
    {
        foreach (var order in orderList)
        {
            order.RefreshState();
        }
        OrderCome();
    }
}