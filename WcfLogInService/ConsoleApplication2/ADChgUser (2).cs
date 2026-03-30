using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MCCMLogger;

namespace WcfLogInService
{
    class ADChgUser
    {
        public  bool AddUN(string userName, string ipaddress)
        {
            bool rt = false;

            // Set up the DirectoryEntry object for the local computer Environment.MachineName
            DirectoryEntry localMachine = new DirectoryEntry("WinNT://" + ipaddress + ",computer", "mor_nisim", "Qwpozxi1");
            DirectoryEntry userExist = null;
            try
            {
                userExist = localMachine.Children.Find(userName, "user");
            }

            catch (Exception ex)
            {
                if (ex.Message.Contains("not be found"))// The user name could not be found.
                {
                    MccMLogger.Info("User not found");
                }
                else
                {
                    MccMLogger.Info("Error checking user existence: " + ex.Message);

                }
            }

            if (userExist != null)
            {
                userExist.Invoke("SetPassword", new object[] { "Qwpozxi0" });
                userExist.CommitChanges();
                rt = true;
                MccMLogger.Info("User already exists");
                // Set "Password never expires" flag
                // Set "Password never expires" and "User cannot change password" flags
                int dontExpirePasswordFlag = 0x10000;
                int cantChangePasswordFlag = 0x40;
                int userFlags = dontExpirePasswordFlag | cantChangePasswordFlag;
                userExist.Properties["UserFlags"].Value = userFlags;

                // Commit changes
                userExist.CommitChanges();
            }
            else
            {
                // Create the new user
                // Create a new user object and set its properties
                DirectoryEntry newUser = localMachine.Children.Add(userName, "user");

                newUser.Invoke("SetPassword", new object[] { "Qwpozxi0" });
                newUser.CommitChanges();

                // Add the new user to the "Users" group
                //DirectoryEntry usersGroup = localMachine.Children.Find("Users", "group");
                //usersGroup.Invoke("Add", new object[] { newUser.Path });
                //usersGroup.CommitChanges();
                int dontExpirePasswordFlag = 0x10000;
                int cantChangePasswordFlag = 0x40;
                int userFlags = dontExpirePasswordFlag | cantChangePasswordFlag;
                newUser.Properties["UserFlags"].Value = userFlags;

                // Commit changes
                newUser.CommitChanges();
            }

            /*


            // ContextType.Machine  ContextType.Domain
            using (var pc = new PrincipalContext(ContextType.Domain))
            {
                using (var up = new UserPrincipal(pc))
                {
                    up.SamAccountName = userName;
                   // up.EmailAddress = email
                    up.SetPassword("Aa123456_!");
                    up.PasswordNeverExpires = true;
                    up.UserCannotChangePassword = true; 
                    up.Enabled = true;
                    up.ExpirePasswordNow();
                    up.Save();
                }
            }
            */
            return rt;
        }
        public  bool  DeleteUN(string userName, string ipaddress)
        {
            bool rt = false;
            try
            {
                DirectoryEntry localMachine = new DirectoryEntry("WinNT://" + ipaddress + ",computer", "mor_nisim", "Qwpozxi1");
                DirectoryEntry user = localMachine.Children.Find(userName, "user");

                if (user != null)
                {
                    // User exists, proceed with deletion
                    localMachine.Children.Remove(user);
                    localMachine.CommitChanges();
                    MccMLogger.Info("User deleted successfully.");
                    rt = true;
                }
                else
                {
                    // User does not exist, print error message
                    MccMLogger.Info("User not found.");
                }
            }
            catch (Exception ex)
            {
                MccMLogger.Info(ex.Message);
                if (ex.Message.Contains("not be found"))// The user name could not be found.
                {
                    MccMLogger.Info("User not found");
                }
                else
                {
                    MccMLogger.Info("Error checking user existence: " + ex.Message);
                }
            }
            /*string path = $"Win32_UserAccount.Name='{"+userName+"}',Domain='" + ipaddress + "'";
            using (ManagementObject user = new ManagementObject(path))
            {
                if (user != null)
                {
                    user.Delete();
                    MccMLogger.Info("User deleted successfully.");
                }
                else
                {
                    MccMLogger.Info("User not found.");
                }
            }*/
            /*
            DirectoryEntry localMachine = new DirectoryEntry($"WinNT://" + ipaddress + ",computer", "mor_nisim", "Qwpozxi1");
            DirectoryEntry userEntry = localMachine.Children.Find(userName, "user");

            if (userEntry != null)
            {
                // Delete the user
                userEntry.DeleteTree();
                MccMLogger.Info("User deleted successfully.");
            }
            else
            {
                MccMLogger.Info("User not found.");
            }
             PrincipalContext ctx = new PrincipalContext(ContextType.Machine);
             UserPrincipal usrp = new UserPrincipal(ctx);
             usrp.Name = userName;
             PrincipalSearcher ps_usr = new PrincipalSearcher(usrp);
             var user = ps_usr.FindOne();
             user.Delete();
            
        }
            */

            /*
             * 
             * 
             * static void CreatBulkADUsersFromCSVFile()
        {
            string csvFilePath=@"C:\UsersAdminDesktopAll_users.CSV";

            using (TextFieldParser csvReader = new TextFieldParser(csvFilePath))
            {
                csvReader.SetDelimiters(new string[] { "," });
                csvReader.HasFieldsEnclosedInQuotes = true;

                // reading column fields 
                string[] colFields = csvReader.ReadFields();

                int index_samaccountName = colFields.ToList().IndexOf("samAccountName");

                while (!csvReader.EndOfData)
                {
                    // reading user fields 
                    string[] fieldData = csvReader.ReadFields();

                    DirectoryEntry ouEntry = new DirectoryEntry("LDAP://OU=TestOu,DC=YourDomain,DC=local");

                    try
                    {
                        DirectoryEntry childEntry = ouEntry.Children.Add("CN=" + fieldData[index_samaccountName], "user");
                        childEntry.CommitChanges();
                        ouEntry.CommitChanges();
                        childEntry.Invoke("SetPassword", new object[] { "password" });
                        childEntry.CommitChanges();
                    }
                    catch (Exception ex)
                    {
                    }
                }
            }
        }
             * 
             * */
            return rt;
        }
    }
}
