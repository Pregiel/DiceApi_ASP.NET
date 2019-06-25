# Show room info

Get info about single Room if current User has access permission on it

**URL** : `/api/rooms/:room_id`

**URL Parameters** : `room_id`=[integer] where `room_id` is the ID of the room.

**Method** : `GET`

**Auth required** : YES

## Success Response

**Condition** : If Room exists and Authorized User required permissions.

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

**Condition** : If Room exists but Authorized User does not have required permissions.

**Code** : `400 BAD REQUEST`

**Content** :

```json
userRoom.notFound
```