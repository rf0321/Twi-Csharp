using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Twitter;

namespace TweetApp
{
    class Program
    {

        [STAThread]
        static void Main()
        {
            Application.Run(new Form1());
        }
        class Form1 : Form
        {
            
            Button TweetButton;　//いつも通りのGUIステート
            TextBox TweetWindow;
            //string tweet;
            public Form1()
            {
                //this.TweetWindow.Text = tweet;
                this.TweetWindow = new TextBox();
                this.TweetWindow.Location = new System.Drawing.Point(50, 70);
                this.TweetWindow.Size = new System.Drawing.Size(380, 170);
                this.TweetWindow.Multiline = true; //TextBoxのHeightを変更できるようになる
                this.TweetWindow.Height = 200;
                this.Controls.Add(this.TweetWindow);

                this.Width = 500;
                this.Height = 380;
                this.Text = "TweetAppTest";
                this.MaximumSize = this.Size;
                this.MinimumSize = this.Size;

                this.TweetButton = new Button();
                this.TweetButton.Text = "Tweet";
                this.TweetButton.Location = new System.Drawing.Point(10, 10);
                this.TweetButton.Size = new System.Drawing.Size(120, 40);
                this.TweetButton.Click += new EventHandler(this.OnClickTweetButton);
                this.Controls.Add(this.TweetButton);
            }
            public void OnClickTweetButton(object sender, EventArgs e)
            {
                var twitter = new TwitterAPI("o9Osz8TFS6JIFDsfOQn3UjwXz", "kvaR1DSKoSCQvMwy7IGKa7lCN6pt1wFixLB2lQmiNIm1ma6kPI", 
                 "3103766516-FbGQouXoqeoSEjKPiXII2NQGyQ9UwsZzrC4enct", "O9DVRdtPKVrRC4YQjmWwlB4CU16JN9DPeK3pextwNUSEX");
                twitter.Tweet("Hello");
            }
        }
    }
}
