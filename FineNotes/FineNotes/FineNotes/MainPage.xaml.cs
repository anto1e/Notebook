using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace FineNotes
{
    public partial class MainPage : ContentPage
    {
        NotesCollection notCol = NotesCollection.getInstance();
        public MainPage()
        {
            InitializeComponent();
            notesList.BindingContext = notCol.Notes;
            SubscribeColChanging();
            MessagingCenter.Send<Page>(this, "CollectionChanged!");
            
        }
        bool toolBarBlocked = false;
        private string currentPage = "All";
        private void SubscribeColChanging()     //Функция для изменения отображения кол-ва заметок на главной странице при изменении коллекции
        {
              MessagingCenter.Subscribe<Page>(
                this, // кто подписывается на сообщения
                "CollectionChanged!",   // название сообщения
                (sender) => {
                    int cntNotCol = notCol.Notes.Count;
                    int cntNotColPriv = notCol.Notes.Count(i => i.Allowers.Count() == 0 && i.Author == "123@mail.ru");
                    int cntNotColGroup = notCol.Notes.Count(i => i.Allowers.Count() != 0 || i.Author != "123@mail.ru");
                    if (cntNotCol < 100)
                        label_all.Text = "Все(" + cntNotCol.ToString() + ")";
                    else
                        label_all.Text = "Все>99";
                    if (cntNotColPriv < 100)
                        label_priv.Text = "Личные(" + cntNotColPriv.ToString() + ")";
                    else
                        label_priv.Text = "Личные>99";
                    if (cntNotColGroup < 100)
                        label_group.Text = "Групповые(" + cntNotColGroup.ToString() + ")";
                    else
                        label_group.Text = "Групповые>99";
                    if (currentPage == "Private")
                    {
                        notCol.fillPrivateTemp();
                    }
                    if (currentPage == "Group")
                    {
                        notCol.fillGroupTemp();
                    }
                });    // вызываемое действие

        }
        private async void addBtnClicked(object sender, EventArgs args)       //Обработка нажатия на кнопку "Добавить"
        {
            await Navigation.PushAsync(new AddPage());
        }
        private double previousScrollPosition = 0;          //Переменная для хранения значения предыдущей позиции
        private int scrollXCnt = 0;                         //Счетик, отвечающий за кол-во прокрутки прежде чем уберется тулбар
        private int scrollYCnt = 0;                         //Счетик, отвечающий за кол-во прокрутки прежде чем появится тулбар
        private void scrollHandler(object sender, Xamarin.Forms.ScrolledEventArgs e)        //Функция отображения/скрытия тулбара
        {
            if (!toolBarBlocked)
            {
                if (Convert.ToInt16(e.ScrollY) > 10)
                {
                    if (previousScrollPosition < e.ScrollY)
                    {
                        if (scrollXCnt > 5)
                        {
                            //scrolled down
                            previousScrollPosition = e.ScrollY;
                            toolbar_layout.TranslateTo(0, 250, 150);
                        }
                        else
                        {
                            scrollXCnt++;
                            scrollYCnt = 0;
                        }
                    }
                    else
                    {
                        if (scrollYCnt > 1)
                        {
                            if (Convert.ToInt16(e.ScrollY) == 0)
                                previousScrollPosition = 0;
                            previousScrollPosition = e.ScrollY;
                            toolbar_layout.TranslateTo(0, 0, 150);
                        }
                        else
                        {
                            scrollYCnt++;
                            scrollXCnt = 0;
                        }
                    }
                }
            }
        }
        bool hided = true;
        private async void OnListViewItemSelected(object sender, EventArgs args) {                //Функция полученя заметки, на которую нажал пользователь
            var frame = (Frame)sender;
            frame.BackgroundColor = Color.FromHex("DCE1C6");
            Note SelectedItem = notCol.Notes.FirstOrDefault(itm => itm.Number == Convert.ToInt32(((TappedEventArgs)args).Parameter.ToString()));
            await Navigation.PushAsync(new ModifyPage(SelectedItem));
            frame.BackgroundColor = Color.FromHex("F9FFE2");
        }
        private async void PrivateNotesClicked(object sender, EventArgs e)      //Переход на страницу с личными заметками
        {
            underline_priv.TranslationX = 0;
            underline_priv.BackgroundColor = Color.FromHex("BBFA8A");
            if (currentPage == "All")
            {
                currentPage = "Private";
                if (!hided)
                {
                    SearchClicked(null, null);
                }else
                    notCol.fillPrivateTemp();
                SearchToolbarLayoutArrow.IsVisible = false;
                SearchToolbarLayout.IsVisible = false;
                underline_all.BackgroundColor = Color.FromHex("5873FF");
                underline_all.TranslationX = 1000;
                await Grid_messages.TranslateTo(-1000, 0, 150);
                notesList.BindingContext = notCol.Notes_temp;
                await Grid_messages.TranslateTo(1000, 0, 0);
                await Grid_messages.TranslateTo(0, 0, 150);
            }
            if (currentPage == "Group")
            {
                currentPage = "Private";
                if (!hided)
                {
                    SearchClicked(null, null);
                }else
                    notCol.fillPrivateTemp();
                SearchToolbarLayoutArrow.IsVisible = false;
                SearchToolbarLayout.IsVisible = false;
                underline_group.BackgroundColor = Color.FromHex("5873FF");
                underline_group.TranslationX = 1000;
                await Grid_messages.TranslateTo(1000, 0, 150);
                notesList.BindingContext = notCol.Notes_temp;
                await Grid_messages.TranslateTo(-1000, 0, 0);
                await Grid_messages.TranslateTo(0, 0, 150);
            }
            SearchToolbarLayoutArrow.IsVisible = true;
            SearchToolbarLayout.IsVisible = true;
        }
        private async void AllNotesClicked(object sender, EventArgs e)      //Переход на страницу со всеми заметками
        {
            underline_all.TranslationX = 0;
            underline_all.BackgroundColor = Color.FromHex("BBFA8A");
            if (currentPage == "Private")
            {
                currentPage = "All";
                if (!hided)
                {   
                    SearchClicked(null, null);
                }
                SearchToolbarLayoutArrow.IsVisible = false;
                SearchToolbarLayout.IsVisible = false;
                underline_priv.BackgroundColor = Color.FromHex("5873FF");
                underline_priv.TranslationX = 1000;
                await Grid_messages.TranslateTo(1000, 0, 150);
                if (hided)
                {
                    notesList.BindingContext = notCol.Notes;
                }
                await Grid_messages.TranslateTo(-1000, 0, 0);
                await Grid_messages.TranslateTo(0, 0, 150);
            }
            if (currentPage == "Group")
            {
                currentPage = "All";
                if (!hided)
                {
                    SearchClicked(null, null);
                }
                SearchToolbarLayoutArrow.IsVisible = false;
                SearchToolbarLayout.IsVisible = false;
                underline_group.BackgroundColor = Color.FromHex("5873FF");
                underline_group.TranslationX = 1000;
                await Grid_messages.TranslateTo(1000, 0, 150);
                if (hided)
                {
                    notesList.BindingContext = notCol.Notes;
                }
                await Grid_messages.TranslateTo(-1000, 0, 0);
                await Grid_messages.TranslateTo(0, 0, 150);
            }
            SearchToolbarLayoutArrow.IsVisible = true;
            SearchToolbarLayout.IsVisible = true;
        }
        private async void GroupNotesClicked(object sender, EventArgs e)      //Переход на страницу с групповыми заметками
        {
            underline_group.TranslationX = 0;
            underline_group.BackgroundColor = Color.FromHex("BBFA8A");
            if (currentPage == "Private")
            {
                currentPage = "Group";
                if (!hided)
                {
                    SearchClicked(null, null);
                }else
                    notCol.fillGroupTemp();
                SearchToolbarLayoutArrow.IsVisible = false;
                SearchToolbarLayout.IsVisible = false;
                underline_priv.BackgroundColor = Color.FromHex("5873FF");
                underline_priv.TranslationX = 1000;
                await Grid_messages.TranslateTo(-1000, 0, 150);
                notesList.BindingContext = notCol.Notes_temp;
                await Grid_messages.TranslateTo(1000, 0, 0);
                await Grid_messages.TranslateTo(0, 0, 150);
            }
            if (currentPage == "All")
            {
                currentPage = "Group";
                if (!hided)
                {
                    SearchClicked(null, null);
                }else
                    notCol.fillGroupTemp();
                SearchToolbarLayoutArrow.IsVisible = false;
                SearchToolbarLayout.IsVisible = false;
                underline_all.BackgroundColor = Color.FromHex("5873FF");
                underline_all.TranslationX = 1000;
                await Grid_messages.TranslateTo(-1000, 0, 150);
                notesList.BindingContext = notCol.Notes_temp;
                await Grid_messages.TranslateTo(1000, 0, 0);
                await Grid_messages.TranslateTo(0, 0, 150);
            }
            SearchToolbarLayoutArrow.IsVisible = true;
            SearchToolbarLayout.IsVisible = true;
        }

        private void FindBtdClicked(object sender, EventArgs e)                //Функция инициализации поиска
        {
                toolbar_layout.TranslateTo(0, 250, 150);
                var rotateAnimation = new Animation(v => SearchEntry.HeightRequest = v, 0, 60);
                rotateAnimation.Commit(this, "HeightAnimation", 16, 500, Easing.SinIn, null, null);
                SearchToolbarLayout.TranslateTo(0, 0, 250);
                SearchToolbarLayoutArrow.TranslateTo(0, 0, 250);
                toolBarBlocked = true;
        }
        private void ShowBtnsClicked(object sender, EventArgs e) //Функция скрывания/показа кнопок удаления и изменения
        {
            if (!hided)
            {
                buttons_layout.TranslateTo(100, 0, 250);
                ArrowBtnTool.RotateYTo(180, 250);
                ArrowFrame.Opacity = 0.2;
                hided = true;
            }
            else
            {
                buttons_layout.TranslateTo(0, 0, 250);
                ArrowBtnTool.RotateYTo(0, 250);
                ArrowFrame.Opacity = 1;
                hided = false;
            }
        }
        private void SearchBackClicked(object sender, EventArgs e)      //Функция сохранения изменений в заметке
        {var rotateAnimation = new Animation(v => SearchEntry.HeightRequest = v, 0, 60);
            notCol.Notes_temp.Clear();
            if (currentPage == "All")
            {
                notesList.BindingContext = notCol.Notes;
            }
            else if (currentPage == "Private")
            {
                notCol.fillPrivateTemp();
                notesList.BindingContext = notCol.Notes_temp;
            }
            else if (currentPage == "Group")
            {
                notCol.fillGroupTemp();
                notesList.BindingContext = notCol.Notes_temp;
            }
            var rotateAnimationBack = new Animation(v => SearchEntry.HeightRequest = v, 60, 0);
            rotateAnimationBack.Commit(this, "HeightAnimationBack", 16, 500, Easing.SinIn, null, null);
            toolBarBlocked = false;
            toolbar_layout.TranslateTo(0, 0, 150);
            SearchToolbarLayout.TranslateTo(100, 0, 250);
            SearchToolbarLayoutArrow.TranslateTo(100, 0, 250);
            hided = true;
            SearchEntry.Text = "";
        }
        private void SearchClicked(object sender, EventArgs e)
        {       //Функция поиска заметки
            string find_str = SearchEntry.Text;
            if (currentPage == "All")
            {
                notCol.findAllByPart(find_str);
            }
            else if (currentPage == "Private")
            {
                notCol.findPrivateByPart(find_str);
            }
            else if (currentPage == "Group")
            {
                notCol.findGroupByPart(find_str);
            }
            notesList.BindingContext = notCol.Notes_temp;
        }
    }
}

