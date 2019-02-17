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
using System.Windows.Threading;
using System.Net.NetworkInformation;
using Microsoft.Win32;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.ComponentModel;
using System.Management;
using System.Security.Cryptography;
using System.Security;
using System.Collections;
using MySql.Data.MySqlClient;

namespace Client
{
    public partial class MainWindow : Window
    {
        public string AlMacAdresi()
        {
            NetworkInterface[] nics = NetworkInterface.GetAllNetworkInterfaces();
            String sMacAdresi = string.Empty;
            foreach (NetworkInterface adapter in nics)
            {
                if (sMacAdresi == String.Empty)
                {
                    IPInterfaceProperties properties = adapter.GetIPProperties();
                    sMacAdresi = adapter.GetPhysicalAddress().ToString();
                }
            }
            return sMacAdresi;
        }
        public string tempdosyasi = System.IO.Path.Combine(System.IO.Path.GetTempPath(), "Rota_HP.ini"); 
        public string mysqlgiris = "Server=localhost;Database=oyun;Uid=root;Pwd='';";
        public string version = "1.0";
        public string macadres;
        DispatcherTimer timer = new DispatcherTimer();
        public MainWindow()
        {
            try
            {

                InitializeComponent();
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.IsEnabled = true;
                timer.Tick += Timergenel;
                if (File.Exists(tempdosyasi))
                {
                    TextReader tr = new StreamReader(tempdosyasi);
                    string a1 = tr.ReadLine();
                    string a2 = tr.ReadLine();
                    string a3 = tr.ReadLine();
                    string a4 = tr.ReadLine();
                    KullaniciAdı.Text = Convert.ToString(a2);
                    SampKonum.Text = Convert.ToString(a4);
                    tr.Close();
                    MAC.Text = AlMacAdresi();
                }
                else
                {
                    TextWriter tw = new StreamWriter(tempdosyasi);
                    tw.Close();
                }
                Surum.Text = version;
                Tarih.Text = DateTime.Now.ToString("d/M/yyyy");
                LogEkleyici();
            }
            catch (Exception h)
            {
                MessageBox.Show("Muhtemelen verilen .dll dosyaları, .exe dosyası ile aynı klasörde değil\nLütfen Çalıştırabilmek için .dll dosyalarını ve .exe dosyasını aynı klasörde tutun.\n Eğer başka bir hata ise 'Tamam' butonuna basarak Hata koduna erişebilirsiniz.(Hata Kodunu geliştiriciye gönderin.)", "Client Hatası");
                MessageBox.Show("Hata: " + h, "Hata Kodu");
                Close();
            }

        }
        private void LogEkleyici()
        {
            MySqlConnection baglan = new MySqlConnection(mysqlgiris);
            baglan.Open();
            if (KullaniciAdı.Text != "")
            {
                string Saat = DateTime.Now.ToString("hh:mm:ss");
                MySqlCommand ekle = new MySqlCommand("insert into loglar (kullaniciadi,macadresi,surum,tarih,saat) values ('" + KullaniciAdı.Text + "','" + MAC.Text + "','" + Surum.Text + "', '" + Tarih.Text + "', '" + Saat + "')", baglan);
                object sonuc = ekle.ExecuteNonQuery();
            }
            baglan.Close();
        }
        private void Btn2_Click(object Sender, RoutedEventArgs e)
        {
            using (var dialog = new System.Windows.Forms.FolderBrowserDialog())
            {
                bas:
                dialog.Description = "GTA:SA'nın kurulu olduğu klasörü seçiniz.";
                System.Windows.Forms.DialogResult result = dialog.ShowDialog();
                SampKonum.Text = dialog.SelectedPath;
                if (dialog.SelectedPath == "")
                {
                    MessageBox.Show("Lütfen bu alanı boş bırakmayın!", "Rota Roleplay");
                    goto bas;
                }
            }
        }
        private void X_PreviewMouseDown(object Sender, RoutedEventArgs e)
        {
            Close();
        }
        private void Btn1_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (new StreamReader(new WebClient().OpenRead("http://185.169.52.93/bannedmacids.php")).ReadToEnd().Contains(AlMacAdresi()))
                {
                    MessageBox.Show("Bilgisayarınız bu sunucuda yasaklanmış.\nHata olduğunu düşünüyorsanız,\nDiscord sunucumuzdan bildirebilirsiniz.", "Rota RP");
                    Close();
                }   
                else
                {
                    try
                    {
                        WebClient webclient = new WebClient();
                        Stream stream = webclient.OpenRead("http://185.169.52.93/updatecheck.php?version=" + version);
                        StreamReader reader = new StreamReader(stream);
                        string result = reader.ReadToEnd();
                        if (result == "OK" || result == "RETURN")
                        {
                            MessageBox.Show("Client güncel değil, Discord sunucumuzdan yeni versiyonunu indirebilirsiniz.", "Rota RP Güncelleme");
                            Close();
                        }
                        else if (result == "NO")
                        {
                            if (KullaniciAdı.Text == "Allah" || KullaniciAdı.Text == "Erdogan" || KullaniciAdı.Text == "Atatürk" || KullaniciAdı.Text == "Ataturk" || KullaniciAdı.Text == "")
                            {
                                MessageBox.Show("Bu adı kullanamazsınız. -> " + KullaniciAdı.Text, KullaniciAdı.Text);
                            }
                            else
                            {
                                if (Process.GetProcessesByName("samp").Length > 0 || Process.GetProcessesByName("gta_sa").Length > 0)
                                {
                                    MessageBox.Show("Arkaplanda SAMP veya GTA açıkken oyunu başlatamazsınız.", "Rota RP");
                                }
                                else
                                {
                                    if (Process.GetProcessesByName("Rota RP Client").Length > 1 || Process.GetProcessesByName("Client").Length > 1 || Process.GetProcessesByName("Rota_HP_Client").Length > 1)
                                    {
                                        MessageBox.Show("Fazladan Client açık iken oyunu başlatamazsınız.", "Rota RP");
                                    }
                                    else if (Directory.Exists(SampKonum.Text + "/mod_sa"))
                                    {
                                        MessageBox.Show("mod_sa adlı klasörü silmeniz gerekmektedir.", "Rota RP");
                                    }
                                    else
                                    {
                                        try
                                        {
                                            if (Directory.Exists(SampKonum.Text + "/cleo"))
                                            {
                                                if (Directory.GetFiles(SampKonum.Text + "/cleo", "*.cs").Length == 0)
                                                {

                                                }
                                                else
                                                {
                                                    var cleodir = new DirectoryInfo(SampKonum.Text + "/cleo");
                                                    var bulunandosyalar = cleodir.GetFiles("*.cs");

                                                    var izinsizDosyalar = bulunandosyalar.Where(x => !Izinler.Contains((int)x.Length));
                                                    if (!izinsizDosyalar.Any())
                                                    {
                                                        SAMPCalistir();
                                                    }
                                                    else
                                                    {
                                                        var str = string.Empty;
                                                        foreach (var dosya in izinsizDosyalar)
                                                        {
                                                            MessageBox.Show(str += "İzinsiz CLEO Modu bulundu! -> " + dosya + " [" + dosya.Length + "]", "İzinli CLEO");
                                                        }
                                                        Close();
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                SAMPCalistir();
                                            }
                                        }
                                        catch (Exception h)
                                        {
                                            MessageBox.Show("Hata: " + h, "Client Hatası");
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception h)
                    {
                        MessageBox.Show("Hata: " + h, "Client Hatası");
                    }
                }
            }
            catch (Exception h)
            {
                MessageBox.Show("Hata: " + h, "Client Hatası");
            }
        }
		private static readonly HashSet<int> Izinler = new HashSet<int>
        {
            18,
            18114,
            18138,
            17924,
            17560,
            18692,
            18218,
            44338,
            18039,
            17662,
            19288
        };
        private void SAMPCalistir()
        {
            RegistryKey kayitdefteri = Registry.CurrentUser.OpenSubKey("Software\\SAMP", true);
            if (kayitdefteri != null)
            {
                kayitdefteri.SetValue("PlayerName", KullaniciAdı.Text, RegistryValueKind.String);
                kayitdefteri.Close();
            }
            Process.Start(SampKonum.Text + "/samp.exe", "localhost ");
            Close();
        }
        private void Timergenel(object sender, EventArgs e)
        {
            Ping p = new Ping();
            PingReply olcer;
            string ip;
            ip = "185.169.52.93";
            olcer = p.Send(ip);
            if (olcer.Status == IPStatus.Success)
            {
                PingSayisi.Text = olcer.RoundtripTime.ToString() + " ms";
            }
            Query.Query sQuery = new Query.Query("185.169.52.93", 7777);
            sQuery.Send('c');
            int count = sQuery.Receive();
            string[] info = sQuery.Store(count);
            for (int i = 1; i < count; i++)
                AktifSayisi.Text = (info[i]);
            MySqlConnection connect = new MySqlConnection(mysqlgiris);
            using (MySqlCommand cmd = new MySqlCommand("SELECT COUNT(*) FROM loglar", connect))
            {
                connect.Open();
                long girisyapansayisi = (long)cmd.ExecuteScalar();
                GirisSayisi.Text = Convert.ToString(girisyapansayisi);
                connect.Close();
            }
            MySqlConnection connect2 = new MySqlConnection(mysqlgiris);
            using (MySqlCommand cmd = new MySqlCommand("SELECT COUNT(*) FROM accounts", connect2))
            {
                connect2.Open();
                long kayitlisayisi = (long)cmd.ExecuteScalar();
                KayitliSayi.Text = Convert.ToString(kayitlisayisi);
                connect2.Close();
            }
        }
        private void Ayarlar_click(object sender, RoutedEventArgs e)
        {

            TextWriter tw = new StreamWriter(tempdosyasi);
            tw.WriteLine("[Kullanıcı Adı]");
            tw.WriteLine(KullaniciAdı.Text);
            tw.WriteLine("[Dosya Konumu]");
            tw.WriteLine(SampKonum.Text);
            tw.WriteLine("[MAC]");
            tw.WriteLine(MAC.Text);
            tw.WriteLine("[Sürüm]");
            tw.WriteLine(Surum.Text);
            tw.Close();
            MessageBox.Show("Ayarlar başarıyla kaydedildi!", "Rota RP");
        }
        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            if (e.ButtonState == MouseButtonState.Pressed)
                DragMove();
        }
    }
namespace Query
    {
        class RCONQuery
        {
            Socket qSocket;
            IPAddress address;
            int _port = 0;
            string _password = null;

            string[] results = new string[50];
            int _count = 0;

            public RCONQuery(string IP, int port, string password)
            {
                qSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                qSocket.SendTimeout = 5000;
                qSocket.ReceiveTimeout = 5000;

                try
                {
                    address = Dns.GetHostAddresses(IP)[0];
                }

                catch
                {

                }

                _port = port;
                _password = password;
            }

            public bool Send(string command)
            {
                try
                {
                    IPEndPoint endpoint = new IPEndPoint(address, _port);

                    using (MemoryStream stream = new MemoryStream())
                    {
                        using (BinaryWriter writer = new BinaryWriter(stream))
                        {
                            writer.Write("SAMP".ToCharArray());

                            string[] SplitIP = address.ToString().Split('.');

                            writer.Write(Convert.ToByte(Convert.ToInt32(SplitIP[0])));
                            writer.Write(Convert.ToByte(Convert.ToInt32(SplitIP[1])));
                            writer.Write(Convert.ToByte(Convert.ToInt32(SplitIP[2])));
                            writer.Write(Convert.ToByte(Convert.ToInt32(SplitIP[3])));

                            writer.Write((ushort)_port);

                            writer.Write('x');

                            writer.Write((ushort)_password.Length);
                            writer.Write(_password.ToCharArray());

                            writer.Write((ushort)command.Length);
                            writer.Write(command.ToCharArray());
                        }

                        if (qSocket.SendTo(stream.ToArray(), endpoint) > 0)
                            return true;
                    }
                }

                catch
                {
                    return false;
                }

                return false;
            }

            public int Rceive()
            {
                try
                {
                    for (int i = 0; i < results.GetLength(0); i++)
                        results.SetValue(null, i);

                    _count = 0;

                    EndPoint endpoint = new IPEndPoint(address, _port);

                    byte[] rBuffer = new byte[500];

                    int count = qSocket.ReceiveFrom(rBuffer, ref endpoint);

                    using (MemoryStream stream = new MemoryStream(rBuffer))
                    {
                        using (BinaryReader reader = new BinaryReader(stream))
                        {
                            if (stream.Length <= 11)
                                return _count;

                            reader.ReadBytes(11);
                            short len;

                            try
                            {
                                while ((len = reader.ReadInt16()) != 0)
                                    results[_count++] = new string(reader.ReadChars(Convert.ToInt32(len)));
                            }

                            catch
                            {
                                return _count;
                            }
                        }
                    }
                }

                catch
                {
                    return _count;
                }

                return _count;
            }

            public string[] Store(int count)
            {
                string[] rString = new string[count];

                for (int i = 0; i < count && i < _count; i++)
                    rString[i] = results[i];

                _count = 0;

                return rString;
            }
        }

        class Query
        {
            Socket qSocket;
            IPAddress address;
            int _port = 0;

            string[] results;
            int _count = 0;

            DateTime[] timestamp = new DateTime[2];

            public Query(string IP, int port)
            {
                qSocket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

                qSocket.SendTimeout = 5000;
                qSocket.ReceiveTimeout = 5000;

                try
                {
                    address = Dns.GetHostAddresses(IP)[0];
                }

                catch
                {

                }

                _port = port;
            }

            public bool Send(char opcode)
            {
                try
                {
                    EndPoint endpoint = new IPEndPoint(address, _port);

                    using (MemoryStream stream = new MemoryStream())
                    {
                        using (BinaryWriter writer = new BinaryWriter(stream))
                        {
                            writer.Write("SAMP".ToCharArray());

                            string[] SplitIP = address.ToString().Split('.');

                            writer.Write(Convert.ToByte(Convert.ToInt32(SplitIP[0])));
                            writer.Write(Convert.ToByte(Convert.ToInt32(SplitIP[1])));
                            writer.Write(Convert.ToByte(Convert.ToInt32(SplitIP[2])));
                            writer.Write(Convert.ToByte(Convert.ToInt32(SplitIP[3])));

                            writer.Write((ushort)_port);

                            writer.Write(opcode);

                            if (opcode == 'p')
                                writer.Write("8493".ToCharArray());

                            timestamp[0] = DateTime.Now;
                        }

                        if (qSocket.SendTo(stream.ToArray(), endpoint) > 0)
                            return true;
                    }
                }

                catch
                {
                    return false;
                }

                return false;
            }

            public int Receive()
            {
                try
                {
                    _count = 0;

                    EndPoint endpoint = new IPEndPoint(address, _port);

                    byte[] rBuffer = new byte[3402];
                    qSocket.ReceiveFrom(rBuffer, ref endpoint);

                    timestamp[1] = DateTime.Now;

                    using (MemoryStream stream = new MemoryStream(rBuffer))
                    {
                        using (BinaryReader reader = new BinaryReader(stream))
                        {
                            if (stream.Length <= 10)
                                return _count;

                            reader.ReadBytes(10);

                            switch (reader.ReadChar())
                            {
                                case 'i': // Information
                                    {
                                        results = new string[6];

                                        results[_count++] = Convert.ToString(reader.ReadByte());

                                        results[_count++] = Convert.ToString(reader.ReadInt16());

                                        results[_count++] = Convert.ToString(reader.ReadInt16());

                                        int hostnamelen = reader.ReadInt32();
                                        results[_count++] = new string(reader.ReadChars(hostnamelen));

                                        int gamemodelen = reader.ReadInt32();
                                        results[_count++] = new string(reader.ReadChars(gamemodelen));

                                        int languagelen = reader.ReadInt32();
                                        results[_count++] = new string(reader.ReadChars(languagelen));

                                        return _count;
                                    }

                                case 'r': // Rules
                                    {
                                        int rulecount = reader.ReadInt16();

                                        results = new string[rulecount * 2];

                                        for (int i = 0; i < rulecount; i++)
                                        {
                                            int rulelen = reader.ReadByte();
                                            results[_count++] = new string(reader.ReadChars(rulelen));

                                            int valuelen = reader.ReadByte();
                                            results[_count++] = new string(reader.ReadChars(valuelen));
                                        }

                                        return _count;
                                    }

                                case 'c': // Client list
                                    {
                                        int playercount = reader.ReadInt16();

                                        results = new string[playercount * 2];

                                        for (int i = 0; i < playercount; i++)
                                        {
                                            int namelen = reader.ReadByte();
                                            results[_count++] = new string(reader.ReadChars(namelen));

                                            results[_count++] = Convert.ToString(reader.ReadInt32());
                                        }

                                        return _count;
                                    }

                                case 'd': // Detailed player information
                                    {
                                        int playercount = reader.ReadInt16();

                                        results = new string[playercount * 4];

                                        for (int i = 0; i < playercount; i++)
                                        {
                                            results[_count++] = Convert.ToString(reader.ReadByte());

                                            int namelen = reader.ReadByte();
                                            results[_count++] = new string(reader.ReadChars(namelen));

                                            results[_count++] = Convert.ToString(reader.ReadInt32());
                                            results[_count++] = Convert.ToString(reader.ReadInt32());
                                        }

                                        return _count;
                                    }

                                case 'p': // Ping
                                    {
                                        results = new string[1];

                                        results[_count++] = timestamp[1].Subtract(timestamp[0]).Milliseconds.ToString();

                                        return _count;
                                    }

                                default:
                                    return _count;
                            }
                        }
                    }
                }

                catch
                {
                    return _count;
                }
            }

            public string[] Store(int count)
            {
                string[] rString = new string[count];

                for (int i = 0; i < count && i < _count; i++)
                    rString[i] = results[i];

                _count = 0;

                return rString;
            }
        }
    }
}