# Join room

Used to get permission by Authorized User to enter room.

**URL** : `/api/rooms/:room_id`

**URL Parameters** : `room_id`=[integer] where `room_id` is the ID of the room.

**Method** : `POST`

**Auth required** : YES

**Data constraints**

Provide passsword of Room to join.

```json
{
    "password": "[valid password]"
}
```

**Data example** 

```json
{
    "password": "abc1234"
}
```

## Success Response

**Code** : `200 OK`

**Content example**

```json
{
    "id": 1,
    "title": "TestRoom1",
    "owner": {
        "id": 1,
        "username": "TestUser1"
    },
    "users": [
        {
            "id": 1,
            "username": "TestUser1"
        },
        {
            "id": 2,
            "username": "TestUser2"
        }
    ]
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

**Condition** : If 'password' is invalid.

**Code** : `400 BAD REQUEST`

**Content** :

```json
credentials.invalid
```

### Or

**Condition** : If 'password' length is null.

**Code** : `400 BAD REQUEST`

**Content** :

```json
password.null
```