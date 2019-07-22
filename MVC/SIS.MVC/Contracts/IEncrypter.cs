namespace SIS.MVC.Contracts
{
    public  interface IEncrypter
    {
        string Encrypt(string clearText);
        string Decrypt(string cipherText);
    }
}