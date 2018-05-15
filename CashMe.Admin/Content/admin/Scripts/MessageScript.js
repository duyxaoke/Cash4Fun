$(document).ready(function () {
    _GetMessage();
    //real time socket
    var reltime = $.connection.trackingHub;
    // Create a function that the hub can call back to display messages.
    reltime.client.message = function (status) {
        _GetMessage(); // có thay đỗi gọi thông báoc cho ác user hợp lệ
    };
    $.connection.hub.start();

    //checkAllRoles
    $('#checkAllRoles').on('ifChecked', function (event) {
        $('.checkRoles').iCheck('check');
    });
    $('#checkAllRoles').on('ifUnchecked', function (event) {
        $('.checkRoles').iCheck('uncheck');
    });
    // Removed the checked state from "All" if any checkbox is unchecked
    $('#checkAllRoles').on('ifChanged', function (event) {
        if(!this.changed) {
            this.changed=true;
            $('#checkAllRoles').iCheck('check');
        } else {
            this.changed=false;
            $('#checkAllRoles').iCheck('uncheck');
        }
        $('#checkAllRoles').iCheck('update');
    });

    //checkAllUsers
    $('#checkAllUsers').on('ifChecked', function (event) {
        $('.checkUsers').iCheck('check');
    });
    $('#checkAllUsers').on('ifUnchecked', function (event) {
        $('.checkUsers').iCheck('uncheck');
    });
    // Removed the checked state from "All" if any checkbox is unchecked
    $('#checkAllUsers').on('ifChanged', function (event) {
        if (!this.changed) {
            this.changed = true;
            $('#checkAllUsers').iCheck('check');
        } else {
            this.changed = false;
            $('#checkAllUsers').iCheck('uncheck');
        }
        $('#checkAllUsers').iCheck('update');
    });

    var ListRoles = [];
    var ListUsers = [];
    //submit
    $('#frmMessage').submit(function () {
        if ($(this).valid()) {
            //sys.Loading();
            var FromUserId = $("#frmMessage input[name=FromUserId]").val(),
                Title = $("#frmMessage input[name=Title]").val(),
                Body = $(".summernote").code();
            //get value Roles
            ListRoles = $(".checkRoles:checked").map(function () {
                return $(this).val();
            }).toArray();
            //get value Users
            ListUsers = $(".checkUsers:checked").map(function () {
                return $(this).val();
            }).toArray();
            if (Body === null || Body === "")
            {
                sys.Alert("error", "Chưa nhập nội dung!");
                return false;
            }
            if (ListRoles.length == 0 && ListUsers.length == 0)
            {
                sys.Alert("error", "Chưa chọn người gửi!");
                return false;
            }
            var obj = new Object();
            obj.Title = Title;
            obj.Body = Body;
            obj.FromUserId = FromUserId;
            obj.ListUsers = ListUsers;
            obj.ListRoles = ListRoles;

            $.when(sys.CallAjaxPost("/Message/Add", obj)).done(function (data) {
                if (data.Status == "Success") {
                    sys.Alert("success");
                }
                else {
                    sys.Error("Lưu lại thất bại!");
                }
            });
            sys.HideLoading();
        }
        event.preventDefault();

    });

    

});

function _GetMessage() {
    $.when(sys.CallAjax("/Message/_GetMessageByUser")).done(function (message) {

        //show top 5 thong bao ra
        if (message.Messagenew !== null) {
            var html = "";
            html += '<ul class="dropdown-menu-list withScroll show-scroll" data-height="300">';
            $.each(message.Messagenew, function (k, v) {
                html += '<a  onclick="javascript: ShowMessage(' + v.Id + ');" data-toggle="modal" data-target="#detail_mesage_modal">'; //show modal o MessagePartial
                if (v.Status == 0) {
                    html += '<li class="clearfix" style="background:#eaeaea">';
                }
                else {
                    html += '<li class="clearfix">';
                }
                html += '<div class="clearfix">';
                html += '<div>';
                html += '<strong>' + v.FromUserName + '</strong>';

                html += '<small class="pull-right text-muted">';
                html += '<span class="glyphicon glyphicon-time p-r-5"></span>' + moment(v.CreateDate).fromNow();
                html += '</small>';
                html += '</div>';
                //tieu de
                html += '<p>'+ v.Title +'</p>';
                html += '</div>';
                html += "</li>";
                html += "</a>";
            });
            html += "</ul>";
            $(".ContentMessage").html(html);
        }
        // show đếm thông báo
        $(".CountMessage").text(message.CountMessage);
        $(".CountMessageUnread").text(message.CountMessageUnread);
    });
};

function ShowMessage(id) {
    $.when(sys.CallAjaxPost("/Message/getMessageById", { id: id })).done(function (data) {
        if (sys.checkNull(data)) {
            $("#frmDetailMessage .Title").html(data.Title);
            $("#frmDetailMessage .Body").html(data.Body);

            var obj = Object();
            obj.Id = id;
            obj.Status = 1; //đã đọc
            $.when(sys.CallAjaxPost("/Message/UpdateMessage", obj)).done(function (v) {
                if (v.Status == "Success") {
                    _GetMessage();
                }
            });
        }
    });
}

function ShowMessageIndex(id) {
    $.when(sys.CallAjaxPost("/Message/getMessageById", { id: id })).done(function (data) {
        if (sys.checkNull(data)) {
            $(".email-details .Title").html(data.Title);
            $(".email-details .Body").html(data.Body);

            //var obj = Object();
            //obj.Id = id;
            //obj.Status = 1; //đã đọc
            //$.when(sys.CallAjaxPost("/Message/UpdateMessage", obj)).done(function (v) {
            //    if (v.Status == "Success") {
            //        _GetMessage();
            //    }
            //});
        }
    });
}