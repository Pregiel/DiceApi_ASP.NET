var currentPage = 1;
$(() => {
    var paginationOptions = {
        limit: 5,
        visiblePages: 5
    }

    var getUsers = () => {
        $.ajax({
            type: 'get',
            url: '/api/users/myRooms',
            data: "page=" + currentPage + "&limit=" + paginationOptions.limit,
            beforeSend: (xhr) => {
                xhr.setRequestHeader('Authorization', 'Bearer ' + Cookies.get('token'));
            }
        }).done((response) => {
            $('#room-list').empty();
            $.each(response.rooms, (i, room) => {
                var item =
                    `<li class="list-group-item d-flex justify-content-between align-items-center">
                        <span class="entry">
                            <strong>${room.id}. ${room.title} </strong>
                            <span class="badge badge-primary badge-pill">${room.onlineClientAmount}/${room.clientAmount}
                                <span class="fa fa-user"></span>
                            </span>
                            </br>
                            <small>  Owner: ${room.owner.username}</small>
                        </span>
                        <span>
                            <button class="btn btn-primary btn-sm"`;
                if (Cookies.get('token') == null) {
                    item += `data-toggle="modal" data-target="#loginModal" data-redirect="/room/${room.id}"`;
                } else {
                    item += `onclick="location.href='/room/${room.id}';"`;
                }

                item += `>Enter</buttton>
                        </span>
                    </li>`;
                $('#room-list').append(item);
            });
            makePagination(
                response.size,
                currentPage,
                paginationOptions);
        });
    };

    var goToPage = (page) => {
        if (page === undefined) {
            page = currentPage;
        }
        if (page < 1) {
            page = 1;
        }
        currentPage = page;

        getUsers();
    };

    var makePagination = (size, page, options) => {
        var view = `<li class="page-item` + (page == 1 ? ` disabled` : ``) + `">
                        <a class="page-link" href="#" id="previousPage">
                            <span>&laquo;</span>
                        </a>
                    </li>`;
        var totalPages = Math.ceil(size / options.limit);
        var startPage = 1;
        var endPage = totalPages;
        if (totalPages > options.visiblePages) {
            var center = Math.floor(options.visiblePages / 2);
            startPage = page - center + (options.visiblePages % 2 == 0 ? 1 : 0);
            if (startPage <= 1) {
                startPage = 1;
                endPage = options.visiblePages;
            } else {
                endPage = page + center;
            }

            if (endPage > totalPages) {
                endPage = totalPages;
                startPage = endPage - options.visiblePages + 1;
            }
        }
        for (var i = startPage; i <= endPage; i++) {
            view += `<li class="page-item page-number` + (i == (page) ? ` active` : ``) + `">
                        <a class="page-link" href="#" data-page="`+ i + `">` + i + `</a>
                    </li>`;
        }

        view += `<li class="page-item` + (page == totalPages ? ` disabled` : ``) + `">
                    <a class="page-link" href="#" id="nextPage">
                        <span>&raquo;</span>
                    </a>
                </li>`;

        $('#pagination').empty().append(view);

        $('#pagination > .page-number > .page-link').on('click', (event) => {
            goToPage($(event.target).data("page"));
        });

        $('#nextPage').on('click', () => {
            currentPage += 1;
            goToPage();
        });

        $('#previousPage').on('click', () => {
            currentPage -= 1;
            goToPage();
        });
    };

    getUsers();

    $('#submitCreateRoom').on('click', (e) => {
        e.preventDefault()
        var room = $('#createRoomForm').serializeJSON();
        if (validateCreateRoomForm(room) == true) {
            $.ajax({
                type: 'post',
                url: '/api/rooms',
                data: JSON.stringify(room),
                contentType: 'application/json',
                beforeSend: (xhr) => {
                    xhr.setRequestHeader('Authorization', 'Bearer ' + Cookies.get('token'));
                }
            }).done((response) => {
                window.location = '/room/' + response.id;
            }).fail((err) => {
                err.responseText.split(',').forEach((entry) => {
                    if (entry == "title.length") {
                        $("#createRoomForm #titleLength").removeClass("d-none");
                    } else if (entry == "password.length") {
                        $("#createRoomForm #passwordLength").removeClass("d-none");

                    }
                });
            });
        }
    });

    var validateCreateRoomForm = function (room) {
        var returnedValue = true;
        if (room.title.length < 4 || room.title.length > 32) {
            $("#createRoomForm #titleLength").removeClass("d-none");
            returnedValue = false;
        }
        if (room.password.length < 4 || room.password.length > 32) {
            $("#createRoomForm #passwordLength").removeClass("d-none");
            returnedValue = false;
        }
        if (room.password != room.confirmPassword) {
            $("#createRoomForm #confirmPasswordDiff").removeClass("d-none");
            returnedValue = false;
        }

        return returnedValue;
    };

    $('#createRoomForm input[name=title]').on('focus', function (e) {
        $("#createRoomForm #titleLength").addClass("d-none");
    });

    $('#createRoomForm input[name=password]').on('focus', function (e) {
        $("#createRoomForm #passwordLength").addClass("d-none");
        $("#createRoomForm #confirmPasswordDiff").addClass("d-none");
    });

    $('#createRoomForm input[name=confirmPassword]').on('focus', function (e) {
        $("#createRoomForm #confirmPasswordDiff").addClass("d-none");
    });
})