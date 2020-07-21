using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;
public static class Unsafe
{
    public static SecureString ToSecureString(this string value)
    {
        unsafe
        {
            fixed (char* value3 = value)
            {
                SecureString ss = new System.Security.SecureString(value3, value.Length);
                ss.MakeReadOnly();
                return ss;
            }
        }
    }

    public static string ToInsecureString(SecureString securePassword)
    {
        IntPtr unmanagedString = IntPtr.Zero;
        try
        {
            unmanagedString = Marshal.SecureStringToGlobalAllocUnicode(securePassword);
            return Marshal.PtrToStringUni(unmanagedString);
        }
        finally
        {
            Marshal.ZeroFreeGlobalAllocUnicode(unmanagedString);
        }
    }

    public static string DecryptString(string salt, string encrypted)
    {
        return Unsafe.ToInsecureString(ProtectedDataHelper.DecryptString(salt, encrypted));
    }

    public static string EncryptString(string salt, string encrypted)
    {
        SecureString ss = encrypted.ToSecureString();
        
        return ProtectedDataHelper.EncryptString(salt,  ss);
    }

    public static CryptDelegates CreateCryptDelegates()
    {
        CryptDelegates cd = new CryptDelegates();
        cd.decryptString = DecryptString;
        cd.encryptString = EncryptString;
        return cd;
    }
}