using System.Diagnostics;
using System.Security;
using System.Text;
using System.Runtime.InteropServices;

namespace Microsoft.EntityFrameworkCore.DataEncryption.Converters;

internal static class EncodingExtensions
{
    internal static byte[] GetBytes(this Encoding encoding, SecureString value)
    {
        if (encoding is null)
        {
            throw new ArgumentNullException(nameof(encoding));
        }

        if (value is null || value.Length == 0)
        {
            return Array.Empty<byte>();
        }

        IntPtr valuePtr = IntPtr.Zero;
        try
        {
            valuePtr = Marshal.SecureStringToGlobalAllocUnicode(value);
            if (valuePtr == IntPtr.Zero)
            {
                return Array.Empty<byte>();
            }

            unsafe
            {
                char* chars = (char*)valuePtr;
                Debug.Assert(chars != null);

                int byteCount = encoding.GetByteCount(chars, value.Length);

                var result = new byte[byteCount];
                fixed (byte* bytes = result)
                {
                    encoding.GetBytes(chars, value.Length, bytes, byteCount);
                }

                return result;
            }
        }
        finally
        {
            if (valuePtr != IntPtr.Zero)
            {
                Marshal.ZeroFreeGlobalAllocUnicode(valuePtr);
            }
        }
    }
}