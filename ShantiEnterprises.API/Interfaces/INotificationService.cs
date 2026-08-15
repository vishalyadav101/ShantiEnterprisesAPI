using ShantiEnterprises.API.DTOs.Notification;

namespace ShantiEnterprises.API.Interfaces
{
    public interface INotificationService
    {
        Task<List<NotificationResponseDto>> GetMyNotificationsAsync(
            int userId);

        Task<List<NotificationResponseDto>> GetUnreadNotificationsAsync(
            int userId);

        Task<NotificationResponseDto?> GetByIdAsync(
            int notificationId,
            int userId);

        Task<NotificationResponseDto> CreateAsync(
            int userId,
            CreateNotificationDto dto);

        Task<bool> MarkAsReadAsync(
            int notificationId,
            int userId);

        Task MarkAllAsReadAsync(int userId);

        Task<bool> DeleteAsync(
            int notificationId,
            int userId);
    }
}