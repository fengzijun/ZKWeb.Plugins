﻿using DryIoc;
using DryIocAttributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using ZKWeb.Core;
using ZKWeb.Plugins.Common.Admin.src.Database;
using ZKWeb.Plugins.Common.Admin.src.Extensions;
using ZKWeb.Plugins.Common.Admin.src.Model;
using ZKWeb.Plugins.Common.Base.src;
using ZKWeb.Plugins.Common.Base.src.Extensions;
using ZKWeb.Plugins.Common.Base.src.Model;
using ZKWeb.Utils.Extensions;

namespace ZKWeb.Plugins.Common.Admin.src.AdminApps {
	/// <summary>
	/// 用户管理
	/// </summary>
	[ExportMany]
	public class UserManageApp : AdminAppBuilder<User, UserManageApp> {
		public override string Name { get { return "User Manage"; } }
		public override string Url { get { return "/admin/users"; } }
		public override string TileClass { get { return "tile bg-blue-hoki"; } }
		public override string IconClass { get { return "fa fa-user"; } }
		protected override IAjaxTableCallback<User> GetTableCallback() { return new TableCallback(); }
		protected override IModelFormBuilder GetAddForm() { return new AddForm(); }
		protected override IModelFormBuilder GetEditForm() { return new EditForm(); }

		/// <summary>
		/// 表格回调
		/// </summary>
		public class TableCallback : IAjaxTableCallback<User> {
			/// <summary>
			/// 构建表格时的处理
			/// </summary>
			public void OnBuildTable(AjaxTableBuilder table, AjaxTableSearchBarBuilder searchBar) {
				table.MenuItems.AddDivider();
				table.MenuItems.AddEditActionForAdminApp<UserManageApp>();
				table.MenuItems.AddAddActionForAdminApp<UserManageApp>();
				searchBar.KeywordPlaceHolder = new T("Username");
				searchBar.MenuItems.AddDivider();
				searchBar.MenuItems.AddRecycleBin();
				searchBar.MenuItems.AddAddActionForAdminApp<UserManageApp>();
			}

			/// <summary>
			/// 过滤数据
			/// </summary>
			public void OnQuery(
				AjaxTableSearchRequest request, DatabaseContext context, ref IQueryable<User> query) {
				query = query.FilterByRecycleBin(request);
				if (!string.IsNullOrEmpty(request.Keyword)) {
					query = query.Where(u => u.Username.Contains(request.Keyword));
				}
			}

			/// <summary>
			/// 排序数据
			/// </summary>
			public void OnSort(
				AjaxTableSearchRequest request, DatabaseContext context, ref IQueryable<User> query) {
				query = query.OrderByDescending(u => u.Id);
			}

			/// <summary>
			/// 选择数据
			/// </summary>
			public void OnSelect(
				AjaxTableSearchRequest request, List<KeyValuePair<User, Dictionary<string, object>>> pairs) {
				var userManager = Application.Ioc.Resolve<UserManager>();
				foreach (var pair in pairs) {
					var role = pair.Key.Role;
					pair.Value["Id"] = pair.Key.Id;
					pair.Value["Avatar"] = userManager.GetAvatarWebPath(pair.Key.Id);
					pair.Value["Username"] = pair.Key.Username;
					pair.Value["UserType"] = new T(pair.Key.Type.GetDescription());
					pair.Value["Role"] = role == null ? null : role.Name;
					pair.Value["RoleId"] = role == null ? null : (long?)role.Id;
					pair.Value["CreateTime"] = pair.Key.CreateTime.ToClientTimeString();
					pair.Value["Deleted"] = pair.Key.Deleted ? EnumDeleted.Deleted : EnumDeleted.None;
				}
			}

