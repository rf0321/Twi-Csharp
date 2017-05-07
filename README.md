Use it for free.

# C# Oauth Twitter Library
You can use this library when you want to tweet other CsharpApplication.This is simple POST method code.
# How to use
Input your four API Tokenkey and please write this code

```C#
using System.Twitter;
```
Tweet method example
```C#
  var twitter = new TwitterAPI(ConsumerKey,ConsumerKeySecret,AccessToken,AccessTokenSecret);

  var tweetValue = "HELLO"; //or Textbox.text and so on
  twitter.Tweet(tweetValue);　//tweet value
```
            
# Other
Nothing now
