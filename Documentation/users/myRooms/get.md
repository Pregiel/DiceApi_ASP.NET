# Show already joined rooms

Get room list which Authenticated User joined.

**URL** : `/api/users/myRooms`

**Method** : `GET`

**Auth required** : YES

## Success Response

**Code** : `200 OK`

**Content example**

```json
{
    "rooms": [
        {
            "id": 2,
            "title": "Room2",
            "owner": {
                "id": 1,
                "username": "MyUser1"
            },
            "clientAmount": 2,
            "onlineClientAmount": 0
        },
        {
            "id": 3,
            "title": "TestRoom18",
            "owner": {
                "id": 2,
                "username": "User2"
            },
            "clientAmount": 1,
            "onlineClientAmount": 0
        }
    ],
    "size": 2
}
```