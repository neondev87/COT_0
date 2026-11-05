using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Media.Effects;

namespace COT_0.Host.Controls;

// Compila un shader HLSL de Assets/Shaders con el compilador que trae Windows (d3dcompiler_47); null si falla
internal static class ShaderCompiler
{
    private const string Target = "ps_2_0";

    public static PixelShader? Compile(string fileName)
    {
        try
        {
            var uri = new Uri($"pack://application:,,,/COT_0.Host;component/Assets/Shaders/{fileName}");
            using var reader = new StreamReader(Application.GetResourceStream(uri).Stream, Encoding.UTF8);
            var source = Encoding.UTF8.GetBytes(reader.ReadToEnd());
            if (D3DCompile(source, (nuint)source.Length, fileName, 0, 0, "main", Target, 0, 0, out var code, out _) < 0) return null;
            var bytes = new byte[(int)code.GetBufferSize()];
            Marshal.Copy(code.GetBufferPointer(), bytes, 0, bytes.Length);
            var shader = new PixelShader();
            shader.SetStreamSource(new MemoryStream(bytes));
            shader.Freeze();
            return shader;
        }
        catch (Exception exception) when (exception is DllNotFoundException or EntryPointNotFoundException or IOException)
        {
            return null;
        }
    }

    [DllImport("d3dcompiler_47.dll", CharSet = CharSet.Ansi)]
    private static extern int D3DCompile(byte[] source, nuint sourceSize, string sourceName, nint defines, nint include,
        string entryPoint, string target, uint flags1, uint flags2, out IBlob code, out IBlob? errors);

    [ComImport, Guid("8BA5FB08-5195-40e2-AC58-0D989C3A0102"), InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    private interface IBlob
    {
        [PreserveSig] nint GetBufferPointer();
        [PreserveSig] nuint GetBufferSize();
    }
}
