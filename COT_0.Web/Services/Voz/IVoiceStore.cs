
namespace COT_0.Web.Services.Voz;

// Notas de voz en memoria (Singleton)
public interface IVoiceStore
{
    void Save(VoiceClip clip);
    VoiceClip? Get(string id);
    void Clear();
}
