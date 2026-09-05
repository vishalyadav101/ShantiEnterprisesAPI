using ShantiEnterprises.API.DTOs.Shipment;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Services
{
    public class ShipmentService : IShipmentService
    {
        private readonly IShipmentRepository _shipmentRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly INotificationService _notificationService;

        public ShipmentService(
            IShipmentRepository shipmentRepository,
            IOrderRepository orderRepository,
            INotificationService notificationService)
        {
            _shipmentRepository = shipmentRepository;
            _orderRepository = orderRepository;
            _notificationService = notificationService;
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
            GetByIdAsync(
                int shipmentId)
        {
            var shipment =
                await _shipmentRepository.GetByIdAsync(
                    shipmentId);

            if (shipment == null)
            {
                throw new Exception(
                    "Shipment not found.");
            }

            return MapToResponse(
                shipment);
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
                    .GetByOrderIdAsync(
                        orderId);

            if (shipment == null)
            {
                throw new Exception(
                    "Shipment not found for this order.");
            }

            return MapToResponse(
                shipment);
        }


        // ==========================================
        // CREATE SHIPMENT
        // ==========================================

        public async Task<ShipmentResponseDto>
            CreateAsync(
                ShipmentCreateDto dto)
        {
            // =========================
            // CHECK ORDER
            // =========================

            var order =
                await _orderRepository
                    .GetByIdForAdminAsync(
                        dto.OrderId);

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
                    .GetByOrderIdAsync(
                        dto.OrderId);

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
                OrderId =
                    dto.OrderId,

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
                .AddAsync(
                    shipment);


            // =========================
            // RELOAD WITH ORDER
            // =========================

            shipment =
                await _shipmentRepository
                    .GetByIdAsync(
                        shipment.ShipmentId);


            // =========================
            // CREATE CUSTOMER NOTIFICATION
            // =========================

            if (shipment?.Order != null)
            {
                await CreateShipmentNotificationAsync(
                    shipment,
                    "Created");
            }


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
                    .GetByIdAsync(
                        shipmentId);

            if (shipment == null)
            {
                throw new Exception(
                    "Shipment not found.");
            }


            // =========================
            // KEEP OLD STATUS
            // =========================

            var oldStatus =
                shipment.ShipmentStatus;


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
                .UpdateAsync(
                    shipment);


            // =========================
            // CREATE STATUS NOTIFICATION
            // ONLY WHEN STATUS CHANGES
            // =========================

            if (!string.Equals(
                    oldStatus,
                    shipment.ShipmentStatus,
                    StringComparison.OrdinalIgnoreCase))
            {
                await CreateShipmentNotificationAsync(
                    shipment,
                    shipment.ShipmentStatus);
            }


            // =========================
            // RELOAD
            // =========================

            var updatedShipment =
                await _shipmentRepository
                    .GetByIdAsync(
                        shipmentId);


            return MapToResponse(
                updatedShipment!);
        }


        // ==========================================
        // STATUS HANDLER
        // ==========================================

        private static void UpdateStatus(
            Shipment shipment,
            string status)
        {
            var normalizedStatus =
                status.Trim();

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


            var matchedStatus =
                validStatuses.FirstOrDefault(
                    x => x.Equals(
                        normalizedStatus,
                        StringComparison.OrdinalIgnoreCase));


            if (matchedStatus == null)
            {
                throw new Exception(
                    "Invalid shipment status.");
            }


            shipment.ShipmentStatus =
                matchedStatus;


            // =========================
            // AUTOMATIC DATES
            // =========================

            switch (matchedStatus)
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
        // CREATE SHIPMENT NOTIFICATION
        // ==========================================

        private async Task
            CreateShipmentNotificationAsync(
                Shipment shipment,
                string eventStatus)
        {
            if (shipment.Order == null)
            {
                return;
            }


            var userId =
                shipment.Order.UserId;


            string title;

            string message;


            switch (eventStatus.ToLower())
            {
                case "created":

                    title =
                        "Shipment Created";

                    message =
                        $"Your order {shipment.Order.OrderNumber} has been prepared for shipment.";

                    break;


                case "shipped":

                    title =
                        "Order Shipped";

                    message =
                        $"Your order {shipment.Order.OrderNumber} has been shipped successfully.";

                    break;


                case "intransit":

                    title =
                        "Order In Transit";

                    message =
                        $"Your order {shipment.Order.OrderNumber} is currently in transit.";

                    break;


                case "outfordelivery":

                    title =
                        "Out for Delivery";

                    message =
                        $"Your order {shipment.Order.OrderNumber} is out for delivery.";

                    break;


                case "delivered":

                    title =
                        "Order Delivered";

                    message =
                        $"Your order {shipment.Order.OrderNumber} has been delivered successfully.";

                    break;


                case "cancelled":

                    title =
                        "Shipment Cancelled";

                    message =
                        $"Shipment for your order {shipment.Order.OrderNumber} has been cancelled.";

                    break;


                case "failed":

                    title =
                        "Shipment Failed";

                    message =
                        $"There was an issue with the shipment of your order {shipment.Order.OrderNumber}.";

                    break;


                case "returned":

                    title =
                        "Shipment Returned";

                    message =
                        $"Shipment for your order {shipment.Order.OrderNumber} has been returned.";

                    break;


                case "processing":

                    title =
                        "Shipment Processing";

                    message =
                        $"Shipment for your order {shipment.Order.OrderNumber} is being processed.";

                    break;


                case "readytoship":

                    title =
                        "Order Ready to Ship";

                    message =
                        $"Your order {shipment.Order.OrderNumber} is ready to ship.";

                    break;


                default:

                    title =
                        "Shipment Status Updated";

                    message =
                        $"Shipment status for your order {shipment.Order.OrderNumber} has been updated to {shipment.ShipmentStatus}.";

                    break;
            }


            await _notificationService.CreateAsync(
                userId,
                new DTOs.Notification.CreateNotificationDto
                {
                    Title =
                        title,

                    Message =
                        message,

                    Type =
                        "Shipment",

                    ReferenceType =
                        "Order",

                    ReferenceId =
                        shipment.OrderId
                });
        }


        // ==========================================
        // DELETE SHIPMENT
        // ==========================================

        public async Task DeleteAsync(
            int shipmentId)
        {
            var shipment =
                await _shipmentRepository
                    .GetByIdAsync(
                        shipmentId);

            if (shipment == null)
            {
                throw new Exception(
                    "Shipment not found.");
            }


            await _shipmentRepository
                .DeleteAsync(
                    shipmentId);
        }


        // ==========================================
        // MAP RESPONSE
        // ==========================================

        private static ShipmentResponseDto
            MapToResponse(
                Shipment shipment)
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