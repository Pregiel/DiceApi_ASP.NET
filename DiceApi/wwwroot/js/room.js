$(() => {
    $('#submitRoom').on('click', (e) => {
        e.preventDefault();
        $("#roomError").empty();
        var room = $('#roomForm').serializeJSON();
        $.ajax({
            type: 'post',
            url: '/api/rooms/' + roomId,
            data: JSON.stringify(room),
            contentType: 'application/json',
            beforeSend: (xhr) => {
                xhr.setRequestHeader('Authorization', 'Bearer ' + Cookies.get('token'));
            }
        }).done((e) => {
            window.location = window.location;
        }).fail((e) => {
            var errorMessage =
                `<div class="alert alert-danger alert-dismissible" id="errorMessage">
                        <span class="fa fa-warning" aria-hidden="true"></span>
                        <span class="sr-only">Error:</span>
                        <a href = "#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
                        Invalid password.
                    </div>`;
            $("#roomError").append(errorMessage);
        });
    });

    var joinRoom = () => {
        var room = $('#joinModal').serializeJSON();
        $('#errorcode').empty();
        $.ajax({
            type: 'post',
            url: '/api/rooms/' + roomId,
            data: JSON.stringify(room),
            contentType: 'application/json',
            beforeSend: (xhr) => {
                xhr.setRequestHeader('Authorization', 'Bearer ' + Cookies.get('token'));
            }
        }).done((response) => {
            updateRoomDetails(response);
            joinRoomHub();
        }).fail((e) => {
            $('#errorCode').append(e.status);
            if (e.status == 401) {
                window.location = "/login";
            } else if (e.status == 400) {
                if (e.responseText == "credentials.invalid" || e.responseText == "password.null") {
                    $("#joinModal").modal();
                } else if (e.responseText == "userRoom.exists") {
                    window.location = window.location;
                } else {
                    window.location = "/";
                }
            }
        });
    }
    joinRoom();

    var joinRoomHub = () => {
        var connection = new signalR.HubConnectionBuilder().withUrl("/hub",
            { accessTokenFactory: () => Cookies.get('token') }).build();

        connection.on('RoomDetails', (roomDetails) => {
            updateRoomDetails(roomDetails);
        });

        connection.on('NewRoll', (roll) => {
            addRollToList(roll);
            $("#roll-list").scrollTop($("#roll-list")[0].scrollHeight);
        });

        connection.on('RollList', (rollList) => {
            updateRollList(rollList);
        });

        connection.on('UsersOnlineList', (onlineUsers, offlineUsers) => {
            $('#usersOnline').text(onlineUsers.length);
            var item = ``;
            for (var user of onlineUsers) {
                item += `<div><i class="fa fa-circle text-success"></i> ` + user.username + `</div>`;
            }
            for (var user of offlineUsers) {
                item += `<div><i class="fa fa-circle text-black-50"></i> ` + user.username + `</div>`;
            }
            $('#user-list').empty().append(item);
        });

        connection.start().then(() => {
            connection.invoke("JoinRoom", roomId);
        }).catch((err) => {
            return console.error(err.toString());
        })

    }

    var getRoomDetails = () => {
        $.ajax({
            type: 'get',
            url: '/api/rooms/' + roomId,
            contentType: 'application/json',
            beforeSend: (xhr) => {
                xhr.setRequestHeader('Authorization', 'Bearer ' + Cookies.get('token'));
            }
        }).done((response) => {
            updateRoomDetails(response);
        }).fail((e) => {
            $('#errorCode').append(e.status);
            if (e.status == 401) {
                window.location = "/login";
            } else if (e.status == 400) {
                window.location = "/";
            }
        });
    }

    var updateRoomDetails = (roomDetails) => {
        $('#roomTitle').empty().append(roomDetails.id + `. ` + roomDetails.title);
    }

    var getRolls = () => {
        $('#roll-list').empty();
        $.ajax({
            type: 'get',
            url: '/api/rooms/' + roomId + '/rolls',
            contentType: 'application/json',
            beforeSend: (xhr) => {
                xhr.setRequestHeader('Authorization', 'Bearer ' + Cookies.get('token'));
            }
        }).done((response) => {
            updateRollList(response);
        }).fail((e) => {
            $('#errorCode').append(e.status);
            if (e.status == 401) {
                window.location = "/login";
            } else if (e.status == 400) {
                window.location = "/";
            }
        });
    }

    var updateRollList = (rollList) => {
        $('#roll-list').empty();
        $.each(rollList, (i, roll) => {
            addRollToList(roll);
        });
        $("#roll-list").scrollTop($("#roll-list")[0].scrollHeight);
    };

    var addRollToList = (roll) => {
        var rollValue = roll.modifier;
        $.each(roll.rollValues, (ii, value) => {
            rollValue += value.value;
        })
        var rollClasses;
        var rollUsername;
        if (Cookies.get('username') == roll.username) {
            rollClasses = 'roll-entry my-roll';
            rollUsername = 'You';
        } else {
            rollClasses = 'roll-entry';
            rollUsername = roll.username;
        }

        var item =
            `<div class="` + rollClasses + `">
                        <div class="roll-user">` + rollUsername + ` rolled</div>
                        <div class="roll-value">` + rollValue + `</div>
                        <div class="roll-string">` + rollToString(roll) + `</div>
                    </div>`;
        $('#roll-list').append(item);
    };

    var rollToString = (roll, ignoreValue = false) => {
        var stringBuilder = ``;
        var negativeValueMap = new Map();
        var positiveValueMap = new Map();
        $.each(roll.rollValues, (i, value) => {
            if (value.maxValue < 0) {
                if (negativeValueMap.has(value.maxValue)) {
                    negativeValueMap.set(value.maxValue, negativeValueMap.get(value.maxValue).concat(Math.abs(value.value)));
                } else {
                    negativeValueMap.set(value.maxValue, [Math.abs(value.value)]);
                }
            } else {
                if (positiveValueMap.has(value.maxValue)) {
                    positiveValueMap.set(value.maxValue, positiveValueMap.get(value.maxValue).concat(Math.abs(value.value)));
                } else {
                    positiveValueMap.set(value.maxValue, [Math.abs(value.value)]);
                }
            }
        })
        var comparer = (a, b) => {
            return a[0] - b[0];
        };
        var negativeValueArray = [...negativeValueMap.entries()].sort(comparer);
        var positiveValueArray = [...positiveValueMap.entries()].sort(comparer).reverse();
        var map = new Map(positiveValueArray.concat(negativeValueArray));

        for (var [k, v] of map.entries()) {
            if (k < 0) {
                stringBuilder += ` - `;
            } else {
                stringBuilder += ` + `;
            }

            stringBuilder += v.length + `d` + Math.abs(k);
            if (!ignoreValue) {
                stringBuilder += ` (`;
                $.each(v, (i, value) => {
                    stringBuilder += value + `, `;
                });
                stringBuilder = stringBuilder.slice(0, -2);

                stringBuilder += `)`;
            }
        }

        if (stringBuilder.startsWith(` + `)) {
            stringBuilder = stringBuilder.slice(3);
        } else {
            stringBuilder = stringBuilder.slice(1);
        }

        if (roll.modifier < 0) {
            stringBuilder += ` - ` + Math.abs(roll.modifier);
        } else if (roll.modifier > 0) {
            stringBuilder += ` + ` + roll.modifier;
        }

        return stringBuilder;
    }

    var myRoll = {
        rollValues: [],
        modifier: 0
    };

    var action = 0;

    var addRoll = (dice) => {
        if (action == 1) {
            dice = -dice;
        }
        myRoll.rollValues.push({ maxValue: dice, value: 0 });
        $('#rollString').val(rollToString(myRoll, true));

        $('#rollString').removeClass('border-danger');
        $('#rollEmpty').addClass('d-none');
    }

    var addModifier = (value) => {
        if (action == 1) {
            value = -value;
        }
        myRoll.modifier += value;
        $('#rollString').val(rollToString(myRoll, true));
    }

    $('#d4').on('click', () => { addRoll(4); });
    $('#d6').on('click', () => { addRoll(6); });
    $('#d8').on('click', () => { addRoll(8); });
    $('#d10').on('click', () => { addRoll(10); });
    $('#d12').on('click', () => { addRoll(12); });
    $('#d20').on('click', () => { addRoll(20); });
    $('#d100').on('click', () => { addRoll(100); });

    $('#mod1').on('click', () => { addModifier(1); });
    $('#mod2').on('click', () => { addModifier(2); });
    $('#mod3').on('click', () => { addModifier(3); });

    $('#action').on('click', () => {
        if (action == 0) {
            action = 1;
            $('#action').text('Add');
            $('#mod1').text('-1');
            $('#mod2').text('-2');
            $('#mod3').text('-3');
        } else {
            action = 0;
            $('#action').text('Substract');
            $('#mod1').text('+1');
            $('#mod2').text('+2');
            $('#mod3').text('+3');
        }
    });

    $('#clear').on('click', () => {
        myRoll = { rollValues: [], modifier: 0 };
        $('#rollString').val(rollToString(myRoll, true));
    });

    $('#sendRoll').on('click', () => {
        if (myRoll.rollValues.length > 0) {
            $.ajax({
                type: 'post',
                url: '/api/rooms/' + roomId + '/rolls',
                data: JSON.stringify(myRoll),
                contentType: 'application/json',
                beforeSend: (xhr) => {
                    xhr.setRequestHeader('Authorization', 'Bearer ' + Cookies.get('token'));
                }
            }).done((response) => {

            }).fail((response) => {
                alert(response.responseText);
            });
        } else {
            $('#rollString').addClass('border-danger');
            $('#rollEmpty').removeClass('d-none');
        }
    });

    $('#refresh').on('click', () => {
        getRoomDetails();
        getRolls();
    });

    $('#userList').on('click', () => {
        if ($('#user-list-container').hasClass('d-none')) {
            $('#user-list-container').removeClass('d-none');

        } else {
            $('#user-list-container').addClass('d-none');
        }
    });
})