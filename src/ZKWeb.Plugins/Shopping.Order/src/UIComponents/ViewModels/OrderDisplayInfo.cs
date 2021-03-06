﻿using DotLiquid;
using System;
using System.Collections.Generic;
using ZKWeb.Plugins.Common.Currency.src.Components.Interfaces;
using ZKWeb.Plugins.Shopping.Order.src.Domain.Enums;
using ZKWeb.Plugins.Shopping.Order.src.Domain.Structs;

namespace ZKWeb.Plugins.Shopping.Order.src.UIComponents.ViewModels {
	/// <summary>
	/// 订单的显示信息
	/// 用于显示在订单详情页和订单列表页等
	/// </summary>
	public class OrderDisplayInfo : ILiquidizable {
		/// <summary>
		/// 流水号
		/// </summary>
		public string Serial { get; set; }
		/// <summary>
		/// 买家Id
		/// </summary>
		public Guid? BuyerId { get; set; }
		/// <summary>
		/// 买家用户名
		/// </summary>
		public string Buyer { get; set; }
		/// <summary>
		/// 卖家Id
		/// </summary>
		public Guid? SellerId { get; set; }
		/// <summary>
		/// 卖家用户名
		/// </summary>
		public string Seller { get; set; }
		/// <summary>
		/// 订单状态
		/// </summary>
		public OrderState State { get; set; }
		/// <summary>
		/// 订单状态的描述
		/// </summary>
		public string StateDescription { get; set; }
		/// <summary>
		/// 订单参数
		/// </summary>
		public OrderParameters OrderParameters { get; set; }
		/// <summary>
		/// 订单总金额
		/// </summary>
		public decimal TotalCost { get; set; }
		/// <summary>
		/// 原始订单总金额
		/// </summary>
		public decimal OriginalTotalCost { get; set; }
		/// <summary>
		/// 货币单位
		/// </summary>
		public ICurrency Currency { get; set; }
		/// <summary>
		/// 备注旗帜
		/// </summary>
		public OrderRemarkFlags RemarkFlags { get; set; }
		/// <summary>
		/// 创建时间
		/// </summary>
		public string CreateTime { get; set; }
		/// <summary>
		/// 订单商品的显示信息列表
		/// </summary>
		public IList<OrderProductDisplayInfo> OrderProducts { get; set; }

		/// <summary>
		/// 初始化
		/// </summary>
		public OrderDisplayInfo() {
			OrderProducts = new List<OrderProductDisplayInfo>();
		}

		/// <summary>
		/// 允许描画到模板
		/// </summary>
		/// <returns></returns>
		object ILiquidizable.ToLiquid() {
			return Hash.FromAnonymousObject(this);
		}
	}
}
