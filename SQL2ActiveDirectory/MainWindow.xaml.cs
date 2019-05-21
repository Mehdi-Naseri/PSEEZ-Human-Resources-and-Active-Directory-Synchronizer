using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.DirectoryServices.AccountManagement;

namespace SQL2ActiveDirectory
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            textboxUser.Text = "مهدي ناصري";
            passwordboxPass.Password = "MehdiNaseri!";
            stringDomainName = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
        }

        string stringDomainName;
        string stringMehdiADPassword = "MehdiNaseri!";
        string stringSQLpassword = "user-ad";
        string stringSQLusername = "user-ad@P3eez.sts";
        


        

        #region ADD : Analyze & Report & Add
        private void buttonAnalyze_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FindActiveDirectoryUsers();
                FindSQLserverfiltered();
                FindNewUsersActive();
                datagridResult.ItemsSource = UsersNewActive; 
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can't read data:"+ex.Message);
            }
        }

        private void buttonReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.SaveFileDialog SaveFileDialog1 = new Microsoft.Win32.SaveFileDialog();
                SaveFileDialog1.Filter = "Comma-Seprated Value (*.csv)|*.cvs";
                if ((bool)SaveFileDialog1.ShowDialog())
                {
                    ExportUserstoCSV(SaveFileDialog1.FileName, UsersNewActive);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can't save report:" + ex.Message);
            }
        }

        private void buttonAdd_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FindActiveDirectoryUsers();
                FindSQLserverfiltered();
                FindNewUsersActive();
                datagridResult.ItemsSource = UsersNewActive;
                ReportAddedorDeletedUsersCVSTitle("AddedUsers.csv");

                foreach (User user1 in UsersNewActive)
                {
                        switch (user1.Description)
                        {
                            case "پارس 2":
                                AddUser2AD(user1, "OU=Users,OU=Pars2-Tonbak,OU=Pars-Zone,DC=pseez,DC=local");
                                break;
                            case "پارس3":
                                AddUser2AD(user1, "OU=Users,OU=Pars3-Choghadak,OU=Pars-Zone,DC=pseez,DC=local");
                                break;
                            case "دفتر تهران":
                                AddUser2AD(user1, "OU=Users,OU=Tehran Office,OU=Pars-Zone,DC=pseez,DC=local");
                                break;
                            case "دفتر بوشهر":
                                AddUser2AD(user1, "OU=Users,OU=Boushehr Office,OU=Pars-Zone,DC=pseez,DC=local");
                                break;
                            default:
                                AddUser2AD(user1, "OU=Users,OU=Pars1-Asaluye,OU=Pars-Zone,DC=pseez,DC=local");
                                break;
                        }
                }
                MessageBox.Show("All users sucessfully added to Active Directory");
            }
            catch (Exception ex)
            {
                string stringFileNameFull = System.Environment.CurrentDirectory + "\\" + "Erorr.txt";
                   System.IO.File.AppendAllText(stringFileNameFull, ex.Message+"\n\n\n", Encoding.UTF8);
            }
        }
        #endregion

        #region   Delete: Analyze & Report & Delete
        //Find users in Active Directory Who are not in SqlServer DataBase 
        public Users UserstoDelete = new Users();
        private void buttonAnalyzeDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FindActiveDirectoryUsers();
                FindSqlServerUsers();
                FindUserstoDelete();
                datagridResult.ItemsSource = UserstoDelete;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can't read data:" + ex.Message);
            }
        }
        private void buttonReportDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Microsoft.Win32.SaveFileDialog SaveFileDialog1 = new Microsoft.Win32.SaveFileDialog();
                SaveFileDialog1.Filter = "Comma-Seprated Value (*.csv)|*.cvs";
                if ((bool)SaveFileDialog1.ShowDialog())
                {
                    ExportUserstoCSV(SaveFileDialog1.FileName, UserstoDelete);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can't save report:" + ex.Message);
            }
        }
        private void buttonDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                FindActiveDirectoryUsers();
                FindSqlServerUsers();
                FindUserstoDelete();
                datagridResult.ItemsSource = UserstoDelete;
                ReportAddedorDeletedUsersCVSTitle("DeletedUsers");
                foreach (User user1 in UserstoDelete)
                {
                    switch (user1.Description)
                    {
                        case "پارس 2":
                            DeleteUserFromSQLserver(user1, "OU=Users,OU=Pars2-Tonbak,OU=Pars-Zone,DC=pseez,DC=local");
                            break;
                        case "پارس3":
                            DeleteUserFromSQLserver(user1, "OU=Users,OU=Pars3-Choghadak,OU=Pars-Zone,DC=pseez,DC=local");
                            break;
                        case "دفتر تهران":
                            DeleteUserFromSQLserver(user1, "OU=Users,OU=Tehran Office,OU=Pars-Zone,DC=pseez,DC=local");
                            break;
                        case "دفتر بوشهر":
                            DeleteUserFromSQLserver(user1, "OU=Users,OU=Boushehr Office,OU=Pars-Zone,DC=pseez,DC=local");
                            break;
                        default:
                            DeleteUserFromSQLserver(user1, "OU=Users,OU=Pars1-Asaluye,OU=Pars-Zone,DC=pseez,DC=local");
                            break;
                    }
                }
                MessageBox.Show("All users sucessfully Deleted from Active Directory");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        #endregion

        #region  Functions: Find
        public void FindUsers()
        {
            FindActiveDirectoryUsers();
            FindSqlServerUsers();
            FindNewUsers();
            datagridResult.ItemsSource = UsersNew;
        }

        public void FindActiveDirectoryUsers()
        {
                    UsersActiveDirectory.Clear();
                    string stringDomainName = System.Net.NetworkInformation.IPGlobalProperties.GetIPGlobalProperties().DomainName;
                    if (stringDomainName != null)
                    {
                        PrincipalContext AD = new PrincipalContext(ContextType.Domain, stringDomainName);
                        UserPrincipal u = new UserPrincipal(AD);
                        PrincipalSearcher search = new PrincipalSearcher(u);
                        foreach (UserPrincipal result in search.FindAll())
                        {
                            User User1 = new User(result.SamAccountName, result.DisplayName, result.Name, result.GivenName, result.Surname,
                                result.Description, result.Enabled, result.LastLogon);
                            UsersActiveDirectory.Add(User1);
                        }
                        search.Dispose();
                    }
                    else
                    {
                        MessageBox.Show("Your computer is not a member of domain", "Active Directory Users", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
        }

        public void FindSqlServerUsers()
        {
            UsersSqlServer.Clear();
            using (var TotalSystemGAEntities1 = new TotalSystemGAEntities())
            {
                var q = from People1 in TotalSystemGAEntities1.People
                        join Employee1 in TotalSystemGAEntities1.Employees on People1.Id equals Employee1.Id
                        join EmployeeOrganCharts1 in TotalSystemGAEntities1.EmployeeOrganCharts on Employee1.Id equals EmployeeOrganCharts1.Employee_Id
                        join OrganCharts1 in TotalSystemGAEntities1.OrganCharts on EmployeeOrganCharts1.OrganChart_Id equals OrganCharts1.Id
                        join UnitsIns in TotalSystemGAEntities1.Units on OrganCharts1.Unit_Id equals UnitsIns.Id
                        select new { ResultID = Employee1.Code, ResultName = People1.Name, ResultFamily = People1.Family, ResultDepartment = UnitsIns.Name };
                foreach (var a in q)
                {
                    User user1 = new User();
                    user1.Description = a.ResultDepartment;
                    user1.DisplayName = string.Concat(a.ResultName.Trim() + ' ' + a.ResultFamily.Trim());
                    user1.Enabled = true;
                    user1.GivenName = a.ResultName ;
                    user1.Name = string.Concat(a.ResultName.Trim() + ' ' + a.ResultFamily.Trim());
                    user1.SamAccountName = a.ResultID.ToString();
                    user1.Surname = a.ResultFamily;
                    UsersSqlServer.Add(user1);
                }
            }
        }
        public void FindNewUsers()
        {
            UsersNew.Clear();
            var qSql = from u in UsersSqlServer
                       select u.SamAccountName.Trim();
            var qAD = from u in UsersActiveDirectory
                      select u.SamAccountName.Trim();

            var qNew = qSql.Except(qAD);

            foreach (var a in UsersSqlServer)
            {
                User user1 = new User();
                user1.Description = a.Description;
                user1.DisplayName = a.DisplayName;
                user1.Enabled = true;
                user1.GivenName = a.GivenName;
                user1.Name = a.Name;
                user1.SamAccountName = a.SamAccountName;
                user1.Surname = a.Surname;
                UsersNew.Add(user1);
            }
            UsersNew.RemoveAll(e => qAD.Contains(e.SamAccountName));
        }

        private void FindSQLserverfiltered()
        {
            UsersSqlServerActive.Clear();
            using (var TotalSystemGAEntities1 = new TotalSystemGAEntities())
            {
                var q = from People1 in TotalSystemGAEntities1.People
                        join Employee1 in TotalSystemGAEntities1.Employees on People1.Id equals Employee1.Id
                        join EmployeeOrganCharts1 in TotalSystemGAEntities1.EmployeeOrganCharts on Employee1.Id equals EmployeeOrganCharts1.Employee_Id
                        join OrganCharts1 in TotalSystemGAEntities1.OrganCharts on EmployeeOrganCharts1.OrganChart_Id equals OrganCharts1.Id
                        join UnitsIns in TotalSystemGAEntities1.Units on OrganCharts1.Unit_Id equals UnitsIns.Id
                        where EmployeeOrganCharts1.ToDate == null
                        orderby Employee1.Code
                        select new { ResultID = Employee1.Code, ResultName = People1.Name, ResultFamily = People1.Family, ResultDepartment = UnitsIns.Name };

                foreach (var a in q)
                {
                    User user1 = new User();
                    user1.Description = a.ResultDepartment;
                    user1.DisplayName = string.Concat(a.ResultName.Trim() + ' ' + a.ResultFamily.Trim());
                    user1.Enabled = true;
                    user1.GivenName = a.ResultName;
                    user1.Name = string.Concat(a.ResultName.Trim() + ' ' + a.ResultFamily.Trim());
                    user1.SamAccountName = a.ResultID.ToString();
                    user1.Surname = a.ResultFamily;
                    UsersSqlServerActive.Add(user1);
                }
            }
        }
        public void FindNewUsersActive()
        {
            UsersNewActive.Clear();
            var qAD = from u in UsersActiveDirectory
                      select u.SamAccountName.Trim();

            string stringTemp = ""; 
            //filter users with duplicate Description(department)
            foreach (var a in UsersSqlServerActive)
            {
                if (!string.Equals(a.SamAccountName,stringTemp))
                {
                User user1 = new User();
                user1.Description = a.Description;
                user1.DisplayName = a.DisplayName;
                user1.Enabled = true;
                user1.GivenName = a.GivenName;
                user1.Name = a.Name;
                user1.SamAccountName = a.SamAccountName;
                user1.Surname = a.Surname;
                UsersNewActive.Add(user1);
            }
            stringTemp = a.SamAccountName;
            }
            UsersNewActive.RemoveAll(e => qAD.Contains(e.SamAccountName));
        }

        //Find users in 
        public void FindUserstoDelete ()
        {
            UserstoDelete.Clear();
            var qAD = from u in UsersSqlServer
                      select u.SamAccountName.Trim();

            //filter users with duplicate Description(department)
            foreach (var a in UsersActiveDirectory)
            {
                    User user1 = new User();
                    user1.Description = a.Description;
                    user1.DisplayName = a.DisplayName;
                    user1.Enabled = a.Enabled;
                    user1.GivenName = a.GivenName;
                    user1.Name = a.Name;
                    user1.SamAccountName = a.SamAccountName;
                    user1.Surname = a.Surname;
                    user1.LastLogon = a.LastLogon;
                    UserstoDelete.Add(user1);
            }
            UserstoDelete.RemoveAll(e => qAD.Contains(e.SamAccountName));
        }
        #endregion

        #region Users
        public Users UsersActiveDirectory = new Users();
        public Users UsersSqlServer = new Users();
        public Users UsersNew = new Users();
        public Users UsersSqlServerActive = new Users();
        public Users UsersNewActive = new Users();
        public class Users : List<User> { }
        public class User : IEquatable<User>
        {
            public String SamAccountName { get; set; }
            public String DisplayName { get; set; }
            public String Name { get; set; }
            public String GivenName { get; set; }
            public String Surname { get; set; }
            public String Description { get; set; }
            public Boolean? Enabled { get; set; }
            public DateTime? LastLogon { get; set; }
            public User()
            {
            }
            public User(String SamAccountName, String DisplayName, String Name, String GivenName, String Surname, String Description,
                Boolean? Enabled, DateTime? LastLogon)
            {
                this.SamAccountName = SamAccountName;
                this.DisplayName = DisplayName;
                this.Name = Name;
                this.GivenName = GivenName;
                this.Surname = Surname;
                this.Description = Description;
                this.Enabled = Enabled;
                this.LastLogon = LastLogon;
            }
            public List<string> Properties()
            {
                return new List<string> { SamAccountName, DisplayName, Name, GivenName, Surname, Description, Enabled.ToString(), LastLogon.ToString() };
            }
            public int UserPropertiesTotal = 8;
            public static string[] StringArrayUesrProperties = { "SamAccountName", "DisplayName", "Name", "GivenName", "Surname", "Description", "Enabled", "LastLogon" };

            public bool Equals(User other)
            {
                //Check whether the compared object is null. 
                if (Object.ReferenceEquals(other, null)) return false;
                //Check whether the compared object references the same data. 
                if (Object.ReferenceEquals(this, other)) return true;
                //Check whether the properties are equal. 
                return SamAccountName.Equals(other.SamAccountName);
            }
        }
        #endregion


        #region TestButons
        //button Active Directory Users
        private void button1_Click(object sender, RoutedEventArgs e)
        {
            FindActiveDirectoryUsers();
            datagridResult.ItemsSource = UsersActiveDirectory;
        }
        private void button2_Click(object sender, RoutedEventArgs e)
        {
            FindSqlServerUsers();
            datagridResult.ItemsSource = UsersSqlServer;
        }

        private void button3_Click(object sender, RoutedEventArgs e)
        {
            FindNewUsers();
            datagridResult.ItemsSource = UsersNew;
        }
        private void buttonSqlFiltered_Click(object sender, RoutedEventArgs e)
        {
            FindSQLserverfiltered();
            datagridResult.ItemsSource = UsersSqlServerActive;
        }

        //button New Users Filtered
        private void button4_Click(object sender, RoutedEventArgs e)
        {
            FindNewUsersActive();
            datagridResult.ItemsSource = UsersNewActive;
        }
        private void buttonExport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Forms.FolderBrowserDialog FolderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
                Microsoft.Win32.SaveFileDialog SaveFileDialog1 = new Microsoft.Win32.SaveFileDialog();
                SaveFileDialog1.Filter = "Comma-Seprated Value (*.csv)|*.cvs";
                if (FolderBrowserDialog1.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    if (CheckBoxAD.IsChecked == true)
                    {
                        ExportUserstoCSV(string.Concat(FolderBrowserDialog1.SelectedPath, "\\ActiveDirectoryUsers.cvs"), UsersActiveDirectory);
                    }
                    if (CheckBoxSQL.IsChecked == true)
                    {
                        ExportUserstoCSV(string.Concat(FolderBrowserDialog1.SelectedPath, "\\SqlServerUsers.cvs"), UsersSqlServer);
                    }
                    if (CheckBoxNewUser.IsChecked == true)
                    {
                        ExportUserstoCSV(string.Concat(FolderBrowserDialog1.SelectedPath, "\\NewUsers.cvs"), UsersNew);
                    }
                    if (CheckBoxSQLfiltered.IsChecked == true)
                    {
                        ExportUserstoCSV(string.Concat(FolderBrowserDialog1.SelectedPath, "\\SqlServerUsersActive.cvs"), UsersSqlServerActive);
                    }
                    if (CheckBoxNewUserActive.IsChecked == true)
                    {
                        ExportUserstoCSV(string.Concat(FolderBrowserDialog1.SelectedPath, "\\NewUsersActive.cvs"), UsersNewActive);
                    }
                    MessageBox.Show("Saved Successfully", "Export to CVS", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Can't save Reports:" + ex.Message);
            }
        }
        #endregion

        #region New Users
        //Find users in SQL Server who are not in Active directory
        private void ExportUserstoCSV(string stringFileName,Users Users1)
        {
            //   File FileStream1 = new System.IO.File();
            StringBuilder StringBuilder1 = new StringBuilder(null);
            foreach (string string1 in User.StringArrayUesrProperties)
            {
                if (StringBuilder1.Length == 0)
                    StringBuilder1.Append(string1);
                else
                StringBuilder1.Append(',' + string1);
            }
            StringBuilder1.AppendLine();
            foreach (User User1 in Users1)
            {
                StringBuilder StringBuilderTemp = new StringBuilder(null);
                foreach (string string1 in User1.Properties())
                {
                    if (StringBuilderTemp.Length == 0)
                        StringBuilderTemp.Append(string1);
                    else
                    StringBuilderTemp.Append(',' + string1);
                }
                //   StringBuilder1.AppendLine();
                StringBuilder1.AppendLine(StringBuilderTemp.ToString());
            }
            System.IO.File.WriteAllText(stringFileName, StringBuilder1.ToString(), Encoding.UTF8);
        }

        private void AddUser2AD(User user1,String stringOU)
        {
            try
            {
                //  textboxOu.Text = "CN=Users,DC=Mehdi,DC=Local";
                //user admin 
                PrincipalContext PrincipalContext1 = new PrincipalContext(ContextType.Domain, stringDomainName, stringOU, ContextOptions.SimpleBind, textboxUser.Text, passwordboxPass.Password);
                //new user to add
                UserPrincipal UserPrincipal1 = new UserPrincipal(PrincipalContext1, user1.SamAccountName, "1234abcd!", true);
                //User Log on Name
                UserPrincipal1.UserPrincipalName = user1.SamAccountName;
                UserPrincipal1.Name = user1.Name;
                UserPrincipal1.GivenName = user1.GivenName;
                UserPrincipal1.Surname = user1.Surname;
                UserPrincipal1.DisplayName = user1.DisplayName;
                UserPrincipal1.Description = user1.Description;
                UserPrincipal1.Enabled = true;
                UserPrincipal1.Save();

                ReportAddedorDeletedUsersCVS(user1, stringOU, "AddedUsers.csv");
            }
            catch (Exception ex)
            {
                string stringFileNameFull = System.Environment.CurrentDirectory + "\\" + "Erorr.txt";
                System.IO.File.AppendAllText(stringFileNameFull, ex.Message + "\n\n\n", Encoding.UTF8);
            }
        }

        private void DeleteUserFromSQLserver(User user1, string stringOU)
        {
            //  textboxOu.Text = "CN=Users,DC=Mehdi,DC=Local";
            //user admin 
            /*            PrincipalContext PrincipalContext1 = new PrincipalContext(ContextType.Domain, stringDomainName, stringOU, ContextOptions.SimpleBind, textboxUser.Text, passwordboxPass.Password);
                        //new user to add
                        UserPrincipal UserPrincipal1 = new UserPrincipal(PrincipalContext1, user1.Name, "1234abcd!", true);
                        //User Log on Name
                        UserPrincipal1.UserPrincipalName = user1.SamAccountName;
                        UserPrincipal1.Name = user1.Name;
                        UserPrincipal1.GivenName = user1.GivenName;
                        UserPrincipal1.Surname = user1.Surname;
                        UserPrincipal1.Description = user1.Description;
                        UserPrincipal1.Enabled = true;
                        UserPrincipal1.Save();
             */
            ReportAddedorDeletedUsersCVS(user1, stringOU,"DeletedUsers.csv");
        }

        private void ReportAddedorDeletedUsersCVSTitle(string stringFileName)
        {
            string stringFileNameFull = System.Environment.CurrentDirectory + "\\" + stringFileName;
            StringBuilder StringBuilder1 = new StringBuilder();
            foreach (string string1 in User.StringArrayUesrProperties)
            {
                if (StringBuilder1.Length == 0)
                    StringBuilder1.Append(string1);
                else
                StringBuilder1.Append(',' + string1);
            }
            StringBuilder1.AppendLine(',' + "OU");
            System.IO.File.WriteAllText(stringFileNameFull, StringBuilder1.ToString(), Encoding.UTF8);
        }
        private void ReportAddedorDeletedUsersCVS(User user1, string stringOU, string stringFileName)
        {
            string stringFileNameFull = System.Environment.CurrentDirectory + "\\" + stringFileName;
            //Save Users
            StringBuilder StringBuilder1 = new StringBuilder();
            foreach (string string1 in user1.Properties())
                {
                    if (StringBuilder1.Length == 0)
                        StringBuilder1.Append(string1);
                    else
                    StringBuilder1.Append(',' + string1);
                }
            StringBuilder1.AppendLine( ',' + stringOU);
            System.IO.File.AppendAllText(stringFileNameFull, StringBuilder1.ToString(), Encoding.UTF8);
        }
        #endregion

        
    }
}
