using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using ShantiEnterprises.API.DTOs.WebsiteSetting;
using ShantiEnterprises.API.Interfaces;
using ShantiEnterprises.API.Models;

namespace ShantiEnterprises.API.Services
{
    public class WebsiteSettingService
        : IWebsiteSettingService
    {
        private readonly IWebsiteSettingRepository _repository;
        private readonly ICloudinaryService _cloudinaryService;

        private readonly string[] _allowedImageExtensions =
        {
            ".jpg",
            ".jpeg",
            ".png",
            ".webp",
            ".ico"
        };

        private const long MaxFileSize = 5 * 1024 * 1024;

        private static readonly Regex HexColorRegex =
            new("^#[0-9A-Fa-f]{6}$", RegexOptions.Compiled);

        public WebsiteSettingService(
            IWebsiteSettingRepository repository,
            ICloudinaryService cloudinaryService)
        {
            _repository = repository;
            _cloudinaryService = cloudinaryService;
        }

        // =========================
        // GET SETTINGS
        // =========================

        public async Task<WebsiteSettingResponseDto?> GetAsync()
        {
            var setting = await _repository.GetAsync();

            if (setting == null)
            {
                return null;
            }

            return Map(setting);
        }

        // =========================
        // CREATE / UPDATE SETTINGS
        // =========================

        public async Task<WebsiteSettingResponseDto> SaveAsync(
            WebsiteSettingCreateUpdateDto dto)
        {
            var setting = await _repository.GetAsync();

            // =========================
            // CREATE
            // =========================

            if (setting == null)
            {
                setting = new WebsiteSetting
                {
                    CompanyName = dto.CompanyName,
                    Email = dto.Email,
                    Phone = dto.Phone,
                    WhatsAppNumber = dto.WhatsAppNumber,

                    // Business details
                    CustomerCare = dto.CustomerCare,
                    GSTIN = dto.GSTIN,
                    Proprietor = dto.Proprietor,
                    SinceYear = dto.SinceYear,

                    Address = dto.Address,
                    FacebookUrl = dto.FacebookUrl,
                    InstagramUrl = dto.InstagramUrl,
                    TwitterUrl = dto.TwitterUrl,
                    LinkedInUrl = dto.LinkedInUrl,
                    YouTubeUrl = dto.YouTubeUrl,

                    // Footer details
                    HighlightText = dto.HighlightText,
                    FooterText = dto.FooterText,

                    UpdatedDate = DateTime.UtcNow
                };

                // Navbar theme colors
                ApplyNavbarColors(setting, dto);

                // -------------------------
                // Upload Logo
                // -------------------------

                if (dto.Logo != null)
                {
                    ValidateImage(dto.Logo);

                    var logoResult =
                        await _cloudinaryService.UploadImageAsync(
                            dto.Logo,
                            "shanti-enterprises/settings/logo");

                    setting.LogoUrl = logoResult.Url;
                }

                // -------------------------
                // Upload Favicon
                // -------------------------

                if (dto.Favicon != null)
                {
                    ValidateImage(dto.Favicon);

                    var faviconResult =
                        await _cloudinaryService.UploadImageAsync(
                            dto.Favicon,
                            "shanti-enterprises/settings/favicon");

                    setting.FaviconUrl = faviconResult.Url;
                }

                var created =
                    await _repository.CreateAsync(setting);

                return Map(created);
            }

            // =========================
            // UPDATE
            // =========================

            // Navbar colors pehle validate karo, taaki galat color par kuch save na ho
            ApplyNavbarColors(setting, dto);

            setting.CompanyName = dto.CompanyName;
            setting.Email = dto.Email;
            setting.Phone = dto.Phone;
            setting.WhatsAppNumber = dto.WhatsAppNumber;

            // Business details
            setting.CustomerCare = dto.CustomerCare;
            setting.GSTIN = dto.GSTIN;
            setting.Proprietor = dto.Proprietor;
            setting.SinceYear = dto.SinceYear;

            setting.Address = dto.Address;
            setting.FacebookUrl = dto.FacebookUrl;
            setting.InstagramUrl = dto.InstagramUrl;
            setting.TwitterUrl = dto.TwitterUrl;
            setting.LinkedInUrl = dto.LinkedInUrl;
            setting.YouTubeUrl = dto.YouTubeUrl;

            // Footer details
            setting.HighlightText = dto.HighlightText;
            setting.FooterText = dto.FooterText;

            // -------------------------
            // Replace Logo
            // -------------------------

            if (dto.Logo != null)
            {
                ValidateImage(dto.Logo);

                var oldLogoUrl = setting.LogoUrl;

                var logoResult =
                    await _cloudinaryService.UploadImageAsync(
                        dto.Logo,
                        "shanti-enterprises/settings/logo");

                setting.LogoUrl = logoResult.Url;

                // Delete old Cloudinary image
                if (!string.IsNullOrWhiteSpace(oldLogoUrl))
                {
                    var oldPublicId =
                        ExtractCloudinaryPublicId(oldLogoUrl);

                    if (!string.IsNullOrWhiteSpace(oldPublicId))
                    {
                        try
                        {
                            await _cloudinaryService.DeleteImageAsync(
                                oldPublicId);
                        }
                        catch
                        {
                            // Do not fail settings update
                            // if old image deletion fails.
                        }
                    }
                }
            }

            // -------------------------
            // Replace Favicon
            // -------------------------

            if (dto.Favicon != null)
            {
                ValidateImage(dto.Favicon);

                var oldFaviconUrl = setting.FaviconUrl;

                var faviconResult =
                    await _cloudinaryService.UploadImageAsync(
                        dto.Favicon,
                        "shanti-enterprises/settings/favicon");

                setting.FaviconUrl = faviconResult.Url;

                // Delete old Cloudinary image
                if (!string.IsNullOrWhiteSpace(oldFaviconUrl))
                {
                    var oldPublicId =
                        ExtractCloudinaryPublicId(oldFaviconUrl);

                    if (!string.IsNullOrWhiteSpace(oldPublicId))
                    {
                        try
                        {
                            await _cloudinaryService.DeleteImageAsync(
                                oldPublicId);
                        }
                        catch
                        {
                            // Do not fail settings update
                            // if old image deletion fails.
                        }
                    }
                }
            }

            setting.UpdatedDate = DateTime.UtcNow;

            await _repository.UpdateAsync(setting);

            return Map(setting);
        }

        // =========================
        // NAVBAR COLORS
        // =========================

        private static void ApplyNavbarColors(
            WebsiteSetting setting,
            WebsiteSettingCreateUpdateDto dto)
        {
            var color1 = NormalizeColor(dto.NavbarColor1, "Navbar Color 1");
            var color2 = NormalizeColor(dto.NavbarColor2, "Navbar Color 2");
            var glow = NormalizeColor(dto.NavbarGlowColor, "Navbar Glow Color");
            var button = NormalizeColor(dto.NavbarButtonColor, "Navbar Button Color");

            // Jo color nahi aaya, uski purani value rahegi
            var finalColor1 = color1 ?? setting.NavbarColor1;
            var finalColor2 = color2 ?? setting.NavbarColor2;

            // Navbar ka text white hai, isliye light background allowed nahi
            if ((finalColor1 != null && Luminance(finalColor1) > 0.35) ||
                (finalColor2 != null && Luminance(finalColor2) > 0.35))
            {
                throw new ArgumentException(
                    "Navbar Color 1 and Color 2 must be dark colors so the white text stays readable.");
            }

            setting.NavbarColor1 = finalColor1;
            setting.NavbarColor2 = finalColor2;
            setting.NavbarGlowColor = glow ?? setting.NavbarGlowColor;
            setting.NavbarButtonColor = button ?? setting.NavbarButtonColor;
        }

        private static string? NormalizeColor(string? value, string label)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var color = value.Trim();

            if (!HexColorRegex.IsMatch(color))
            {
                throw new ArgumentException(
                    $"{label} must be a valid hex color like #0D6EFD.");
            }

            return color.ToUpperInvariant();
        }

