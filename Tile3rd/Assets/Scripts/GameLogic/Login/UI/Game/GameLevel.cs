using CSFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class GameLevel : BaseUI
{
    public GameUI gameUI;
    public GameCustomer gameCustomer;
    public List<CustomerConfig> current_customerlist;

    protected override void on_create()
    {
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
    public GameLevel Init(GameUI game)
    {
        gameUI = game;
        return this;
    }
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