using System.Security;
using CredentialManagement;

namespace MailClipboard
{
    public class AuthenticationHelper
    {
        public static SecureString GetPassword(string email)
        {
            using (Credential credential = GetCredential(email))
            {
                if (credential.Exists())
                {
                    credential.Load();
                    return credential.SecurePassword;
                }
                return PromptForPassword(email);
            }
        }

        private static Credential GetCredential(string email)
        {
            return new Credential
            {
                Target = email,
                Type = CredentialType.Generic
            };
        }

        public static void ForgetPassword(string email)
        {
            using (var credential = GetCredential(email))
                credential.Delete();
        }

        private static SecureString PromptForPassword(string email)
        {
            var prompt = new VistaPrompt
            {
                Message = $"Please enter password for {email}",
                Username = email,
                Title = "Sender email password",
                ShowSaveCheckBox = true,
                GenericCredentials = true
            };
            if (prompt.ShowDialog() == DialogResult.OK)
            {
                using (var credential = new Credential(email, prompt.Password, email, CredentialType.Generic))
                {
                    if (prompt.SaveChecked)
                        credential.Save();
                    return credential.SecurePassword;
                }
            }
            return null;
        }
    }
}