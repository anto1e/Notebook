using System;
using System.Collections.Generic;
using System.Linq;
using MySqlConnector;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace FineNotes
{
    public partial class ModifyPage : ContentPage
    {
        List<string> users;
        Session session = Session.getInstance();
        NotesCollection notCol = NotesCollection.getInstance();
        int number;
        int type;
        public ModifyPage(Note note)
        {
            InitializeComponent();
            if (note.Author != session.Email)
                SidebarBtn.IsVisible = false;
            if (Device.RuntimePlatform == Device.iOS)
            {
                Header.Padding = new Thickness(0, 35, 0, 0);
            }
            number = note.Number;
            Note_header.Text = note.Header;
            Note_msg.Text = note.Message;
            Email_label.Text = note.Author;
            Date_label.Text = note.Date;
            type = note.Type;
        }
        private bool side_hided = true;
        private List<string> getSharedUsers()           //Получение списка расшареных пользователей из БД для данной заметки
        {
            string connStr = "server=sql11.freesqldatabase.com;user=sql11505068;database=sql11505068;port=3306;password=qGc1gqsgCv";
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();
                List<string> temp = new List<string>();
                string note_to_find = "'" + number*-1 + "'";
                string databaseTable = "SharedUsers";
                var query = "SELECT Shared FROM " + databaseTable + " WHERE Number = " + note_to_find;
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        temp.Add(reader.GetString(0));
                    }
                }
                conn.Close();
                return temp;
            }
            catch (Exception e)
            {
                conn.Close();
                return null;
            }
        }
        private async void showSidebarClicked(object sender, EventArgs e)   //Функция показа / скрытия сайдбара
        {
            users = getSharedUsers();
            var template = new DataTemplate(typeof(TextCell));
            template.SetValue(TextCell.TextColorProperty, Color.FromHex("FFEEB1"));
            usersList.ItemTemplate = template;
            template.SetBinding(TextCell.TextProperty, ".");
            if (side_hided)
            {
                usersList.ItemsSource = users;
                Sidebar.IsVisible = true;
                await Sidebar.TranslateTo(0, 0, 250);
                side_hided = false;
            }
            else
            {
                await Sidebar.TranslateTo(500, 0, 250);
                Sidebar.IsVisible = false;
                side_hided = true;
            }
        }
        private async void BackButton_Clicked(object sender, EventArgs e)
        {
            await Navigation.PopAsync();
        }
        private bool hided = true;
        private void ShowBtnsClicked(object sender, EventArgs e) //Функция скрывания/показа кнопок удаления и изменения
        {
            if (!hided)
            {
                buttons_layout.TranslateTo(100, 0, 250);
                ArrowFrame.RotateYTo(180, 250);
                ArrowFrame.Opacity = 0.2;
                hided = true;
            }
            else
            {
                buttons_layout.TranslateTo(0, 0, 250);
                ArrowFrame.RotateYTo(0, 250);
                ArrowFrame.Opacity = 1;
                hided = false;
            }
        }
        private void NoteChangeClicked(object sender, EventArgs e)      //Функция сохранения изменений в заметке
        {
            var note = notCol.Notes.FirstOrDefault(i => i.Number == number);
            if (note != null)
            {
                note.Header = Note_header.Text;
                note.Message = Note_msg.Text;
                note.Date = DateTime.Now.ToString();
                notCol.Save();
                session.Modified = true;
            }
            MessagingCenter.Send<Page>(this, "CollectionChanged!");
            
        }
        private async void NoteDeleteClicked(object sender, EventArgs e)        //Функция удаления заметки
        {
            var note = notCol.Notes.FirstOrDefault(i => i.Number == number);
            notCol.Notes.Remove(note);
            notCol.change_indexes();
            notCol.Save();
            session.Modified = true;
            MessagingCenter.Send<Page>(this, "CollectionChanged!");
            MessagingCenter.Send<Page>(this, "Show Toolbar!");
            await Navigation.PopAsync();
        }
        void editor_Focused(System.Object sender, Xamarin.Forms.FocusEventArgs e)
        {
            if (DeviceInfo.Platform.Equals(DevicePlatform.iOS))
            {
                scrollView.ScrollToAsync(scrollView.ScrollX, scrollView.ScrollY, true);
            }
        }
        private void SharedUserSelected(object sender, SelectedItemChangedEventArgs e)      //Функция удаления расшаренного пользователя
        {
            if (e.SelectedItem != null)
            {
                string user_to_del = "'"+e.SelectedItem.ToString()+"'";
                string connStr = "server=sql11.freesqldatabase.com;user=sql11505068;database=sql11505068;port=3306;password=qGc1gqsgCv";
                MySqlConnection conn = new MySqlConnection(connStr);
                try
                {
                    conn.Open();
                    string databaseTable = "SharedUsers";
                    var query = "DELETE FROM " + databaseTable + " WHERE Shared=" + user_to_del + " AND Number = " + number*-1;
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    var result = cmd.ExecuteScalar();
                    conn.Close();
                    users.Remove(e.SelectedItem.ToString());
                    usersList.ItemsSource = null;
                    usersList.ItemsSource = users;
                }
                catch (Exception ex)
                {
                    conn.Close();
                    return;
                }
            }
        }
        private void AddSharedUser(object sender, EventArgs e)      //Функция добавления расшареного пользователя
        {
            string user_to_add = email_shared.Text.ToString();
            string connStr = "server=sql11.freesqldatabase.com;user=sql11505068;database=sql11505068;port=3306;password=qGc1gqsgCv";
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();
                string databaseTable = "SharedUsers";
                if (isUserExists(user_to_add))
                {
                    var query = "INSERT INTO " + databaseTable + " VALUES (" + "'" + number * -1 + "','"+ user_to_add + "'"+")";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    var result = cmd.ExecuteScalar();
                    conn.Close();
                    users.Add(user_to_add);
                    usersList.ItemsSource = null;
                    usersList.ItemsSource = users;
                }
            }
            catch (Exception ex)
            {
                conn.Close();
                return;
            }
        }
        private bool isUserExists(string user_to_add)     //Функция проверки существования пользователя
        {
            string connStr = "server=sql11.freesqldatabase.com;user=sql11505068;database=sql11505068;port=3306;password=qGc1gqsgCv";
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();
                string databaseTable = "Users";
                var query = "SELECT * FROM " + databaseTable + " WHERE Email=" + "'"+user_to_add+"'";
                MySqlCommand cmd = new MySqlCommand(query, conn);
                var result = cmd.ExecuteScalar();
                conn.Close();
                email_shared.Text = "";
                if (result != null && user_to_add!=session.Email)
                    return true;
                else
                    return false;
            }
            catch (Exception e)
            {
                conn.Close();
                return false;
            }
        }
    }
}   

