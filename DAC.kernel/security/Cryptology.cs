using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DAC.kernel.security
{
    public static class Cryptology
    {

        public static string Crypt(this int input)
        {
            return CryptString(input.ToString());
        }

        public static string Crypt(this string input)
        {
            return CryptString(input);
        }

        public static string Decrypt(this string input)
        {
            return DecryptString(input);
        }

        public static string MD5Hash(this string text)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] dizi = Encoding.UTF8.GetBytes(text);
            dizi = md5.ComputeHash(dizi);
            StringBuilder sb = new StringBuilder();
            foreach (byte ba in dizi)
            {
                sb.Append(ba.ToString("x2").ToLower());
            }
            return sb.ToString();
        }

        private static string CryptString(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            try
            {
                byte[] Results;
                System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();
                MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
                byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes("123456789"));
                TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider
                {
                    Key = TDESKey,
                    Mode = CipherMode.ECB,
                    Padding = PaddingMode.PKCS7
                };
                byte[] DataToEncrypt = UTF8.GetBytes(input);
                try
                {
                    ICryptoTransform Encryptor = TDESAlgorithm.CreateEncryptor();
                    Results = Encryptor.TransformFinalBlock(DataToEncrypt, 0, DataToEncrypt.Length);
                }
                finally
                {
                    TDESAlgorithm.Clear();
                    HashProvider.Clear();
                    HashProvider.Dispose();
                    TDESAlgorithm.Dispose();
                }
                return Convert.ToBase64String(Results);
            }
            catch { }

            return string.Empty;
        }

        private static string DecryptString(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            try
            {
                byte[] Results;
                System.Text.UTF8Encoding UTF8 = new System.Text.UTF8Encoding();
                MD5CryptoServiceProvider HashProvider = new MD5CryptoServiceProvider();
                byte[] TDESKey = HashProvider.ComputeHash(UTF8.GetBytes("123456789"));
                TripleDESCryptoServiceProvider TDESAlgorithm = new TripleDESCryptoServiceProvider
                {
                    Key = TDESKey,
                    Mode = CipherMode.ECB,
                    Padding = PaddingMode.PKCS7
                };

                try
                {


                    byte[] DataToDecrypt = Convert.FromBase64String(input);
                    ICryptoTransform Decryptor = TDESAlgorithm.CreateDecryptor();
                    Results = Decryptor.TransformFinalBlock(DataToDecrypt, 0, DataToDecrypt.Length);
                }
                finally
                {
                    TDESAlgorithm.Clear();
                    HashProvider.Clear();
                    HashProvider.Dispose();
                    TDESAlgorithm.Dispose();
                }
                return UTF8.GetString(Results);
            }
            catch
            {
                return input;
            }
        }

        public static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        public static bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                for (int i = 0; i < computedHash.Length; i++)
                {
                    if (computedHash[i] != passwordHash[i])
                    {
                        return false;
                    }
                }
            }

            return true;
        }


        public static void cr()
        {
            UnicodeEncoding ByteConverter = new UnicodeEncoding();
            byte[] dataToEncrypt = ByteConverter.GetBytes("Data to Encrypt");
            byte[] encryptedData;
            byte[] decryptedData;
            using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider(1024, new CspParameters()))
            {
                encryptedData = RSAEncrypt(dataToEncrypt, RSA.ExportParameters(false), false);
                decryptedData = RSADecrypt(encryptedData, RSA.ExportParameters(true), false);
            }

        }

        private static byte[] RSAEncrypt(byte[] DataToEncrypt, RSAParameters RSAKeyInfo, bool DoOAEPPadding)
        {
            try
            {
                byte[] encryptedData;
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {
                    RSA.ImportParameters(RSAKeyInfo);
                    encryptedData = RSA.Encrypt(DataToEncrypt, DoOAEPPadding);
                }
                return encryptedData;
            }
            catch (CryptographicException e)
            {
                return null;
            }
        }

        private static byte[] RSADecrypt(byte[] DataToDecrypt, RSAParameters RSAKeyInfo, bool DoOAEPPadding)
        {
            try
            {
                byte[] decryptedData;
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {
                    RSA.ImportParameters(RSAKeyInfo);
                    decryptedData = RSA.Decrypt(DataToDecrypt, DoOAEPPadding);
                }
                return decryptedData;
            }
            catch (CryptographicException e)
            {
                return null;
            }
        }

        public static string EncryptFromCerificate(X509Certificate2 cert, string d)
        {
            using (RSA rsa = cert.GetRSAPublicKey())
            {
                byte[] data = System.Text.Encoding.UTF8.GetBytes(d);
                return Convert.ToBase64String(rsa.Encrypt(data, RSAEncryptionPadding.OaepSHA1));
            }
        }

        public static string DecryptFromCerificate(X509Certificate2 cert, string d)
        {

            byte[] data = Convert.FromBase64String(d);
            using (RSA rsa = cert.GetRSAPrivateKey())
            {
                return System.Text.Encoding.UTF8.GetString(rsa.Decrypt(data, RSAEncryptionPadding.OaepSHA1));
            }
        }

    }
}
