using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ECommerce.API.Controllers
{
    [ApiController]
    public abstract class BaseController : ControllerBase
    {
        /// <summary>
        /// ��ǰ�û�ID��δ��¼ʱΪ null��
        /// </summary>
        protected Guid? CurrentUserId
        {
            get
            {
                var userId = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                return Guid.TryParse(userId, out var guid) ? guid : null;
            }
        }

        /// <summary>
        /// ��ǰ�û���
        /// </summary>
        protected string? CurrentUserName => User?.Identity?.Name;

        /// <summary>
        /// ��ǰ�û��Ƿ�Ϊ����Ա
        /// </summary>
        protected bool IsAdmin => User?.IsInRole("Admin") ?? false;
    }
}