        private static double Luminance(string hex)
        {
            double Channel(int index)
            {
                var value =
                    Convert.ToInt32(hex.Substring(index, 2), 16) / 255.0;

                return value <= 0.03928
                    ? value / 12.92
                    : Math.Pow((value + 0.055) / 1.055, 2.4);
            }

            return 0.2126 * Channel(1)
                 + 0.7152 * Channel(3)
                 + 0.0722 * Channel(5);
        }

        // =========================
        // IMAGE VALIDATION
        // =========================

        private void ValidateImage(IFormFile file)
        {
            if (file.Length == 0)
            {
                throw new ArgumentException(
                    "Uploaded image is empty.");
            }

            if (file.Length > MaxFileSize)
            {
                throw new ArgumentException(
                    "Image size must be 5 MB or less.");
            }

            var extension =
                Path.GetExtension(file.FileName)
                    .ToLowerInvariant();

            if (!_allowedImageExtensions.Contains(extension))
            {
                throw new ArgumentException(
                    "Only JPG, JPEG, PNG, WEBP or ICO images are allowed.");
            }
        }

        // =========================
        // CLOUDINARY PUBLIC ID
        // =========================

        private static string ExtractCloudinaryPublicId(
            string imageUrl)
        {
            try
            {
                if (!Uri.TryCreate(
                    imageUrl,
                    UriKind.Absolute,
                    out var uri))
                {
                    return string.Empty;
                }

                var path = uri.AbsolutePath;

                const string uploadMarker = "/upload/";

                var uploadIndex =
                    path.IndexOf(
                        uploadMarker,
                        StringComparison.OrdinalIgnoreCase);

                if (uploadIndex < 0)
                {
                    return string.Empty;
                }

                var publicPath =
                    path.Substring(
                        uploadIndex + uploadMarker.Length);

                var segments =
                    publicPath.Split(
                        '/',
                        StringSplitOptions.RemoveEmptyEntries)
                    .ToList();

                if (segments.Count == 0)
                {
                    return string.Empty;
                }

                // Remove Cloudinary version segment
                // Example: v1234567890
                if (segments[0].StartsWith("v") &&
                    segments[0].Length > 1 &&
                    long.TryParse(
                        segments[0].Substring(1),
                        out _))
                {
                    segments.RemoveAt(0);
                }

                if (segments.Count == 0)
                {
                    return string.Empty;
                }

                var fileName =
                    segments[^1];

                var extension =
                    Path.GetExtension(fileName);

                if (!string.IsNullOrWhiteSpace(extension))
                {
                    segments[^1] =
                        Path.GetFileNameWithoutExtension(
                            fileName);
                }

                return string.Join("/", segments);
            }
            catch
            {
                return string.Empty;
            }
        }

