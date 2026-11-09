namespace COT_0.Web.Services.Emotes;

/// <summary>Consulta de GraphQL v3 de 7TV para buscar en todo el catálogo (verificada el 09/11 en https://7tv.io/v3/gql)</summary>
internal static class SevenTvSearchQuery
{
    public const string Text =
        "query($q:String!,$p:Int,$l:Int){emotes(query:$q,page:$p,limit:$l,"
        + "filter:{exact_match:false,case_sensitive:false,ignore_tags:true}){count items{id name animated host{url files{name static_name format width height}}}}}";

    public static object Body(string? query, int page, int pageSize) =>
        new { query = Text, variables = new { q = query?.Trim() ?? string.Empty, p = page, l = pageSize } };
}
