# Mapa de carpetas

Convención: en cada proyecto, primer nivel en inglés solo `Views`, `Core`, `Assets`, `Models` y `Services`; adentro, subcarpetas en español (PascalCase, sin acentos ni ñ) según su propósito. Los namespaces siguen a la carpeta (`COT_0.Host.Views.Componentes.Manual`, `COT_0.Web.Services.Voz`, …).

En Web, los namespaces nuevos se importan una sola vez: `<Using>` en `COT_0.Web.csproj` para el C# y `_Imports.razor` para los `.razor`.

## Movimientos

| Ruta actual | Ruta nueva | Motivo |
|---|---|---|
| `COT_0.Host/Views/Componentes/AvisoLain.xaml` | `COT_0.Host/Views/Componentes/Manual/AvisoLain.xaml` | Pieza que solo usa el manual de Lain |
| `COT_0.Host/Views/Componentes/AvisoLain.xaml.cs` | `COT_0.Host/Views/Componentes/Manual/AvisoLain.xaml.cs` | Pieza que solo usa el manual de Lain |
| `COT_0.Host/Views/Componentes/EncabezadoEjemplo.xaml` | `COT_0.Host/Views/Componentes/Manual/EncabezadoEjemplo.xaml` | Pieza que solo usa el manual de Lain |
| `COT_0.Host/Views/Componentes/EncabezadoEjemplo.xaml.cs` | `COT_0.Host/Views/Componentes/Manual/EncabezadoEjemplo.xaml.cs` | Pieza que solo usa el manual de Lain |
| `COT_0.Host/Views/Componentes/ResaltadoManual.xaml` | `COT_0.Host/Views/Componentes/Manual/ResaltadoManual.xaml` | Pieza que solo usa el manual de Lain |
| `COT_0.Host/Views/Componentes/ResaltadoManual.xaml.cs` | `COT_0.Host/Views/Componentes/Manual/ResaltadoManual.xaml.cs` | Pieza que solo usa el manual de Lain |
| `COT_0.Host/Views/Componentes/VistaEjemploEmotes.xaml` | `COT_0.Host/Views/Componentes/Manual/VistaEjemploEmotes.xaml` | Pieza que solo usa el manual de Lain |
| `COT_0.Host/Views/Componentes/VistaEjemploEmotes.xaml.cs` | `COT_0.Host/Views/Componentes/Manual/VistaEjemploEmotes.xaml.cs` | Pieza que solo usa el manual de Lain |
| `COT_0.Host/Views/Componentes/VistaEjemploInicio.xaml` | `COT_0.Host/Views/Componentes/Manual/VistaEjemploInicio.xaml` | Pieza que solo usa el manual de Lain |
| `COT_0.Host/Views/Componentes/VistaEjemploInicio.xaml.cs` | `COT_0.Host/Views/Componentes/Manual/VistaEjemploInicio.xaml.cs` | Pieza que solo usa el manual de Lain |
| `COT_0.Host/Views/Componentes/VistaEjemploLogin.xaml` | `COT_0.Host/Views/Componentes/Manual/VistaEjemploLogin.xaml` | Pieza que solo usa el manual de Lain |
| `COT_0.Host/Views/Componentes/VistaEjemploLogin.xaml.cs` | `COT_0.Host/Views/Componentes/Manual/VistaEjemploLogin.xaml.cs` | Pieza que solo usa el manual de Lain |
| `COT_0.Host/Views/Componentes/VistaEjemploManual.xaml` | `COT_0.Host/Views/Componentes/Manual/VistaEjemploManual.xaml` | Pieza que solo usa el manual de Lain |
| `COT_0.Host/Views/Componentes/VistaEjemploManual.xaml.cs` | `COT_0.Host/Views/Componentes/Manual/VistaEjemploManual.xaml.cs` | Pieza que solo usa el manual de Lain |
| `COT_0.Host/Views/Componentes/VistaEjemploMusica.xaml` | `COT_0.Host/Views/Componentes/Manual/VistaEjemploMusica.xaml` | Pieza que solo usa el manual de Lain |
| `COT_0.Host/Views/Componentes/VistaEjemploMusica.xaml.cs` | `COT_0.Host/Views/Componentes/Manual/VistaEjemploMusica.xaml.cs` | Pieza que solo usa el manual de Lain |
| `COT_0.Host/Views/Componentes/VistaEjemploSala.xaml` | `COT_0.Host/Views/Componentes/Manual/VistaEjemploSala.xaml` | Pieza que solo usa el manual de Lain |
| `COT_0.Host/Views/Componentes/VistaEjemploSala.xaml.cs` | `COT_0.Host/Views/Componentes/Manual/VistaEjemploSala.xaml.cs` | Pieza que solo usa el manual de Lain |
| `COT_0.Host/Views/Componentes/BotonManual.cs` | `COT_0.Host/Views/Componentes/Manual/BotonManual.cs` | Pieza que solo usa el manual de Lain |
| `COT_0.Host/Views/Componentes/VistaEjemplo.cs` | `COT_0.Host/Views/Componentes/Manual/VistaEjemplo.cs` | Pieza que solo usa el manual de Lain |
| `COT_0.Host/Views/Componentes/AvatarTones.cs` | `COT_0.Host/Views/Convertidores/AvatarTones.cs` | No es un control: convierte un estado en pincel o icono para x:Bind |
| `COT_0.Host/Views/Componentes/EmoteStatusBrushes.cs` | `COT_0.Host/Views/Convertidores/EmoteStatusBrushes.cs` | No es un control: convierte un estado en pincel o icono para x:Bind |
| `COT_0.Host/Views/Componentes/MusicGlyphs.cs` | `COT_0.Host/Views/Convertidores/MusicGlyphs.cs` | No es un control: convierte un estado en pincel o icono para x:Bind |
| `COT_0.Web/Pages/Chat.razor` | `COT_0.Web/Views/Pantallas/Chat.razor` | Primer nivel solo Views/Models/Services; páginas con ruta (@page) → Pantallas |
| `COT_0.Web/Pages/Error.razor` | `COT_0.Web/Views/Pantallas/Error.razor` | Primer nivel solo Views/Models/Services; páginas con ruta (@page) → Pantallas |
| `COT_0.Web/Pages/Join.razor` | `COT_0.Web/Views/Pantallas/Join.razor` | Primer nivel solo Views/Models/Services; páginas con ruta (@page) → Pantallas |
| `COT_0.Web/Pages/Manual.razor` | `COT_0.Web/Views/Pantallas/Manual.razor` | Primer nivel solo Views/Models/Services; páginas con ruta (@page) → Pantallas |
| `COT_0.Web/Components/AshLayer.razor` | `COT_0.Web/Views/Componentes/AshLayer.razor` | Components (inglés, primer nivel) → Views/Componentes |
| `COT_0.Web/Components/Backdrop.razor` | `COT_0.Web/Views/Componentes/Backdrop.razor` | Components (inglés, primer nivel) → Views/Componentes |
| `COT_0.Web/Components/ChatComposer.razor` | `COT_0.Web/Views/Componentes/ChatComposer.razor` | Components (inglés, primer nivel) → Views/Componentes |
| `COT_0.Web/Components/ChatHeader.razor` | `COT_0.Web/Views/Componentes/ChatHeader.razor` | Components (inglés, primer nivel) → Views/Componentes |
| `COT_0.Web/Components/CotLogo.razor` | `COT_0.Web/Views/Componentes/CotLogo.razor` | Components (inglés, primer nivel) → Views/Componentes |
| `COT_0.Web/Components/EmoteImage.razor` | `COT_0.Web/Views/Componentes/EmoteImage.razor` | Components (inglés, primer nivel) → Views/Componentes |
| `COT_0.Web/Components/EmoteMenu.razor` | `COT_0.Web/Views/Componentes/EmoteMenu.razor` | Components (inglés, primer nivel) → Views/Componentes |
| `COT_0.Web/Components/EmotePicker.razor` | `COT_0.Web/Views/Componentes/EmotePicker.razor` | Components (inglés, primer nivel) → Views/Componentes |
| `COT_0.Web/Components/FacultyField.razor` | `COT_0.Web/Views/Componentes/FacultyField.razor` | Components (inglés, primer nivel) → Views/Componentes |
| `COT_0.Web/Components/FacultySheet.razor` | `COT_0.Web/Views/Componentes/FacultySheet.razor` | Components (inglés, primer nivel) → Views/Componentes |
| `COT_0.Web/Components/FacultyShield.razor` | `COT_0.Web/Views/Componentes/FacultyShield.razor` | Components (inglés, primer nivel) → Views/Componentes |
| `COT_0.Web/Components/FlagIcon.razor` | `COT_0.Web/Views/Componentes/FlagIcon.razor` | Components (inglés, primer nivel) → Views/Componentes |
| `COT_0.Web/Components/Icon.razor` | `COT_0.Web/Views/Componentes/Icon.razor` | Components (inglés, primer nivel) → Views/Componentes |
| `COT_0.Web/Components/ImageBubble.razor` | `COT_0.Web/Views/Componentes/ImageBubble.razor` | Components (inglés, primer nivel) → Views/Componentes |
| `COT_0.Web/Components/ImageDraft.razor` | `COT_0.Web/Views/Componentes/ImageDraft.razor` | Components (inglés, primer nivel) → Views/Componentes |
| `COT_0.Web/Components/ImageViewer.razor` | `COT_0.Web/Views/Componentes/ImageViewer.razor` | Components (inglés, primer nivel) → Views/Componentes |
| `COT_0.Web/Components/MessageBlock.razor` | `COT_0.Web/Views/Componentes/MessageBlock.razor` | Components (inglés, primer nivel) → Views/Componentes |
| `COT_0.Web/Components/MessageList.razor` | `COT_0.Web/Views/Componentes/MessageList.razor` | Components (inglés, primer nivel) → Views/Componentes |
| `COT_0.Web/Components/MessageText.razor` | `COT_0.Web/Views/Componentes/MessageText.razor` | Components (inglés, primer nivel) → Views/Componentes |
| `COT_0.Web/Components/PhotoUpload.razor` | `COT_0.Web/Views/Componentes/PhotoUpload.razor` | Components (inglés, primer nivel) → Views/Componentes |
| `COT_0.Web/Components/ProfileEditor.razor` | `COT_0.Web/Views/Componentes/ProfileEditor.razor` | Components (inglés, primer nivel) → Views/Componentes |
| `COT_0.Web/Components/ProfileForm.razor` | `COT_0.Web/Views/Componentes/ProfileForm.razor` | Components (inglés, primer nivel) → Views/Componentes |
| `COT_0.Web/Components/UserAvatar.razor` | `COT_0.Web/Views/Componentes/UserAvatar.razor` | Components (inglés, primer nivel) → Views/Componentes |
| `COT_0.Web/Components/UserPanel.razor` | `COT_0.Web/Views/Componentes/UserPanel.razor` | Components (inglés, primer nivel) → Views/Componentes |
| `COT_0.Web/Components/VoiceBubble.razor` | `COT_0.Web/Views/Componentes/VoiceBubble.razor` | Components (inglés, primer nivel) → Views/Componentes |
| `COT_0.Web/Components/VoiceRecordingBar.razor` | `COT_0.Web/Views/Componentes/VoiceRecordingBar.razor` | Components (inglés, primer nivel) → Views/Componentes |
| `COT_0.Web/Components/Manual/DemoChat.razor` | `COT_0.Web/Views/Componentes/Manual/DemoChat.razor` | Components/Manual → Views/Componentes/Manual |
| `COT_0.Web/Components/Manual/LainTv.razor` | `COT_0.Web/Views/Componentes/Manual/LainTv.razor` | Components/Manual → Views/Componentes/Manual |
| `COT_0.Web/Components/Manual/ManualDemoView.razor` | `COT_0.Web/Views/Componentes/Manual/ManualDemoView.razor` | Components/Manual → Views/Componentes/Manual |
| `COT_0.Web/Components/Manual/ManualFinish.razor` | `COT_0.Web/Views/Componentes/Manual/ManualFinish.razor` | Components/Manual → Views/Componentes/Manual |
| `COT_0.Web/Components/Manual/ManualFx.razor` | `COT_0.Web/Views/Componentes/Manual/ManualFx.razor` | Components/Manual → Views/Componentes/Manual |
| `COT_0.Web/Components/Manual/ManualNav.razor` | `COT_0.Web/Views/Componentes/Manual/ManualNav.razor` | Components/Manual → Views/Componentes/Manual |
| `COT_0.Web/Components/Manual/ManualNotices.razor` | `COT_0.Web/Views/Componentes/Manual/ManualNotices.razor` | Components/Manual → Views/Componentes/Manual |
| `COT_0.Web/Components/Manual/ManualSignal.razor` | `COT_0.Web/Views/Componentes/Manual/ManualSignal.razor` | Components/Manual → Views/Componentes/Manual |
| `COT_0.Web/Components/Manual/ManualStory.razor` | `COT_0.Web/Views/Componentes/Manual/ManualStory.razor` | Components/Manual → Views/Componentes/Manual |
| `COT_0.Web/Components/Manual/ManualWelcome.razor` | `COT_0.Web/Views/Componentes/Manual/ManualWelcome.razor` | Components/Manual → Views/Componentes/Manual |
| `COT_0.Web/Components/Manual/MuteButton.razor` | `COT_0.Web/Views/Componentes/Manual/MuteButton.razor` | Components/Manual → Views/Componentes/Manual |
| `COT_0.Web/Layout/MainLayout.razor` | `COT_0.Web/Views/Plantillas/MainLayout.razor` | Layout (inglés, primer nivel) → Views/Plantillas |
| `COT_0.Web/Routes.razor` | `COT_0.Web/Views/Plantillas/Routes.razor` | Nada suelto en la raíz: el enrutador es parte de las vistas |
| `COT_0.Web/Hubs/ChatBroadcaster.cs` | `COT_0.Web/Services/Sala/ChatBroadcaster.cs` | Hubs (inglés, primer nivel) → Services/Sala: tiempo real de la sala |
| `COT_0.Web/Hubs/ChatEvents.cs` | `COT_0.Web/Services/Sala/ChatEvents.cs` | Hubs (inglés, primer nivel) → Services/Sala: tiempo real de la sala |
| `COT_0.Web/Hubs/ChatHub.cs` | `COT_0.Web/Services/Sala/ChatHub.cs` | Hubs (inglés, primer nivel) → Services/Sala: tiempo real de la sala |
| `COT_0.Web/Options/ChatOptions.cs` | `COT_0.Web/Services/Configuracion/ChatOptions.cs` | Options (inglés, primer nivel) → Services/Configuracion, igual que el Host |
| `COT_0.Web/Options/ManualOptions.cs` | `COT_0.Web/Services/Configuracion/ManualOptions.cs` | Options (inglés, primer nivel) → Services/Configuracion, igual que el Host |
| `COT_0.Web/Options/ManualSteps.cs` | `COT_0.Web/Services/Configuracion/ManualSteps.cs` | Options (inglés, primer nivel) → Services/Configuracion, igual que el Host |
| `COT_0.Web/Options/VoiceOptions.cs` | `COT_0.Web/Services/Configuracion/VoiceOptions.cs` | Options (inglés, primer nivel) → Services/Configuracion, igual que el Host |
| `COT_0.Web/Services/ChatRegistration.cs` | `COT_0.Web/Services/Sala/ChatRegistration.cs` | Services tenía 52 archivos sueltos → Chat en tiempo real, presencia y estado de la sala |
| `COT_0.Web/Services/ChatSession.cs` | `COT_0.Web/Services/Sala/ChatSession.cs` | Services tenía 52 archivos sueltos → Chat en tiempo real, presencia y estado de la sala |
| `COT_0.Web/Services/IChatSession.cs` | `COT_0.Web/Services/Sala/IChatSession.cs` | Services tenía 52 archivos sueltos → Chat en tiempo real, presencia y estado de la sala |
| `COT_0.Web/Services/IChatFeed.cs` | `COT_0.Web/Services/Sala/IChatFeed.cs` | Services tenía 52 archivos sueltos → Chat en tiempo real, presencia y estado de la sala |
| `COT_0.Web/Services/InMemoryChatService.cs` | `COT_0.Web/Services/Sala/InMemoryChatService.cs` | Services tenía 52 archivos sueltos → Chat en tiempo real, presencia y estado de la sala |
| `COT_0.Web/Services/PresenceTracker.cs` | `COT_0.Web/Services/Sala/PresenceTracker.cs` | Services tenía 52 archivos sueltos → Chat en tiempo real, presencia y estado de la sala |
| `COT_0.Web/Services/IPresenceTracker.cs` | `COT_0.Web/Services/Sala/IPresenceTracker.cs` | Services tenía 52 archivos sueltos → Chat en tiempo real, presencia y estado de la sala |
| `COT_0.Web/Services/RoomService.cs` | `COT_0.Web/Services/Sala/RoomService.cs` | Services tenía 52 archivos sueltos → Chat en tiempo real, presencia y estado de la sala |
| `COT_0.Web/Services/SystemNotice.cs` | `COT_0.Web/Services/Sala/SystemNotice.cs` | Services tenía 52 archivos sueltos → Chat en tiempo real, presencia y estado de la sala |
| `COT_0.Web/Services/ChatBrowser.cs` | `COT_0.Web/Services/Sala/ChatBrowser.cs` | Services tenía 52 archivos sueltos → Chat en tiempo real, presencia y estado de la sala |
| `COT_0.Web/Services/IChatBrowser.cs` | `COT_0.Web/Services/Sala/IChatBrowser.cs` | Services tenía 52 archivos sueltos → Chat en tiempo real, presencia y estado de la sala |
| `COT_0.Web/Services/MessageAuthor.cs` | `COT_0.Web/Services/Mensajes/MessageAuthor.cs` | Services tenía 52 archivos sueltos → Formato, agrupado y cuadro de escritura de mensajes |
| `COT_0.Web/Services/MessageFormatter.cs` | `COT_0.Web/Services/Mensajes/MessageFormatter.cs` | Services tenía 52 archivos sueltos → Formato, agrupado y cuadro de escritura de mensajes |
| `COT_0.Web/Services/IMessageFormatter.cs` | `COT_0.Web/Services/Mensajes/IMessageFormatter.cs` | Services tenía 52 archivos sueltos → Formato, agrupado y cuadro de escritura de mensajes |
| `COT_0.Web/Services/MessageGrouping.cs` | `COT_0.Web/Services/Mensajes/MessageGrouping.cs` | Services tenía 52 archivos sueltos → Formato, agrupado y cuadro de escritura de mensajes |
| `COT_0.Web/Services/ComposerEditor.cs` | `COT_0.Web/Services/Mensajes/ComposerEditor.cs` | Services tenía 52 archivos sueltos → Formato, agrupado y cuadro de escritura de mensajes |
| `COT_0.Web/Services/IComposerEditor.cs` | `COT_0.Web/Services/Mensajes/IComposerEditor.cs` | Services tenía 52 archivos sueltos → Formato, agrupado y cuadro de escritura de mensajes |
| `COT_0.Web/Services/AudioSniffer.cs` | `COT_0.Web/Services/Voz/AudioSniffer.cs` | Services tenía 52 archivos sueltos → Notas de voz: grabar, validar, guardar y convertir a WAV |
| `COT_0.Web/Services/IAudioTranscoder.cs` | `COT_0.Web/Services/Voz/IAudioTranscoder.cs` | Services tenía 52 archivos sueltos → Notas de voz: grabar, validar, guardar y convertir a WAV |
| `COT_0.Web/Services/OpusWavTranscoder.cs` | `COT_0.Web/Services/Voz/OpusWavTranscoder.cs` | Services tenía 52 archivos sueltos → Notas de voz: grabar, validar, guardar y convertir a WAV |
| `COT_0.Web/Services/IVoiceRecorder.cs` | `COT_0.Web/Services/Voz/IVoiceRecorder.cs` | Services tenía 52 archivos sueltos → Notas de voz: grabar, validar, guardar y convertir a WAV |
| `COT_0.Web/Services/VoiceRecorder.cs` | `COT_0.Web/Services/Voz/VoiceRecorder.cs` | Services tenía 52 archivos sueltos → Notas de voz: grabar, validar, guardar y convertir a WAV |
| `COT_0.Web/Services/IVoiceService.cs` | `COT_0.Web/Services/Voz/IVoiceService.cs` | Services tenía 52 archivos sueltos → Notas de voz: grabar, validar, guardar y convertir a WAV |
| `COT_0.Web/Services/VoiceService.cs` | `COT_0.Web/Services/Voz/VoiceService.cs` | Services tenía 52 archivos sueltos → Notas de voz: grabar, validar, guardar y convertir a WAV |
| `COT_0.Web/Services/IVoiceStore.cs` | `COT_0.Web/Services/Voz/IVoiceStore.cs` | Services tenía 52 archivos sueltos → Notas de voz: grabar, validar, guardar y convertir a WAV |
| `COT_0.Web/Services/InMemoryVoiceStore.cs` | `COT_0.Web/Services/Voz/InMemoryVoiceStore.cs` | Services tenía 52 archivos sueltos → Notas de voz: grabar, validar, guardar y convertir a WAV |
| `COT_0.Web/Services/VoiceFormat.cs` | `COT_0.Web/Services/Voz/VoiceFormat.cs` | Services tenía 52 archivos sueltos → Notas de voz: grabar, validar, guardar y convertir a WAV |
| `COT_0.Web/Services/VoiceNote.cs` | `COT_0.Web/Services/Voz/VoiceNote.cs` | Services tenía 52 archivos sueltos → Notas de voz: grabar, validar, guardar y convertir a WAV |
| `COT_0.Web/Services/WavWriter.cs` | `COT_0.Web/Services/Voz/WavWriter.cs` | Services tenía 52 archivos sueltos → Notas de voz: grabar, validar, guardar y convertir a WAV |
| `COT_0.Web/Services/WebmOpusFrames.cs` | `COT_0.Web/Services/Voz/WebmOpusFrames.cs` | Services tenía 52 archivos sueltos → Notas de voz: grabar, validar, guardar y convertir a WAV |
| `COT_0.Web/Services/IImageService.cs` | `COT_0.Web/Services/Imagenes/IImageService.cs` | Services tenía 52 archivos sueltos → Imágenes del chat: validar, achicar, guardar |
| `COT_0.Web/Services/ImageService.cs` | `COT_0.Web/Services/Imagenes/ImageService.cs` | Services tenía 52 archivos sueltos → Imágenes del chat: validar, achicar, guardar |
| `COT_0.Web/Services/IImageStore.cs` | `COT_0.Web/Services/Imagenes/IImageStore.cs` | Services tenía 52 archivos sueltos → Imágenes del chat: validar, achicar, guardar |
| `COT_0.Web/Services/InMemoryImageStore.cs` | `COT_0.Web/Services/Imagenes/InMemoryImageStore.cs` | Services tenía 52 archivos sueltos → Imágenes del chat: validar, achicar, guardar |
| `COT_0.Web/Services/IImageUploader.cs` | `COT_0.Web/Services/Imagenes/IImageUploader.cs` | Services tenía 52 archivos sueltos → Imágenes del chat: validar, achicar, guardar |
| `COT_0.Web/Services/ImageUploader.cs` | `COT_0.Web/Services/Imagenes/ImageUploader.cs` | Services tenía 52 archivos sueltos → Imágenes del chat: validar, achicar, guardar |
| `COT_0.Web/Services/ImageNote.cs` | `COT_0.Web/Services/Imagenes/ImageNote.cs` | Services tenía 52 archivos sueltos → Imágenes del chat: validar, achicar, guardar |
| `COT_0.Web/Services/ImageSniffer.cs` | `COT_0.Web/Services/Imagenes/ImageSniffer.cs` | Services tenía 52 archivos sueltos → Imágenes del chat: validar, achicar, guardar |
| `COT_0.Web/Services/IProfileService.cs` | `COT_0.Web/Services/Perfil/IProfileService.cs` | Services tenía 52 archivos sueltos → Perfil del invitado: foto, escudo de facultad y datos guardados |
| `COT_0.Web/Services/ProfileService.cs` | `COT_0.Web/Services/Perfil/ProfileService.cs` | Services tenía 52 archivos sueltos → Perfil del invitado: foto, escudo de facultad y datos guardados |
| `COT_0.Web/Services/IAvatarStore.cs` | `COT_0.Web/Services/Perfil/IAvatarStore.cs` | Services tenía 52 archivos sueltos → Perfil del invitado: foto, escudo de facultad y datos guardados |
| `COT_0.Web/Services/InMemoryAvatarStore.cs` | `COT_0.Web/Services/Perfil/InMemoryAvatarStore.cs` | Services tenía 52 archivos sueltos → Perfil del invitado: foto, escudo de facultad y datos guardados |
| `COT_0.Web/Services/IGuestStore.cs` | `COT_0.Web/Services/Perfil/IGuestStore.cs` | Services tenía 52 archivos sueltos → Perfil del invitado: foto, escudo de facultad y datos guardados |
| `COT_0.Web/Services/GuestStore.cs` | `COT_0.Web/Services/Perfil/GuestStore.cs` | Services tenía 52 archivos sueltos → Perfil del invitado: foto, escudo de facultad y datos guardados |
| `COT_0.Web/Services/FlagSearch.cs` | `COT_0.Web/Services/Perfil/FlagSearch.cs` | Services tenía 52 archivos sueltos → Perfil del invitado: foto, escudo de facultad y datos guardados |
| `COT_0.Web/Services/FlagService.cs` | `COT_0.Web/Services/Perfil/FlagService.cs` | Services tenía 52 archivos sueltos → Perfil del invitado: foto, escudo de facultad y datos guardados |
| `COT_0.Web/Services/IManualDemo.cs` | `COT_0.Web/Services/Manual/IManualDemo.cs` | Services tenía 52 archivos sueltos → Manual del invitado (/manual) |
| `COT_0.Web/Services/ManualDemo.cs` | `COT_0.Web/Services/Manual/ManualDemo.cs` | Services tenía 52 archivos sueltos → Manual del invitado (/manual) |
| `COT_0.Web/Services/IManualTour.cs` | `COT_0.Web/Services/Manual/IManualTour.cs` | Services tenía 52 archivos sueltos → Manual del invitado (/manual) |
| `COT_0.Web/Services/ManualTour.cs` | `COT_0.Web/Services/Manual/ManualTour.cs` | Services tenía 52 archivos sueltos → Manual del invitado (/manual) |
| `COT_0.Web/Services/ManualRegistration.cs` | `COT_0.Web/Services/Manual/ManualRegistration.cs` | Services tenía 52 archivos sueltos → Manual del invitado (/manual) |
| `COT_0.Web/Services/ManualTimeline.cs` | `COT_0.Web/Services/Manual/ManualTimeline.cs` | Services tenía 52 archivos sueltos → Manual del invitado (/manual) |

