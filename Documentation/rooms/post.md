# Create room

Create a new room.

**URL** : `/api/rooms`

**Method** : `POST`

**Auth required** : YES

**Data constraints**

Provide title and password of Room to be created.

```json
{
    "title": "[between 4 - 32 chars]",
    "password": "[between 6 - 32 chars]"
}
```

**Data example** 

```json
{
    "title": "Room123",
    "password": "abc1234"
}
```

## Success Response

**Condition** : If everything is OK.

**Code** : `201 CREATED`

**Content example**

```json
{
    "id": 5,
    "username": "Room123",
}
```

## Error Responses

**Condition** : If 'title' and/or 'password' length is null.

**Code** : `400 BAD REQUEST`

**Content** :

```json
title.null,password.null
```

```json
title.null
```

```json
password.null
```

### Or

**Condition** : If 'title' and/or 'password' length is too short.

**Code** : `400 BAD REQUEST`

**Content** :

```json
title.length,password.length
```

```json
title.length
```

```json
password.length
```