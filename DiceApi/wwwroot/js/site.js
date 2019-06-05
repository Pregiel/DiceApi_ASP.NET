// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your Javascript code.
$(function () {
    var dataRedirect = null;
    $('#loginModal').on('show.bs.modal', function (e) {
        var button = $(e.relatedTarget);
        dataRedirect = button.data('redirect');
    });

    $('#submitLogin').on('click', function (e) {
        e.preventDefault()
        var user = $('#loginForm').serializeJSON();
        $.ajax({
            type: 'post',
            url: '/api/users/authenticate',
            data: JSON.stringify(user),
            contentType: 'application/json'
        }).done(function (e) {
            Cookies.set('token', e.token);
            Cookies.set('username', e.username);

            if (dataRedirect == null) {
                window.location = window.location;
            } else {
                window.location = dataRedirect;
            }
        }).fail(function (e) {
            $("#loginError").empty();
            var errorMessage =
                `<div class="alert alert-danger alert-dismissible" id="errorMessage">
                    <span class="fa fa-warning" aria-hidden="true"></span>
                    <span class="sr-only">Error:</span>
                    <a href = "#" class="close" data-dismiss="alert" aria-label="close">&times;</a>
                    Invalid username or password.
                </div>`;
            $("#loginError").append(errorMessage);
        });
    });

    $('#submitRegister').on('click', function (e) {
        e.preventDefault()
        if ($("#registerForm input[name=password]").val() == $("#registerForm input[name=confirmPassword]").val()) {
            var user = $('#registerForm').serializeJSON();
            $.ajax({
                type: 'post',
                url: '/api/users',
                data: JSON.stringify(user),
                contentType: 'application/json'
            }).done(function (e) {
                Cookies.set('token', e.token);
                Cookies.set('username', e.username);
                window.location = window.location;
            }).fail(function (e) {
                e.responseText.split(',').forEach(function (entry, index) {
                    if (entry == "username.length") {
                        $("#usernameLength").removeClass("d-none");
                    } else if (entry == "username.duplicate") {
                        $("#usernameExists").removeClass("d-none");
                    } else if (entry == "password.length") {
                        $("#passwordLength").removeClass("d-none");

                    }
                });
            });
        } else {
            $("#confirmPasswordDiff").removeClass("d-none");
        }
    });

    $('#registerForm input[name=username]').on('focus', function (e) {
        $("#usernameLength").addClass("d-none");
        $("#usernameExists").addClass("d-none");
    });

    $('#registerForm input[name=password]').on('focus', function (e) {
        $("#passwordLength").addClass("d-none");
        $("#confirmPasswordDiff").addClass("d-none");
    });

    $('#registerForm input[name=confirmPassword]').on('focus', function (e) {
        $("#confirmPasswordDiff").addClass("d-none");
    });
})