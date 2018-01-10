using Android.Util;
using Java.Lang;
using Java.Security;
using Java.Security.Spec;
using System;
using System.Text;
namespace Xamarin.InAppBilling.Utilities
{

    public sealed class Security
    {
        private const string KeyFactoryAlgorithm = "RSA";
        private const string SignatureAlgorithm = "SHA1withRSA";

        public static IPublicKey GeneratePublicKey(string encodedPublicKey)
        {
            IPublicKey key;
            try
            {
                key = KeyFactory.GetInstance("RSA").GeneratePublic(new X509EncodedKeySpec(Base64.Decode(encodedPublicKey, 0)));
            }
            catch (NoSuchAlgorithmException exception)
            {
                Logger.Error(exception.Message, new object[0]);
                throw new RuntimeException(exception);
            }
            catch (System.Exception exception2)
            {
                Logger.Error(exception2.Message, new object[0]);
                throw new IllegalArgumentException();
            }
            return key;
        }

        public static string Unify(string[] element, int[] segment)
        {
            string str = string.Empty;
            foreach (int num in segment)
            {
                str = str + element[num];
            }
            return str;
        }

        public static string Unify(string[] element, int[] segment, string[] hash)
        {
            string str = string.Empty;
            str = Unify(element, segment);
            for (int i = 0; i < hash.Length; i += 2)
            {
                str = str.Replace(hash[i], hash[i + 1]);
            }
            return str;
        }

        public static bool Verify(IPublicKey publicKey, string signedData, string signature)
        {
            object[] args = new object[] { signature };
            Logger.Debug("Signature: {0}", args);
            try
            {
                Signature instance = Signature.GetInstance("SHA1withRSA");
                instance.InitVerify(publicKey);
                instance.Update(Encoding.UTF8.GetBytes(signedData));
                if (!instance.Verify(Base64.Decode(signature, 0)))
                {
                    Logger.Error("Security. Signature verification failed.", new object[0]);
                    return false;
                }
                return true;
            }
            catch (System.Exception exception)
            {
                Logger.Error(exception.Message, new object[0]);
            }
            return false;
        }

        public static bool VerifyPurchase(string publicKey, string signedData, string signature)
        {
            if (signedData == null)
            {
                Logger.Error("Security. data is null", new object[0]);
                return false;
            }
            try
            {
                if (!string.IsNullOrEmpty(signature))
                {
                    if (Verify(GeneratePublicKey(publicKey), signedData, signature))
                    {
                        return true;
                    }
                    Logger.Error("Security. Signature does not match data.", new object[0]);
                    return false;
                }
            }
            catch
            {
                return false;
            }
            return false;
        }
    }
}

