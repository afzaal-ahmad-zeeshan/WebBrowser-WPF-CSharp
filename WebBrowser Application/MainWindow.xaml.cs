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
using System.Runtime.InteropServices;

namespace WebBrowser_Application
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // change the UA string
        [DllImport("urlmon.dll", CharSet = CharSet.Ansi)]
        private static extern int UrlMkSetSessionOption(int dwOption, string pBuffer, int dwBufferLength, int dwReserved);
        const int URLMON_OPTION_USERAGENT = 0x10000001;

        public void ChangeUserAgent(String Agent)
        {
            UrlMkSetSessionOption(URLMON_OPTION_USERAGENT, Agent, Agent.Length, 0);
        }

        // global variable
        private string uri = "";

        // MainWindow class constructor
        public MainWindow()
        {
            InitializeComponent();

            myBrowser.Navigate("http://www.google.com");
            myUrl.Text = "http://www.google.com";

            ChangeUserAgent("Mozilla/5.0 (compatible; MSIE 9.0; Windows NT 6.1; WOW64; Trident/5.0)");
            uri = "http://www.google.com";

            this.setView();
            this.navigationKeys();
        }

        // The navigation keys settings
        private void navigationKeys()
        {
            if (!myBrowser.CanGoForward)
            {
                BrowserGoForward.IsEnabled = false;
            }
            else
            {
                BrowserGoForward.IsEnabled = true;
            }

            if (!myBrowser.CanGoBack)
            {
                BrowserGoBack.IsEnabled = false;
            }
            else
            {
                BrowserGoBack.IsEnabled = true;
            }
        }

        private void setView() 
        {
            // this code sets the height and the width of the WebBrowser element.
            myBrowser.Width = this.Width;
            myBrowser.Height = (this.Height - 59);
        }

        // set the browser's width and height
        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (this.WindowState == WindowState.Maximized)
            {

                // get the Screen properties.
                myBrowser.Width = SystemParameters.PrimaryScreenWidth;
                myBrowser.Height = SystemParameters.PrimaryScreenHeight - 119;
            }
            else
            {
                myBrowser.Width = this.Width;
                myBrowser.Height = (this.Height - 59);
            }
        }

        private void myUrl_KeyDown(object sender, KeyEventArgs e)
        {
            TextBox textBox = sender as TextBox;
            string url = textBox.Text;

            // get if the key is ENTER key and then navigate
            if (e.Key == Key.Enter)
            {
                try
                {
                    myBrowser.Navigate(url);
                }
                catch (Exception er)
                {
                    // there was an error in the URI, complete it!
                    if (url.IndexOf("http://") == -1 || url.IndexOf("https://") == -1)
                    {
                        // there was no URI indicator, append it to string!
                        url = "http://" + url;
                        myBrowser.Focus();
                        try
                        {
                            myBrowser.Navigate(url.Replace("..", "."));
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("The URL you provided is not corrent, check it twice.");
                        }
                        myUrl.Text = myBrowser.Source.ToString();
                    }
                }
            }
        }

        // Enter button handler, loads page if ENTER key was pressed
        private void myBrowser_KeyDown(object sender, KeyEventArgs e)
        {
            // get the web browser
            WebBrowser myBrowser = sender as WebBrowser;

            // get if the key is BACKSPACE then go back!
            if (e.Key == Key.Back)
            {
                if (myBrowser.CanGoBack)
                {
                    myBrowser.GoBack();
                }
            }
        }

        // Back button handler, loads previous page in history (if present)
        private void BrowserGoBack_Click(object sender, RoutedEventArgs e)
        {
            if (myBrowser.CanGoBack)
            {
                myBrowser.GoBack();
            }
        }

        // Forward button handler, loads next page in history (if present)
        private void BrowserGoForward_Click(object sender, RoutedEventArgs e)
        {
            if (myBrowser.CanGoForward)
            {
                myBrowser.GoForward();
            }
        }

        // following three methods are used in the Navigation of the browser
        // from one page to another.
        private void myBrowser_Navigating(object sender, NavigatingCancelEventArgs e)
        {
            uri = myUrl.Text;
            this.Title = "Navigating to " + uri;
        }

        private void myBrowser_Navigated(object sender, NavigationEventArgs e)
        {
            // get the document and set its title
            dynamic doc = myBrowser.Document;
            this.Title = doc.Title;
            this.navigationKeys();
            myUrl.Text = myBrowser.Source.ToString();
        }

        private void myBrowser_LoadCompleted(object sender, NavigationEventArgs e)
        {
            dynamic doc = myBrowser.Document;
            this.Title = doc.Title;
            this.navigationKeys();
            myUrl.Text = myBrowser.Source.ToString();
        }

        // Reload the current page function.
        private void BrowserRefresh_Click(object sender, RoutedEventArgs e)
        {
            myBrowser.Refresh();
            myUrl.Text = myBrowser.Source.ToString();
        }

        // the about me dialog box
        private void aboutMe()
        {
            MessageBox.Show("WebBrowser version 1.0.0.0.\n\n\nJust a testing version, don't expect a Google Chrome!");
        }

        // Click on the Menu
        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            // get the method caller
            MenuItem item = sender as MenuItem;

            // switch to check which menu item was clicked
            switch (item.Header.ToString())
            {
                case "Exit":
                    this.Close();
                    break;
                case "About":
                    this.aboutMe();
                    break;
                default:
                    break;
            }
        }
    }
}
