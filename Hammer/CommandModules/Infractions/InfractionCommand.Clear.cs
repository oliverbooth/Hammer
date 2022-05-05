using System.Collections.Generic;
using System.Threading.Tasks;
using BrackeysBot.API.Extensions;
using DSharpPlus.Entities;
using DSharpPlus.SlashCommands;
using DSharpPlus.SlashCommands.Attributes;
using Hammer.Data;
using Humanizer;

namespace Hammer.CommandModules.Infractions;

internal sealed partial class InfractionCommand
{
    [SlashCommand("clear", "Clears all infractions from the specified user.", false)]
    [SlashRequireGuild]
    public async Task ClearAsync(InteractionContext context,
        [Option("user", "The user whose infractions to clear")] DiscordUser user)
    {
        await context.DeferAsync(true).ConfigureAwait(false);

        IReadOnlyList<Infraction> infractions = _infractionService.GetInfractions(user, context.Guild);
        await _infractionService.RemoveInfractionsAsync(infractions).ConfigureAwait(false);
        int infractionCount = _infractionService.GetInfractionCount(user, context.Guild);

        var embed = new DiscordEmbedBuilder();
        embed.WithAuthor(user);
        embed.WithColor(DiscordColor.Green);
        embed.WithTitle("Infractions cleared");
        embed.WithDescription($"Cleared {"infraction".ToQuantity(infractions.Count - infractionCount)} infractions " +
                              $"for {user.Mention}.");

        var builder = new DiscordWebhookBuilder();
        builder.AddEmbed(embed);
        await context.EditResponseAsync(builder).ConfigureAwait(false);
    }
}