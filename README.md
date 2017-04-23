# WinFormTwitterAPP
You can use this library when you want to tweet other Application.This is Simple POSTMethod code.
# How to use
Input your four APITokenKey and please write this code

```csharp:sample
using System.Twitter;
```
Tweet function
```csharp:sample
public void OnClickTweetButton(object sender, EventArgs e)
{
  var twitter = new TwitterAPI("your ConsumerKey", "your ConsumerKeySecret",
  "your AccessToken", "your AccessTokenSecret");

   var tweetValue = "HELLO"; //or Textbox.text and so on
   twitter.Tweet(tweetValue);　//tweet value
}
```
            
# other
I will make GETTimeLine and login system soon..
