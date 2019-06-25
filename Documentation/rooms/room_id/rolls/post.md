# Create roll

Create a new dices roll for room.

**URL** : `/api/rooms/:room_id/rolls`

**Method** : `POST`

**Auth required** : YES

**Data constraints**

Provide title and password of Room to be created.

```json
{
    "modifier" : "[integer]",
    "rollValues" : [ "[array of dices]"
        {
            "maxValue" : "[dice pips amount]"
        },
    ]
}
```

**Data example** 

```json
{
    "modifier": 5,
    "rollValues": [
        {
            "maxValue" : 20
        },
        {
            "maxValue" : 20
        },
        {
            "maxValue" : -10
        }
    ]
}
```

## Success Response

**Condition** : If everything is OK.

**Code** : `200 OK`

**Content example**

```json
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
            "maxValue": 20,
            "value": 5
        },
        {
            "maxValue": -10,
            "value": -4
        }
    ],
    "createdTime": "23.05.2019 19:42:27",
    "modifier": 5
}
```

## Error Responses

**Condition** : If Room does not exist with `id` of provided `room_id` parameter.

**Code** : `400 BAD REQUEST`

**Content** :

```json
room.notFound
```

### Or

**Condition** : If RollValues is null.

**Code** : `400 BAD REQUEST`

**Content** :

```json
rollValue.null
```

### Or

**Condition** : If RollValues is empty.

**Code** : `400 BAD REQUEST`

**Content** :

```json
rollValue.empty
```