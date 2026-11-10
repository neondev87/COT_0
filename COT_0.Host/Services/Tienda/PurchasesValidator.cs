// Deja compras.json en un estado válido: sin nulos, sin usuarios sin nombre, sin emotes repetidos y con fechas en UTC
using COT_0.Host.Models;

namespace COT_0.Host.Services.Tienda;

/// <summary>Se aplica al leer y al guardar; descarta lo que no se puede usar en vez de tronar.</summary>
public static class PurchasesValidator
{
    public static PurchasesData Clean(PurchasesData data)
    {
        data.Users = (data.Users ?? [])
            .Where(u => u is not null && !string.IsNullOrWhiteSpace(u.UserName))
            .DistinctBy(u => u.UserName, StringComparer.OrdinalIgnoreCase)
            .ToList();

        foreach (var user in data.Users) Clean(user);

        var highest = data.Users.SelectMany(u => u.Tickets).Select(t => t.Number).DefaultIfEmpty(0).Max();
        data.LastTicketNumber = Math.Max(Math.Max(data.LastTicketNumber, 0), highest);
        return data;
    }

    private static void Clean(UserPurchases user)
    {
        user.Emotes = CleanEmotes(user.Emotes);
        user.Tickets = (user.Tickets ?? []).Where(t => t is not null && t.Number > 0).ToList();
        foreach (var ticket in user.Tickets)
        {
            ticket.Emotes = CleanEmotes(ticket.Emotes);
            ticket.CreatedAt = AsUtc(ticket.CreatedAt);
            ticket.ResetsAt = AsUtc(ticket.ResetsAt);
        }
        if (user.WindowStart is { } start) user.WindowStart = AsUtc(start);
        if (user.LimitReachedAt is { } reached) user.LimitReachedAt = AsUtc(reached);
    }

    private static List<PurchasedEmote> CleanEmotes(List<PurchasedEmote>? emotes)
    {
        var valid = (emotes ?? [])
            .Where(e => e is not null && !string.IsNullOrWhiteSpace(e.Id) && !string.IsNullOrWhiteSpace(e.Name)
                && Uri.IsWellFormedUriString(e.Url, UriKind.Absolute))
            .DistinctBy(e => e.Id)
            .ToList();
        foreach (var emote in valid) emote.PurchasedAt = AsUtc(emote.PurchasedAt);
        return valid;
    }

    // Una fecha editada a mano sin "Z" se toma como UTC
    private static DateTime AsUtc(DateTime value) =>
        value.Kind == DateTimeKind.Utc ? value : DateTime.SpecifyKind(value, DateTimeKind.Utc);
}
