using System.Security.Cryptography;
using System.Text;

namespace InvestCloudFrontTest.Services
{
    public interface IEncryptionService
    {
        string HashInput(string input);
    }
    public class EncryptionService : IEncryptionService
    {

        public string HashInput(string input)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
