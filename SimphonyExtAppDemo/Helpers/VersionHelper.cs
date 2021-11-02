using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;

namespace SimphonyExtAppDemo.Helpers
{
    public static class VersionHelper
    {
        private struct _IMAGE_FILE_HEADER
        {
            public ushort Machine;
            public ushort NumberOfSections;
            public uint TimeDateStamp;
            public uint PointerToSymbolTable;
            public uint NumberOfSymbols;
            public ushort SizeOfOptionalHeader;
            public ushort Characteristics;
        };

        static VersionHelper()
        {
            ApplicationVersion = Assembly.GetExecutingAssembly().GetName().Version.ToString(4);
            EnvironmentVersion = Environment.Version.ToString(4);
            ServiceHostVersion = Assembly.GetCallingAssembly().GetName().Version;
            ReadIntegrationBuildTimeUtc();
        }

        public static void ReadIntegrationBuildTimeUtc()
        {

            var buffer = new byte[Math.Max(Marshal.SizeOf(typeof(_IMAGE_FILE_HEADER)), 4)];
            using (var fileStream = new FileStream(Assembly.GetExecutingAssembly().Location, FileMode.Open, FileAccess.Read))
            {
                fileStream.Position = 0x3C;
                fileStream.Read(buffer, 0, 4);
                fileStream.Position = BitConverter.ToUInt32(buffer, 0); // COFF header offset
                fileStream.Read(buffer, 0, 4); // "PE\0\0"
                fileStream.Read(buffer, 0, buffer.Length);
            }
            var pinnedBuffer = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            try
            {
                var coffHeader = (_IMAGE_FILE_HEADER)Marshal.PtrToStructure(pinnedBuffer.AddrOfPinnedObject(), typeof(_IMAGE_FILE_HEADER));

                IntegrationBuildDateTimeUtc = new DateTime(1970, 1, 1) + new TimeSpan(coffHeader.TimeDateStamp * TimeSpan.TicksPerSecond);
            }
            finally
            {
                pinnedBuffer.Free();
            }
        }

        public static string ApplicationVersion { get; }
        public static string EnvironmentVersion { get; set; }
        public static DateTime IntegrationBuildDateTimeUtc;
        public static string NameAndVersion => $"Simphony Extension Application Demo v{ApplicationVersion}";
        public static Version ServiceHostVersion { get; set; }
    }
}