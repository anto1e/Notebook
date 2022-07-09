using System;
using System.Collections.Generic;
using Xamarin.Forms;
using MySqlConnector;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Text;

namespace FineNotes
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }
        private bool hided = true;
        private void ShowBtnsClicked(object sender, EventArgs e)
        {
            if (!hided)
            {
                buttons_layout.TranslateTo(100, 0, 250);
                ArrowFrame.RotateYTo(180, 250);
                ArrowFrame.Opacity = 0.2;
                hided = true;
                Email_entry.Text = "";
                Password_entry.Text = "";
            }
            else
            {
                buttons_layout.TranslateTo(0, 0, 250);
                ArrowFrame.RotateYTo(0, 250);
                ArrowFrame.Opacity = 1;
                hided = false;
                Email_entry_reg.Text = "";
                Password_entry_reg.Text = "";
                RePassword_entry_reg.Text = "";
            }
        }
        private string hashing(string password)     //Хэширование паролея
        {
            // step 1, calculate MD5 hash from input
            MD5 md5 = System.Security.Cryptography.MD5.Create();
            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(password);
            byte[] hash = md5.ComputeHash(inputBytes);
            // step 2, convert byte array to hex string
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString();
        }
        private bool isReg = false;
        private async void RegSignBtnClicked(object sender, EventArgs e)        //Нажата кнопка смены Входа/Регистрации
        {
            if (!isReg)
            {
                await SignInLayout.TranslateTo(-500, 0, 200);
                SignInLayout.IsVisible = false;
                RegisterLayout.IsVisible = true;
                await RegisterLayout.TranslateTo(0, 0, 200);
                isReg = true;
                SignBtn.IsVisible = false;
                RegBtn.IsVisible = true;
            }
            else
            {
                await RegisterLayout.TranslateTo(500, 0, 200);
                RegisterLayout.IsVisible = false;
                SignInLayout.IsVisible = true;
                await SignInLayout.TranslateTo(0, 0, 200);
                isReg = false;
                RegBtn.IsVisible = false;
                SignBtn.IsVisible = true;
            }
        }
        private async void OfflineClicked (object sender, EventArgs e)          //Нажата кнопку Оффлайн режима
        {
            await Navigation.PushAsync(new MainPage());
        }
        private async void LoginButton_Clicked(object sender, EventArgs e)            //Нажата кнопка входа
        {
            string connStr = "server=sql11.freesqldatabase.com;user=sql11505068;database=sql11505068;port=3306;password=qGc1gqsgCv";
            MySqlConnection conn = new MySqlConnection(connStr);
            try
            {
                conn.Open();
                string databaseTable = "Users";
                if (!isReg)
                {
                    string email = "'" + Email_entry.Text + "'";
                    string password = "'" + Password_entry.Text + "'";
                    //var query = "SELECT * FROM Users WHERE Email='123@mail.ru'";
                    string pass_hashed = "'" + hashing(password) + "'";
                    var query = "SELECT * FROM " + databaseTable + " WHERE Email=" + email + " AND Password=" + pass_hashed;
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    var result = cmd.ExecuteScalar();
                    if (result == null)
                        error_label.Text = "Неверный ввод!";
                    else
                    {
                        await Navigation.PushAsync(new MainPage());
                        error_label.Text = "";
                        Email_entry.Text = "";
                        Password_entry.Text = "";
                    }
                }
                else
                {
                    string email = "'" + Email_entry_reg.Text + "'";
                    string password = "'" + Password_entry_reg.Text + "'";
                    string repass = "'" + RePassword_entry_reg.Text + "'";
                    if (password != repass)
                    {
                        error_label_reg.Text = "Пароли не совпадают!";
                    }
                    else
                    {
                        var query = "SELECT * FROM " + databaseTable + " WHERE Email=" + email;
                        MySqlCommand cmd = new MySqlCommand(query, conn);
                        var result = cmd.ExecuteScalar();
                        if (result != null)
                        {
                            error_label_reg.Text = "Пользователь уже зарегистрирован!";
                        }
                        else
                        {
                            string pass_hashed = "'"+hashing(password)+"'";
                            query = "INSERT INTO " + databaseTable + "(Email,Password) VALUES (" + email + "," + pass_hashed + ")";
                            cmd = new MySqlCommand(query, conn);
                            cmd.ExecuteNonQuery();
                            await Navigation.PushAsync(new MainPage());
                            error_label_reg.Text = "";
                            Email_entry_reg.Text = "";
                            Password_entry_reg.Text = "";
                            RePassword_entry_reg.Text = "";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                error_label.Text = "Не удалось подключиться!";
            }
            conn.Close();
        }
    }
}

