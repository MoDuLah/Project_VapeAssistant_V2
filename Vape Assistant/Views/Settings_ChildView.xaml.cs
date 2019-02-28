using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Vape_Assistant.Properties;

namespace Vape_Assistant.Views
{
    /// <summary>
    /// Interaction logic for EighthChildViewModel.xaml
    /// </summary>
    public partial class Settings_ChildView : UserControl
    {
        public string CurrentCulture = Settings.Default.Culture;
        public string connectionString = Settings.Default.VaConnect;
        bool IsPinEnabled = Settings.Default.isPinEnabled;
        public int autotimeout = 5000;
        SQLiteDataReader reader;
        SQLiteConnection dbConn; // Declare the SQLiteConnection-Object
        SQLiteCommand dbCmd;
        public string query;
        public string message, errmsg;
        public string caption;
        public string title;
        public string de = "de-DE";
        public string en = "en-US";
        public string es = "es-ES";
        public string gr = "el-GR";
        public string fr = "fr-FR";
        public string it = "it-IT";
        public string ro = "ro-RO";
        public string ru = "ru-RU";

        CultureInfo resourceCulture = Thread.CurrentThread.CurrentCulture;

        public Settings_ChildView()
        {
            InitializeComponent();
            Settings_Language.SelectedIndex = Settings.Default.CultureIndex;
            if (IsPinEnabled == true)
            {
                pin_Slider.Value = 1;
                disablingPin.Visibility = Visibility.Visible;
                enablingPin.Visibility = Visibility.Hidden;
                pin_Slider.IsEnabled = false;
                GroupBoxer.Visibility = Visibility.Collapsed;
                db_selector.Visibility = Visibility.Collapsed;
                db_description.Visibility = Visibility.Collapsed;
                GroupImport.Visibility = Visibility.Collapsed;
            }
            else
            {
                pin_Slider.Value = 0;
                pin_Slider.IsEnabled = true;
                GroupBoxer.Visibility = Visibility.Visible;
                db_selector.Visibility = Visibility.Visible;
                db_description.Visibility = Visibility.Visible;
                GroupImport.Visibility = Visibility.Visible;
            }
            CultureInfo currentCulture = Thread.CurrentThread.CurrentCulture;
            string currentLanguage = currentCulture.ToString();
            if (currentLanguage == de)
            {
                Settings_Language.SelectedIndex = 0;
            }
            if (currentLanguage == en)
            {
                Settings_Language.SelectedIndex = 1;
            }
            if (currentLanguage == es)
            {
                Settings_Language.SelectedIndex = 2;
            }
            if (currentLanguage == fr)
            {
                Settings_Language.SelectedIndex = 3;
            }
            if (currentLanguage == gr)
            {
                Settings_Language.SelectedIndex = 4;
            }
            if (currentLanguage == it)
            {
                Settings_Language.SelectedIndex = 5;
            }
            if (currentLanguage == ro)
            {
                Settings_Language.SelectedIndex = 6;
            }
            if (currentLanguage == ru)
            {
                Settings_Language.SelectedIndex = 7;
            }
        }


        private void Lang_Label_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show(Settings_Language.SelectedIndex.ToString());
        }
        private void Pin_Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {

            if (pin_Slider.Value == 1)
            {
                enablingPin.Visibility = Visibility.Visible;
                disablingPin.Visibility = Visibility.Hidden;
                pin_Slider.IsEnabled = false;
            }
            else
            {
                enablingPin.Visibility = Visibility.Hidden;
                disablingPin.Visibility = Visibility.Visible;
                pin_Slider.IsEnabled = false;
            }
        }

