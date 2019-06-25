# Login

Used to collect a Token for a registered User.

**URL** : `/api/users/login`

**Method** : `POST`

**Auth required** : NO

**Data constraints**

```json
{
    "username": "[valid username]",
    "password": "[valid password]"
}
```

**Data example**

```json
{
    "username": "User2",
    "password": "abc1234"
}
```

## Success Response

**Code** : `200 OK`

**Content example**

```json
{
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJ1bmlxdWVfbmFtZSI6IjIiLCJuYmYiOjE1NjEwMjEzMjQsImV4cCI6MTU2MTYyNjEyNCwiaWF0IjoxNTYxMDIxMzI0fQ.p6Nq5q8F7GrXZjOnwfESdNaN7K4jEgDtXuIW8h2VvwQ"
}
```

## Error Response

**Condition** : If 'username' or 'password' is invalid/null.

**Code** : `400 BAD REQUEST`

**Content** :

```json
credentials.invalid
```