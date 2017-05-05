# C# Oauth Twitter Library
You can use this library when you want to tweet other CsharpApplication.This is Simple POSTMethod code.
# How to use
Input your four APITokenKey and please write this code

```C#
using System.Twitter;
```
Tweet Method
```C#
  var twitter = new TwitterAPI(ConsumerKey,ConsumerKeySecret,AccessToken,AccessTokenSecret);

  var tweetValue = "HELLO"; //or Textbox.text and so on
  twitter.Tweet(tweetValue);　//tweet value
```
            
# other
I will make GETTimeLine and login system soon..
