namespace Application.Services
{
    using System.Security.Cryptography;
    using System.Text;
    public class EncriptionManager
    {

        public string Encrypt(string text)
        {
            SHA256CryptoServiceProvider cryptoSha = new SHA256CryptoServiceProvider();

            byte[] bs1 = Encoding.UTF8.GetBytes(text);
            bs1 = cryptoSha.ComputeHash(bs1);
            StringBuilder s1 = new System.Text.StringBuilder();

            foreach (byte b in bs1)
            {
                s1.Append(b.ToString("x2").ToLower());
            }

            return s1.ToString();

        }







    }
}
