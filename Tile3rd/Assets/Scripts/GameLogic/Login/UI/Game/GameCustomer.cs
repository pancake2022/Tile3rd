using CSFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class GameCustomer : BaseUI
{
    public class Customer : BaseUI
    {
        public CustomerConfig Data;
        private GameCustomer controller;
        public GameOrder gameOrder;
        public List<CustomerOrderConfig> current_orderlist;
        public int customerStatus = 0;

        protected override void on_create()
        {
            current_orderlist = new List<CustomerOrderConfig>();//当前客人身上有哪些order
        }
        public Customer Init(CustomerConfig data, GameCustomer controller)
        {
            Data = data;
            this.controller = controller;
            
            SetCondition(1);
            return this;
        }
        private void SetCustomerImage()
        {
            var BG = find_component<Image>("Panel/BG");
            BG.sprite = _ui_manager.FindSprite($"{Data.Pack}",$"{Data.Image}", true);
        }
        public void RefreshState()
        {
            int condition = customerStatus;

            if (condition == 1)
            {
                // 点单 → 支付

                SetCustomerImage();
                GetOrder();
                Debug.Log($"{Data.ID} 客人就坐，开始点单" + OrderFinish());
            }
            else if (condition == 2)
            {
                // 支付 → 离开
                Debug.Log($"{Data.ID} 支付完成，准备离开");
                GetLike();
                SetCondition(3);
            }
            else if (condition == 3)
            {
                // 离开 → 从列表移除
                Debug.Log($"{Data.ID} 离开");
                controller.RemoveCustomer(this);
            }
        }
        private void SetCondition(int newCondition)
        {
            customerStatus = newCondition;
            controller.SetCustomerState(Data.ID, newCondition);
            RefreshState();
        }
        public void CustomerIn()
        {
            //播放出现动画
            Debug.Log("fortest");
        }
        public void CustomerOut()
        {
            //播放消失动画
        }
        private void GetLike()
        {
            //获得点赞
            Debug.Log("ID：" + Data.ID + "获得点赞：" + Data.Like);
        }
        private void GetOrder()
        {
            List<int> IDList = Data.OrderList;
            var all_order = GameConfigManager.GameConfigGroup.CustomerOrderConfigList;
            current_orderlist = all_order.Where(c => IDList.Contains(c.ID)).ToList();

            gameOrder = create_ui<GameOrder>("Panel/Orders").Init(this);
        }
        private bool OrderFinish()
        {
            // 检查当前 orderList 所有订单状态为 3
            return Data.OrderList.All(unlockId =>
                gameOrder.orderList.Any(c => c.Data.ID == unlockId && c.Status == 3));
        }
        public void CheckIfOrderFinish()
        {
            if (OrderFinish())
            {
                Debug.Log($"{Data.ID} 点单完成，准备支付");
                SetCondition(2);
            }
        }
    }


    public GameLevel gameLevel;
    public List<Customer> customerList;
    public Customer current_customer;
    private HashSet<int> addedCustomerIDs = new HashSet<int>();
    private Dictionary<int, int> customerStates = new Dictionary<int, int>();

    protected override void on_create()
    {
        customerList = new List<Customer>();
    }
    public GameCustomer Init(GameLevel gamelevel)
    {
        gameLevel = gamelevel;
        CustomerCome();
        return this;
    }
    private void CustomerCome()
    {
        foreach (var config in gameLevel.current_customerlist)
        {
            if (customerList.Count >= 3) break; // 顾客最多 3 个
            TryAddCustomer(config);
        }
    }
    public void TryAddCustomer(CustomerConfig config)
    {
        Debug.Log($"尝试添加顾客：{config.ID}");

        if (addedCustomerIDs.Contains(config.ID))
        {
            Debug.Log($"顾客 {config.ID} 已经添加过，跳过");
            return;
        }

        if (customerList.Count >= 3)
        {
            Debug.Log("顾客已满（3人），跳过");
            return;
        }

        if (CanAddCustomer(config))
        {
            Transform emptyPos = GetEmptyPos();
            if (emptyPos != null)
            {
                var customerUI = create_ui<Customer>("Template/CustomerTemplate", emptyPos.name);
                customerUI.Init(config, this);
                customerList.Add(customerUI);
                addedCustomerIDs.Add(config.ID);
                Debug.Log($"添加顾客成功：{config.ID}");
            }
            else
            {
                Debug.Log($"没有空位添加顾客 {config.ID}");
            }
        }
        else
        {
            Debug.Log($"顾客 {config.ID} 不满足解锁条件");
        }
    }
    private bool CanAddCustomer(CustomerConfig config)
    {
        if (config.Type == 0)
            return true;

        // 检查当前 customerList 中是否包含所有前置顾客，且这些顾客状态为 3
        return config.Unlock.All(unlockId =>
        customerStates.TryGetValue(unlockId, out int status) && status == 3);
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
    public void RemoveCustomer(Customer customer)
    {
        destroy_ui(customer);
        customerList.Remove(customer);
        RefreshAllCustomerStates();
    }

    public void RefreshAllCustomerStates()
    {
        foreach (var customer in customerList)
        {
            customer.RefreshState();
        }
        CustomerCome();
    }
    public void SetCustomerState(int id, int status)
    {
        customerStates[id] = status;
    }
}