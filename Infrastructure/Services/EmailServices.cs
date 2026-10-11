using Application.Common.Dtos;
using Application.Contract.Settings;
using Application.Services.Interfaces;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Json;

namespace Infrastructure.Services
{
    public class EmailService(
        HttpClient httpClient,
        IOptions<EmailSettings> options,
        ILogger<EmailService> logger)
        : IEmailService
    {
        private readonly EmailSettings settings = options.Value;

        private const string BrevoEndpoint = "https://api.brevo.com/v3/smtp/email";

        public async Task<Result<string>> SendEmailAsync(EmailRequest request)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(settings.ApiKey) ||
                    string.IsNullOrWhiteSpace(settings.From))
                {
                    logger.LogError("EmailSettings ApiKey or From is not configured.");
                    return Result<string>.Failure("Email failed: email service is not configured.");
                }

                var payload = new
                {
                    sender = new { name = settings.DisplayName, email = settings.From },
                    to = new[] { new { email = request.To } },
                    subject = request.Subject,
                    htmlContent = request.Body
                };

                using var message = new HttpRequestMessage(HttpMethod.Post, BrevoEndpoint)
                {
                    Content = JsonContent.Create(payload)
                };
                message.Headers.Add("api-key", settings.ApiKey);
                message.Headers.Add("accept", "application/json");

                using var response = await httpClient.SendAsync(message);

                if (!response.IsSuccessStatusCode)
                {
                    var error = await response.Content.ReadAsStringAsync();
                    logger.LogError(
                        "Brevo email API returned {StatusCode}: {Body}",
                        (int)response.StatusCode,
                        error);

                    return Result<string>.Failure(
                        $"Email failed: provider returned {(int)response.StatusCode}.");
                }

                return Result<string>.Success("Email sent successfully", "sent");
            }
            catch (TaskCanceledException)
            {
                logger.LogError("Email request to Brevo timed out.");
                return Result<string>.Failure("Email failed: the request timed out.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Email sending failed.");
                return Result<string>.Failure($"Email failed: {ex.Message}");
            }
        }
    }
}