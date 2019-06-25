# Show room rolls
Get roll list for room.

**URL** : `/api/rooms/:room_id/rolls`

**Method** : `GET`

**Auth required** : YES

## Success Response

**Code** : `200 OK`

**Content example**

```json
[
    {
        "id": 1,
        "userId": 2,
        "username": "TestUser2",
        "roomId": 1,
        "rollValues": [
            {
                "maxValue": 20,
                "value": 19
            },
            {
                "maxValue": -10,
                "value": -6
            },
            {
                "maxValue": 10,
                "value": 4
            },
            {
                "maxValue": -6,
                "value": -1
            }
        ],
        "createdTime": "23.05.2019 19:42:27",
        "modifier": 5
    },
    {
        "id": 2,
        "userId": 2,
        "username": "TestUser2",
        "roomId": 1,
        "rollValues": [
            {
                "maxValue": 20,
                "value": 10
            },
            {
                "maxValue": -10,
                "value": -6
            },
            {
                "maxValue": 10,
                "value": 8
            },
            {
                "maxValue": -6,
                "value": -3
            }
        ],
        "createdTime": "23.05.2019 19:48:03",
        "modifier": 5
    },
    {
        "id": 3,
        "userId": 2,
        "username": "TestUser2",
        "roomId": 1,
        "rollValues": [
            {
                "maxValue": 20,
                "value": 18
            },
            {
                "maxValue": -10,
                "value": -4
            },
            {
                "maxValue": 10,
                "value": 5
            },
            {
                "maxValue": -6,
                "value": -3
            }
        ],
        "createdTime": "23.05.2019 19:48:07",
        "modifier": 5
    }
]
```