using ShantiEnterprises.API.DTOs.Return;

namespace ShantiEnterprises.API.Interfaces
{
    public interface IReturnService
    {
        // ==========================================
        // CREATE RETURN
        // ==========================================

        Task<ReturnResponseDto> CreateAsync(
            int userId,
            ReturnCreateDto dto);


        // ==========================================
        // GET ALL RETURNS
        // ADMIN
        // ==========================================

        Task<List<ReturnResponseDto>> GetAllAsync();


        // ==========================================
        // GET RETURN BY ID
        // ==========================================

        Task<ReturnResponseDto> GetByIdAsync(
            int returnId,
            int userId,
            bool isAdmin);


        // ==========================================
        // GET USER RETURNS
        // CUSTOMER
        // ==========================================

        Task<List<ReturnResponseDto>> GetByUserIdAsync(
            int userId);


        // ==========================================
        // UPDATE RETURN STATUS
        // ADMIN
        // ==========================================

        Task<ReturnResponseDto> UpdateStatusAsync(
            int returnId,
            ReturnUpdateDto dto);


        // ==========================================
        // DELETE RETURN
        // ADMIN
        // ==========================================

        Task DeleteAsync(
            int returnId);
    }
}