# BitPaywall

# ApeSats

### A platform that allows content creator post their articles, set value for the articles. Users/reader get to pay the defined amount (as low as 5 sats) to be able to read the article content. built with Bitcoin, Lightning (LND) and C#

## Requirements

 -         .Net core 6 and above
-          Visual Studio
-          A running Bitcoin core node on a signet or testnet network.
-          A running lightning Node (LND)
-          OR Docker and Polar installed.
-          MySQL Database & SSMS installed
-          LNURL proto file

## API ROUTES

##### POST
 - POST /api/Post/create = Upload a post to the system, including title, Body, Article image, etc.
- POST /api/Post/update = Allows a user make necessary content edit before the content is being published and open to the general public
- POST /api/Post/publishpost = Once a user has uploaded their content and verified that all things are good to go. They can decide to publish the content to the general public on the platform
 - GET /api/Post/getall/{skip}/{take}/{userid} = Returns a list of all the posts available on the system apart from the post main conent. So only information like post title, post description, post image are available to view amongst others.
 - GET /api/Post/getallpublishedpost/{skip}/{take}/{userid} = Returns a list of all the post that the user has published on the system.
 - GET /api/Post/getallpostdraft/{skip}/{take}/{userid} = Returns a list of all the post that the user have uploaded but not yet published
 - GET /api/Post/getbyid/{id}/{userid} = This endpoint returns the content and every information regarding a post only if it is the author/uploader of the post, or someone that has previously paid for the post. Else it returns an invoice for the user, and when the payment is settled, the user can then go ahead and view all contents of the post.


##### TRANSACTION
- GET /api/Transaction/gettransactionsbyid/{skip}/{take}/{userid} = Returns a list of all transactions done by user
- GET /api/Transaction/getbyid/{id}/{userid} = Retrieve a particular transacion by id
- GET /api/Transaction/getallcredit/{skip}/{take}/{userid} = Returns a list of all credit transactions on the system
- GET /api/Transaction/getcredittransactions/{skip}/{take}/{accountnumber}/{userid} = Returns a list of credit transactions done by a particular user
- GET /api/Transaction/getalldebit/{skip}/{take}/{userid} = Returns a list of all debit transactions on the system
- GET /api/Transaction/getdebittransactions/{skip}/{take}/{accountnumber}/{userid} = Returns a list of debit transactions done by a particular user


##### USERS
 - POST /api/User/create = Registers a user on the system
 - POST /api/User/login = User login
 - POST /api/User/getusersbyid/{userid} = Returns a single user given an Id
 - POST /api/User/getall/{skip}/{take}/{email}= Returns a list of users avaialble on the system

