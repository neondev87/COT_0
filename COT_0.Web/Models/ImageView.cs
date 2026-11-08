namespace COT_0.Web.Models;

/// <summary>Lo que necesita la burbuja: URL y tamaño de la miniatura ya calculado (reserva el lugar antes de cargar)</summary>
public sealed record ImageView(string Url, int ThumbWidth, int ThumbHeight, bool Available);
