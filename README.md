# BitPaywall

A platform that allows content creators upload their contents (articles), and earn for every read on their content. Works similar to [Bitcoin Magazine](https://bitcoinmagazine.com/) (formerly Carrot).
For Bitcoin magazine, users earn (5 sats) for every article they read, whereas in BitPaywall, user pays for every article they intend to read, giving room for the content creators to earn.
This project is written with C#.


### How It Works

The platform is a collection of articles, and styles written by different creators on the platform. The articles are open to anyone (registered users or not), to read through.
The major difference is as a non-registered user, you pay in sats (max cost is 6 sats) for every article you read, however once registered on the platform, you pay only once for each article. Once you (as a registered user) has paid for a particular article, the article is available to you to read till forever unless maybe you lose access to the account.

So a user comes upon the platform, registered his details. He can then go ahead and create contents on the platform. By default every content is placed in draft, which gives the user the ability to modify their contents. Once he is satisfied, he then goes ahead and publish the content, making it visible for every other users and non-users to view.

For a reader, he clicks on a particular story, and sees a brief description about that particular article. For the reader to read all the contents provided in that post he is prompted to pay using the lightning invoice (or from its already funded account, if the user is a registered user)

A user also has the ability to rate a story, comment (coming soon) on a story.
Users have the ability to see analytics around a story such as the post rating, read counts, comments e.t.c.

Remember, there are advantages to being a registered user. You don't get to pay for an article article more than once, you can create your own contents and earn from them, you also have the ability to archive, and unarchive all of your paid posts.

## Requirements

 -         .Net core 6 and above
-          Visual Studio
-          A running lightning Node (LND)
-          OR Docker and Polar installed.
-          MySQL Database & SSMS installed
-          LNURL proto file


## Configuration

For the required environment file, please see the 'requirements.txt' file

The following are a list of currently available configuration options that needs to be setup in the appsettings.json file and a short explanation of what each does.

`AccountNumberPrefix`
Used to generate system account for every user. This signifies the first 3 digits of 10

`Minimum withdrawal amount`
Minimum amount that can be withdrawn from the system as a user.

`MinimumAmount`
Minimum amount (1sat) that can be pegged to a post as value

`MaximumAmount`
Maximum amount (7sats) that can be pegged to a post as value

`TokenConstants`
Handles JWT token authentication. 

`DefaultConnection`
Connection string to your sql database server

`cloudinary` (required)
All connection necessary to be able to upload photo on the system. The required keys include secred, cloudname as well as key

`Lightning` (required)
All connection necessary to connect to your bitcoin node, which includes macaroon path, ssl cert path, grpc host name


## Initializing the database

To initialize the database which would create the database file and all the 
necessary tables, open your package manager console and run the commands:

```
$ Add-Migration migration_name
$ Update-Database

```

Please ensure you already have your sql server setup.


## Running the application server

After installing the dependencies, configuring the application, initializing the database, you can start the application backend by 
running the command:

```
$ dotnet run
```


## API ROUTES

Run the application to see the list of all available endpoints