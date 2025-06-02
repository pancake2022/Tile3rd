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
        public GameCustomer controller;
        public GameOrder gameOrder;
        public List<CustomerOrderConfig> current_orderlist;
        public List<bool> isOrderFinish;
        public int customerStatus = 0;
        public Transform mySeatPos;

        protected override void on_create()
        {
            current_orderlist = new List<CustomerOrderConfig>();//当前客人身上有哪些order
            isOrderFinish = new List<bool>();
        }
        public Customer Init(CustomerConfig data, GameCustomer controller)
        {
            Data = data;
            this.controller = controller;
            SetCustomerImage();
            return this;
        }
        private void SetCustomerImage()
        {
            var BG = find_component<Image>("Panel/BG");
            BG.sprite = _ui_manager.FindSprite($"{Data.Pack}",$"{Data.Image}", true);
        }
        public void SetCondition(int newCondition)
        {
            customerStatus = newCondition;
            controller.SetCustomerState(Data.ID, newCondition);
            RefreshState();
        }
        public void RefreshState()
        {
            if (customerStatus == 1)
            {
                // 点单 → 支付
                GetOrder();
                Debug.Log($"{Data.ID} 客人就坐，开始点单" + OrderFinish());
            }
            else if (customerStatus == 2)
            {
                // 支付 → 离开
                GetLike();
                Debug.Log($"{Data.ID} 支付完成，准备离开");
                SetCondition(3);
            }
            else if (customerStatus == 3)
            {
                // 离开 → 从列表移除
                Debug.Log($"{Data.ID} 离开");
                controller.RemoveCustomer(this);
            }
        }
        private void GetOrder()
        {
            // 🛡️ 保护：已经有订单就不重复生成
            if (current_orderlist != null && current_orderlist.Count > 0)
                return;

            List<int> IDList = Data.OrderList;
            var all_order = GameConfigManager.GameConfigGroup.CustomerOrderConfigList;

            // 让每个 ID 都变成一个 config，允许重复
            current_orderlist = IDList
                .Select(id => all_order.FirstOrDefault(c => c.ID == id))
                .Where(c => c != null) // 过滤掉没找到的
                .ToList();

            //记录全部任务的状态
            isOrderFinish.Clear();
            for (int i = 0; i < current_orderlist.Count; i++)
            {
                isOrderFinish.Add(false);
            }

            gameOrder = create_ui<GameOrder>("Panel/Orders").Init(this);
        }
        private bool OrderFinish()
        {
            for (int i = 0; i < isOrderFinish.Count; i++)
            {
                Debug.Log($"检查客人的订单[{i}]是否完成: 状态 = {isOrderFinish[i]}");
            }
            if (isOrderFinish.Count < current_orderlist.Count)
                return false;

            return isOrderFinish.All(finished => finished);
        }
        public void CheckIfOrderFinish()
        {
            if (OrderFinish())
            {
                Debug.Log($"{Data.ID} 点单完成，准备支付");
                SetCondition(2);
            }
        }
        private void GetLike()
        {
            //获得点赞
            Debug.Log("ID：" + Data.ID + "获得点赞：" + Data.Like);
        }
    }

    public GameLevel gameLevel;
    public List<Customer> customerList;
    private HashSet<int> addedCustomerIDs = new HashSet<int>();
    private Dictionary<int, int> customerStates = new Dictionary<int, int>();
    public Transform emptyPos;
    private bool isFirstCustomer = true;
    private Dictionary<CustomerConfig, bool> isAllCustomerFinish;

    protected override void on_create()
    {
        customerList = new List<Customer>();
        isAllCustomerFinish = new Dictionary<CustomerConfig, bool>();
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
            if (!isAllCustomerFinish.ContainsKey(config))
            {
                isAllCustomerFinish.Add(config, false);
            }
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
            emptyPos = GetEmptyPos(config);
            if (emptyPos != null)
            {
                Customer newCustomer = null;
                newCustomer = create_ui<Customer>("Template/CustomerTemplate", emptyPos.name);
                newCustomer.mySeatPos = emptyPos;
                newCustomer.Init(config, this);
                newCustomer.SetCondition(1);
                customerList.Add(newCustomer);
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
    private Transform GetEmptyPos(CustomerConfig config)
    {
        List<Transform> emptySlots = new List<Transform>();

        foreach (Transform pos in gameObject.transform)
        {
            if (pos.childCount == 0)
                emptySlots.Add(pos);
        }

        //if (emptySlots.Count > 0)
        ////return emptySlots[Random.Range(0, emptySlots.Count)];
        //{
        //    if (config.Type == 0)
        //        return emptySlots[1];
        //    else
        //        return emptySlots[Random.Range(0, emptySlots.Count)];
        //}
        if (emptySlots.Count > 0)
        {
            if (isFirstCustomer)
            {
                isFirstCustomer = false; // 确保只执行一次
                if (emptySlots.Count > 1)
                    return emptySlots[1]; // 优先选择中间位（假设是 index 1）
                else
                    return emptySlots[0]; // fallback：只有一个空位
            }
            else
            {
                return emptySlots[Random.Range(0, emptySlots.Count)];
            }
        }
        else
            return null;
    }
    public void RemoveCustomer(Customer customer)
    {
        destroy_ui(customer);
        customerList.Remove(customer);
        customer.transform.SetParent(null);
        isAllCustomerFinish[customer.Data] = true;
        CheckIfCustomerFinish();
        RefreshAllCustomerStates();
    }

    public void RefreshAllCustomerStates()
    {
        Debug.Log("开始刷新顾客");
        foreach (var customer in customerList)
        {
            customer.RefreshState();
        }
        CustomerCome();
    }
    //这是判断前置客人的
    public void SetCustomerState(int id, int status)
    {
        customerStates[id] = status;
    }
    private bool CustomerFinish()
    {
        return isAllCustomerFinish.Values.All(finished => finished);
    }
    public void CheckIfCustomerFinish()
    {
        Debug.Log($"判断关卡是否完成");
        if (CustomerFinish())
        {
            Debug.Log($"关卡完成");
        }
    }
}