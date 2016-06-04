﻿using DryIocAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZKWeb.Localize.Interfaces;
using ZKWeb.Utils.Extensions;

namespace ZKWeb.Plugins.Shopping.Order.src.Translates {
	/// <summary>
	/// 中文翻译
	/// </summary>
	[ExportMany, SingletonReuse]
	public class zh_CN : ITranslateProvider {
		private static HashSet<string> Codes = new HashSet<string>() { "zh-CN" };
		private static Dictionary<string, string> Translates = new Dictionary<string, string>()
		{
			{ "Order", "订单" },
			{ "OrderManage", "订单管理" },
			{ "Order management for ec site", "商城使用的订单管理功能" },
			// TODO: 以下未翻译到其他语言
			{ "Buynow", "立刻购买" },
			{ "AddToCart", "加入购物车" },
			{ "The product you are try to purchase does not exist.", "您尝试购买的商品不存在" },
			{ "The product you are try to purchase does not purchasable.", "您尝试购买的商品目前不允许购买" },
			{ "Order count must large than 0", "订购数量必须大于0" },

		};

		public bool CanTranslate(string code) {
			return Codes.Contains(code);
		}

		public string Translate(string text) {
			return Translates.GetOrDefault(text);
		}
	}
}