        private void Settings_Language_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int i = Settings_Language.SelectedIndex;
            string message = "";
            switch (i)
            {
                case 0:
                    if (Settings.Default.Culture != de)
                    {
                        message = "Das Programm hat einen Neustart, damit die Änderungen wirksam werden können." +
                            " Drücken Sie die Taste rechts.";
                        message += "\n\nDie Übersetzung in dieser Sprache ist nicht vollständig." +
                            " Wenn Sie uns helfen möchten, senden Sie uns eine Nachricht.";
                        Settings.Default.Culture = de;
                        Settings.Default.CultureIndex = 0;

                    }
                    break;
                case 1:
                    if (Settings.Default.Culture != en)
                    {
                        message = "The program has restart so changes can take effect." +
                                  " Press the button on the right.";
                        Settings.Default.Culture = en;
                        Settings.Default.CultureIndex = 1;

                    }
                    break;
                case 2:
                    if (Settings.Default.Culture != es)
                    {
                        message = "El programa se ha reiniciado para que los cambios surtan efecto." +
                            " Presiona el botón de la derecha.";
                        message += "La traducción en este idioma no está completa." +
                            " Si quieres ayudarnos, envíanos un mensaje.";
                        Settings.Default.Culture = es;
                        Settings.Default.CultureIndex = 2;

                    }
                    break;
                case 3:
                    if (Settings.Default.Culture != fr)
                    {
                        message = "Le programme a redémarré pour que les modifications puissent prendre effet." +
                            " Appuyez sur le bouton à droite.";
                        message += "La traduction dans cette langue n'est pas complète." +
                            " Si vous voulez nous aider, envoyez-nous un message.";
                        Settings.Default.Culture = fr;
                        Settings.Default.CultureIndex = 3;

                    }
                    break;
                case 4:
                    if (Settings.Default.Culture != gr)
                    {
                        message = "Το πρόγραμμα πρέπει να επανεκκινήσει για να εφαρμοστούν οι αλλαγές." +
                            " Πατήστε το κουμπί στα δεξιά.";
                        Settings.Default.Culture = gr;
                        Settings.Default.CultureIndex = 4;
                    }
                    break;
                case 5:
                    if (Settings.Default.Culture != it)
                    {
                        message = "Il programma è stato riavviato in modo che le modifiche possano avere effetto." +
                            " Premi il pulsante a destra.";
                        message += "La traduzione in questa lingua non è completa." +
                            " Se vuoi aiutarci, inviaci un messaggio.";
                        Settings.Default.Culture = it;
                        Settings.Default.CultureIndex = 5;
                    }
                    break;
                case 6:
                    if (Settings.Default.Culture != ro)
                    {
                        message = "Programul are o repornire, astfel încât schimbările pot intra în vigoare." +
                            " Apăsați butonul din dreapta.";
                        message += "\n\nTraducerea în această limbă nu este completă. Dacă vrei să ne ajuți, trimite-ne un mesaj.";
                        Settings.Default.Culture = ro;
                        Settings.Default.CultureIndex = 6;
                    }
                    break;
                case 7:
                    if (Settings.Default.Culture != ru)
                    {
                        message = "Программа перезапустится, чтобы изменения вступили в силу." +
                            " Нажмите кнопку справа.";
                        message += "Перевод на этом языке не завершен. Если вы хотите помочь нам, отправьте нам сообщение.";
                        Settings.Default.Culture = ru;
                        Settings.Default.CultureIndex = 7;
                    }
                    break;
            }
            if (message.Length > 0)
            {
                AutoClosingMessageBox.Show(message, "", autotimeout);
            }
            Settings.Default.Save();
        }

        private void app_restart_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(Application.ResourceAssembly.Location);
            Application.Current.Shutdown();
        }

        private void EnablingPinSubmit_Click(object sender, RoutedEventArgs e)
        {
            string chpw_error_catch = "";

            if (string.IsNullOrEmpty(enablePinCode.Password))
            {
                chpw_error_catch = chpw_error_catch + "1";
                message = "";
                title = "";
                int i = Settings_Language.SelectedIndex;
                switch (i)
                {
                    case 0:
                        if (Settings.Default.Culture == de)
                        {
                            message = "Die PIN darf nicht leer sein.";
                            title = "Fehler";
                        }
                        break;
                    case 1:
                        if (Settings.Default.Culture == en)
                        {
                            message = "The PIN cannot be blank.";
                            title = "Error: ";
                        }
                        break;
                    case 2:
                        if (Settings.Default.Culture == es)
                        {
                            message = "El número de PIN no puede estar en blanco.";
                            title = "Error: ";
                        }
                        break;
                    case 3:
                        if (Settings.Default.Culture == fr)
                        {
                            message = "Le code PIN ne peut pas être vide.";
                            title = "Erreur: ";
                        }
                        break;
                    case 4:
                        if (Settings.Default.Culture == gr)
                        {
                            message = "To PIN δεν μπορεί να είναι κενό.";
                            title = "Σφάλμα: ";
                        }
                        break;
                    case 5:
                        if (Settings.Default.Culture == it)
                        {
                            message = "Il PIN non può essere vuoto.";
                            title = "Errore: ";
                        }
                        break;
                    case 6:
                        if (Settings.Default.Culture == ro)
                        {
                            message = "Codul PIN nu poate fi gol.";
                            title = "Eroare: ";
                        }
                        break;
                    case 7:
                        if (Settings.Default.Culture == ru)
                        {
                            message = "PIN-код не может быть пустым.";
                            title = "Ошибка: ";
                        }
                        break;
                }

                AutoClosingMessageBox.Show(message, title + chpw_error_catch,autotimeout);
                IsPinEnabled = false;
                return;
            }
            if (IsPinEnabled == false)
            {
                Settings.Default.isPinEnabled = true;
                IsPinEnabled = true;
            }

            try
            {
                using (SQLiteConnection dbConn = new SQLiteConnection(connectionString))
                {
                    if (dbConn.State == ConnectionState.Closed)
                    {
                        dbConn.Open();
                    }

                    string query = $"UPDATE Accounts SET passWord = '{Hash(enablePinCode.Password)}' WHERE id = '1' ; ";
                    dbCmd = new SQLiteCommand(query, dbConn);

                    int retVal = dbCmd.ExecuteNonQuery();
                    message = "";
                    title = "";
                    int i = Settings_Language.SelectedIndex;
                    switch (i)
                    {
                        case 0:
                            if (Settings.Default.Culture == de)
                            {
                                message = "Pin hat sich erfolgreich geändert!";
                                title = "Erfolg";
                            }
                            break;
                        case 1:
                            if (Settings.Default.Culture == en)
                            {
                                message = "Pin has changed successfully!";
                                title = "Success";
                            }
                            break;
                        case 2:
                            if (Settings.Default.Culture == es)
                            {
                                message = "Pin ha cambiado con éxito!";
                                title = "El exito";
                            }
                            break;
                        case 3:
                            if (Settings.Default.Culture == fr)
                            {
                                message = "Le Pin a été changé avec succès!";
                                title = "Le succès";
                            }
                            break;
                        case 4:
                            if (Settings.Default.Culture == gr)
                            {
                                message = "Το Pin έχει αλλάξει επιτυχώς!";
                                title = "Επιτυχία";
                            }
                            break;
                        case 5:
                            if (Settings.Default.Culture == it)
                            {
                                message = "Il pin è cambiato con successo!";
                                title = "Successo";
                            }
                            break;
                        case 6:
                            if (Settings.Default.Culture == ro)
                            {
                                message = "PIN-ul sa schimbat cu succes!";
                                title = "Succes";
                            }
                            break;
                        case 7:
                            if (Settings.Default.Culture == ru)
                            {
                                message = "Пин успешно изменен!";
                                title = "Успех";
                            }
                            break;
                    }
                    AutoClosingMessageBox.Show(message, title, autotimeout);

                    if (dbConn.State == ConnectionState.Open)
                    {
                        dbConn.Close();
                    }
                    Settings.Default.isPinEnabled = true;
                    pin_Slider.IsEnabled = false;
                    pin_Slider.Value = 1;
                    disablingPin.Visibility = Visibility.Visible;
                    enablingPin.Visibility = Visibility.Hidden;
                    GroupBoxer.Visibility = Visibility.Collapsed;
                    GroupImport.Visibility = Visibility.Collapsed;
                    db_selector.Visibility = Visibility.Collapsed;
                    db_description.Visibility = Visibility.Collapsed;

                }
                dbCmd.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,title);
                Settings.Default.isPinEnabled = false;
                
            }
            finally
            {
                enablingPin.Visibility = Visibility.Collapsed;
                enablePinCode.Password = null;
                Settings.Default.Save();
                GC.Collect();
            }
        }
        private void DisablingPinSubmit_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(disablePin_Box.Password)) { return; }
            using (SQLiteConnection con = new SQLiteConnection(connectionString))
            {
                try
                {
                    con.Open();
                    if (con.State == ConnectionState.Open)
                    {

                        string Query = $"SELECT * FROM [Accounts] WHERE [passWord]='{Hash(disablePin_Box.Password)}' ; ";
                        SQLiteCommand createCommand = new SQLiteCommand(Query, con);

                        createCommand.ExecuteNonQuery();
                        SQLiteDataReader dr = createCommand.ExecuteReader();

                        int count = 0;
                        while (dr.Read())
                        {
                            count++;
                        }
                        if (count == 1)
                        {
                            pin_Slider.IsEnabled = true;
                            pin_Slider.Value = 0;
                            Settings.Default.isPinEnabled = false;
                            Settings.Default.Save();
                            dr.Dispose();
                            createCommand.Dispose();
                            disablingPin.Visibility = Visibility.Hidden;
                            enablingPin.Visibility = Visibility.Visible;
                            GroupBoxer.Visibility = Visibility.Visible;
                            db_selector.Visibility = Visibility.Visible;
                            db_description.Visibility = Visibility.Visible;
                            GroupImport.Visibility = Visibility.Visible;
                            GC.Collect();
                        }
                        else
                        {
                            message = "";
                            title = "";
                            int i = Settings_Language.SelectedIndex;
                            switch (i)
                            {
                                case 0:
                                    if (Settings.Default.Culture == de)
                                    {
                                        message = "Die PIN ist falsch.";
                                        title = "Fehler";
                                    }
                                    break;
                                case 1:
                                    if (Settings.Default.Culture == en)
                                    {
                                        message = "The Pin is incorrect.";
                                        title = "Error";
                                    }
                                    break;
                                case 2:
                                    if (Settings.Default.Culture == es)
                                    {
                                        message = "El PIN es incorrecto.";
                                        title = "Error";
                                    }
                                    break;
                                case 3:
                                    if (Settings.Default.Culture == fr)
                                    {
                                        message = "Le code PIN est incorrect.";
                                        title = "Erreur";
                                    }
                                    break;
                                case 4:
                                    if (Settings.Default.Culture == gr)
                                    {
                                        message = "Το Pin είναι λάθος.";
                                        title = "Σφάλμα";
                                    }
                                    break;
                                case 5:
                                    if (Settings.Default.Culture == it)
                                    {
                                        message = "Il PIN non è corretto.";
                                        title = "Errore";
                                    }
                                    break;
                                case 6:
                                    if (Settings.Default.Culture == ro)
                                    {
                                        message = "Codul PIN este incorect.";
                                        title = "Eroare";
                                    }
                                    break;
                                case 7:
                                    if (Settings.Default.Culture == ru)
                                    {
                                        message = "PIN-код неверен.";
                                        title = "Ошибка";
                                    }
                                    break;
                            }
                            AutoClosingMessageBox.Show(message, title, autotimeout);

                            disablePin_Box.Password = null;
                            Settings.Default.isPinEnabled = true;
                            Settings.Default.Save();
                            dr.Dispose();
                            createCommand.Dispose();
                        }

                    }
                }
                catch (Exception ex)
                {
                    AutoClosingMessageBox.Show(ex.ToString(),"Error",autotimeout);
                }
                finally
                {
                    disablePin_Box.Password = null;
                    Settings.Default.Save();
                    if (con.State == ConnectionState.Open)
                    {
                        con.Close();
                    }
                }
            }
        }


        static string Hash(string input)
        {
            using (SHA1Managed sha1 = new SHA1Managed())
            {
                var hash = sha1.ComputeHash(Encoding.UTF8.GetBytes(input));
                var sb = new StringBuilder(hash.Length * 2);

                foreach (byte b in hash)
                {
                    // can be "x2" if you want lowercase
                    sb.Append(b.ToString("x2"));
                }

                return sb.ToString();
            }
        }

        private void db_selector_LostFocus(object sender, RoutedEventArgs e)
        {

            if (CurrentCulture == de)
            {
                if (db_selector.SelectedIndex < 0)
                {
                    db_description.Text = null;
                }
                if (db_selector.SelectedIndex == 0)
                {
                    db_description.Text = "Umfasst Ihre Einkäufe.";
                    export.Name = "Purchases";
                }
                if (db_selector.SelectedIndex == 1)
                {
                    db_description.Text = "Es beinhaltet die Düfte, die Sie besitzen.";
                    export.Name = "Warehouse";
                }
                if (db_selector.SelectedIndex == 2)
                {
                    db_description.Text = "Es enthält die Rezepte, die Sie hinzugefügt haben. Teil A.";
                    export.Name = "RecipeBook";
                }
                if (db_selector.SelectedIndex == 3)
                {
                    db_description.Text = "Es enthält die Rezepte, die Sie hinzugefügt haben. Teil B.";
                    export.Name = "hash";
                }
                if (db_selector.SelectedIndex == 4)
                {
                    db_description.Text = "Enthält die Historienaufzeichnung gemischter Rezepte.";
                    export.Name = "RecipeLog";
                }
            }
            if (CurrentCulture == en) { 
                if (db_selector.SelectedIndex < 0)
                {
                    db_description.Text = "";
                }
                if (db_selector.SelectedIndex == 0)
                {
                    db_description.Text = "Exports your Purchases.";
                    export.Name = "Purchases";
                }
                if (db_selector.SelectedIndex == 1)
                {
                    db_description.Text = "Includes the Flavors and amounts you have in your possession.";
                    export.Name = "Warehouse";
                }
                if (db_selector.SelectedIndex == 2)
                {
                    db_description.Text = "Includes all the recipes you have added. Part A";
                    export.Name = "RecipeBook";
                }
                if (db_selector.SelectedIndex == 3)
                {
                    db_description.Text = "Includes all the recipes you have added. Part B";
                    export.Name = "hash";
                }
                if (db_selector.SelectedIndex == 4)
                {
                    db_description.Text = "Includes the history log of the recipes you have mixed";
                    export.Name = "RecipeLog";
                }
            }
            if (CurrentCulture == es)
            {
                if (db_selector.SelectedIndex < 0)
                {
                    db_description.Text = null;
                }
                if (db_selector.SelectedIndex == 0)
                {
                    db_description.Text = "Incluye tus compras.";
                    export.Name = "Purchases";
                }
                if (db_selector.SelectedIndex == 1)
                {
                    db_description.Text = "Incluye las fragancias que posees.";
                    export.Name = "Warehouse";
                }
                if (db_selector.SelectedIndex == 2)
                {
                    db_description.Text = "Incluye las recetas que has añadido. Parte A.";
                    export.Name = "RecipeBook";
                }
                if (db_selector.SelectedIndex == 3)
                {
                    db_description.Text = "Incluye las recetas que has añadido. Parte B.";
                    export.Name = "hash";
                }
                if (db_selector.SelectedIndex == 4)
                {
                    db_description.Text = "Incluye registro histórico de recetas mixtas.";
                    export.Name = "RecipeLog";
                }
            }
            if (CurrentCulture == fr)
            {
                if (db_selector.SelectedIndex < 0)
                {
                    db_description.Text = null;
                }
                if (db_selector.SelectedIndex == 0)
                {
                    db_description.Text = "Inclut vos achats.";
                    export.Name = "Purchases";
                }
                if (db_selector.SelectedIndex == 1)
                {
                    db_description.Text = "Cela inclut les parfums que vous possédez.";
                    export.Name = "Warehouse";
                }
                if (db_selector.SelectedIndex == 2)
                {
                    db_description.Text = "Il comprend les recettes que vous avez ajoutées. Partie A.";
                    export.Name = "RecipeBook";
                }
                if (db_selector.SelectedIndex == 3)
                {
                    db_description.Text = "Il comprend les recettes que vous avez ajoutées. Partie B.";
                    export.Name = "hash";
                }
                if (db_selector.SelectedIndex == 4)
                {
                    db_description.Text = "Comprend un historique des recettes variées.";
                    export.Name = "RecipeLog";
                }
            }
            if (CurrentCulture == gr)
            {
                if (db_selector.SelectedIndex < 0)
                {
                    db_description.Text = null;
                }
                if (db_selector.SelectedIndex == 0)
                {
                    db_description.Text = "Περιλαμβάνει τις αγορές σας.";
                    export.Name = "Purchases";
                }
                if (db_selector.SelectedIndex == 1)
                {
                    db_description.Text = "Περιλαμβάνει τα αρώματα που έχετε στην κατοχή σας.";
                    export.Name = "Warehouse";
                }
                if (db_selector.SelectedIndex == 2)
                {
                    db_description.Text = "Περιλαμβάνει τις συνταγές που έχετε προσθέσει. Μέρος Α.";
                    export.Name = "RecipeBook";
                }
                if (db_selector.SelectedIndex == 3)
                {
                    db_description.Text = "Περιλαμβάνει τις συνταγές που έχετε προσθέσει. Μέρος Β.";
                    export.Name = "hash";
                }
                if (db_selector.SelectedIndex == 4)
                {
                    db_description.Text = "Περιλαμβάνει το αρχείο καταγραφής ιστορικού των συνταγών που έχουν αναμειχθεί.";
                    export.Name = "RecipeLog";
                }
            }
            if (CurrentCulture == it)
            {
                if (db_selector.SelectedIndex < 0)
                {
                    db_description.Text = null;
                }
                if (db_selector.SelectedIndex == 0)
                {
                    db_description.Text = "Include i tuoi acquisti.";
                    export.Name = "Purchases";
                }
                if (db_selector.SelectedIndex == 1)
                {
                    db_description.Text = "Include i profumi che possiedi.";
                    export.Name = "Warehouse";
                }
                if (db_selector.SelectedIndex == 2)
                {
                    db_description.Text = "Include le ricette che hai aggiunto. Parte A.";
                    export.Name = "RecipeBook";
                }
                if (db_selector.SelectedIndex == 3)
                {
                    db_description.Text = "Include le ricette che hai aggiunto. Parte B.";
                    export.Name = "hash";
                }
                if (db_selector.SelectedIndex == 4)
                {
                    db_description.Text = "Include la cronologia delle ricette miste.";
                    export.Name = "RecipeLog";
                }
            }
            if (CurrentCulture == ro)
            {
                if (db_selector.SelectedIndex < 0)
                {
                    db_description.Text = null;
                }
                if (db_selector.SelectedIndex == 0)
                {
                    db_description.Text = "Include achizițiile dvs.";
                    export.Name = "Purchases";
                }
                if (db_selector.SelectedIndex == 1)
                {
                    db_description.Text = "Acesta include aromele pe care le dețineți.";
                    export.Name = "Warehouse";
                }
                if (db_selector.SelectedIndex == 2)
                {
                    db_description.Text = "Acesta include rețetele pe care le-ați adăugat. Partea A.";
                    export.Name = "RecipeBook";
                }
                if (db_selector.SelectedIndex == 3)
                {
                    db_description.Text = "Acesta include rețetele pe care le-ați adăugat. Partea B.";
                    export.Name = "hash";
                }
                if (db_selector.SelectedIndex == 4)
                {
                    db_description.Text = "Include istoricul rețetelor mixte.";
                    export.Name = "RecipeLog";
                }
            }
            if (CurrentCulture == ru)
            {
                if (db_selector.SelectedIndex < 0)
                {
                    db_description.Text = null;
                }
                if (db_selector.SelectedIndex == 0)
                {
                    db_description.Text = "Включает ваши покупки.";
                    export.Name = "Purchases";
                }
                if (db_selector.SelectedIndex == 1)
                {
                    db_description.Text = "Он включает в себя ароматы, которыми вы владеете.";
                    export.Name = "Warehouse";
                }
                if (db_selector.SelectedIndex == 2)
                {
                    db_description.Text = "Он включает в себя рецепты, которые вы добавили. Часть А.";
                    export.Name = "RecipeBook";
                }
                if (db_selector.SelectedIndex == 3)
                {
                    db_description.Text = "Он включает в себя рецепты, которые вы добавили. Часть Б.";
                    export.Name = "hash";
                }
                if (db_selector.SelectedIndex == 4)
                {
                    db_description.Text = "Включает запись истории смешанных рецептов.";
                    export.Name = "RecipeLog";
                }
            }
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string btnName = button.Name.ToString();
            string query;
            int count = 0;
            using (SQLiteConnection dbConn = new SQLiteConnection(connectionString))
            {
                try
                {
                    dbConn.Open();
                    if (dbConn.State == ConnectionState.Open)
                    {
                        if (btnName == "Warehouse")
                        {
                            query = $"SELECT Count(*) FROM Flavors where Owned LIKE '1' order by Brand ; ";
                        }
                        else
                        {
                            query = $"Select seq from sqlite_sequence where name = '{btnName}'";
                        }
                        dbCmd = new SQLiteCommand(query, dbConn);
                        dbCmd.ExecuteNonQuery();
                        reader = dbCmd.ExecuteReader();
                        while (reader.Read())
                        {
                            count = int.Parse(reader[0].ToString());
                        }

                    }
                    if (count == 0)
                    {
                        string message = "", title = "";
                        if (CurrentCulture == de)
                        {
                            title = "Fehler";
                            message = "Es gibt keine Datensätze in Ihrer Datenbank.\n\nDer Export wurde abgebrochen.";
                        }
                        if (CurrentCulture == en)
                        {
                            title = "Error";
                            message = "There are no entries in your database.\n\nExport Canceled.";
                        }
                        if (CurrentCulture == es)
                        {
                            title = "Error";
                            message = "No hay registros en su base de datos.\n\nSe canceló la exportación.";
                        }
                        if (CurrentCulture == fr)
                        {
                            title = "Erreur";
                            message = "Il n'y a aucun enregistrement dans votre base de données.\n\nL'exportation a été annulée.";
                        }
                        if (CurrentCulture == gr)
                        {
                            title = "Σφάλμα";
                            message = "Δεν υπάρχουν εγγραφές στη βάση δεδομένων σας.\n\nΗ εξαγωγή ακυρώθηκε.";
                        }
                        if (CurrentCulture == it)
                        {
                            title = "Errore";
                            message = "Non ci sono record nel tuo database.\n\nL'esportazione è stata annullata.";
                        }
                        if (CurrentCulture == ro)
                        {
                            title = "Eroare";
                            message = "Nu există înregistrări în baza dvs. de date.\n\nExportul a fost anulat.";
                        }
                        if (CurrentCulture == ru)
                        {
                            title = "ошибка";
                            message = "В вашей базе данных нет записей.\n\nЭкспорт был отменен.";
                        }
                        AutoClosingMessageBox.Show(message, title, 2000);
                        return;
                    }
                    if (btnName == "Warehouse")
                    {
                        doCsvWrite(btnName, "Flavors");
                    }
                    else
                    {
                        doCsvWrite(btnName, btnName);
                    }
                }
                catch (Exception ex)
                {
                    AutoClosingMessageBox.Show(ex.ToString(), "Error", autotimeout);
                    return;
                }
            }
        }
        public static DataTable NewDataTable(string fileName, string delimiters, bool firstRowContainsFieldNames = true)
        {
            DataTable result = new DataTable();

            using (TextFieldParser tfp = new TextFieldParser(fileName))
            {
                tfp.SetDelimiters(delimiters);

                // Get Some Column Names
                if (!tfp.EndOfData)
                {
                    string[] fields = tfp.ReadFields();

                    for (int i = 0; i < fields.Length; i++)
                    {
                        if (firstRowContainsFieldNames)
                            result.Columns.Add(fields[i]);
                        else
                            result.Columns.Add("Col" + i);
                    }

                    // If first line is data then add it
                    if (!firstRowContainsFieldNames)
                        result.Rows.Add(fields);
                }

                // Get Remaining Rows
                while (!tfp.EndOfData)
                    result.Rows.Add(tfp.ReadFields());
            }
            return result;
        }
        private void FilePicker_Click(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            string btnName = button.Name.ToString();
            string Path = AppDomain.CurrentDomain.BaseDirectory + "Backup\\";
            // Create OpenFileDialog 
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog
            {
                InitialDirectory = Path,
                // Set filter for file extension and default file extension 
                DefaultExt = ".csv",
                Filter = "Comma-Seperated Values (*.csv)|*.csv"//|JPEG Files (*.jpeg)|*.jpeg|PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|GIF Files (*.gif)|*.gif
            };


            // Display OpenFileDialog by calling ShowDialog method 
            bool? result = dlg.ShowDialog();


            // Get the selected file name and display in a TextBox 
            if (result == true)
            {
                // Open document 
                string filename = dlg.FileName;
                hiddenpath.Text = filename;
                int len = Path.Length;
                string tmp = filename.Substring(len, filename.Length - len);
                fileName.Text = tmp;
                string tmp2 = tmp.Substring(0, tmp.Length - 15);
                fileSelect.IsEnabled = true;
                filePreview.IsEnabled = true;
                filePreview.Visibility = Visibility.Visible;
                Preview.Visibility = Visibility.Visible;
                importTable.Visibility = Visibility.Visible;
                import_btn.Visibility = Visibility.Visible;
            }
        }

        private void Import_Click(object sender, RoutedEventArgs e)
        {
            Button btn = (Button)sender;
            string fileName = hiddenpath.Text;
            string delimiters = "|";
            DataTable dt = NewDataTable(fileName, delimiters, true);
            importTable.ItemsSource = dt.DefaultView;
            btn.IsEnabled = false;
        }

        public static string TruncateLongString(string str, int maxLength)
        {
            return str.Substring(0, Math.Min(str.Length, maxLength));
        }
        public IEnumerable<DataGridRow> GetDataGridRows(DataGrid grid)
        {
            var itemsSource = grid.ItemsSource as IEnumerable;
            if (null == itemsSource)
            {
                yield return null;
            }

            foreach (object item in itemsSource)
            {
                if (grid.ItemContainerGenerator.ContainerFromItem(item) is DataGridRow row)
                {
                    yield return row;
                }
            }
        }

        private void import_btn_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(hiddenpath.Text) || string.IsNullOrEmpty(fileName.Text)) { return; };
            string query = "";
            TextBox[] x = { x0, x1, x2, x3, x4, x5, x6, x7, x8, x9, x10, x11 };
            using (dbConn = new SQLiteConnection(connectionString))
            {
                string fName = TruncateLongString(fileName.Text, (fileName.Text.Length - 15));
                using (var reader = new StreamReader(hiddenpath.Text))
                {
                    List<string> listA = new List<string>();

                    int y = 0;
                    int i = 0;
                    int q = 0;

                    if (fName == "Purchases")
                    {
                        q = 11;
                        dbConn.Open();
                        query = $"DROP TABLE Purchases; ";
                        query += $"CREATE TABLE Purchases ( `Id` INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, `Date` TEXT NOT NULL, `Reference` TEXT NOT NULL, `Name` TEXT NOT NULL, `VID` INTEGER NOT NULL, `Description` TEXT NOT NULL, `PID` TEXT, `Quantity` REAL NOT NULL, `Price` REAL NOT NULL, `Discount` REAL NOT NULL, `Shipping` REAL NOT NULL, `Total` REAL NOT NULL )";
                        dbCmd = new SQLiteCommand(query, dbConn);
                        dbCmd.ExecuteNonQuery();
                        dbCmd = null;
                        dbConn.Close();
                    }

                    if (fName == "Warehouse")
                    {
                        q = 8;
                    }
                    if (fName == "RecipeBook" || fName == "hash")
                    {
                        q = 4;
                        if (fName == "RecipeBook")
                        {
                            dbConn.Open();
                            query = $"DROP TABLE RecipeBook; ";
                            query += $"CREATE TABLE IF NOT EXISTS `RecipeBook`(`Id` INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT UNIQUE, `RecipeName` TEXT NOT NULL, `TimesMade` INTEGER NOT NULL, `Author` TEXT NOT NULL )";
                            dbCmd = new SQLiteCommand(query, dbConn);
                            dbCmd.ExecuteNonQuery();
                            dbCmd = null;
                            dbConn.Close();
                        }
                        else
                        {
                            dbConn.Open();
                            query = $"DROP TABLE hash; ";
                            query += $"CREATE TABLE IF NOT EXISTS `hash`( `ID` INTEGER NOT NULL PRIMARY KEY AUTOINCREMENT, `RECIPE_ID` bigint ( 255 ) NOT NULL, `FLAVOUR_ID` bigint ( 255 ) NOT NULL, `PERCENTAGE` float ( 5 , 2 ) NOT NULL )";
                            dbCmd = new SQLiteCommand(query, dbConn);
                            dbCmd.ExecuteNonQuery();
                            dbCmd = null;
                            dbConn.Close();
                        }
                    }
                    if (fName == "RecipeLog")
                    {
                        q = 7;
                    }

                    while (!reader.EndOfStream)
                    {
                        query = "";
                        string line = reader.ReadLine();
                        if (!string.IsNullOrWhiteSpace(line))
                        {
                            var values = line.Split('|');

                            if (y >= 1)
                            {
                                for (i = 0; i < q; i++)
                                {
                                    listA.Add(values[i]);
                                }
                                //string x0 = "", x1 = "", x2 = "", x3 = "", x4 = "", x5 = "", x6 = "", x7 = "", x8 = "", x9 = "", x10 = "", x11 = "", x12 = ""΄;
                                dbConn.Open();
                                //for (i = 0; i < q; i++)
                                //{
                                if (fName == "Purchases")
                                {
                                    i = 0;
                                    string myString = values[i];
                                    string[] tagArray = line.Split('|');
                                    for (int o = 0; o < tagArray.Length; o++)
                                    {
                                        x[o].Text = tagArray[o].ToString();
                                    }
                                    if (i == 0)
                                    {
                                        query = $"Insert INTO {fName} ([Date], [Reference], " +
                                            $"[Name], [VID], [Description], [PID], [Quantity], [Price], [Discount], [Shipping], [Total]) " +
                                            $"VALUES( ";
                                    }
                                    for (int u = 0; u < q; u++)
                                    {
                                        if (u < (q - 1))
                                        {
                                            query += $"'{values[u].ToString().Replace("'", "''")}', ";
                                        }
                                        else
                                        {
                                            query += $"'{values[u].ToString().Replace("'", "''")}'";
                                        }
                                    }
                                    query += $") ; ";
                                    //break;

                                }
                                else
                                { 
                                if (fName == "Warehouse")
                                {
                                    string myString = values[i]; //Can be of any length and with many '-'s
                                    string[] tagArray = line.Split('|');
                                    for (int o = 0; o < tagArray.Length; o++)
                                    {
                                        x[o].Text = tagArray[o].ToString();
                                    }
                                    if (i == 0)
                                    {
                                        query = $"Update Flavors SET Amount = '{x7.Text}',Owned = '{x8.Text}' WHERE Id = '{x0.Text}' ; ";
                                    }
                                    else
                                    {
                                        query += $"Update Flavors SET Amount = '{x7.Text}',Owned = '{x8.Text}' WHERE Id = '{x0.Text}' ; ";
                                    }
                                }
                                else
                                {
                                    query = $"Insert INTO {fName} ";
                                    if (fName == "RecipeBook")
                                    {
                                        query += $"([Id], [RecipeName], [TimesMade], [Author]) ";
                                    }
                                    if (fName == "hash")
                                    {
                                        query += $"([RECIPE_ID], [FLAVOUR_ID], [PERCENTAGE]) ";
                                    }
                                    query += $"VALUES(";
                                    int Lol = 0;
                                    if (fName == "hash")
                                    {
                                        Lol = 1;
                                    }
                                    else
                                    {
                                        Lol = 0;
                                    }
                                    for (int u = Lol; u < q; u++)
                                    {
                                        if (u < (q - 1))
                                        {
                                            query += $"'{values[u].ToString().Replace("'", "''")}', ";
                                        }
                                        else
                                        {
                                            query += $"'{values[u].ToString().Replace("'", "''")}'";
                                        }
                                    }
                                    query += $") ; ";
                                    //break;
                                }
                                }
                                //}
                                dbCmd = new SQLiteCommand(query, dbConn);
                                dbCmd.ExecuteNonQuery();
                                dbCmd = null;
                                dbConn.Close();
                            }
                        }
                        y++;
                    }
                }
                importTable.ItemsSource = "";
                Preview.Visibility = Visibility.Collapsed;
                importTable.Visibility = Visibility.Collapsed;
                import_btn.Visibility = Visibility.Collapsed;
                hiddenpath.Text = "";
                fileName.Text = "";
            }

        }

        public void doCsvWrite(string sender, string TableName)
        {
            string button = sender;
            string btnName = button;
            dbConn = new SQLiteConnection(connectionString);
            try
            {
                string Path = AppDomain.CurrentDomain.BaseDirectory;
                string timeStamp = DateTime.Now.ToString("yyyy.MM.dd");
                string csvHeader = "";
                string delimiter = "|";
                //specify file name of log file (csv).
                string newfileName = Path + "Backup\\" + btnName + "-" + timeStamp + ".csv";
                if (File.Exists(newfileName))
                {
                    File.Delete(newfileName);
                }
                    //check to see if file exists, if not create an empty file with the specified file name.
                if (!File.Exists(newfileName))
                {
                    FileStream fs = new FileStream(newfileName, FileMode.CreateNew);
                    fs.Close();

                    query = $"pragma table_info({TableName.ToString()}) ;";
                    dbConn.Open();
                    dbCmd = new SQLiteCommand(query, dbConn);
                    dbCmd.ExecuteNonQuery();
                    SQLiteDataReader dr = dbCmd.ExecuteReader();

                    int count = 0;
                    while (dr.Read())
                    {
                        count++;
                        if (count == 0)
                        {
                            csvHeader = dr[1].ToString() + delimiter;
                        }
                        else
                        {
                            csvHeader = csvHeader + dr[1].ToString() + delimiter;
                        }
                    }
                    if (string.IsNullOrEmpty(csvHeader))
                    {
                    }
                    else
                    {
                        csvHeader = csvHeader.TrimEnd(csvHeader[csvHeader.Length - 1]);
                    }
                    //define header of new file, and write header to file.
                    using (FileStream fsWHT = new FileStream(newfileName, FileMode.Append, FileAccess.Write))
                    using (StreamWriter swT = new StreamWriter(fsWHT))
                    {
                        swT.WriteLine(csvHeader.ToString());
                    }
                    dr.Close();
                    dbConn.Close();
                }
                else
                {
                    query = $"pragma table_info({TableName.ToString()}) ;";
                    dbConn.Open();
                    dbCmd = new SQLiteCommand(query, dbConn);
                    dbCmd.ExecuteNonQuery();
                    SQLiteDataReader dr = dbCmd.ExecuteReader();

                    int count = 0;
                    while (dr.Read())
                    {
                        count++;
                        if (count == 0)
                        {
                            csvHeader = dr[1].ToString() + delimiter;
                        }
                        else
                        {
                            csvHeader = csvHeader + dr[1].ToString() + delimiter;
                        }
                    }
                    if (string.IsNullOrEmpty(csvHeader))
                    {
                    }
                    else
                    {
                        csvHeader = csvHeader.TrimEnd(csvHeader[csvHeader.Length - 1]);
                    }
                    dr.Close();
                    dbConn.Close();
                }

                //set up connection to database.
                if (btnName == "Warehouse")
                {
                    query = $"SELECT * FROM Flavors where Amount NOT LIKE '0.0' order by Brand ;";
                }
                else
                {
                    query = $"SELECT * FROM {TableName.ToString()} where id > 0 ; ";
                }

                try
                {
                    dbConn = new SQLiteConnection(connectionString);
                }
                catch (Exception)
                {
                    //error handling here.
                    return;
                }

                try
                {
                    dbConn.Open();
                }
                catch (Exception)
                {
                    //error handling here.
                    return;
                }
                reader = null;
                dbCmd = new SQLiteCommand(query, dbConn);
                try
                {
                    reader = dbCmd.ExecuteReader();
                    while (reader.Read())
                    {
                        if (reader[0].ToString() == "")
                        {
                            //does nothing if the current line is "bugged" (containing no values at all, typically happens after reboot of 3rd party equipment).
                        }
                        else
                        {
                            int count = (csvHeader.Split('|').Length - 1) + 1;
                            string csvDetails = "";
                            //grab relevant tag data and set the csv line for the current row.
                            for (int i = 0; i < count; i++)
                            {
                                string x = reader[i].ToString();
                                csvDetails = csvDetails + x.Trim() + "|";
                            }
                            csvDetails = csvDetails.TrimEnd(csvDetails[csvDetails.Length - 1]);
                            using (FileStream fsWDT = new FileStream(newfileName, FileMode.Append, FileAccess.Write))
                            using (StreamWriter swDT = new StreamWriter(fsWDT))
                            {
                                //write csv line to file.
                                swDT.WriteLine(csvDetails.ToString());
                            }
                        }
                    }
                    string message = "";
                    string title = "";
                    if (CurrentCulture == de)
                    {
                        message = $"Exportieren der Tabelle: {btnName} aus der Datenbank war erfolgreich! Sie finden die Datei hier: " + newfileName;
                        title = "Erfolg!";
                    }
                    if (CurrentCulture == en)
                    {
                        message = $"Exporting the table: { btnName } from the database was successful!\nYou will find the file here: "+ newfileName;
                        title = "Success!";
                    }
                    if (CurrentCulture == es)
                    {
                        message = $"La exportación de la tabla: {btnName} de la base de datos fue exitosa!\nEncontrará el archivo aquí: " + newfileName;
                        title = "El exito!";
                    }
                    if (CurrentCulture == fr)
                    {
                        message = $"L'exportation de la table: {btnName} de la base de données a réussi!\nVous trouverez le fichier ici: " + newfileName;
                        title = "Succès!";
                    }
                    if (CurrentCulture == gr)
                    {
                        message = $"Η εξαγωγή του πίνακα: {btnName} από τη βάση ήταν επιτυχής!\nΘα το βρείτε εδώ: " + newfileName;
                        title = "Επιτυχία!";
                    }
                    if (CurrentCulture == it)
                    {
                        message = $"Esportare la tabella: {btnName} dal database ha avuto successo! Troverete il file qui: " + newfileName;
                        title = "Successo!";
                    }
                    if (CurrentCulture == ro)
                    {
                        message = $"Exportarea tabelului: {btnName} din baza de date a avut succes! Veți găsi fișierul aici: " + newfileName;
                        title = "Succes!";
                    }
                    if (CurrentCulture == ru)
                    {
                        message = $"Экспорт таблицы: {btnName} из базы данных выполнен успешно! Вы найдете файл здесь: " + newfileName;
                        title = "Успех!";
                    }
                    AutoClosingMessageBox.Show(message, title, autotimeout);
                }
                catch (Exception)
                {
                    //error handling here.
                    dbConn.Close();
                    return;
                }
                dbConn.Close();
            }
            catch (Exception ex)
            {
                //error handling here.
                AutoClosingMessageBox.Show(ex.Message, "Error", autotimeout);
            }
        }

    }
}
