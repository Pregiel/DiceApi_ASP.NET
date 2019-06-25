# Change password

Allow the Authenticated User to change their password.

**URL** : `/api/user/`

**Method** : `PUT`

**Auth required** : YES

**Data constraints**

```json
{
    "oldPassword": "password321",
    "newPassword": "[between 6 - 32 chars]"
}
```

## Success Responses

**Condition** : Old and new password is valid and User is Authenticated.

**Code** : `200 OK`

**Content** : ` `

## Error Response

**Condition** : If old password is invalid.

**Code** : `400 BAD REQUEST`

**Content** :

```json
oldPassword.invalid
```

### Or

**Condition** : If new password is too short

**Code** : `400 BAD REQUEST`

**Content** :

```json
password.length
```