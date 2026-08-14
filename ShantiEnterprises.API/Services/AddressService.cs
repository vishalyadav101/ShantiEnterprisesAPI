using ShantiEnterprises.API.DTOs.Address;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Services
{
    public class AddressService : IAddressService
    {
        private readonly IAddressRepository _repository;

        public AddressService(
            IAddressRepository repository)
        {
            _repository = repository;
        }

        public async Task<List<AddressResponseDto>> GetAllAsync(
            int userId)
        {
            var addresses =
                await _repository.GetByUserIdAsync(userId);

            return addresses
                .Select(MapToResponse)
                .ToList();
        }

        public async Task<AddressResponseDto?> GetByIdAsync(
            int userId,
            int addressId)
        {
            var address =
                await _repository.GetByIdAsync(
                    addressId,
                    userId);

            return address == null
                ? null
                : MapToResponse(address);
        }

        public async Task<AddressResponseDto> CreateAsync(
            int userId,
            AddressCreateDto dto)
        {
            var existingAddresses =
                await _repository.GetByUserIdAsync(userId);

            if (!existingAddresses.Any())
            {
                dto.IsDefault = true;
            }
            else if (dto.IsDefault)
            {
                await _repository.ClearDefaultAsync(userId);
            }

            var address = new Address
            {
                UserId = userId,

                FullName = dto.FullName.Trim(),

                Mobile = dto.MobileNumber.Trim(),

                AddressLine1 = dto.AddressLine1.Trim(),

                AddressLine2 =
                    dto.AddressLine2?.Trim(),

                City = dto.City.Trim(),

                State = dto.State.Trim(),

                Pincode = dto.Pincode.Trim(),

                Country = dto.Country.Trim(),

                AddressType = dto.AddressType.Trim(),

                IsDefault = dto.IsDefault,

                CreatedDate = DateTime.UtcNow
            };

            var result =
                await _repository.AddAsync(address);

            return MapToResponse(result);
        }

        public async Task<AddressResponseDto?> UpdateAsync(
            int userId,
            int addressId,
            AddressUpdateDto dto)
        {
            var address =
                await _repository.GetByIdAsync(
                    addressId,
                    userId);

            if (address == null)
            {
                return null;
            }

            if (dto.IsDefault)
            {
                await _repository.ClearDefaultAsync(userId);
            }

            address.FullName =
                dto.FullName.Trim();

            address.Mobile =
                dto.MobileNumber.Trim();

            address.AddressLine1 =
                dto.AddressLine1.Trim();

            address.AddressLine2 =
                dto.AddressLine2?.Trim();

            address.City =
                dto.City.Trim();

            address.State =
                dto.State.Trim();

            address.Pincode =
                dto.Pincode.Trim();

            address.Country =
                dto.Country.Trim();

            address.AddressType =
                dto.AddressType.Trim();

            address.IsDefault =
                dto.IsDefault;

            address.UpdatedDate =
                DateTime.UtcNow;

            await _repository.UpdateAsync(address);

            return MapToResponse(address);
        }

        public async Task<bool> DeleteAsync(
            int userId,
            int addressId)
        {
            var address =
                await _repository.GetByIdAsync(
                    addressId,
                    userId);

            if (address == null)
            {
                return false;
            }

            await _repository.DeleteAsync(address);

            return true;
        }

        public async Task<bool> SetDefaultAsync(
            int userId,
            int addressId)
        {
            var address =
                await _repository.GetByIdAsync(
                    addressId,
                    userId);

            if (address == null)
            {
                return false;
            }

            await _repository.ClearDefaultAsync(userId);

            address.IsDefault = true;

            await _repository.UpdateAsync(address);

            return true;
        }

        private static AddressResponseDto MapToResponse(
            Address address)
        {
            return new AddressResponseDto
            {
                AddressId = address.AddressId,

                UserId = address.UserId,

                FullName = address.FullName,

                MobileNumber = address.Mobile,

                AddressLine1 = address.AddressLine1,

                AddressLine2 = address.AddressLine2,

                City = address.City,

                State = address.State,

                Pincode = address.Pincode,

                Country = address.Country,

                AddressType = address.AddressType,

                IsDefault = address.IsDefault,

                CreatedDate = address.CreatedDate
            };
        }
    }
}