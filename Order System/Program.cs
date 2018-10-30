                            //##############################################
                            //#                                            #
                            //#                                            #
                            //#        Author: Mihai Daniel Eremia         #
                            //#      Reference Number: BMC171812756        #
                            //#         Release Date: 30/09/2018           #
                            //#                                            #
                            //#                                            #
                            //##############################################
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Runtime.InteropServices;
using System.Threading;
using System.Text.RegularExpressions;
using ConsoleTables;


namespace Order_System
{
    public class Database
    {
        public const string PathDB = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename='C:\Users\mihai\OneDrive - Birmingham Metropolitan College\Visual Projects\Order System\Order System\System.mdf';Integrated Security=True";
        public static bool Login(string email, string password)
        {
            var con = new SqlConnection();
            con.ConnectionString = PathDB;
            SqlCommand scmd = new SqlCommand("select count(*) from dbo.Customers where email=@mail and password=@pwd", con);
            scmd.Parameters.Clear();
            scmd.Parameters.AddWithValue("@mail", email);
            scmd.Parameters.AddWithValue("@pwd", Encrypt.MD5(password));
            con.Open();
            if (scmd.ExecuteScalar().ToString() == "1")
            {
                con.Close();
                return true;
            }
            else
            {
                con.Close();
                return false;
            }
        }
        public static bool Register(string email, string first_name, string last_name, string password, string postcode, string address, string number)
        {
            var con = new SqlConnection();
            con.ConnectionString = PathDB;
            SqlCommand scmd = new SqlCommand("INSERT INTO dbo.Customers (first_name,last_name,address,postcode,password,number,email) VALUES (@first,@last,@address,@postcode,@password,@number,@email)", con);
            scmd.Parameters.AddWithValue("@first", first_name.Trim());
            scmd.Parameters.AddWithValue("@last", last_name.Trim());
            scmd.Parameters.AddWithValue("@address", address.Trim());
            scmd.Parameters.AddWithValue("@postcode", postcode.Trim());
            scmd.Parameters.AddWithValue("@password", Encrypt.MD5(password.Trim()));
            scmd.Parameters.AddWithValue("@number", number.Trim());
            scmd.Parameters.AddWithValue("@email", email.Trim());
            try
            {
                con.Open();
                int recordsAffected = scmd.ExecuteNonQuery();
                return true;
            }
            catch (SqlException)
            {
                return false;
            }
            finally
            {
                con.Close();
            }
        }
        public static bool CheckEmail(string email)
        {
            var con = new SqlConnection();
            con.ConnectionString = PathDB;
            SqlCommand scmd = new SqlCommand("select count(*) from dbo.Customers where email=@mail", con);
            scmd.Parameters.Clear();
            scmd.Parameters.AddWithValue("@mail", email);
            con.Open();
            if (scmd.ExecuteScalar().ToString() == "1")
            {
                con.Close();
                return true;
            }
            else
            {
                con.Close();
                return false;
            }
        }
        public static bool CheckProductExists(int id)
        {
            var con = new SqlConnection();
            con.ConnectionString = PathDB;
            SqlCommand scmd = new SqlCommand("select count(*) from dbo.Meniu where id=@id", con);
            scmd.Parameters.Clear();
            scmd.Parameters.AddWithValue("@id", id);
            con.Open();
            if (scmd.ExecuteScalar().ToString() == "1")
            {
                con.Close();
                return true;
            }
            else
            {
                con.Close();
                return false;
            }
        }
        public static bool CheckHistoryOrderByID(int id,int id_customer)
        {
            var con = new SqlConnection();
            con.ConnectionString = PathDB;
            SqlCommand scmd = new SqlCommand("select count(*) from dbo.Orders where id=@id AND id_customer=@id_customer", con);
            scmd.Parameters.Clear();
            scmd.Parameters.AddWithValue("@id", id);
            scmd.Parameters.AddWithValue("@id_customer", id_customer);
            con.Open();
            if (scmd.ExecuteScalar().ToString() == "1")
            {
                con.Close();
                return true;
            }
            else
            {
                con.Close();
                return false;
            }
        }
        public static void CheckHistoryOrderByID(int id)
        {
            var con = new SqlConnection();
            con.ConnectionString = PathDB;
            SqlCommand scmd = new SqlCommand("select * from dbo.Orders WHERE id=@id", con);
            scmd.Parameters.Clear();
            scmd.Parameters.AddWithValue("@id", id);
            con.Open();
            SqlDataReader reader = scmd.ExecuteReader();
            string items = null;
            float total = 0.0f;
            while (reader.Read())
            {
                items = reader["items"].ToString();
                total = Convert.ToSingle(reader["cost"]);
            }
            var table = new ConsoleTable("Name", "Price","Quantity","Total Price");
            var data = items.Split('-');
            Dictionary<string, string> product = new Dictionary<string, string>();
            foreach(var pair in data)
            {
                var item = pair.Split('.');
                product = GetMeniuDataById(Convert.ToInt32(item[0]));
                table.AddRow(product["name"], "£"+Convert.ToSingle(product["price"]), item[1], "£"+Convert.ToSingle(product["price"])* Convert.ToSingle(item[1]));
            }
            table.Write();
            Console.WriteLine(" The total cost for this order was £{0}",total);
        }
        public static void GetMeniu()
        {
            var con = new SqlConnection();
            con.ConnectionString = PathDB;
            SqlCommand scmd = new SqlCommand("select * from dbo.Meniu", con);

            con.Open();
            SqlDataReader reader = scmd.ExecuteReader();
            var table = new ConsoleTable("ID", "Name", "Price");
           
            while (reader.Read())
            {

                table.AddRow(reader[0], reader[1],"£" + (Convert.ToSingle(reader[2])).ToString());
                 
            }
            table.Write();
            Console.WriteLine();
            con.Close();
        }
        public static void Checkout(string items,int id,float cost)
        {
            var con = new SqlConnection();
            con.ConnectionString = PathDB;
            SqlCommand scmd = new SqlCommand("INSERT INTO dbo.Orders (items,id_customer,cost) VALUES (@items,@id_customer,@cost)", con);
            scmd.Parameters.AddWithValue("@items", items.Trim());
            scmd.Parameters.AddWithValue("@id_customer", id);
            scmd.Parameters.AddWithValue("@cost", cost);
            try
            {
                con.Open();
                int recordsAffected = scmd.ExecuteNonQuery();
               
            }
            catch (SqlException)
            {

            }
            finally
            {
                con.Close();
            }
        }
        public static void GetHistoryOrders(int id_customer)
        {
            var con = new SqlConnection();
            con.ConnectionString = PathDB;
            SqlCommand scmd = new SqlCommand("select * from dbo.Orders WHERE id_customer=@id", con);
            scmd.Parameters.Clear();
            scmd.Parameters.AddWithValue("@id", id_customer);
            con.Open();
            SqlDataReader reader = scmd.ExecuteReader();
            var table = new ConsoleTable("ID Order", "Price", "Date");

            while (reader.Read())
            {

                table.AddRow(reader["Id"], "£" + (Convert.ToSingle(reader["cost"])).ToString(),reader["date"]);

            }
            table.Write();
            Console.WriteLine();
            con.Close();
        }
        public static Dictionary<string,string> GetMeniuDataById(int id)
        {
            var con = new SqlConnection();
            con.ConnectionString = PathDB;
            SqlCommand scmd = new SqlCommand("select * from dbo.Meniu where Id=@id", con);
            scmd.Parameters.Clear();
            scmd.Parameters.AddWithValue("@id", id);
            con.Open();
            SqlDataReader reader = scmd.ExecuteReader();
            Dictionary<string, string> data = new Dictionary<string, string>();
            while (reader.Read())
            {
                data["name"] = reader["name"].ToString();
                data["price"] = reader["price"].ToString();
            }
            con.Close();
            return data;
        }
        public static Dictionary<string, string> GetDataCustomer(string email, string password)
        {
            Dictionary<string, string> data = new Dictionary<string, string>();
            var con = new SqlConnection();
            con.ConnectionString = PathDB;
            SqlCommand scmd = new SqlCommand("select * from dbo.Customers where email=@mail and password=@pwd", con);
            scmd.Parameters.Clear();
            scmd.Parameters.AddWithValue("@mail", email);
            scmd.Parameters.AddWithValue("@pwd", Encrypt.MD5(password));
            con.Open();
            using (SqlDataReader oReader = scmd.ExecuteReader())
            {
                while (oReader.Read())
                {
                    data["email"] = oReader["email"].ToString();
                    data["address"] = oReader["address"].ToString();
                    data["postcode"] = oReader["postcode"].ToString();
                    data["number"] = oReader["number"].ToString();
                    data["first_name"] = oReader["first_name"].ToString();
                    data["last_name"] = oReader["last_name"].ToString();
                    data["id"] = oReader["id"].ToString();
                }
                con.Close();
            }
            return data;
        }
    }
    class Font
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Unicode)]
        internal unsafe struct CONSOLE_FONT_INFO_EX
        {
            internal uint cbSize;
            internal uint nFont;
            internal COORD dwFontSize;
            internal int FontFamily;
            internal int FontWeight;
            internal fixed char FaceName[LF_FACESIZE];
        }
        [StructLayout(LayoutKind.Sequential)]
        internal struct COORD
        {
            internal short X;
            internal short Y;

            internal COORD(short x, short y)
            {
                X = x;
                Y = y;
            }
        }
        private const int STD_OUTPUT_HANDLE = -11;
        private const int TMPF_TRUETYPE = 4;
        private const int LF_FACESIZE = 32;
        private static IntPtr INVALID_HANDLE_VALUE = new IntPtr(-1);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool SetCurrentConsoleFontEx(
            IntPtr consoleOutput,
            bool maximumWindow,
            ref CONSOLE_FONT_INFO_EX consoleCurrentFontEx);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr GetStdHandle(int dwType);


        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int SetConsoleFont(
            IntPtr hOut,
            uint dwFontNum
            );

        public static void SetConsoleFont(string fontName, short size)
        {
            unsafe
            {
                IntPtr hnd = GetStdHandle(STD_OUTPUT_HANDLE);
                if (hnd != INVALID_HANDLE_VALUE)
                {
                    CONSOLE_FONT_INFO_EX info = new CONSOLE_FONT_INFO_EX();
                    info.cbSize = (uint)Marshal.SizeOf(info);

                    // Set console font to Lucida Console.
                    CONSOLE_FONT_INFO_EX newInfo = new CONSOLE_FONT_INFO_EX();
                    newInfo.cbSize = (uint)Marshal.SizeOf(newInfo);
                    newInfo.FontFamily = TMPF_TRUETYPE;
                    IntPtr ptr = new IntPtr(newInfo.FaceName);
                    Marshal.Copy(fontName.ToCharArray(), 0, ptr, fontName.Length);
                    info.dwFontSize.X = (short)size;
                    info.dwFontSize.Y = (short)size;
                    // Get some settings from current font.
                    newInfo.dwFontSize = new COORD(info.dwFontSize.X, info.dwFontSize.Y);
                    newInfo.FontWeight = info.FontWeight;
                    SetCurrentConsoleFontEx(hnd, false, ref newInfo);
                }
            }
        }

    }
    class Encrypt
    {
        public static string MD5(string password)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(password);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                // Convert the byte array to hexadecimal string
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
    class Customers
    {
        public Customers(int action, string email, string password, string first_name = null, string last_name = null, string postcode = null, string address = null, string number = null)
        {
            switch (action)
            {
                case 0: //Register
                    {
                        Register(email, first_name, last_name, password, postcode, address, number);
                        break;
                    }
                case 1: //Login
                    {
                        Login(email, password);
                        break;
                    }
            }
        }
        public void Register(string email, string first_name, string last_name, string password, string postcode, string address, string number)
        {
            if (Database.CheckEmail(email))
            {
                Auth = false;
                Error = "The email is used. Try with other email.";
            }
            else
            {
                if (Database.Register(email, first_name, last_name, password, postcode, address, number))
                {
                    Auth = true;
                    Data = Database.GetDataCustomer(email, password);
                }
                else
                {
                    Auth = false;
                    Error = "The database have a problem.";
                }
            }
        }
        public void Login(string email, string password)
        {
            if (Database.Login(email, password))
            {
                Data = Database.GetDataCustomer(email, password);
                Auth = true;
            }
            else
            {
                Auth = false;
            }
        }
        public Dictionary<string, string> Data { get; set; }
        public bool Auth { get; set; }
        public string Error { get; set; }
    }
    public class Meniu
    {
        Customers client;
        Dictionary<int, int> cart = new Dictionary<int, int>();
        public void DrawBanner()
        {
            Console.WriteLine(Properties.Resources.Banner);       
        }
        public void Help()
        {
            Step1:
            Console.Clear();
            DrawBanner();
            Console.WriteLine();
            Console.WriteLine(Properties.Resources.Help);
            Console.Write("#:");
            switch (Console.ReadLine().ToString().ToLower())
            {
                case "/back":
                    {
                        MainMeniu();
                        break;
                    }
                default:
                    goto Step1;
            }
        }
        bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }
        public void ClientChecker() //Register || Login
        {
            LoopWrongChar:
            DrawBanner();
            Console.WriteLine();
            Console.Write("Do you already have an account? (Y|N):");
            switch (Console.ReadKey().Key)
            {
                case ConsoleKey.Y:
                    {
                        var attemptsFailed = 0;
                        var attempts = 0;
                        LoopWrongData:
                        if (attemptsFailed > 2)
                        {
                            Console.Clear();
                            Console.WriteLine("You have used too many login attempts, the application will go out in 5 seconds.");
                            Thread.Sleep(5000);
                            Environment.Exit(0);
                        }
                        LoopValidation:
                        if (attempts > 2)
                        {
                            Console.Clear();
                            Console.WriteLine("You have used too many login attempts, the application will go out in 5 seconds.");
                            Thread.Sleep(5000);
                            Environment.Exit(0);
                        }
                        Console.Clear();
                        DrawBanner();
                        Console.WriteLine("Now you need to login in your account.");
                        Console.Write(Environment.NewLine + "Email:"); var email = Console.ReadLine();
                        Console.Write(Environment.NewLine + "Password:"); var pass = Console.ReadLine();

                        if (!IsValidEmail(email))
                        {
                            attempts++;
                            Console.WriteLine("Attempts: {0}/3", attempts);
                            Console.WriteLine("This email isn't a valid email.Retry login in 3 seconds.");
                            Thread.Sleep(3000);
                            goto LoopValidation;
                        }
                        if (pass.Trim().Length < 6 || pass.Trim().Length > 25)
                        {
                            attempts++;
                            Console.WriteLine("Attempts: {0}/3", attempts);
                            Console.WriteLine("The password must be between 6 and 25 characters.Retry login in 3 seconds.");
                            Thread.Sleep(3000);
                            goto LoopValidation;
                        }
                        client = new Customers(1, email, pass);
                        Thread.Sleep(100);
                        if (!client.Auth)
                        {
                            attemptsFailed++;
                            Console.WriteLine("Attempts: {0}/3", attemptsFailed);
                            Console.WriteLine("Logging failed, check your email and password.");
                            Thread.Sleep(3000);
                            goto LoopWrongData;
                        }
                        Console.Clear();
                        Console.WriteLine("                         Welcome Back {0} {1}", client.Data["first_name"].Trim(), client.Data["last_name"].Trim());
                        Console.Title = client.Data["first_name"].Trim() + " " + client.Data["last_name"].Trim();
                        Thread.Sleep(1000);
                        break;
                    }
                case ConsoleKey.N:
                    {
                        var attemptsFailed = 0;
                        var attempts = 0;
                        LoopWrongData:
                        if (attemptsFailed > 3)
                        {
                            Console.Clear();
                            Console.WriteLine("You have used too many register attempts, the application will go out in 5 seconds.");
                            Thread.Sleep(5000);
                            Environment.Exit(0);
                        }
                        LoopValidation:
                        if (attempts > 9)
                        {
                            Console.Clear();
                            Console.WriteLine("You have used too many register attempts, the application will go out in 5 seconds.");
                            Thread.Sleep(5000);
                            Environment.Exit(0);
                        }
                        Console.Clear();
                        DrawBanner();
                        Console.WriteLine("Now you need to create your own account.");
                        Console.Write(Environment.NewLine + "Email:"); var email = Console.ReadLine();
                        if (!IsValidEmail(email))
                        {
                            attempts++;
                            Console.WriteLine("Attempts: {0}/10", attempts);
                            Console.WriteLine("This email isn't a valid email. Retry register in 3 seconds.");
                            Thread.Sleep(3000);
                            goto LoopValidation;
                        }
                        if (Database.CheckEmail(email))
                        {
                            Console.WriteLine("This email is used. Retry register with other email in 3 seconds.");
                            Thread.Sleep(3000);
                            goto LoopValidation;
                        }
                        Console.Write(Environment.NewLine + "Password:"); var pass = Console.ReadLine();
                        if (pass.Trim().Length < 6 || pass.Trim().Length > 25)
                        {
                            attempts++;
                            Console.WriteLine("Attempts: {0}/10", attempts);
                            Console.WriteLine("The password must be between 6 and 25 characters. Retry register in 3 seconds.");
                            Thread.Sleep(3000);
                            goto LoopValidation;
                        }
                        Console.Write(Environment.NewLine + "First Name:"); var first_name = Console.ReadLine();
                        if (first_name.Length < 4 || first_name.Length > 30)
                        {
                            attempts++;
                            Console.WriteLine("Attempts: {0}/10", attempts);
                            Console.WriteLine("The first name must be between 4 and 30 characters. Retry register in 3 seconds.");
                            Thread.Sleep(3000);
                            goto LoopValidation;
                        }
                        Console.Write(Environment.NewLine + "Last Name:"); var last_name = Console.ReadLine();
                        if (last_name.Length < 4 || last_name.Length > 30)
                        {
                            attempts++;
                            Console.WriteLine("Attempts: {0}/10", attempts);
                            Console.WriteLine("The last name must be between 4 and 30 characters. Retry register in 3 seconds.");
                            Thread.Sleep(3000);
                            goto LoopValidation;
                        }
                        Console.Write(Environment.NewLine + "Number (Ex:+447743997976):"); var number = Console.ReadLine();
                        if (number.Length < 9 || last_name.Length > 15)
                        {
                            attempts++;
                            Console.WriteLine("Attempts: {0}/10", attempts);
                            Console.WriteLine("The number must be between 9 and 15 digit.Ex:+447743997974. Retry register in 3 seconds.");
                            Thread.Sleep(3000);
                            goto LoopValidation;
                        }
                        Console.Write(Environment.NewLine + "Adress:"); var address = Console.ReadLine();
                        if (address.Length < 25 || address.Length > 255)
                        {
                            attempts++;
                            Console.WriteLine("Attempts: {0}/10", attempts);
                            Console.WriteLine("The address must be between 25 and 255 characters. Retry register in 3 seconds.");
                            Thread.Sleep(3000);
                            goto LoopValidation;
                        }
                        Console.Write(Environment.NewLine + "Postcode:"); var postcoded = Console.ReadLine();
                        if(postcoded.Length<3 || postcoded.Length>9)
                        {
                            attempts++;
                            Console.WriteLine("Attempts: {0}/10", attempts);
                            Console.WriteLine("The postcode must be between 3 and 10 characters. Retry register in 3 seconds.");
                            Thread.Sleep(3000);
                            goto LoopValidation;
                        }


                        client = new Customers(0, email, pass, first_name, last_name, postcoded, address, number);
                        Thread.Sleep(100);
                        if (!client.Auth)
                        {
                            attemptsFailed++;
                            Console.WriteLine("Attempts: {0}/3", attemptsFailed);
                            Console.WriteLine(client.Error);
                            Thread.Sleep(3000);
                            goto LoopWrongData;
                        }
                        Console.Clear();
                        Console.WriteLine("                         Welcome {0} {1}", client.Data["first_name"].Trim(), client.Data["last_name"].Trim());
                        Console.Title = client.Data["first_name"].Trim() + " " + client.Data["last_name"].Trim();
                        Thread.Sleep(1000);
                        break;
                    }
                default:
                    {
                        Console.Clear();
                        goto LoopWrongChar;
                    }
            }
        }
        public void MainMeniu()
        {
            Main:
            Console.Clear();
            DrawBanner();
            Console.WriteLine();
            Console.WriteLine("                         We have few commands to use our software.\n");
            Console.WriteLine("[/history] - This command will show to you, your last orders.");
            Console.WriteLine("[/basket]  - This command will show to you, your current basket.");
            Console.WriteLine("[/menu]    - This command will show to you, our main menu.");
            Console.WriteLine("[/help]    - This command will show the help section.");
            Console.WriteLine("[/exit]    - This command will close the software.");
            Console.WriteLine("[/me]      - This command will show your details.\n");
            Console.Write("#:");
            switch (Console.ReadLine().ToString().ToLower())
            {
                case "/me":
                    {
                        Step:
                        Console.Clear();
                        Console.WriteLine("                         Your details are here.");
                        Console.WriteLine();
                        Console.WriteLine("First Name : {0}", client.Data["first_name"].Trim());
                        Console.WriteLine("Last Name  : {0}", client.Data["last_name"].Trim());
                        Console.WriteLine("Email      : {0}", client.Data["email"].Trim());
                        Console.WriteLine("Number     : {0}", client.Data["number"].Trim());
                        Console.WriteLine("Address    : {0}", client.Data["address"].Trim());
                        Console.WriteLine("Postcode   : {0}", client.Data["postcode"].Trim());
                        Console.WriteLine();
                        Console.WriteLine("Use [/back] to go to main meniu.");
                        Console.Write("#:");
                        if(Console.ReadLine().ToLower()=="/back")
                        {
                            goto Main;
                        }
                        else
                        {
                            goto Step;
                        }
                    }
                case "/menu":
                    {
                        AddAgain:
                        Console.Clear();
                        Database.GetMeniu();
                        Console.WriteLine(" If you want to add a product in basket, you need to use this command:");
                        Console.WriteLine();
                        Console.WriteLine("     Ex: /add [ID] [Quantity] "+Environment.NewLine+ "       [ID]       = Unique ID from meniu."+Environment.NewLine+"       [Quantity] = How many products do you want to buy.");
                        Console.WriteLine("\n If you want to go to main menu, you need to use [/back].");
                        Console.Write("#:");
                        var data = Console.ReadLine();
                        var command = data.Split(' ');
                        switch(command[0].ToString().ToLower())
                        {
                            case "/add":
                                {
                                    if (command.Length==3 && Database.CheckProductExists(Convert.ToInt32(command[1])) && Regex.IsMatch(Convert.ToString(command[2]), @"^\d+$"))
                                    {
                                        cart[Convert.ToInt32(command[1])] = Convert.ToInt32(command[2]);
                                        Console.WriteLine("The product was added in basket. Use the same command to add other product or [/back].");
                                        Thread.Sleep(1500);
                                        goto AddAgain;
                                    }
                                    else
                                    {
                                        goto AddAgain;
                                    }
                                
                                }
                            case "/back":
                                {
                                    goto Main;
                                }
                            default:
                                goto AddAgain;
                               
                        }
                    }
                case "/exit":
                    {
                        Environment.Exit(0);
                        break;
                    }
                case "/basket":
                    {
                        Step1:
                        Console.Clear();
                        var keys = cart.Keys.ToArray();
                        var table = new ConsoleTable("ID", "Name", "Price","Quantity","Total");
                        var total = 0.0f;
                        var items = "";
                        foreach (var data in keys)
                        {
                            var Product = Database.GetMeniuDataById(Convert.ToInt32(data));
                            items = items +data+"."+cart[data]+"-";
                            table.AddRow(data, Product["name"], "£" +Convert.ToSingle(Product["price"]), cart[data], "£" + Convert.ToSingle(Product["price"]) * Convert.ToSingle(cart[data]));
                            total = total + (Convert.ToSingle(Product["price"]) * Convert.ToSingle(cart[data]));
                            
                        }
                        try
                        {
                            items = items.Remove(items.Length - 1);
                        }
                        catch (Exception)
                        {
                            
                        }
                        table.Write();
                        Console.WriteLine();
                        Console.WriteLine(" The total price is: £{0}",total);
                        Console.WriteLine(" /edit [ID] [New Quantity] - Edit quantity.");
                        Console.WriteLine();
                        Console.WriteLine(" /remove [ID] - Remove a product from basket.");
                        Console.WriteLine(" /checkout    - Send order to shop.");
                        Console.WriteLine(" /back        - Go to main menu.");
                        Console.Write("#:");
                        var read = Console.ReadLine();
                        var command = read.Split(' ');
                        switch(command[0].ToLower())
                        {
                            case "/edit":
                                {
                                    if(command.Length==3)
                                    {
                                        if (Regex.IsMatch(Convert.ToString(command[1]), @"^\d+$")  &&
                                            cart.ContainsKey(Convert.ToInt32(command[1])) &&
                                            Convert.ToInt32(command[2]) > 0 && Regex.IsMatch(Convert.ToString(command[2]), @"^\d+$"))
                                        {
                                            cart[Convert.ToInt32(command[1])] = Convert.ToInt32(command[2]);
                                            goto Step1;
                                        }
                                        else
                                        {
                                            goto Step1;
                                        }
                                    }
                                    else
                                    {
                                        goto Step1;
                                    }
                                }
                            case "/remove":
                                {
                                    if(command.Length==2)
                                    {
                                        if(Regex.IsMatch(Convert.ToString(command[1]), @"^\d+$") &&
                                            cart.ContainsKey(Convert.ToInt32(command[1])))
                                        {
                                            cart.Remove(Convert.ToInt32(command[1]));
                                            goto Step1;
                                        }
                                        else
                                        {
                                            goto Step1;
                                        }
                                    }
                                    else
                                    {
                                        goto Step1;
                                    }
                                    
                                }
                            case "/back":
                                {
                                    goto Main;
                                }
                            case "/checkout":
                                {
                                    if (keys.Length > 0)
                                    {
                                        Database.Checkout(items, Convert.ToInt32(client.Data["id"]), total);
                                        cart.Clear();
                                        Console.WriteLine("Your basket was sent to shop, you can view your orders history in [/history].");
                                        Thread.Sleep(1000);
                                    }
                                    else
                                    {
                                        Console.WriteLine("You don't have products in basket.");
                                        Thread.Sleep(1000);
                                    }
                                    goto Main;
                                }
                            default:
                                goto Step1;
                        }
                        
                    }
                case "/history":
                    {
                        Step:
                        Console.Clear();
                        Database.GetHistoryOrders(Convert.ToInt32(client.Data["id"]));
                        Console.WriteLine(" /view [ID] - View more about order.");
                        Console.WriteLine(" /back      - Go to main menu.");
                        Console.Write("#:");
                        var data = Console.ReadLine();
                        var command = data.Split(' ');
                        switch (command[0].ToLower())
                        {
                            case "/back":
                                {
                                    goto Main;
                                }
                            case "/view":
                                {
                                    if(command.Length==2 && Regex.IsMatch(Convert.ToString(command[1]), @"^\d+$") && Database.CheckHistoryOrderByID(Convert.ToInt32(command[1]),Convert.ToInt32(client.Data["id"])))
                                    {
                                        Step1:
                                        Console.Clear();
                                        Database.CheckHistoryOrderByID(Convert.ToInt32(command[1]));
                                        Console.WriteLine(" /back - Go to all history orders.");
                                        Console.Write("#:");
                                        if(Console.ReadLine().ToLower()=="/back")
                                        {
                                            goto Step;
                                        }
                                        else
                                        {
                                            goto Step1;
                                        }
                                    }
                                    else
                                    {
                                        goto Step;
                                    }
                                }
                        }
                        break;
                    }
                case "/help":
                    {
                        Help();
                        break;
                    }
                default:
                    {
                        goto Main; 
                    }
            }
        }
    }
    class Program
    {
        private const int MF_BYCOMMAND = 0x00000000;
        public const int SC_CLOSE = 0xF060;
        public const int SC_MINIMIZE = 0xF020;
        public const int SC_MAXIMIZE = 0xF030;
        public const int SC_SIZE = 0xF000;
        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);
        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        [DllImport("kernel32.dll")]
        static extern bool SetConsoleMode(IntPtr hConsoleHandle, int mode);

        [DllImport("kernel32.dll")]
        static extern bool GetConsoleMode(IntPtr hConsoleHandle, out int mode);

        [DllImport("kernel32.dll")]
        static extern IntPtr GetStdHandle(int handle);

        public const int STD_INPUT_HANDLE = -10;
        public const int ENABLE_EXTENDED_FLAGS = 0x80;
        public const int ENABLE_QUICK_EDIT = 0x0040;
        public static void DisableQuickEditMode()
        {
            int mode;
            IntPtr handle = GetStdHandle(STD_INPUT_HANDLE);
            GetConsoleMode(handle, out mode);
            mode |= ENABLE_EXTENDED_FLAGS;
            SetConsoleMode(handle, mode);
            mode &= ~ENABLE_QUICK_EDIT;
            SetConsoleMode(handle, mode);
        }
        static void Main(string[] args)
        {
            Console.WindowWidth = 87;
            Console.WindowHeight = 45;
            DisableQuickEditMode();
            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_MAXIMIZE, MF_BYCOMMAND);
            DeleteMenu(GetSystemMenu(GetConsoleWindow(), false), SC_SIZE, MF_BYCOMMAND);
            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Green;
            Font.SetConsoleFont("Aharoni", 30);
            Meniu steps = new Meniu();
            steps.ClientChecker();
            steps.MainMeniu();
            Console.ReadLine();
        }
    }  
}
                            //##############################################
                            //#                                            #
                            //#                                            #
                            //#        Author: Mihai Daniel Eremia         #
                            //#      Reference Number: BMC171812756        #
                            //#         Release Date: 30/09/2018           #
                            //#                                            #
                            //#                                            #
                            //##############################################