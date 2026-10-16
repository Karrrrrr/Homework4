using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SubscriptionsTracker.Data;
using SubscriptionsTracker.Models;

namespace SubscriptionsTracker.Services
{
    public class EfSubscriptionRepository : ISubscriptionRepository
    {
        private readonly SubscriptionsDbContext _db;

        public EfSubscriptionRepository(SubscriptionsDbContext db)
        {
            _db = db;
        }

        public async Task AddAsync(Subscription subscription)
        {
            // Ensure entity has a valid Id before adding
            if (subscription.Id == Guid.Empty)
                subscription.Id = Guid.NewGuid();

            _db.Subscriptions.Add(subscription);
            await SaveChangesWithConcurrencyRetryAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var entity = await _db.Subscriptions.FindAsync(id);
            if (entity != null)
            {
                _db.Subscriptions.Remove(entity);
                await SaveChangesWithConcurrencyRetryAsync();
            }
        }

        public async Task<IEnumerable<Subscription>> GetAllAsync()
        {
            return await _db.Subscriptions.AsNoTracking().ToListAsync();
        }

        public async Task<Subscription?> GetAsync(Guid id)
        {
            return await _db.Subscriptions.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task UpdateAsync(Subscription subscription)
        {
            // Если в ChangeTracker уже есть отслеживаемая сущность с таким Id — обновляем её поля
            var local = _db.Subscriptions.Local.FirstOrDefault(s => s.Id == subscription.Id);
            if (local != null)
            {
                // Копируем значения из входящей сущности в уже отслеживаемую
                _db.Entry(local).CurrentValues.SetValues(subscription);
            }
            else
            {
                // Иначе прикрепляем/обновляем переданное значение
                _db.Subscriptions.Update(subscription);
            }

            await SaveChangesWithConcurrencyRetryAsync();
        }

        private async Task SaveChangesWithConcurrencyRetryAsync()
        {
            const int maxRetries = 3;
            for (int attempt = 0; attempt < maxRetries; attempt++)
            {
                try
                {
                    await _db.SaveChangesAsync();
                    return;
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    if (attempt == maxRetries - 1)
                    {
                        // последняя попытка провалена — пробрасываем
                        throw;
                    }

                    // Попробовать синхронизировать значения с базой и повторить
                    foreach (var entry in ex.Entries)
                    {
                        var databaseValues = await entry.GetDatabaseValuesAsync();
                        if (databaseValues == null)
                        {
                            // Запись была удалена в БД — отсоединить локальную сущность
                            entry.State = EntityState.Detached;
                        }
                        else
                        {
                            // Обновить оригинальные значения, чтобы повторная попытка учитывала текущее состояние БД
                            entry.OriginalValues.SetValues(databaseValues);
                        }
                    }

                    // Небольшая пауза перед повторной попыткой
                    await Task.Delay(50);
                }
            }
        }

        public async Task<IEnumerable<Subscription>> GetDueTomorrowAsync()
        {
            var tomorrow = DateTime.UtcNow.Date.AddDays(1);
            return await _db.Subscriptions.Where(s => s.NextBillingDate.Date == tomorrow).AsNoTracking().ToListAsync();
        }
    }
}
