using ShantiEnterprises.API.DTOs.Return;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Services
{
    public class ReturnService : IReturnService
    {
        private readonly IReturnRepository _returnRepository;
        private readonly IRefundRepository _refundRepository;

        public ReturnService(
            IReturnRepository returnRepository,
            IRefundRepository refundRepository)
        {
            _returnRepository = returnRepository;
            _refundRepository = refundRepository;
        }


        // ==========================================
        // CREATE RETURN
        // CUSTOMER
        // ==========================================

        public async Task<ReturnResponseDto> CreateAsync(
            int userId,
            ReturnCreateDto dto)
        {
            if (dto.OrderId <= 0)
                throw new Exception("Invalid order ID.");

            if (dto.OrderItemId <= 0)
                throw new Exception("Invalid order item ID.");

            if (string.IsNullOrWhiteSpace(dto.Reason))
                throw new Exception("Return reason is required.");


            var existingReturns =
                await _returnRepository
                    .GetByUserIdAsync(userId);


            // ==========================================
            // CHECK DUPLICATE RETURN
            // ==========================================

            var duplicateReturn =
                existingReturns.FirstOrDefault(x =>
                    x.OrderId == dto.OrderId &&
                    x.OrderItemId == dto.OrderItemId &&
                    x.ReturnStatus != "Rejected");


            if (duplicateReturn != null)
            {
                throw new Exception(
                    "Return request already exists for this order item.");
            }


            // ==========================================
            // GET ORDER
            // ==========================================

            var order =
                await _returnRepository
                    .GetOrderForReturnAsync(dto.OrderId);


            if (order == null)
            {
                throw new Exception(
                    "Order not found.");
            }


            // ==========================================
            // ORDER OWNERSHIP
            // ==========================================

            if (order.UserId != userId)
            {
                throw new UnauthorizedAccessException(
                    "You can only create a return for your own order.");
            }


            // ==========================================
            // ORDER STATUS
            // ==========================================

            if (!string.Equals(
                    order.OrderStatus,
                    "Delivered",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new Exception(
                    "Return can only be requested for a delivered order.");
            }


            // ==========================================
            // FIND ORDER ITEM
            // ==========================================

            var orderItem =
                order.OrderItems
                    .FirstOrDefault(x =>
                        x.OrderItemId == dto.OrderItemId);


            if (orderItem == null)
            {
                throw new Exception(
                    "Order item does not belong to this order.");
            }


            // ==========================================
            // CREATE RETURN
            // ==========================================

            var returnRequest = new Return
            {
                OrderId = order.OrderId,

                OrderItemId = orderItem.OrderItemId,

                UserId = userId,

                Reason = dto.Reason.Trim(),

                Description =
                    string.IsNullOrWhiteSpace(dto.Description)
                        ? null
                        : dto.Description.Trim(),

                ReturnStatus = "Pending",

                RequestedDate = DateTime.UtcNow,

                CreatedDate = DateTime.UtcNow
            };


            var createdReturn =
                await _returnRepository
                    .CreateAsync(returnRequest);


            // ==========================================
            // RESPONSE
            // ==========================================

            createdReturn.Order = order;

            createdReturn.OrderItem = orderItem;


            return MapToResponse(createdReturn);
        }


        // ==========================================
        // GET ALL RETURNS
        // ADMIN
        // ==========================================

        public async Task<List<ReturnResponseDto>> GetAllAsync()
        {
            var returns =
                await _returnRepository
                    .GetAllAsync();

            return returns
                .Select(MapToResponse)
                .ToList();
        }


        // ==========================================
        // GET RETURN BY ID
        // ==========================================

        public async Task<ReturnResponseDto> GetByIdAsync(
            int returnId,
            int userId,
            bool isAdmin)
        {
            var returnRequest =
                await _returnRepository
                    .GetByIdAsync(returnId);


            if (returnRequest == null)
            {
                throw new Exception(
                    "Return request not found.");
            }


            // ==========================================
            // CUSTOMER OWNERSHIP
            // ==========================================

            if (!isAdmin &&
                returnRequest.UserId != userId)
            {
                throw new UnauthorizedAccessException(
                    "You are not authorized to access this return.");
            }


            return MapToResponse(returnRequest);
        }


        // ==========================================
        // GET USER RETURNS
        // CUSTOMER
        // ==========================================

        public async Task<List<ReturnResponseDto>> GetByUserIdAsync(
            int userId)
        {
            var returns =
                await _returnRepository
                    .GetByUserIdAsync(userId);

            return returns
                .Select(MapToResponse)
                .ToList();
        }


        // ==========================================
        // UPDATE RETURN STATUS
        // ADMIN
        // ==========================================

        public async Task<ReturnResponseDto> UpdateStatusAsync(
            int returnId,
            ReturnUpdateDto dto)
        {
            var returnRequest =
                await _returnRepository
                    .GetByIdAsync(returnId);


            if (returnRequest == null)
            {
                throw new Exception(
                    "Return request not found.");
            }


            if (string.IsNullOrWhiteSpace(
                    dto.ReturnStatus))
            {
                throw new Exception(
                    "Return status is required.");
            }


            var newStatus =
                dto.ReturnStatus.Trim();


            // ==========================================
            // VALID STATUSES
            // ==========================================

            var validStatuses =
                new[]
                {
                    "Pending",
                    "Approved",
                    "Rejected",
                    "ProductReceived",
                    "RefundProcessing",
                    "Completed"
                };


            if (!validStatuses.Contains(
                    newStatus,
                    StringComparer.OrdinalIgnoreCase))
            {
                throw new Exception(
                    "Invalid return status.");
            }


            newStatus =
                validStatuses.First(x =>
                    x.Equals(
                        newStatus,
                        StringComparison.OrdinalIgnoreCase));


            // ==========================================
            // UPDATE STATUS
            // ==========================================

            returnRequest.ReturnStatus =
                newStatus;


            returnRequest.AdminComment =
                string.IsNullOrWhiteSpace(
                    dto.AdminComment)
                    ? null
                    : dto.AdminComment.Trim();


            returnRequest.UpdatedDate =
                DateTime.UtcNow;


            // ==========================================
            // APPROVED
            // ==========================================

            if (newStatus == "Approved")
            {
                if (returnRequest.ApprovedDate == null)
                {
                    returnRequest.ApprovedDate =
                        DateTime.UtcNow;
                }
            }


            // ==========================================
            // PRODUCT RECEIVED
            // ==========================================

            if (newStatus == "ProductReceived")
            {
                if (returnRequest.ApprovedDate == null)
                {
                    throw new Exception(
                        "Return must be approved before product is received.");
                }

                if (returnRequest.ReceivedDate == null)
                {
                    returnRequest.ReceivedDate =
                        DateTime.UtcNow;
                }
            }


            // ==========================================
            // REFUND PROCESSING
            // ==========================================

            if (newStatus == "RefundProcessing")
            {
                if (returnRequest.ReceivedDate == null)
                {
                    throw new Exception(
                        "Product must be received before refund processing.");
                }


                if (returnRequest.Refund == null)
                {
                    throw new Exception(
                        "Refund record not found.");
                }
            }


            // ==========================================
            // COMPLETED
            // ==========================================

            if (newStatus == "Completed")
            {
                if (returnRequest.ReceivedDate == null)
                {
                    throw new Exception(
                        "Product must be received before completing return.");
                }

                if (returnRequest.Refund == null)
                {
                    throw new Exception(
                        "Refund record not found.");
                }

                if (!string.Equals(
                        returnRequest.Refund.RefundStatus,
                        "Completed",
                        StringComparison.OrdinalIgnoreCase))
                {
                    throw new Exception(
                        "Refund must be completed before completing return.");
                }


                returnRequest.CompletedDate =
                    DateTime.UtcNow;
            }


            await _returnRepository
                .UpdateAsync(returnRequest);


            return MapToResponse(returnRequest);
        }


        // ==========================================
        // DELETE RETURN
        // ADMIN
        // ==========================================

        public async Task DeleteAsync(
            int returnId)
        {
            var returnRequest =
                await _returnRepository
                    .GetByIdAsync(returnId);


            if (returnRequest == null)
            {
                throw new Exception(
                    "Return request not found.");
            }


            if (returnRequest.ReturnStatus != "Pending" &&
                returnRequest.ReturnStatus != "Rejected")
            {
                throw new Exception(
                    "Only pending or rejected returns can be deleted.");
            }


            await _returnRepository
                .DeleteAsync(returnRequest);
        }


        // ==========================================
        // MAPPING
        // ==========================================

        private static ReturnResponseDto MapToResponse(
            Return returnRequest)
        {
            var orderItem =
                returnRequest.OrderItem;


            return new ReturnResponseDto
            {
                ReturnId =
                    returnRequest.ReturnId,

                OrderId =
                    returnRequest.OrderId,

                OrderNumber =
                    returnRequest.Order?.OrderNumber
                    ?? string.Empty,

                OrderItemId =
                    returnRequest.OrderItemId,

                UserId =
                    returnRequest.UserId,

                CustomerName =
                    returnRequest.User?.FullName
                    ?? string.Empty,

                ProductName =
                    orderItem?.ProductName
                    ?? string.Empty,

                Quantity =
                    orderItem?.Quantity
                    ?? 0,

                RefundAmount =
                    orderItem?.TotalPrice
                    ?? 0,

                Reason =
                    returnRequest.Reason,

                Description =
                    returnRequest.Description,

                ReturnStatus =
                    returnRequest.ReturnStatus,

                AdminComment =
                    returnRequest.AdminComment,

                RequestedDate =
                    returnRequest.RequestedDate,

                ApprovedDate =
                    returnRequest.ApprovedDate,

                ReceivedDate =
                    returnRequest.ReceivedDate,

                CompletedDate =
                    returnRequest.CompletedDate,

                CreatedDate =
                    returnRequest.CreatedDate,

                UpdatedDate =
                    returnRequest.UpdatedDate,

                Refund =
                    returnRequest.Refund == null
                        ? null
                        : new RefundResponseDto
                        {
                            RefundId =
                                returnRequest.Refund.RefundId,

                            ReturnId =
                                returnRequest.Refund.ReturnId,

                            OrderId =
                                returnRequest.Refund.OrderId,

                            PaymentId =
                                returnRequest.Refund.PaymentId,

                            RefundAmount =
                                returnRequest.Refund.RefundAmount,

                            RefundStatus =
                                returnRequest.Refund.RefundStatus,

                            RefundReference =
                                returnRequest.Refund.RefundReference,

                            RefundDate =
                                returnRequest.Refund.RefundDate,

                            FailureReason =
                                returnRequest.Refund.FailureReason,

                            CreatedDate =
                                returnRequest.Refund.CreatedDate,

                            UpdatedDate =
                                returnRequest.Refund.UpdatedDate
                        }
            };
        }
    }
}