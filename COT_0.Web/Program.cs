using COT_0.Core.Services;

// Modo standalone (para probar Web sin el Host): la sala arranca abierta
var app = WebApp.Create(args);
app.Services.GetRequiredService<IRoomService>().Open();
await app.RunAsync();
