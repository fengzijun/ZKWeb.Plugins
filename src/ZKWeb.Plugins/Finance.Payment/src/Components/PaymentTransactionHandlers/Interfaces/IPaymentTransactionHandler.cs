﻿using System.Collections.Generic;
using ZKWeb.Plugins.Finance.Payment.src.Domain.Entities;
using ZKWeb.Plugins.Finance.Payment.src.Domain.Enums;
using ZKWebStandard.Collection;

namespace ZKWeb.Plugins.Finance.Payment.src.Components.PaymentTransactionHandlers.Interfaces {
	/// <summary>
	/// 支付交易处理器的接口
	/// 同一交易类型可以注册多个处理器，按注册顺序调用
	/// </summary>
	public interface IPaymentTransactionHandler {
		/// <summary>
		/// 交易类型
		/// </summary>
		string Type { get; }

		/// <summary>
		/// 交易创建后的处理
		/// </summary>
		/// <param name="transaction">支付交易</param>
		void OnCreated(PaymentTransaction transaction);

		/// <summary>
		/// 等待付款时的处理
		/// 抛出例外可以阻止事务提交
		/// 多线程下同一交易最多只会调用一次
		/// 部分接口不会调用这个函数
		/// </summary>
		/// <param name="transaction">支付交易</param>
		/// <param name="previousState">原有的状态</param>
		void OnWaitingPaying(
			PaymentTransaction transaction,
			PaymentTransactionState previousState);

		/// <summary>
		/// 担保交易已付款时的处理
		/// 抛出例外可以阻止事务提交
		/// 多线程下同一交易最多只会调用一次
		/// </summary>
		/// <param name="transaction">支付交易</param>
		/// <param name="previousState">原有的状态</param>
		/// <param name="parameters">自动发货参数，需要自动发货时请添加这个参数</param>
		void OnSecuredPaid(
			PaymentTransaction transaction,
			PaymentTransactionState previousState,
			IList<AutoSendGoodsParameters> parameters);

		/// <summary>
		/// 交易成功时的处理
		/// 抛出例外可以阻止事务提交
		/// 多线程下同一交易最多只会调用一次
		/// 可以看原有的状态判断是即时到账还是担保交易
		/// </summary>
		/// <param name="transaction">支付交易</param>
		/// <param name="previousState">原有的状态</param>
		void OnSuccess(
			PaymentTransaction transaction,
			PaymentTransactionState previousState);

		/// <summary>
		/// 交易中止时的处理
		/// 抛出例外可以阻止事务提交
		/// 多线程下同一交易最多只会调用一次
		/// </summary>
		/// <param name="transaction">支付交易</param>
		/// <param name="previousState">原有的状态</param>
		void OnAbort(
			PaymentTransaction transaction,
			PaymentTransactionState previousState);

		/// <summary>
		/// 获取支付成功或失败后，显示在结果页面的Html内容
		/// 是否可以查看结果已在外部判断，这个函数不需要判断
		/// 例
		/// - 支付成功
		/// - 即将返回订单页面
		/// </summary>
		/// <param name="transaction">支付交易</param>
		/// <param name="html">显示的Html内容</param>
		void GetResultHtml(
			PaymentTransaction transaction,
			IList<HtmlString> html);
	}
}