			/// <summary>
			/// 添加列和批量操作
			/// </summary>
			public void OnResponse(
				AjaxTableSearchRequest request, AjaxTableSearchResponse response) {
				var idColumn = response.Columns.AddIdColumn("Id");
				response.Columns.AddNoColumn();
				response.Columns.AddImageColumn("Avatar");
				response.Columns.AddMemberColumn("Username", "45%");
				response.Columns.AddMemberColumn("UserType");
				response.Columns.AddMemberColumn("Role");
				response.Columns.AddMemberColumn("CreateTime");
				response.Columns.AddEnumLabelColumn("Deleted", typeof(EnumDeleted));
				var actionColumn = response.Columns.AddActionColumn();
				actionColumn.AddEditActionForAdminApp<UserManageApp>(new T("View"));
				idColumn.AddDivider();
				idColumn.AddDeleteActionsForAdminApp<UserManageApp>(request);
			}
		}

		/// <summary>
		/// 添加和编辑共用的基础表单
		/// </summary>
		public abstract class BaseForm : TabDataEditFormBuilder<User, BaseForm> {
			/// <summary>
			/// 密码
			/// </summary>
			[StringLength(100, MinimumLength = 5)]
			[PasswordField("Password", "Keep empty if you don't want to change", Group = "Change Password")]
			public string Password { get; set; }
			/// <summary>
			/// 确认密码
			/// </summary>
			[StringLength(100, MinimumLength = 5)]
			[PasswordField("ConfirmPassword", "Keep empty if you don't want to change", Group = "Change Password")]
			public string ConfirmPassword { get; set; }
			/// <summary>
			/// 头像
			/// </summary>
			[FileUploaderField("Avatar", Group = "Change Avatar")]
			public HttpPostedFileBase Avatar { get; set; }
			/// <summary>
			/// 删除头像
			/// </summary>
			[CheckBoxField("DeleteAvatar", Group = "Change Avatar")]
			public bool DeleteAvatar { get; set; }

			/// <summary>
			/// 保存表单到数据
			/// </summary>
			protected override object OnSubmit(DatabaseContext context, User saveTo) {
				// 添加时设置创建时间，并要求填密码
				// 添加时设置用户类型是"用户"
				if (saveTo.Id <= 0) {
					saveTo.CreateTime = DateTime.UtcNow;
					if (string.IsNullOrEmpty(Password)) {
						throw new HttpException(400, new T("Please enter password when creating user"));
					}
					saveTo.Type = UserTypes.User;
				}
				// 添加用户或修改密码需要超级管理员
				if (!string.IsNullOrEmpty(Password)) {
					PrivilegesChecker.Check(UserTypes.SuperAdmin);
					if (Password != ConfirmPassword) {
						throw new HttpException(400, new T("Please repeat the password exactly"));
					}
					saveTo.SetPassword(Password);
				}
				return new {
					message = new T("Saved Successfully"),
					script = ScriptStrings.AjaxtableUpdatedAndCloseModal
				};
			}

			/// <summary>
			/// 保存头像
			/// </summary>
			protected override void OnSubmitSaved(DatabaseContext context, User saved) {
				var userManagr = Application.Ioc.Resolve<UserManager>();
				if (Avatar != null) {
					userManagr.SaveAvatar(saved.Id, Avatar.InputStream);
				} else if (DeleteAvatar) {
					userManagr.DeleteAvatar(saved.Id);
				}
			}
		}

		/// <summary>
		/// 添加表单，允许设置用户名
		/// </summary>
		public class AddForm : BaseForm {
			[Required]
			[StringLength(100, MinimumLength = 3)]
			[TextBoxField("Username", "Please enter username")]
			public string Username { get; set; }

			/// <summary>
			/// 绑定数据到表单
			/// </summary>
			protected override void OnBind(DatabaseContext context, User bindFrom) { }

			/// <summary>
			/// 保存表单到数据
			/// </summary>
			protected override object OnSubmit(DatabaseContext context, User saveTo) {
				saveTo.Username = Username;
				return base.OnSubmit(context, saveTo);
			}
		}

		/// <summary>
		/// 编辑表单，用户名只读
		/// </summary>
		public class EditForm : BaseForm {
			[LabelField("Username")]
			public string Username { get; set; }

			/// <summary>
			/// 绑定数据到表单
			/// </summary>
			protected override void OnBind(DatabaseContext context, User bindFrom) {
				Username = bindFrom.Username;
			}
		}
	}
}