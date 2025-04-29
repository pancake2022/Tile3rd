using System;
using System.IO;
using System.Security.Cryptography;

namespace CSFramework
{
    public class RijndaelUtils
    {
        private static byte[] _rijndael_key = new byte[] 
        { 
            0x0F, 0x02, 0x01, 0x02, 0x01, 0x02, 0x01, 0x02,
            0x02, 0x02, 0x01, 0x02, 0x01, 0x02, 0x01, 0x02,
            0x04, 0x02, 0x01, 0x02, 0x01, 0x02, 0x01, 0x02,
            0x04, 0x02, 0x01, 0x02, 0x01, 0x02, 0x01, 0x02 
        };

        private static byte[] _rijndael_iv = new byte[] 
        {  
            0x01, 0x02, 0x0E, 0x02, 0x01, 0x90, 0x01, 0x02,
            0x01, 0x02, 0xFF, 0x02, 0x01, 0xFE, 0x01, 0x02 
        };

        public static byte[] EncryptBytesToBytes(byte[] raw_bytes)
        {
            // Check arguments.
            if (raw_bytes == null || raw_bytes.Length <= 0)
                throw new ArgumentNullException("raw_bytes");
            byte[] encrypted;
            // Create an RijndaelManaged object
            // with the specified key and IV.
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = _rijndael_key;
                rijAlg.IV = _rijndael_iv;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (BinaryWriter swEncrypt = new BinaryWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(raw_bytes);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        public static byte[] EncryptStringToBytes(string raw_text)
        {
            // Check arguments.
            if (raw_text == null || raw_text.Length <= 0)
                throw new ArgumentNullException("raw_text");
            byte[] encrypted;
            // Create an RijndaelManaged object
            // with the specified key and IV.
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = _rijndael_key;
                rijAlg.IV = _rijndael_iv;

                // Create an encryptor to perform the stream transform.
                ICryptoTransform encryptor = rijAlg.CreateEncryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for encryption.
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            //Write all data to the stream.
                            swEncrypt.Write(raw_text);
                        }
                        encrypted = msEncrypt.ToArray();
                    }
                }
            }

            // Return the encrypted bytes from the memory stream.
            return encrypted;
        }

        public static byte[] DecryptBytesFromBytes(byte[] cipherBytes)
        {
            // Check arguments.
            if (cipherBytes == null || cipherBytes.Length <= 0)
                throw new ArgumentNullException("cipherBytes");

            // Declare the string used to hold
            // the decrypted text.
            byte[] raw_bytes = null;

            // Create an RijndaelManaged object
            // with the specified key and IV.
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = _rijndael_key;
                rijAlg.IV = _rijndael_iv;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherBytes))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (BinaryReader srDecrypt = new BinaryReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            raw_bytes = srDecrypt.ReadBytes(int.MaxValue);
                        }
                    }
                }

            }

            return raw_bytes;
        }

        public static string DecryptStringFromBytes(byte[] cipherText)
        {
            // Check arguments.
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");

            // Declare the string used to hold
            // the decrypted text.
            string raw_text = null;

            // Create an RijndaelManaged object
            // with the specified key and IV.
            using (RijndaelManaged rijAlg = new RijndaelManaged())
            {
                rijAlg.Key = _rijndael_key;
                rijAlg.IV = _rijndael_iv;

                // Create a decryptor to perform the stream transform.
                ICryptoTransform decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                // Create the streams used for decryption.
                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            // Read the decrypted bytes from the decrypting stream
                            // and place them in a string.
                            raw_text = srDecrypt.ReadToEnd();
                        }
                    }
                }

            }

            return raw_text;
        }
    }
}