        // =========================
        // MAP
        // =========================

        private static WebsiteSettingResponseDto Map(
            WebsiteSetting setting)
        {
            return new WebsiteSettingResponseDto
            {
                WebsiteSettingId =
                    setting.WebsiteSettingId,

                CompanyName =
                    setting.CompanyName,

                LogoUrl =
                    setting.LogoUrl,

                FaviconUrl =
                    setting.FaviconUrl,

                Email =
                    setting.Email,

                Phone =
                    setting.Phone,

                WhatsAppNumber =
                    setting.WhatsAppNumber,

                // Business details
                CustomerCare =
                    setting.CustomerCare,

                GSTIN =
                    setting.GSTIN,

                Proprietor =
                    setting.Proprietor,

                SinceYear =
                    setting.SinceYear,

                Address =
                    setting.Address,

                FacebookUrl =
                    setting.FacebookUrl,

                InstagramUrl =
                    setting.InstagramUrl,

                TwitterUrl =
                    setting.TwitterUrl,

                LinkedInUrl =
                    setting.LinkedInUrl,

                YouTubeUrl =
                    setting.YouTubeUrl,

                // Footer details
                HighlightText =
                    setting.HighlightText,

                FooterText =
                    setting.FooterText,

                // Navbar theme colors
                NavbarColor1 =
                    setting.NavbarColor1,

                NavbarColor2 =
                    setting.NavbarColor2,

                NavbarGlowColor =
                    setting.NavbarGlowColor,

                NavbarButtonColor =
                    setting.NavbarButtonColor,

                UpdatedDate =
                    setting.UpdatedDate
            };
        }
    }
}