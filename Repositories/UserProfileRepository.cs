using EcommerceAPI.Models;
using MongoDB.Bson;
using MongoDB.Driver;

namespace EcommerceAPI.Repositories
{
    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly IMongoCollection<UserProfile> _profiles;

        public UserProfileRepository(IMongoDatabase database)
        {
            _profiles = database.GetCollection<UserProfile>("UserProfiles");
        }

        public async Task<UserProfile?> GetByUserIdAsync(string userId)
        {
            return await _profiles.Find(p => p.UserId == userId).FirstOrDefaultAsync();
        }

        public async Task<UserProfile> CreateAsync(UserProfile profile)
        {
            await _profiles.InsertOneAsync(profile);
            return profile;
        }

        public async Task<UserProfile> UpdateAsync(string id, UserProfile profile)
        {
            profile.UpdatedAt = DateTime.UtcNow;
            await _profiles.ReplaceOneAsync(p => p.Id == id, profile);
            return profile;
        }

        public async Task<bool> DeleteAsync(string id)
        {
            var result = await _profiles.DeleteOneAsync(p => p.Id == id);
            return result.DeletedCount > 0;
        }

        public async Task<UserProfile?> AddAddressAsync(string userId, Address address)
        {
            address.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
            address.CreatedAt = DateTime.UtcNow;

            // If this is set as default, remove default from other addresses
            if (address.IsDefault)
            {
                var update = Builders<UserProfile>.Update
                    .Set("addresses.$[].isDefault", false)
                    .Push("addresses", address)
                    .Set("updatedAt", DateTime.UtcNow);

                var arrayFilters = new List<ArrayFilterDefinition>();
                var options = new UpdateOptions { ArrayFilters = arrayFilters };

                var result = await _profiles.UpdateOneAsync(
                    p => p.UserId == userId,
                    update,
                    options
                );

                return await GetByUserIdAsync(userId);
            }
            else
            {
                var update = Builders<UserProfile>.Update
                    .Push("addresses", address)
                    .Set("updatedAt", DateTime.UtcNow);

                var result = await _profiles.UpdateOneAsync(
                    p => p.UserId == userId,
                    update
                );

                return result.IsAcknowledged ? await GetByUserIdAsync(userId) : null;
            }
        }

        public async Task<UserProfile?> UpdateAddressAsync(string userId, string addressId, Address address)
        {
            var filter = Builders<UserProfile>.Filter.And(
                Builders<UserProfile>.Filter.Eq(p => p.UserId, userId),
                Builders<UserProfile>.Filter.ElemMatch(p => p.Addresses, a => a.Id == addressId)
            );

            var update = Builders<UserProfile>.Update
                .Set("addresses.$.type", address.Type)
                .Set("addresses.$.firstName", address.FirstName)
                .Set("addresses.$.lastName", address.LastName)
                .Set("addresses.$.address1", address.Address1)
                .Set("addresses.$.address2", address.Address2)
                .Set("addresses.$.city", address.City)
                .Set("addresses.$.county", address.County)
                .Set("addresses.$.postcode", address.Postcode)
                .Set("addresses.$.country", address.Country)
                .Set("addresses.$.isDefault", address.IsDefault)
                .Set("updatedAt", DateTime.UtcNow);

            // If setting as default, remove default from other addresses
            if (address.IsDefault)
            {
                var pullUpdate = Builders<UserProfile>.Update
                    .Set("addresses.$[addr].isDefault", false)
                    .Set("updatedAt", DateTime.UtcNow);

                var arrayFilters = new List<ArrayFilterDefinition>
                {
                    new BsonDocumentArrayFilterDefinition<BsonDocument>(
                        new BsonDocument("addr._id", new BsonDocument("$ne", addressId))
                    )
                };

                await _profiles.UpdateOneAsync(
                    Builders<UserProfile>.Filter.Eq(p => p.UserId, userId),
                    pullUpdate,
                    new UpdateOptions { ArrayFilters = arrayFilters }
                );
            }

            var result = await _profiles.UpdateOneAsync(filter, update);
            return result.IsAcknowledged ? await GetByUserIdAsync(userId) : null;
        }

        public async Task<UserProfile?> DeleteAddressAsync(string userId, string addressId)
        {
            var update = Builders<UserProfile>.Update
                .PullFilter(p => p.Addresses, a => a.Id == addressId)
                .Set("updatedAt", DateTime.UtcNow);

            var result = await _profiles.UpdateOneAsync(p => p.UserId == userId, update);
            return result.IsAcknowledged ? await GetByUserIdAsync(userId) : null;
        }

        public async Task<UserProfile?> SetDefaultAddressAsync(string userId, string addressId)
        {
            // First, set all addresses to non-default
            var clearDefaultUpdate = Builders<UserProfile>.Update
                .Set("addresses.$[].isDefault", false)
                .Set("updatedAt", DateTime.UtcNow);

            await _profiles.UpdateOneAsync(p => p.UserId == userId, clearDefaultUpdate);

            // Then set the specified address as default
            var setDefaultUpdate = Builders<UserProfile>.Update
                .Set("addresses.$.isDefault", true)
                .Set("updatedAt", DateTime.UtcNow);

            var filter = Builders<UserProfile>.Filter.And(
                Builders<UserProfile>.Filter.Eq(p => p.UserId, userId),
                Builders<UserProfile>.Filter.ElemMatch(p => p.Addresses, a => a.Id == addressId)
            );

            var result = await _profiles.UpdateOneAsync(filter, setDefaultUpdate);
            return result.IsAcknowledged ? await GetByUserIdAsync(userId) : null;
        }

        public async Task<UserProfile?> AddOrderAsync(string userId, Order order)
        {
            order.Id = MongoDB.Bson.ObjectId.GenerateNewId().ToString();
            order.CreatedAt = DateTime.UtcNow;

            var update = Builders<UserProfile>.Update
                .Push("orders", order)
                .Set("updatedAt", DateTime.UtcNow);

            var result = await _profiles.UpdateOneAsync(p => p.UserId == userId, update);
            return result.IsAcknowledged ? await GetByUserIdAsync(userId) : null;
        }

        public async Task<UserProfile?> UpdateOrderStatusAsync(string userId, string orderId, string status, string? trackingNumber = null)
        {
            var filter = Builders<UserProfile>.Filter.And(
                Builders<UserProfile>.Filter.Eq(p => p.UserId, userId),
                Builders<UserProfile>.Filter.ElemMatch(p => p.Orders, o => o.OrderId == orderId)
            );

            var update = Builders<UserProfile>.Update
                .Set("orders.$.status", status)
                .Set("updatedAt", DateTime.UtcNow);

            if (trackingNumber != null)
            {
                update = update.Set("orders.$.trackingNumber", trackingNumber);
            }

            var result = await _profiles.UpdateOneAsync(filter, update);
            return result.IsAcknowledged ? await GetByUserIdAsync(userId) : null;
        }

        public async Task<List<Order>> GetUserOrdersAsync(string userId)
        {
            var profile = await _profiles.Find(p => p.UserId == userId).FirstOrDefaultAsync();
            return profile?.Orders ?? new List<Order>();
        }
    }
}
