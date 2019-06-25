# Show room list

Get room list. Response can be modified by `limit` and `page` parameters.

**URL** : `/api/rooms`

**Method** : `GET`

**Auth required** : YES

**Data constraints**

```
{
    "limit": "[integer, how many rooms should be returned]",
    "page": "[integer, how many pages should be skipped]"
}
```

## Success Response

**Code** : `200 OK`

**Content example**

```json
{
    {
    "rooms": [
        {
            "id": 1,
            "title": "MyRoom1",
            "owner": {
                "id": 1,
                "username": "MyUser1"
            },
            "clientAmount": 1,
            "onlineClientAmount": 0
        },
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
        },
        {
            "id": 4,
            "title": "TestRoom20",
            "owner": {
                "id": 2,
                "username": "User2"
            },
            "clientAmount": 1,
            "onlineClientAmount": 0
        }
    ],
    "size": 4
    }
}
```

## Error Responses

**Condition** : If Room does not exist with `id` of provided `room_id` parameter.

**Code** : `400 BAD REQUEST`

**Content** :

```json
room.notFound
```