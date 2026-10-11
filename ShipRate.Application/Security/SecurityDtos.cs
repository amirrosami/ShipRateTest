namespace ShipRate.Application.Security
{
    public sealed class MalwareScanResult
    {
        public bool IsClean { get; init; }
        public bool IsMalicious => !IsClean;
        public string? ThreatName { get; init; }
        public string? Message { get; init; }

        public static MalwareScanResult Clean()
        {
            return new MalwareScanResult
            {
                IsClean = true
            };
        }

        public static MalwareScanResult Infected(string? threatName, string? message = null)
        {
            return new MalwareScanResult
            {
                IsClean = false,
                ThreatName = threatName,
                Message = message
            };
        }
        public sealed class AttachmentSecurityCheckResult
        {
            public bool IsAllowed { get; init; }

            public string? RejectionReason { get; init; }

            public string SafeOriginalFileName { get; init; } = string.Empty;

            public string Extension { get; init; } = string.Empty;

            public string DetectedFileType { get; init; } = string.Empty;

            public string ContentType { get; init; } = "application/octet-stream";

            public static AttachmentSecurityCheckResult Allowed(
                string safeOriginalFileName,
                string extension,
                string detectedFileType,
                string contentType)
            {
                return new AttachmentSecurityCheckResult
                {
                    IsAllowed = true,
                    SafeOriginalFileName = safeOriginalFileName,
                    Extension = extension,
                    DetectedFileType = detectedFileType,
                    ContentType = contentType
                };
            }

            public static AttachmentSecurityCheckResult Rejected(string reason)
            {
                return new AttachmentSecurityCheckResult
                {
                    IsAllowed = false,
                    RejectionReason = reason
                };
            }
        }

    }
}
