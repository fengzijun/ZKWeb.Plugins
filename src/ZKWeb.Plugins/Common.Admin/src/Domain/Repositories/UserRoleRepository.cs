﻿using System;
using ZKWeb.Plugins.Common.Admin.src.Domain.Entities;
using ZKWeb.Plugins.Common.Base.src.Domain.Repositories.Bases;
using ZKWebStandard.Ioc;

namespace ZKWeb.Plugins.Common.Admin.src.Domain.Repositories {
	/// <summary>
	/// 用户角色的仓储
	/// </summary>
	[ExportMany, SingletonReuse]
	public class UserRoleRepository : RepositoryBase<UserRole, Guid> { }
}
