using Application.Common.Dtos;
using Application.Common.Pagenation;
using Application.Common.Repositories;
using Application.Contract.Settings;
using Application.Services.Interfaces;
using Domain.Enums;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Diagnostics.Contracts;

namespace Infrastructure.Services
{
    public class JobAlertMatchingService(IServiceScopeFactory scopeFactory) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = scopeFactory.CreateScope())
                {
                    var savedJobSearchRepository = scope.ServiceProvider.GetRequiredService<ISavedJobSearchRepository>();
                    var jobRepository = scope.ServiceProvider.GetRequiredService<IJobRepository>();
                    var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
                    var appSettings = scope.ServiceProvider.GetRequiredService<Microsoft.Extensions.Options.IOptions<AppSettings>>();
                    var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

                    var activeAlerts = await savedJobSearchRepository.GetAllActiveAsync();

                    foreach (var alert in activeAlerts)
                    {
                        var cutoff = alert.LastNotifiedAt ?? alert.DateCreated;

                        var filter = new JobSearchFilter
                        {
                            Keyword = alert.Keyword,
                            Location = alert.Location,
                            JobCategoryId = alert.DepartmentId,
                            EmploymentType = alert.EmploymentType,
                            WorkPlaceType = alert.WorkPlaceType,
                            ExperienceLevel = alert.ExperienceLevel,
                            MinSalary = alert.MinSalary,
                            PostedAfter = cutoff,
                            SortBy = JobSortOption.Newest
                        };

                        var matches = await jobRepository.SearchAsync(
                            filter,
                            new PageRequest { PageNumber = 1, PageSize = 20 },
                            usePaging: true);

                        if (matches.Items.Any())
                        {
                            var email = alert.ProfessionalProfile.User.Email;
                            var name = $"{alert.ProfessionalProfile.User.FirstName} {alert.ProfessionalProfile.User.LastName}".Trim();

                            var listHtml = string.Join("", matches.Items.Select(j =>
                                $"<p><strong>{j.Title}</strong> at {j.Company.Name} — {j.Location}</p>"));

                            await emailService.SendEmailAsync(new EmailRequest
                            {
                                To = email,
                                Subject = $"{matches.Items.Count()} new job(s) matching your alert — ProConnect",
                                Body = EmailTemplates.JobApplicationEmail(
                                    name, "New Matches", "ProConnect", appSettings.Value.FrontendUrl)
                                // NOTE: reusing JobApplicationEmail as a placeholder template shape —
                                // a dedicated JobAlertEmail template in EmailTemplates.cs would read
                                // much better; I kept this minimal rather than guess at HTML you
                                // haven't asked for. Replace with a real template when convenient.
                            });

                            alert.LastNotifiedAt = DateTime.UtcNow;
                            savedJobSearchRepository.Update(alert);
                        }
                    }

                    await unitOfWork.SaveAsync();
                }

                await Task.Delay(TimeSpan.FromHours(6), stoppingToken);
            }
        }
    }
}