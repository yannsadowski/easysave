using System.Configuration;

namespace EasySave.lib.Services
{
    public class GenerateKey
    {
        public int Generate()
        {
            Random _Random = new Random();
            ulong Key = ((ulong)_Random.Next() << 32 | (ulong)_Random.Next());
            string KeyPath = $"{ConfigurationManager.AppSettings["CryptKeyPath"]}";
            try
            {
                using (StreamWriter _StreamWriter = new StreamWriter(KeyPath))
                {
                    _StreamWriter.WriteLine(Key.ToString());
                }
                return 0;
            }
            catch
            {
                return 1;
            }
        }
    }
}