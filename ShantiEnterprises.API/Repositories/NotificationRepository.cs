using Microsoft.EntityFrameworkCore;
using ShantiEnterprises.API.Data;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Repositories
{
    public class NotificationRepository
        : INotificationRepository
    {
        private readonly ShantiEnterprisesDbContext _context;

        public NotificationRepository(
            ShantiEnterprisesDbContext context)
        {
            _context = context;
        }

        public async Task<List<Notification>>
            GetByUserIdAsync(int userId)
        {
            return await _context.Notifications
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
        }

        public async Task<List<Notification>>
            GetUnreadByUserIdAsync(int userId)
        {
            return await _context.Notifications
                .Where(x =>
                    x.UserId == userId &&
                    !x.IsRead)
                .OrderByDescending(x => x.CreatedDate)
                .ToListAsync();
        }

        public async Task<Notification?>
            GetByIdAsync(
                int notificationId,
                int userId)
        {
            return await _context.Notifications
                .FirstOrDefaultAsync(x =>
                    x.NotificationId == notificationId &&
                    x.UserId == userId);
        }

        public async Task<Notification>
            CreateAsync(
                Notification notification)
        {
            _context.Notifications.Add(notification);

            await _context.SaveChangesAsync();

            return notification;
        }

        public async Task UpdateAsync(
            Notification notification)
        {
            _context.Notifications.Update(notification);

            await _context.SaveChangesAsync();
        }

        public async Task MarkAllAsReadAsync(
            int userId)
        {
            var notifications =
                await _context.Notifications
                    .Where(x =>
                        x.UserId == userId &&
                        !x.IsRead)
                    .ToListAsync();

            foreach (var notification in notifications)
            {
                notification.IsRead = true;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteAsync(
            int notificationId,
            int userId)
        {
            var notification =
                await GetByIdAsync(
                    notificationId,
                    userId);

            if (notification == null)
            {
                return false;
            }

            _context.Notifications.Remove(notification);

            await _context.SaveChangesAsync();

            return true;
        }
    }
}