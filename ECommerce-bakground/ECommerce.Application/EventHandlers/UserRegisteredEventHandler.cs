using ECommerce.Domain.Interfaces;
using ECommerce.Domain.Models;
using Microsoft.Extensions.Logging;

namespace ECommerce.Application.EventHandlers
{
    /// <summary>
    /// 用户注册事件处理器
    /// </summary>
    public class UserRegisteredEventHandler : IEventHandler<UserRegisteredEvent>
    {
        private readonly ILogger<UserRegisteredEventHandler> _logger;
        private readonly IEmailService _emailService;
        private readonly IStatisticsService _statisticsService;
        private readonly ICacheService _cacheService;
        private readonly INotificationService _notificationService;

        public UserRegisteredEventHandler(
            ILogger<UserRegisteredEventHandler> logger,
            IEmailService emailService,
            IStatisticsService statisticsService,
            ICacheService cacheService,
            INotificationService notificationService)
        {
            _logger = logger;
            _emailService = emailService;
            _statisticsService = statisticsService;
            _cacheService = cacheService;
            _notificationService = notificationService;
        }

        public async Task<bool> HandleAsync(UserRegisteredEvent domainEvent, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("UserRegisteredEventHandler: Processing user registration for user {UserId}", domainEvent.UserId);

                // 1. 发送欢迎邮件
                var emailResult = await _emailService.SendWelcomeEmailAsync(domainEvent.Email, domainEvent.UserName);
                if (!emailResult)
                {
                    _logger.LogWarning("UserRegisteredEventHandler: Failed to send welcome email to {Email}", domainEvent.Email);
                }

                // 2. 更新用户活动统计
                await _statisticsService.UpdateUserActivityStatisticsAsync(domainEvent.UserId, "Registered");

                // 3. 清除相关缓存
                await _cacheService.RemoveByPatternAsync("users_list");

                // 4. 发送系统通知
                await _notificationService.SendSystemAlertAsync(
                    $"New user registered: {domainEvent.UserName} ({domainEvent.Email})",
                    "Info");

                // 5. 记录用户注册日志
                await LogUserRegistrationAsync(domainEvent, cancellationToken);

                _logger.LogInformation("UserRegisteredEventHandler: Successfully processed user registration for user {UserId}", domainEvent.UserId);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UserRegisteredEventHandler: Error processing user registration for user {UserId}", domainEvent.UserId);
                return false;
            }
        }

        private async Task LogUserRegistrationAsync(UserRegisteredEvent domainEvent, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("UserRegisteredEventHandler: Logging user registration for user {UserId} with email {Email}", 
                    domainEvent.UserId, domainEvent.Email);
                
                // 在实际环境中，这里会记录到用户注册日志表
                await Task.Delay(50, cancellationToken);
                
                _logger.LogInformation("UserRegisteredEventHandler: User registration logged successfully for user {UserId}", domainEvent.UserId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "UserRegisteredEventHandler: Failed to log user registration for user {UserId}", domainEvent.UserId);
            }
        }
    }
}