## Se queda como está (y por qué)

| Ruta | Motivo |
|---|---|
| `COT_0.Host/App.xaml(.cs)`, `MainWindow.xaml(.cs)`, `appsettings.json`, `COT_0.Host.csproj` | Raíz permitida |
| `COT_0.Host/app.manifest` | Configuración inevitable (DPI); el `.csproj` lo pide por ruta |
| `COT_0.Host/Models/` (15 clases) | Ya es primer nivel válido; son clases simples sin lógica, partirlas solo sumaría `using` |
| `COT_0.Host/Views/ModelosDeVista/` | Ya está en español (equivale a "VistasModelo") |
| `COT_0.Host/Services/*` (Almacenamiento, Cache, Configuracion, Dialogos, Fondos, Manual, Musica, Registro, Sala, Sesiones, Textos, Tiempo, Tunel, Usuarios, Validaciones, Ventana, Visitas) | Ya cumplen la convención |
| `COT_0.Host/Assets/Fuentes`, `Assets/Imagenes` | Ya cumplen; las rutas `ms-appx:///Assets/...` no cambian |
| `COT_0.Web/wwwroot/` | **Riesgoso**: ASP.NET sirve los archivos estáticos desde `wwwroot` por convención y el Host lo copia con ese nombre |
| `COT_0.Web/Properties/launchSettings.json` | **Riesgoso**: `dotnet run` lo busca en `Properties/` |
| `COT_0.Web/App.razor`, `_Imports.razor` | `_Imports.razor` solo aplica a su carpeta y las de abajo: tiene que estar en la raíz; `App` es el componente raíz |
| `COT_0.Web/Program.cs`, `WebApp.cs` | Punto de entrada (standalone) y armado de la app que usa el Host |
| `COT_0.Web/Models/` | Primer nivel válido (ya con una clase por archivo) |
| `COT_0.Web/Services/Emotes/` | Ya agrupado; "emote" se usa igual en español |
| `COT_0.Core/Models/`, `COT_0.Core/Services/` | Primer nivel válido, pocos archivos; los namespaces los usan Web y Host |
