using COT_0.Core.Models;
using COT_0.Core.Services;
using COT_0.Web.Models;
using COT_0.Web.Options;
using Microsoft.Extensions.Options;

namespace COT_0.Web.Services;

public sealed class ManualDemo : IManualDemo
{
    private const string ImageId = "demo-image";
    private const string VoiceId = "demo-voice";

    private readonly IRoomEmotes _emotes;
    private readonly ManualOptions _options;
    private readonly ChatOptions _chat;
    private readonly DateTime _now = DateTime.UtcNow;
    private readonly User _lain = new() { Id = "demo-lain", Name = "Lain", FacultyFlag = "ciencias-fisico-matematicas" };
    private readonly User _rei = new() { Id = "demo-rei", Name = "Rei", FacultyFlag = "facpya" };

    public ManualDemo(IRoomEmotes emotes, IOptions<ManualOptions> options, IOptions<ChatOptions> chat)
    {
        _emotes = emotes;
        _options = options.Value;
        _chat = chat.Value;
        Me = new User { Id = "demo-you", Name = "Alex", FacultyFlag = "fime" };
        Users = [_lain, _rei, Me];
        Profile = JoinForm.From(Me);
        Draft = new MessageDraft(_chat.MaxMessageLength);
    }

    public string RoomCode => _options.DemoRoomCode;
    public User Me { get; }
    public IReadOnlyList<User> Users { get; }
    public JoinForm Profile { get; }
    public MessageDraft Draft { get; }

    // Los emotes salen del set de la sala (si 7TV no responde, los mensajes quedan en texto)
    public RoomEmote? SampleEmote => _emotes.Active.FirstOrDefault();

    public IReadOnlyList<MessageGroup> Groups =>
        MessageGrouping.Group(Messages(), Me.Id, TimeSpan.FromMinutes(_chat.GroupWindowMinutes));

    public VoicePlayback? FindVoice(ChatMessage message) =>
        message.Id == VoiceId ? new VoicePlayback(_options.SoundUrl, TimeSpan.FromSeconds(_options.DemoVoiceSeconds), true) : null;

    public ImageView? FindImage(ChatMessage message) =>
        message.Id == ImageId ? new ImageView(_options.DemoImage, _chat.ImageThumbWidth, _options.DemoImageThumbHeight, true) : null;

    private List<ChatMessage> Messages()
    {
        var emotes = _emotes.Active.Take(2).Select(e => e.Name).ToList();
        var joined = SystemNotice.Create(MessageKind.Joined, _chat.JoinedNotice, _rei);
        joined.Id = "demo-joined";
        joined.SentAt = _now.AddMinutes(-9);

        List<ChatMessage> messages =
        [
            joined,
            Message("demo-1", _lain, "¿ya están todos?", -8),
            Message("demo-2", _lain, "hoy exponemos el proyecto de C#", -8),
            Message(ImageId, _rei, string.Empty, -6),
            Message("demo-3", Me, "aquí ando", -4),
            Message(VoiceId, Me, string.Empty, -4),
            Message("demo-4", _rei, string.Join(' ', emotes.Take(1).Prepend("jajaja")), -1),
        ];
        if (emotes.Count > 1) messages.Add(Message("demo-5", _rei, emotes[1], -1));
        return messages;
    }

    private ChatMessage Message(string id, User author, string text, int minutes) => new()
    {
        Id = id,
        UserId = author.Id,
        UserName = author.Name,
        UserFacultyFlag = author.FacultyFlag,
        Text = text,
        SentAt = _now.AddMinutes(minutes),
    };
}
