# Register

Create a new user account.

**URL** : `/api/users`

**Method** : `POST`

**Auth required** : NO

**Data constraints**

Provide name of Account to be created.

```json
{
    "username": "[between 4 - 32 chars]",
    "password": "[between 6 - 32 chars]"
}
```

**Data example** 

```json
{
    "username": "User123",
    "password": "abc1234"
}
```

## Success Response

**Condition** : If everything is OK and an Account didn't exist for this User.

**Code** : `201 CREATED`

**Content example**

```json
{
    "id": 5,
    "username": "User3",
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IjUiLCJuYmYiOjE1NjEwMjI5OTIsImV4cCI6MTU2MTYyNzc5MiwiaWF0IjoxNTYxMDIyOTkyfQ.ZiVlIVVC8Dml_49Uql-aC01pF99gSXFQR_kTM8Tk3i4"
}
```

## Error Responses

**Condition** : If 'username' and/or 'password' length is null.

**Code** : `400 BAD REQUEST`

**Content** :

```json
username.null,password.null
```

```json
username.null
```

```json
password.null
```

### Or

**Condition** : If 'username' and/or 'password' length is too short.

**Code** : `400 BAD REQUEST`

**Content** :

```json
username.length,password.length
```

```json
username.length
```

```json
password.length
```

### Or

**Condition** : If 'username' already exists.

**Code** : `400 BAD REQUEST`

**Content** :

```json
username.duplicate
```