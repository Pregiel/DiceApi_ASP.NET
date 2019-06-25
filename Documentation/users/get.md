# Show info

Get a info about currently Authenticated User.

**URL** : `/api/users`

**Method** : `GET`

**Auth required** : YES

## Success Response

**Code** : `200 OK`

**Content example**

```json
{
    "id": 2,
    "username": "TestUser2",
    "rooms": [
        {
            "id": 1,
            "title": "TestRoom1",
            "owner": {
                "id": 1,
                "username": "TestUser1"
            },
            "clientAmount": 2
        },
        {
            "id": 2,
            "title": "TestRoom2",
            "owner": {
                "id": 1,
                "username": "TestUser1"
            },
            "clientAmount": 2
        },
        {
            "id": 4,
            "title": "TestRoom4",
            "owner": {
                "id": 2,
                "username": "TestUser2"
            },
            "clientAmount": 1
        },
        {
            "id": 5,
            "title": "TestRoom5",
            "owner": {
                "id": 2,
                "username": "TestUser2"
            },
            "clientAmount": 1
        },
        {
            "id": 6,
            "title": "TestRoom6",
            "owner": {
                "id": 2,
                "username": "TestUser2"
            },
            "clientAmount": 1
        }
    ]
}
```