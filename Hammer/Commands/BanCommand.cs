﻿using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using Hammer.AutocompleteProviders;
using Hammer.Data;
using Hammer.Services;
using Humanizer;
using NLog;
using X10D.DSharpPlus;
using X10D.Text;
using X10D.Time;
using ILogger = NLog.ILogger;

namespace Hammer.Commands;

/// <summary>
///     Represents a module which implements the <c>ban</c> command.
/// </summary>
internal sealed class BanCommand : ApplicationCommandModule
{
    private static readonly ILogger Logger = LogManager.GetCurrentClassLogger();
    private readonly BanService _banService;
    private readonly InfractionCooldownService _cooldownService;
    private readonly InfractionService _infractionService;
    private readonly RuleService _ruleService;

    /// <summary>
    ///     Initializes a new instance of the <see cref="BanCommand" /> class.
    /// </summary>
    /// <param name="banService">The ban service.</param>
    /// <param name="cooldownService">The cooldown service.</param>
    /// <param name="infractionService">The infraction service.</param>
    /// <param name="ruleService">The rule service.</param>
    public BanCommand(
        BanService banService,
        InfractionCooldownService cooldownService,
        InfractionService infractionService,
        RuleService ruleService
    )
    {
        _banService = banService;
        _cooldownService = cooldownService;
        _infractionService = infractionService;
        _ruleService = ruleService;
    }

    [SlashCommand("ban", "Temporarily or permanently bans a user.", false)]
    [SlashRequireGuild]
    public async Task BanAsync(InteractionContext context,
        [Option("user", "The user to ban.")] DiscordUser user,
        [Option("reason", "The reason for the ban.")] string? reason = null,
        [Option("duration", "The duration of the ban.")] string? durationRaw = null,
        [Option("rule", "The rule which was broken."), Autocomplete(typeof(RuleAutocompleteProvider))] long? ruleBroken = null)
    {
        await context.DeferAsync(true).ConfigureAwait(false);

        if (_cooldownService.IsCooldownActive(user, context.Member) &&
            _cooldownService.TryGetInfraction(user, out Infraction? infraction))
        {
            Logger.Info($"User {user.Username} is on cooldown. Prompting for confirmation");
            DiscordEmbed embed = await _infractionService.CreateInfractionEmbedAsync(infraction).ConfigureAwait(false);
            bool result = await _cooldownService.ShowConfirmationAsync(context, user, infraction, embed).ConfigureAwait(false);
            if (!result) return;
        }

        TimeSpan? duration = durationRaw?.ToTimeSpan() ?? null;
        var builder = new DiscordEmbedBuilder();

        Rule? rule = null;
        if (ruleBroken.HasValue)
            rule = _ruleService.GetRuleById(context.Guild, (int) ruleBroken.Value);

        Task<Infraction> infractionTask = duration is null
            ? _banService.BanAsync(user, context.Member!, reason, rule)
            : _banService.TemporaryBanAsync(user, context.Member!, reason, duration.Value, rule);
        try
        {
            infraction = await infractionTask.ConfigureAwait(false);

            builder.WithAuthor(user);
            builder.WithColor(DiscordColor.Red);
            builder.WithTitle("Banned user");
            builder.WithDescription(reason);
            builder.WithFooter($"Infraction {infraction.Id} \u2022 User {user.Id}");
            reason = reason.WithWhiteSpaceAlternative("None");

            if (duration is null)
            {
                builder.WithTitle("Banned user");
                Logger.Info($"{context.Member} banned {user}. Reason: {reason}");
            }
            else
            {
                builder.WithTitle("Temporarily banned user");
                Logger.Info($"{context.Member} temporarily banned {user} for {duration.Value.Humanize()}. Reason: {reason}");
            }
        }
        catch (Exception exception)
        {
            Logger.Error(exception, $"Could not issue ban to {user}");

            builder.WithColor(DiscordColor.Red);
            builder.WithTitle("⚠️ Error issuing ban");
            builder.WithDescription($"{exception.GetType().Name} was thrown while issuing the ban.");
            builder.WithFooter("See log for further details.");
        }

        var message = new DiscordWebhookBuilder();
        message.AddEmbed(builder);
        await context.EditResponseAsync(message).ConfigureAwait(false);
    }
}