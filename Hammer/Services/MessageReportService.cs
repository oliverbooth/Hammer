﻿using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using BrackeysBot.API.Extensions;
using DisCatSharp;
using DisCatSharp.Entities;
using Hammer.Configuration;
using Hammer.Data;
using Hammer.Extensions;
using Hammer.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NLog;
using SmartFormat;

namespace Hammer.Services;

/// <summary>
///     Represents a service which handles message reports from users.
/// </summary>
internal sealed class MessageReportService : BackgroundService
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    private readonly MessageTrackingService _messageTrackingService;
    private readonly ConfigurationService _configurationService;
    private readonly DiscordLogService _logService;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly List<ReportedMessage> _reportedMessages = new();

    /// <summary>
    ///     Initializes a new instance of the <see cref="MessageReportService" /> class.
    /// </summary>
    public MessageReportService(IServiceScopeFactory scopeFactory, ConfigurationService configurationService,
        DiscordLogService logService, MessageTrackingService messageTrackingService)
    {
        _scopeFactory = scopeFactory;
        _configurationService = configurationService;
        _logService = logService;
        _messageTrackingService = messageTrackingService;
    }

    /// <summary>
    ///     Creates a new message report.
    /// </summary>
    /// <param name="message">The message to report.</param>
    /// <param name="reporter">The user issuing the report.</param>
    /// <returns>The reported message.</returns>
    public async Task<ReportedMessage> CreateNewMessageReportAsync(DiscordMessage message, DiscordMember reporter)
    {
        TrackedMessage trackedMessage = await _messageTrackingService.GetTrackedMessageAsync(message);
        await using AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
        await using var context = scope.ServiceProvider.GetRequiredService<HammerContext>();

        EntityEntry<ReportedMessage> entry = await context.AddAsync(new ReportedMessage
        {
            MessageId = trackedMessage.Id,
            ReporterId = reporter.Id
        });

        await context.SaveChangesAsync();
        ReportedMessage report = entry.Entity;
        _reportedMessages.Add(report);
        return report;
    }

    /// <summary>
    ///     Determines if a specified reporter has already
    /// </summary>
    /// <param name="message">The reported message.</param>
    /// <param name="reporter">The user issuing the report.</param>
    /// <returns>
    ///     <see langword="true" /> if <paramref name="reporter" /> has already reported <paramref name="message" />; otherwise,
    ///     <see langword="false" />.</returns>
    public bool HasUserReportedMessage(DiscordMessage message, DiscordMember reporter)
    {
        return _reportedMessages.Exists(m => m.MessageId == message.Id && m.ReporterId == reporter.Id);
    }

    /// <summary>
    ///     Reports a message to staff members.
    /// </summary>
    /// <param name="message">The message to report.</param>
    /// <param name="reporter">The member who reported the message.</param>
    public async Task ReportMessageAsync(DiscordMessage message, DiscordMember reporter)
    {
        MessageTrackState trackState = _messageTrackingService.GetMessageTrackState(message);
        if ((trackState & MessageTrackState.Deleted) != 0)
        {
            Logger.Warn(LoggerMessages.UserReportedDeletedMessage.FormatSmart(new {reporter, message}));

            // we can stop tracking reports for a message which is deleted
            _reportedMessages.RemoveAll(m => m.Message == message);
            return;
        }

        bool duplicateReport = HasUserReportedMessage(message, reporter);
        if (duplicateReport)
        {
            Logger.Info(LogMessages.DuplicateMessageReport.FormatSmart(new {user = reporter, message}));
            return;
        }

        Logger.Info(LogMessages.MessageReported.FormatSmart(new {user = reporter, message}));
        await CreateNewMessageReportAsync(message, reporter);
        await reporter.SendMessageAsync(CreateUserReportEmbed(message, reporter));
        await _logService.LogAsync(reporter.Guild, CreateStaffReportEmbed(message, reporter), StaffNotificationOptions.Here);
    }

    /// <inheritdoc />
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await using AsyncServiceScope scope = _scopeFactory.CreateAsyncScope();
        await using var context = scope.ServiceProvider.GetRequiredService<HammerContext>();
        await context.Database.EnsureCreatedAsync(stoppingToken);

        _reportedMessages.Clear();
        _reportedMessages.AddRange(context.ReportedMessages.Include(m => m.Message).Where(m => !m.Message.IsDeleted));
    }

    private DiscordEmbed CreateStaffReportEmbed(DiscordMessage message, DiscordMember reporter)
    {
        GuildConfiguration guildConfiguration = _configurationService.GetGuildConfiguration(reporter.Guild);

        bool hasContent = !string.IsNullOrWhiteSpace(message.Content);
        bool hasAttachments = message.Attachments.Count > 0;

        string? content = hasContent ? Formatter.BlockCode(Formatter.Sanitize(message.Content)) : null;
        string? attachments = hasAttachments ? string.Join('\n', message.Attachments.Select(a => a.Url)) : null;

        return reporter.Guild.CreateDefaultEmbed()
            .WithColor(guildConfiguration.TertiaryColor)
            .WithTitle(EmbedTitles.MessageReported)
            .WithDescription(EmbedMessages.MessageReported.FormatSmart(new {user = reporter, channel = message.Channel}))
            .AddField(EmbedFieldNames.Channel, message.Channel.Mention, true)
            .AddField(EmbedFieldNames.Author, message.Author.Mention, true)
            .AddField(EmbedFieldNames.Reporter, reporter.Mention, true)
            .AddField(EmbedFieldNames.MessageID, Formatter.MaskedUrl(message.Id.ToString(), message.JumpLink), true)
            .AddField(EmbedFieldNames.MessageTime, Formatter.Timestamp(message.CreationTimestamp, TimestampFormat.ShortDateTime),
                true)
            .AddFieldIf(hasContent, EmbedFieldNames.Content, content)
            .AddFieldIf(hasAttachments, EmbedFieldNames.Attachments, attachments);
    }

    private DiscordEmbed CreateUserReportEmbed(DiscordMessage message, DiscordMember member)
    {
        GuildConfiguration guildConfiguration = _configurationService.GetGuildConfiguration(member.Guild);

        bool hasContent = !string.IsNullOrWhiteSpace(message.Content);
        bool hasAttachments = message.Attachments.Count > 0;

        string? content = hasContent ? Formatter.BlockCode(Formatter.Sanitize(message.Content)) : null;
        string? attachments = hasAttachments ? string.Join('\n', message.Attachments.Select(a => a.Url)) : null;

        return member.Guild.CreateDefaultEmbed()
            .WithColor(guildConfiguration.TertiaryColor)
            .WithTitle(EmbedTitles.MessageReported)
            .WithDescription(EmbedMessages.MessageReportFeedback.FormatSmart(new {user = member}))
            .AddFieldIf(hasContent, EmbedFieldNames.Content, content)
            .AddFieldIf(hasAttachments, EmbedFieldNames.Attachments, attachments);
    }
}