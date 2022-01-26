using System;
using System.IO;
using System.Security.Cryptography;

namespace SQL_Document_Builder
{
    public class Encryption : IDisposable
    {
        private readonly TripleDESCryptoServiceProvider _tripleDes = new TripleDESCryptoServiceProvider();

        public Encryption(string saltkey)
        {
            // Initialize the crypto provider.
            if (saltkey.Length == 0)
                _tripleDes.Key = TruncateHash("gH1zdmxu73bSF_Q%:>4FWHHotpJ<%QUfEd>E6fK%0i_sIrEi2[wMH1GUJLU@iEdo", _tripleDes.KeySize / 8);
            else
                _tripleDes.Key = TruncateHash(saltkey, _tripleDes.KeySize / 8);
            _tripleDes.IV = TruncateHash("", _tripleDes.BlockSize / 8);
        }

        public string EncryptData(string plaintext)
        {

            // Convert the plaintext string to a byte array. 
            byte[] plaintextBytes = System.Text.Encoding.Unicode.GetBytes(plaintext);

            // Create the stream. 
            System.IO.MemoryStream ms = new System.IO.MemoryStream();
            // Create the encoder to write to the stream. 
            using (var encStream = new CryptoStream(ms, _tripleDes.CreateEncryptor(), System.Security.Cryptography.CryptoStreamMode.Write))
            {
                // Use the crypto stream to write the byte array to the stream.
                encStream.Write(plaintextBytes, 0, plaintextBytes.Length);
                encStream.FlushFinalBlock();
            }

            // Convert the encrypted stream to a printable string. 
            return System.Convert.ToBase64String(ms.ToArray());
        }

        public string DecryptData(string encryptedtext)
        {
            //Convert the encrypted text string to a byte array. 
            byte[] encryptedBytes = System.Convert.FromBase64String(encryptedtext);
            //Create the stream. 
            using (var ms = new System.IO.MemoryStream())
            {
                //Create the decoder to write to the stream. 
                using (var decStream = new CryptoStream(ms,
                    _tripleDes.CreateDecryptor(),
                    System.Security.Cryptography.CryptoStreamMode.Write))
                {
                    //Use the crypto stream to write the byte array to the stream.
                    decStream.Write(encryptedBytes, 0, encryptedBytes.Length);
                    decStream.FlushFinalBlock();
                }
                //Convert the plaintext stream to a string. 
                return System.Text.Encoding.Unicode.GetString(ms.ToArray());
            }
        }

        private byte[] TruncateHash(string key, Int32 length)
        {
            SHA1CryptoServiceProvider sha1 = new SHA1CryptoServiceProvider();

            // Hash the key. 
            byte[] keyBytes = System.Text.Encoding.Unicode.GetBytes(key);
            byte[] hash = sha1.ComputeHash(keyBytes);
            var oldHash = hash;
            hash = new byte[length - 1 + 1];

            // Truncate or pad the hash. 
            if (oldHash != null)
                Array.Copy(oldHash, hash, Math.Min(length - 1 + 1, oldHash.Length));
            return hash;
        }


        //public static string Encrypt(string value, string password, string salt)
        //{
        //    DeriveBytes rgb = new Rfc2898DeriveBytes(password, Encoding.Unicode.GetBytes(salt));
        //    SymmetricAlgorithm algorithm = new TripleDESCryptoServiceProvider();
        //    byte[] rgbKey = rgb.GetBytes(algorithm.KeySize >> 3);
        //    byte[] rgbIV = rgb.GetBytes(algorithm.BlockSize >> 3);
        //    ICryptoTransform transform = algorithm.CreateEncryptor(rgbKey, rgbIV);
        //    using (MemoryStream buffer = new MemoryStream())
        //    {
        //        using (CryptoStream stream = new CryptoStream(buffer, transform, CryptoStreamMode.Write))
        //        {
        //            using (StreamWriter writer = new StreamWriter(stream, Encoding.Unicode))
        //            {
        //                writer.Write(value);
        //            }
        //        }
        //        return Convert.ToBase64String(buffer.ToArray());
        //    }
        //}

        //public static string Decrypt(string text, string password, string salt)
        //{
        //    DeriveBytes rgb = new Rfc2898DeriveBytes(password, Encoding.Unicode.GetBytes(salt));
        //    SymmetricAlgorithm algorithm = new TripleDESCryptoServiceProvider();
        //    byte[] rgbKey = rgb.GetBytes(algorithm.KeySize >> 3);
        //    byte[] rgbIV = rgb.GetBytes(algorithm.BlockSize >> 3);
        //    ICryptoTransform transform = algorithm.CreateDecryptor(rgbKey, rgbIV);
        //    using (MemoryStream buffer = new MemoryStream(Convert.FromBase64String(text)))
        //    {
        //        using (CryptoStream stream = new CryptoStream(buffer, transform, CryptoStreamMode.Read))
        //        {
        //            using (StreamReader reader = new StreamReader(stream, Encoding.Unicode))
        //            {
        //                return reader.ReadToEnd();
        //            }
        //        }
        //    }
        //}

        public static byte[] Encrypt(string plainText, byte[] Key, byte[] IV)
        {
            byte[] encrypted;
            // Create a new TripleDESCryptoServiceProvider.  
            using (TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider())
            {
                // Create encryptor  
                ICryptoTransform encryptor = tdes.CreateEncryptor(Key, IV);
                // Create MemoryStream  
                using (MemoryStream ms = new MemoryStream())
                {
                    // Create crypto stream using the CryptoStream class. This class is the key to encryption  
                    // and encrypts and decrypts data from any given stream. In this case, we will pass a memory stream  
                    // to encrypt  
                    using (CryptoStream cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
                    {
                        // Create StreamWriter and write data to a stream  
                        using (StreamWriter sw = new StreamWriter(cs))
                            sw.Write(plainText);
                        encrypted = ms.ToArray();
                    }
                }
            }
            // Return encrypted data  
            return encrypted;
        }

        public static string Decrypt(byte[] cipherText, byte[] Key, byte[] IV)
        {
            string plaintext = null;
            // Create TripleDESCryptoServiceProvider  
            using (TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider())
            {
                // Create a decryptor  
                ICryptoTransform decryptor = tdes.CreateDecryptor(Key, IV);
                // Create the streams used for decryption.  
                using (MemoryStream ms = new MemoryStream(cipherText))
                {
                    // Create crypto stream  
                    using (CryptoStream cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read))
                    {
                        // Read crypto stream  
                        using (StreamReader reader = new StreamReader(cs))
                            plaintext = reader.ReadToEnd();
                    }
                }
            }
            return plaintext;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                    _tripleDes.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        // ~Encryption()
        // {
        //   // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
        //   Dispose(false);
        // }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            // GC.SuppressFinalize(this);
        }
        #endregion
    }

}
