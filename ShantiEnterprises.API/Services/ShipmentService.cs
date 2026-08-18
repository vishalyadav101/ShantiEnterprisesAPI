using ShantiEnterprises.API.DTOs.Shipment;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Services
{
    public class ShipmentService : IShipmentService
    {
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IOrderRepository _orderRepository;

        public ShipmentService(
            IShipmentRepository shipmentRepository,
            IOrderRepository orderRepository)
        {
            _shipmentRepository = shipmentRepository;
            _orderRepository = orderRepository;
        }


        // ==========================================
        // GET ALL SHIPMENTS
        // ==========================================

        public async Task<List<ShipmentResponseDto>>
            GetAllAsync()
        {
            var shipments =
                await _shipmentRepository.GetAllAsync();

            return shipments
                .Select(MapToResponse)
                .ToList();
        }


        // ==========================================
        // GET SHIPMENT BY ID
        // ==========================================

        public async Task<ShipmentResponseDto>
            GetByIdAsync(int shipmentId)
        {
            var shipment =
                await _shipmentRepository.GetByIdAsync(
                    shipmentId);

            if (shipment == null)
            {
                throw new Exception(
                    "Shipment not found.");
            }

            return MapToResponse(shipment);
        }


        // ==========================================
        // GET SHIPMENT BY ORDER
        // ==========================================

        public async Task<ShipmentResponseDto>
            GetByOrderIdAsync(
                int orderId,
                int userId,
                bool isAdmin)
        {
            // =========================
            // CUSTOMER OWNERSHIP CHECK
            // =========================

            if (!isAdmin)
            {
                var order =
                    await _orderRepository
                        .GetByIdAsync(
                            orderId,
                            userId);

                if (order == null)
                {
                    throw new UnauthorizedAccessException(
                        "You can only view shipment of your own order.");
                }
            }

            // =========================
            // GET SHIPMENT
            // =========================

            var shipment =
                await _shipmentRepository
                    .GetByOrderIdAsync(orderId);

            if (shipment == null)
            {
                throw new Exception(
                    "Shipment not found for this order.");
            }

            return MapToResponse(shipment);
        }


        // ==========================================
        // CREATE SHIPMENT
        // ==========================================

        public async Task<ShipmentResponseDto>
            CreateAsync(ShipmentCreateDto dto)
        {
            // =========================
            // CHECK ORDER
            // =========================

            var order =
                await _orderRepository
                    .GetByIdForAdminAsync(dto.OrderId);

            if (order == null)
            {
                throw new Exception(
                    "Order not found.");
            }


            // =========================
            // CHECK EXISTING SHIPMENT
            // =========================

            var existingShipment =
                await _shipmentRepository
                    .GetByOrderIdAsync(dto.OrderId);

            if (existingShipment != null)
            {
                throw new Exception(
                    "Shipment already exists for this order.");
            }


            // =========================
            // CREATE SHIPMENT
            // =========================

            var shipment = new Shipment
            {
                OrderId = dto.OrderId,

                CourierName =
                    dto.CourierName,

                TrackingNumber =
                    dto.TrackingNumber,

                TrackingUrl =
                    dto.TrackingUrl,

                ShippingMethod =
                    string.IsNullOrWhiteSpace(
                        dto.ShippingMethod)
                    ? "Standard"
                    : dto.ShippingMethod,

                ShipmentStatus =
                    "Pending",

                StatusDescription =
                    "Shipment created successfully.",

                EstimatedDeliveryDate =
                    dto.EstimatedDeliveryDate,

                DeliveryNotes =
                    dto.DeliveryNotes,

                CreatedDate =
                    DateTime.UtcNow
            };

            await _shipmentRepository
                .AddAsync(shipment);


            // Reload with Order
            shipment =
                await _shipmentRepository
                    .GetByIdAsync(
                        shipment.ShipmentId);

            return MapToResponse(
                shipment!);
        }


        // ==========================================
        // UPDATE SHIPMENT
        // ==========================================

        public async Task<ShipmentResponseDto>
            UpdateAsync(
                int shipmentId,
                ShipmentUpdateDto dto)
        {
            var shipment =
                await _shipmentRepository
                    .GetByIdAsync(shipmentId);

            if (shipment == null)
            {
                throw new Exception(
                    "Shipment not found.");
            }


            // =========================
            // UPDATE BASIC DETAILS
            // =========================

            if (dto.CourierName != null)
            {
                shipment.CourierName =
                    dto.CourierName;
            }

            if (dto.TrackingNumber != null)
            {
                shipment.TrackingNumber =
                    dto.TrackingNumber;
            }

            if (dto.TrackingUrl != null)
            {
                shipment.TrackingUrl =
                    dto.TrackingUrl;
            }

            if (dto.ShippingMethod != null)
            {
                shipment.ShippingMethod =
                    dto.ShippingMethod;
            }

            if (dto.StatusDescription != null)
            {
                shipment.StatusDescription =
                    dto.StatusDescription;
            }

            if (dto.EstimatedDeliveryDate.HasValue)
            {
                shipment.EstimatedDeliveryDate =
                    dto.EstimatedDeliveryDate;
            }

            if (dto.DeliveredTo != null)
            {
                shipment.DeliveredTo =
                    dto.DeliveredTo;
            }

            if (dto.DeliveryNotes != null)
            {
                shipment.DeliveryNotes =
                    dto.DeliveryNotes;
            }


            // =========================
            // STATUS UPDATE
            // =========================

            if (!string.IsNullOrWhiteSpace(
                dto.ShipmentStatus))
            {
                UpdateStatus(
                    shipment,
                    dto.ShipmentStatus);
            }


            shipment.UpdatedDate =
                DateTime.UtcNow;

            await _shipmentRepository
                .UpdateAsync(shipment);

            return MapToResponse(
                shipment);
        }


        // ==========================================
        // STATUS HANDLER
        // ==========================================

        private static void UpdateStatus(
            Shipment shipment,
            string status)
        {
            var validStatuses = new[]
            {
                "Pending",
                "Processing",
                "ReadyToShip",
                "Shipped",
                "InTransit",
                "OutForDelivery",
                "Delivered",
                "Cancelled",
                "Failed",
                "Returned"
            };

            if (!validStatuses.Contains(status))
            {
                throw new Exception(
                    "Invalid shipment status.");
            }

            shipment.ShipmentStatus =
                status;


            // =========================
            // AUTOMATIC DATES
            // =========================

            switch (status)
            {
                case "Shipped":

                    shipment.ShippedDate ??=
                        DateTime.UtcNow;

                    break;


                case "OutForDelivery":

                    shipment.OutForDeliveryDate ??=
                        DateTime.UtcNow;

                    break;


                case "Delivered":

                    shipment.DeliveredDate ??=
                        DateTime.UtcNow;

                    break;
            }
        }


        // ==========================================
        // DELETE SHIPMENT
        // ==========================================

        public async Task DeleteAsync(
            int shipmentId)
        {
            var shipment =
                await _shipmentRepository
                    .GetByIdAsync(shipmentId);

            if (shipment == null)
            {
                throw new Exception(
                    "Shipment not found.");
            }

            await _shipmentRepository
                .DeleteAsync(shipmentId);
        }


        // ==========================================
        // MAP RESPONSE
        // ==========================================

        private static ShipmentResponseDto
            MapToResponse(Shipment shipment)
        {
            return new ShipmentResponseDto
            {
                ShipmentId =
                    shipment.ShipmentId,

                OrderId =
                    shipment.OrderId,

                OrderNumber =
                    shipment.Order?.OrderNumber
                    ?? string.Empty,

                CourierName =
                    shipment.CourierName,

                TrackingNumber =
                    shipment.TrackingNumber,

                TrackingUrl =
                    shipment.TrackingUrl,

                ShippingMethod =
                    shipment.ShippingMethod,

                ShipmentStatus =
                    shipment.ShipmentStatus,

                StatusDescription =
                    shipment.StatusDescription,

                ShippedDate =
                    shipment.ShippedDate,

                EstimatedDeliveryDate =
                    shipment.EstimatedDeliveryDate,

                OutForDeliveryDate =
                    shipment.OutForDeliveryDate,

                DeliveredDate =
                    shipment.DeliveredDate,

                DeliveredTo =
                    shipment.DeliveredTo,

                DeliveryNotes =
                    shipment.DeliveryNotes,

                CreatedDate =
                    shipment.CreatedDate,

                UpdatedDate =
                    shipment.UpdatedDate
            };
        }
    }
}