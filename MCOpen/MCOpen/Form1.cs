﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Compression;



namespace MCOpen
{
    public partial class Form1 : Form
    {

        string fmap; // az AppDatában található fájl neve
        string appd; // Az AppData helyszíne
        // Az előző kettő kombinálva, VIGYÁZAT! Ha out of sync, abból problémák történhetnek.
        // MINDIG külön állítsd be a fmap/appd párost!
        string folder {
            get {
                return Path.Combine(appd, fmap);
                }
        }

        public Form1()
        {
            fmap = ".MCOpen"; // A launcher mappája, ahol a játék van.
            appd = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            labelInfo.Hide(); // Infó szöveg eltüntetése
            launcherText.Text = "MCOPEN"; //Felső menü neve

            // servers.dat frissítés
            WebClient wc = new WebClient();
            Uri surl = new Uri("https://www.dropbox.com/s/1lhv8dqrafu58tb/servers.dat?dl=1");
            try {
                wc.DownloadFileAsync(surl, folder + "\\servers.dat");

                // Ha nincsen saját mappája, akkor készít és letölti a fájlokat, majd telepíti
                bool exists = Directory.Exists(folder);
                if (!exists)
                {
                    labelInfo.Text = "Letöltés folyamatban...";
                    labelInfo.Show();
                    btnLogin.Enabled = false;
                    MessageBox.Show("Letöltés megkezdődött! Ez eltarthat néhány percig is...");
                    Directory.CreateDirectory(folder);

                    wc.DownloadFileCompleted += new AsyncCompletedEventHandler(FileDownloadComplete);
                    Uri durl = new Uri("https://www.dropbox.com/s/uhpsm7pfj04nbp8/mcopen.zip?dl=1");
                    wc.DownloadFileAsync(durl, folder + "\\mcopen.zip");
                }
                else {
                    // ide kéne valami hash-alapú ellenőrzés, vagy  timestamp.
                    //A status quo az, hogy könnyen out-of-syncben lehet a release verzióval, ha nem töröljük ki manuálisan a mappát.
                }
            }
            catch {
                MessageBox.Show("Valamilyen hálózati hiba történt. Ellenőrizd az internetkapcsolatod.");
            }


        }
        private void FileDownloadComplete(object sender, AsyncCompletedEventArgs e)
        {
            labelInfo.Text = "Fájlok kicsomagolása...";
            // KICSOMAGOLÁS

            string zipPath = folder + @"\mcopen.zip";
            string extractPath = folder + @"";

            ZipFile.ExtractToDirectory(zipPath, extractPath);
            labelInfo.Hide();
            MessageBox.Show("A letöltés befejeződött! Most már elindíthatod a játékot!");
            btnLogin.Enabled = true;
        }

        #region belsős cucc, ne szerkeszd
        
        // ABLAK MOZGATÁS
        public const int WM_NCLBUTTONDOWN = 0xA1;
        public const int HT_CAPTION = 0x2;

