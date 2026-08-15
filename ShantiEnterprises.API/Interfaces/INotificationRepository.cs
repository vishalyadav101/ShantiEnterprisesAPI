using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Interfaces
{
    public interface INotificationRepository
    {
        Task<List<Notification>> GetByUserIdAsync(int userId);

        Task<List<Notification>> GetUnreadByUserIdAsync(int userId);

        Task<Notification?> GetByIdAsync(
            int notificationId,
            int userId);

        Task<Notification> CreateAsync(
            Notification notification);

        Task UpdateAsync(
            Notification notification);

        Task MarkAllAsReadAsync(int userId);

        Task<bool> DeleteAsync(
            int notificationId,
            int userId);
    }
}