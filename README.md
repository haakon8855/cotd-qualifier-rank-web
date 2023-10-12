# Cup of the Day Qualifier Rank<br>Website and API

Cup of the Day Qualifier Rank is a website and API for finding the seeding 
of your current PB in an arbitrary COTD qualifying session.

__Important:__ This project is still a work in progress and is therefore not
publicly available or integrated with an accompanying
[Openplanet Plugin](https://github.com/haakon8855/COTD-qualifier-rank).

## Overview

- [Cup of the Day Qualifier RankWebsite and API](#cup-of-the-day-qualifier-rankwebsite-and-api)
  - [Overview](#overview)
  - [Introduction](#introduction)
    - [The Decentralised Approach](#the-decentralised-approach)
    - [The Centralised Approach](#the-centralised-approach)
  - [API](#api)
    - [Rank Endpoint](#rank-endpoint)
  - [Website](#website)
  - [Setup Guide](#setup-guide)
    - [Prerequisites](#prerequisites)
    - [Install .NET](#install-net)
    - [Download Source Code](#download-source-code)
    - [Create a Trackmania Dedicated Server Account](#create-a-trackmania-dedicated-server-account)
    - [Provide Credentials for the Server](#provide-credentials-for-the-server)
    - [Create the Database](#create-the-database)
    - [Run the Server](#run-the-server)
  - [License](#license)
  - [Credits](#credits)

## Introduction

Have you ever been playing an old Track of the Day and thought:  
_Which division would I have gotten with this PB?_

The main use case of this API is to show users what their seeding would have
been if they drove their current PB during the COTD qualifying session for
that TOTD.
This API tries to provide this functionality in an efficient manner without
contacting Nadeo's own API for every request.

The API is made to accompany the 
openplanet plugin 
[COTD Qualifier Rank](https://github.com/haakon8855/COTD-qualifier-rank)
for Trackmania 2020. 

### The Decentralised Approach
A plugin already exists which provides the aforementioned features. This plugin
does not rely on a centralised server and fetches all
required data directly from Nadeo. There are several problems with this
solution:

- Data is only cached during a single map session, meaning leaderboard data
  needs to be fetched every time a map is reloaded or the game is restarted. 
  If coded efficiently, this creates on average 10-20 requests to Nadeo per map.
- Obtaining the ID of a qualification leaderboard given a MapId is very
  difficult when only relying on Nadeo's own API and can generate a lot of
  requests. Wihtout caching, this can create between 1 and 120 requests to
  Nadeo (__each time a map is loaded by the user__) depending on when the track
  was TOTD.
- Data cannot be shared between users, and thus each user has to fetch the same
  leaderboard data and ID data for each map loaded.

### The Centralised Approach

The obvious solution to this problem is to create a centralised server which
handles all communication with Nadeo's API. This way, both qualification 
leaderboard IDs and leaderboard data can be cached. Additionally, since this
data never changes after qualification commences, caching only needs to happen
once.

The plugin can then be altered to only send requests to the centralised API
where data is cached. The server receives a MapUID along with the player's PB
from and returns the player's "rank".

The only downside to this approach is that someone needs to maintain, and
possibly pay to keep the server running.

## API

The API has a single endpoint designed for use with the 
[COTD Qualifier Rank](https://github.com/haakon8855/COTD-qualifier-rank)
plugin. 

### Rank Endpoint
- **URL**: `/api/rank/{mapUid}/{time}`
- **Method**: GET
- **Description**: Returns the seeding/rank of the given time
  on the given MapUID 
- **Values**:
  - `mapUid`: The UID of any map. If the map is not a TOTD or was TOTD prior to 02.11.2020, no data is returned.
  - `time`: The time in milliseconds for which a rank is returned (e.g. your current PB on a TOTD).

**Example Request**:

```shell
curl -X GET http://localhost:5000/api/rank/ae262K904I12Kbj_AaBGeTqE1F0/49302
```

 **Response**:

```json
{
    "mapUid": "ae262K904I12Kbj_AaBGeTqE1F0",
    "competitionId": 10555,
    "challengeId": 5235,
    "date": "2023-10-07T19:00:00",
    "time": 49302,
    "rank": 4079
}
```

## Website

The website provided by the server is not strictly necessary and only meant to
supplement the API with a more readable graphical user interface showing
details and data from each qualification leaderboard.

The home page on the website shows a list of the most recent Cup of the Days:

<img
    src="img/web-index.png"
    alt="Landing page on website showing a list of the most recent COTDs"
    width="700"
/>

<br>Clicking on _Details_ for a specific COTD redirects to a more detailed
view of the qualification leaderboard for that COTD. This page includes a full
paginated qualification leaderboard, COTD metadata and links to the
corresponding TOTD and COTD on [trackmania.io](trackmania.io). It is also
possible to manually fetch your rank by entering it in the input on the upper
right of this page.

<img
    src="img/web-details.png"
    alt="Detailed view of a specific COTD qualification leaderboard"
    width="700"
/>

## Setup Guide

The following section will guide you through the necessary steps to run this
server locally.

### Prerequisites

To successfully run this server locally, you will need the following:
- A Trackmania Club Access Subscription
- .NET 7.0

### Install .NET

Make sure you have
[.NET 7.0](https://dotnet.microsoft.com/en-us/download/dotnet/7.0)
installed.

To verify you have the correct version, run the following command:

```shell
dotnet --version
```

This should return `7.0.x`

### Download Source Code

Clone the repo:

```shell
git clone https://github.com/haakon8855/COTD-qualifier-rank-web.git
```

Or download the source code as a `.zip`.

### Create a Trackmania Dedicated Server Account

In order to authenticate with the Trackmania Web Services API, we need to
provide our server with credentials with the necessary authorisation.

To create a dedicated server account, go to
<https://www.trackmania.com/player/dedicated-servers>
and create a dedicated server account by entering a server login
of your choosing.  
_Note: This requires you to login with a Ubisoft account_
with an active Trackmania Club Access subsciption

Take note of your server login, as well as the generated
`password` and `account-id`.

### Provide Credentials for the Server

Create a file in `CotdQualifierRankWeb/` named `credentials.json`
```shell
touch CotdQualifierRankWeb/credentials.json
```
and store your dedicated server account credentials inside
with the following format:
```json
{
  "Login": "<dedicated-server-account-login>",
  "Password": "<dedicated-server-account-password>",
  "AccountId":	"<dedicated-server-account-account-id>"
}
```

### Create the Database

To create the database run the following commands:
```shell
cd CotdQualifierRankWeb/
dotnet ef database update
```

### Run the Server

Open a terminal inside the repo and run the following commands:
```shell
cd CotdQualifierRankWeb/
dotnet run
```

## License
The code in this repository is protected by the
[GNU General Public License v3](./LICENSE).

## Credits

This project would not be possible without:
- The Openplanet team's
  [Trackmania Web Services API Documentation](https://webservices.openplanet.dev/)
- The Openplanet discord server
