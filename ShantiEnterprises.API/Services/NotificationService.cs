using ShantiEnterprises.API.DTOs.Notification;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Services
{
    public class NotificationService : INotificationService
    {
        private readonly INotificationRepository _repository;

        public NotificationService(
            INotificationRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<NotificationResponseDto>>
            GetMyNotificationsAsync(int userId)
        {
            var notifications =
                await _repository.GetByUserIdAsync(userId);

            return notifications
                .Select(Map)
                .ToList();
        }

        public async Task<List<NotificationResponseDto>>
            GetUnreadNotificationsAsync(int userId)
        {
            var notifications =
                await _repository.GetUnreadByUserIdAsync(userId);

            return notifications
                .Select(Map)
                .ToList();
        }

        public async Task<NotificationResponseDto?>
            GetByIdAsync(
                int notificationId,
                int userId)
        {
            var notification =
                await _repository.GetByIdAsync(
                    notificationId,
                    userId);

            if (notification == null)
            {
                return null;
            }

            return Map(notification);
        }

        public async Task<NotificationResponseDto>
            CreateAsync(
                int userId,
                CreateNotificationDto dto)
        {
            var notification = new Notification
            {
                UserId = userId,

                Title = dto.Title,

                Message = dto.Message,

                Type = dto.Type,

                ReferenceType = dto.ReferenceType,

                ReferenceId = dto.ReferenceId,

                IsRead = false,

                CreatedDate = DateTime.UtcNow
            };

            var result =
                await _repository.CreateAsync(notification);

            return Map(result);
        }

        public async Task<bool>
            MarkAsReadAsync(
                int notificationId,
                int userId)
        {
            var notification =
                await _repository.GetByIdAsync(
                    notificationId,
                    userId);

            if (notification == null)
            {
                return false;
            }

            notification.IsRead = true;

            await _repository.UpdateAsync(notification);

            return true;
        }

        public async Task MarkAllAsReadAsync(
            int userId)
        {
            await _repository.MarkAllAsReadAsync(userId);
        }

        public async Task<bool>
            DeleteAsync(
                int notificationId,
                int userId)
        {
            return await _repository.DeleteAsync(
                notificationId,
                userId);
        }

        private static NotificationResponseDto Map(
            Notification notification)
        {
            return new NotificationResponseDto
            {
                NotificationId =
                    notification.NotificationId,

                UserId =
                    notification.UserId,

                Title =
                    notification.Title,

                Message =
                    notification.Message,

                Type =
                    notification.Type,

                ReferenceType =
                    notification.ReferenceType,

                ReferenceId =
                    notification.ReferenceId,

                IsRead =
                    notification.IsRead,

                CreatedDate =
                    notification.CreatedDate
            };
        }
    }
}