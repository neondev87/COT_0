using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Microsoft.Win32.SafeHandles;

namespace COT_0.Host.Services;

// Job de Windows: al morir COT_0 (aunque sea de golpe) el sistema cierra el handle y mata los procesos del job
public sealed class KillOnCloseJob : IDisposable
{
    private const int ExtendedLimitInformationClass = 9;
    private const uint KillOnJobCloseFlag = 0x2000;

    private readonly SafeFileHandle _handle;

    public KillOnCloseJob()
    {
        _handle = CreateJobObject(IntPtr.Zero, null);
        if (_handle.IsInvalid) throw new Win32Exception();

        var info = new ExtendedLimitInformation { LimitFlags = KillOnJobCloseFlag };
        var size = (uint)Marshal.SizeOf<ExtendedLimitInformation>();
        if (!SetInformationJobObject(_handle, ExtendedLimitInformationClass, ref info, size))
            throw new Win32Exception();
    }

    public void Add(Process process)
    {
        if (!AssignProcessToJobObject(_handle, process.Handle)) throw new Win32Exception();
    }

    public void Dispose() => _handle.Dispose();

    [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Unicode)]
    private static extern SafeFileHandle CreateJobObject(IntPtr attributes, string? name);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool SetInformationJobObject(SafeFileHandle job, int infoClass, ref ExtendedLimitInformation info, uint length);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool AssignProcessToJobObject(SafeFileHandle job, IntPtr process);

    // JOBOBJECT_EXTENDED_LIMIT_INFORMATION aplanada (básica + IO_COUNTERS + límites de memoria)
    [StructLayout(LayoutKind.Sequential)]
    private struct ExtendedLimitInformation
    {
        public long PerProcessUserTimeLimit;
        public long PerJobUserTimeLimit;
        public uint LimitFlags;
        public UIntPtr MinimumWorkingSetSize;
        public UIntPtr MaximumWorkingSetSize;
        public uint ActiveProcessLimit;
        public UIntPtr Affinity;
        public uint PriorityClass;
        public uint SchedulingClass;
        public ulong ReadOperationCount;
        public ulong WriteOperationCount;
        public ulong OtherOperationCount;
        public ulong ReadTransferCount;
        public ulong WriteTransferCount;
        public ulong OtherTransferCount;
        public UIntPtr ProcessMemoryLimit;
        public UIntPtr JobMemoryLimit;
        public UIntPtr PeakProcessMemoryUsed;
        public UIntPtr PeakJobMemoryUsed;
    }
}
