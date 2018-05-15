$(document).ready(function () {

    //custom column table
    var columnDefs = [{ "width": "200px", "targets": 0 }, {
        "targets": [0],
        orderable: false,
        render: function (data, type, row) {
            var html = '<div class="btn-group">';
            html += '<a href="#" role="button" data-id=' + data + ' onclick="javascript: getUserById(this);" data-toggle="modal" data-target="#detail_modal" class="btn btn-primary btn-embossed btn-sm"><i class="fa fa-edit"></i> Sửa</a>';
            //html += '<a href="#" role="button" onclick="javascript: deleteCategoriesById(' + data + ');"  class="btn btn-warning btn-embossed btn-sm"><i class="fa fa-remove"></i> Xóa</a>';
            html += '</div>';
            return html;
        }
    },
    {
        //"targets": [4],
        //render: function (data, type, row) {
        //    if (data == 1)
        //        return '<span class="label label-primary">Active</span>';
        //    return '<span class="label label-default">Inactive</span>';
        //}
    }];
    //render datatable
    var oTable = sys.Paging($('#tbUsers'), "/Account/AjaxHandler", columnDefs);

    //submit modal Category
    $('#frmUsers').submit(function () {
        if ($(this).valid()) {
            sys.Loading();
            var UserId = $("#frmUsers input[name=Id]").val(),
                UserName = $("#frmUsers input[name=UserName]").val(),
                Email = $("#frmUsers input[name=Email]").val(),
                PhoneNumber = $("#frmUsers input[name=PhoneNumber]").val();
                //RoleId = $("#frmUsers select[name=RoleId]").find("option:selected").val();
              
            //var Flag = 1;
            //$("#frmUsers input[name=Flag]").each(function () {
            //    if ($(this).is(':checked')) {
            //        Flag = $(this).val();
            //    }
            //});
            var ListRoles = [];
            ListRoles = $(".checkRoles:checked").map(function () {
                return $(this).val();
            }).toArray();
            var obj = new Object();
            obj.UserId = UserId;
            obj.ListRole = ListRoles;
            obj.UserName = UserName;
            obj.Email = Email;
            obj.PhoneNumber = PhoneNumber;

            $.when(sys.CallAjaxPost("/Account/Modify", obj)).done(function (data) {
                if (data.Status == "Success") {
                    $('#detail_modal').modal('toggle');
                    oTable.fnDraw();
                    sys.Alert("Lưu thành công");
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

function getUserById(obj) {
    
    var id = $(obj).attr("data-id");
    sys.resetForm("#frmUsers");
    
    //kiem tra neu la update thi show thông tin, nguoc lai thi clear form
    if (id !== 0) {
        $.when(sys.CallAjaxPost("/Account/getUserById", { id: id })).done(function (data) {
            if (sys.checkNull(data)) {
                $("#frmUsers input[name=Id]").val(data.Id);
                $("#frmUsers input[name=UserName]").val(data.UserName);
                $("#frmUsers input[name=Email]").val(data.Email);
                $("#frmUsers input[name=PhoneNumber]").val(data.PhoneNumber);
                $('.checkRoles').iCheck('uncheck');
                $('.checkRoles').iCheck('update');
                $.each(data.Roles, function (k, v) {
                    $('#frmUsers input[name=checkRoles][value=' + v.RoleId + ']').iCheck('check');
                });
            }
        });
    }
}

//function deleteCategoriesById(id) {
//    // disable
//    sys.ConfirmDialog(function () {
//        $.when(sys.CallAjaxPost("/Categories/DeleteCategories", { id: id })).done(function (data) {
//            if (data.Status == "Success") {
//                $('#tbCategories').dataTable().fnDraw();
//                sys.Alert("success");
//            } else {
//                sys.Alert("error");
//            }
//        });
//    });
//}
