using System;
using System.Text;

public static class StringEncoder {
    public static string EncodeString(string input) {
        byte[] bytesToEncode = Encoding.UTF8.GetBytes(input);
        return Convert.ToBase64String(bytesToEncode);
    }

    public static string DecodeString(string encodedData) {
        byte[] decodedBytes = Convert.FromBase64String(encodedData);
        return Encoding.UTF8.GetString(decodedBytes);
    }
}