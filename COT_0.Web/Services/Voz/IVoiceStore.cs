namespace COT_0.Web.Services.Voz;

/// <summary>Notas de voz en memoria (Singleton)</summary>
public interface IVoiceStore
{
    void Save(VoiceClip clip);
    VoiceClip? Get(string id);
    void Clear();
}