        // ne szerkeszd
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int lParam);
        [System.Runtime.InteropServices.DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();
        
        #endregion

        // FORM1 mozgatása egérrel
        private void Form1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        
        private void label2_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void label3_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        // bezárás
        private void pictureBox1_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        //asztalra
        private void pictureBox2_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            string dirr = Environment.GetEnvironmentVariable("APPDATA") + "\\" + @".mcopen"; //alap mc mappa
            if (txtBoxUsername.Text.Length == 0 ) // ha nem írna semmit a név helyére
            {
                MessageBox.Show("Írd be a felhasználóneved!");
            }
            else //ha írt a névre, akkor a kliens indítása
            {
                try 
                {
                    string name = txtBoxUsername.Text;
                    string launch = @"java.exe" + " -Xmx" + 1024 + "M" + " -Djava.library.path=" + dirr + @"\versions\1.13\natives" + @" -cp " + // Egyelőre KÜLÖN natives mappa kell neki! 
                        dirr + @"\libraries\com\mojang\patchy\1.1\patchy-1.1.jar;" +
                        dirr + @"\libraries\oshi-project\oshi-core\1.1\oshi-core-1.1.jar;" +
                        dirr + @"\libraries\net\java\dev\jna\jna\4.4.0\jna-4.4.0.jar;" +
                        dirr + @"\libraries\net\java\dev\jna\platform\3.4.0\platform-3.4.0.jar;" +
                        dirr + @"\libraries\com\ibm\icu\icu4j-core-mojang\51.2\icu4j-core-mojang-51.2.jar;" +
                        dirr + @"\libraries\net\sf\jopt-simple\jopt-simple\5.0.3\jopt-simple-5.0.3.jar;" +
                        dirr + @"\libraries\com\paulscode\codecjorbis\20101023\codecjorbis-20101023.jar;" +
                        dirr + @"\libraries\com\paulscode\codecwav\20101023\codecwav-20101023.jar;" +
                        dirr + @"\libraries\com\paulscode\libraryjavasound\20101123\libraryjavasound-20101123.jar;" +
                        dirr + @"\libraries\com\paulscode\soundsystem\20120107\soundsystem-20120107.jar;" +
                        dirr + @"\libraries\io\netty\netty-all\4.1.25.Final\netty-all-4.1.25.Final.jar;" +
                        dirr + @"\libraries\com\google\guava\guava\21.0\guava-21.0.jar;" +
                        dirr + @"\libraries\org\apache\commons\commons-lang3\3.5\commons-lang3-3.5.jar;" +
                        dirr + @"\libraries\commons-io\commons-io\2.5\commons-io-2.5.jar;" +
                        dirr + @"\libraries\commons-codec\commons-codec\1.10\commons-codec-1.10.jar;" +
                        dirr + @"\libraries\net\java\jinput\jinput\2.0.5\jinput-2.0.5.jar;" +
                        dirr + @"\libraries\net\java\jutils\jutils\1.0.0\jutils-1.0.0.jar;" +
                        dirr + @"\libraries\com\mojang\brigadier\0.1.27\brigadier-0.1.27.jar;" +
                        dirr + @"\libraries\com\mojang\datafixerupper\1.0.16\datafixerupper-1.0.16.jar;" +
                        dirr + @"\libraries\com\google\code\gson\gson\2.8.0\gson-2.8.0.jar;" +
                        dirr + @"\libraries\com\mojang\authlib\1.5.25\authlib-1.5.25.jar;" +
                        dirr + @"\libraries\org\apache\commons\commons-compress\1.8.1\commons-compress-1.8.1.jar;" +
                        dirr + @"\libraries\org\apache\httpcomponents\httpclient\4.3.3\httpclient-4.3.3.jar;" +
                        dirr + @"\libraries\commons-logging\commons-logging\1.1.3\commons-logging-1.1.3.jar;" +
                        dirr + @"\libraries\org\apache\httpcomponents\httpcore\4.3.2\httpcore-4.3.2.jar;" +
                        dirr + @"\libraries\it\unimi\dsi\fastutil\7.1.0\fastutil-7.1.0.jar;" +
                        dirr + @"\libraries\org\apache\logging\log4j\log4j-api\2.8.1\log4j-api-2.8.1.jar;" +
                        dirr + @"\libraries\org\apache\logging\log4j\log4j-core\2.8.1\log4j-core-2.8.1.jar;" +
                        dirr + @"\libraries\org\lwjgl\lwjgl\3.1.6\lwjgl-3.1.6.jar;" +
                        dirr + @"\libraries\org\lwjgl\lwjgl-jemalloc\3.1.6\lwjgl-jemalloc-3.1.6.jar;" +
                        dirr + @"\libraries\org\lwjgl\lwjgl-openal\3.1.6\lwjgl-openal-3.1.6.jar;" +
                        dirr + @"\libraries\org\lwjgl\lwjgl-opengl\3.1.6\lwjgl-opengl-3.1.6.jar;" +
                        dirr + @"\libraries\org\lwjgl\lwjgl-glfw\3.1.6\lwjgl-glfw-3.1.6.jar;" +
                        dirr + @"\libraries\org\lwjgl\lwjgl-stb\3.1.6\lwjgl-stb-3.1.6.jar;" +
                        dirr + @"\libraries\com\mojang\realms\1.13.3\realms-1.13.3.jar;" +
                        dirr + @"\libraries\com\mojang\text2speech\1.10.3\text2speech-1.10.3.jar;" +
                        dirr + @"\versions\1.13\1.13.jar net.minecraft.client.main.Main" +
                        @" --username " + name + @" --version 1.13" + @" --gameDir " + dirr + @" --assetsDir " + dirr + @"\assets\ --assetIndex 1.13 --uuid 00000000-0000-0000-0000-000000000000 --accessToken null --userProperties [] --userType legacy --width 925 --height 530";

                        Process.Start("cmd.exe", "/C" + launch);
                }
                catch
                {
                MessageBox.Show("Nem sikerült elindítani a Minecraftot. A leggyakoribb ok az, hogy a java nincs installálva, vagy nincs benne a PATH-ban. Esetleg még megpróbálkozhatsz több memória adásával.");
                }

            }
        }
    }
}
